﻿using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Group = Implem.Pleasanter.Libraries.DataTypes.Group;
namespace Implem.Pleasanter.Libraries.Server
{
    public static class SiteInfo
    {
        public static Dictionary<int, TenantCache> TenantCaches = new Dictionary<int, TenantCache>();
        public static DateTime SessionCleanedUpDate;
        public static int? AnonymousId;

        public static void Reflesh(Context context, bool force = false)
        {
            SetAnonymousId(context: context);
            if (context.TenantId == 0)
            {
                return;
            }
            var tenantCache = TenantCaches.Get(context.TenantId);
            var monitor = tenantCache.GetUpdateMonitor(context: context);
            if (monitor.DeptsUpdated || monitor.GroupsUpdated || monitor.UsersUpdated || force)
            {
                var dataSet = Repository.ExecuteDataSet(
                    context: context,
                    statements: new SqlStatement[]
                    {
                        Rds.SelectDepts(
                            dataTableName: "Depts",
                            column: Rds.DeptsColumn()
                                .TenantId()
                                .DeptId()
                                .DeptCode()
                                .DeptName()
                                .Body()
                                .Disabled(),
                            where: Rds.DeptsWhere().TenantId(context.TenantId),
                            _using: monitor.DeptsUpdated || force),
                        Rds.SelectGroups(
                            dataTableName: "Groups",
                            column: Rds.GroupsColumn()
                                .TenantId()
                                .GroupId()
                                .GroupName()
                                .Body()
                                .Disabled(),
                            where: Rds.GroupsWhere().TenantId(context.TenantId),
                            _using: monitor.GroupsUpdated || force),
                        Rds.SelectUsers(
                            dataTableName: "Users",
                            column: Rds.UsersColumn()
                                .TenantId()
                                .UserId()
                                .DeptId()
                                .LoginId()
                                .Name()
                                .UserCode()
                                .Body()
                                .TenantManager()
                                .ServiceManager()
                                .AllowCreationAtTopSite()
                                .AllowGroupAdministration()
                                .AllowGroupCreation()
                                .AllowApi()
                                .Disabled(),
                            where: Rds.UsersWhere().TenantId(context.TenantId),
                            _using: monitor.UsersUpdated || force)
                    });
                if (monitor.DeptsUpdated || force)
                {
                    tenantCache.DeptHash = dataSet.Tables["Depts"]
                        .AsEnumerable()
                        .ToDictionary(
                            dataRow => dataRow.Int("DeptId"),
                            dataRow => new Dept(dataRow));
                }
                if (monitor.GroupsUpdated || force)
                {
                    tenantCache.GroupHash = dataSet.Tables["Groups"]
                        .AsEnumerable()
                        .ToDictionary(
                            dataRow => dataRow.Int("GroupId"),
                            dataRow => new Group(dataRow));
                }
                if (monitor.UsersUpdated || force)
                {
                    tenantCache.UserHash = dataSet.Tables["Users"]
                        .AsEnumerable()
                        .ToDictionary(
                            dataRow => dataRow.Int("UserId"),
                            dataRow => new User(
                                context: context,
                                dataRow: dataRow));
                }
            }
            if (monitor.PermissionsUpdated || monitor.GroupsUpdated || monitor.UsersUpdated || force)
            {
                tenantCache.SiteDeptHash = new Dictionary<long, List<int>>();
                tenantCache.SiteGroupHash = new Dictionary<long, List<int>>();
                tenantCache.SiteUserHash = new Dictionary<long, List<int>>();
            }
            SetSites(
                context: context,
                tenantCache: tenantCache);
            if (monitor.Updated || force)
            {
                monitor.Update();
            }
        }

        public static IEnumerable<int> SiteDepts(Context context, long siteId)
        {
            if (context.TenantId == 0)
            {
                return new List<int>();
            }
            var tenantCache = TenantCaches.Get(context.TenantId);
            if (!tenantCache.SiteDeptHash.ContainsKey(siteId))
            {
                SetSiteDeptHash(
                    context: context,
                    siteId: siteId,
                    reload: true);
            }
            return tenantCache.SiteDeptHash.Get(siteId);
        }

        public static IEnumerable<int> SiteGroups(Context context, long siteId)
        {
            if (context.TenantId == 0)
            {
                return new List<int>();
            }
            var tenantCache = TenantCaches.Get(context.TenantId);
            if (!tenantCache.SiteGroupHash.ContainsKey(siteId))
            {
                SetSiteGroupHash(
                    context: context,
                    siteId: siteId,
                    reload: true);
            }
            return tenantCache.SiteGroupHash.Get(siteId);
        }

        public static IEnumerable<int> SiteUsers(Context context, long siteId)
        {
            if (context.TenantId == 0)
            {
                return new List<int>();
            }
            var tenantCache = TenantCaches.Get(context.TenantId);
            if (!tenantCache.SiteUserHash.ContainsKey(siteId))
            {
                SetSiteUserHash(
                    context: context,
                    siteId: siteId,
                    reload: true);
            }
            return tenantCache.SiteUserHash.Get(siteId);
        }

        public static void SetSiteDeptHash(Context context, long siteId, bool reload = false)
        {
            if (context.TenantId == 0)
            {
                return;
            }
            var tenantCache = TenantCaches.Get(context.TenantId);
            var temp = tenantCache.SiteDeptHash.ToDictionary(o => o.Key, o => o.Value);
            temp.AddOrUpdate(siteId, GetSiteDeptHash(
                context: context,
                siteId: siteId));
            tenantCache.SiteDeptHash = temp;
        }

        public static void SetSiteGroupHash(Context context, long siteId, bool reload = false)
        {
            if (context.TenantId == 0)
            {
                return;
            }
            var tenantCache = TenantCaches.Get(context.TenantId);
            var temp = tenantCache.SiteGroupHash.ToDictionary(o => o.Key, o => o.Value);
            temp.AddOrUpdate(siteId, GetSiteGroupHash(
                context: context,
                siteId: siteId));
            tenantCache.SiteGroupHash = temp;
        }

        public static void SetSiteUserHash(Context context, long siteId, bool reload = false)
        {
            if (context.TenantId == 0)
            {
                return;
            }
            var tenantCache = TenantCaches.Get(context.TenantId);
            var temp = tenantCache.SiteUserHash.ToDictionary(o => o.Key, o => o.Value);
            temp.AddOrUpdate(siteId, GetSiteUserHash(
                context: context,
                siteId: siteId));
            tenantCache.SiteUserHash = temp;
        }

        private static List<int> GetSiteDeptHash(Context context, long siteId)
        {
            var siteDeptCollection = new List<int>();
            foreach (DataRow dataRow in SiteDeptDataTable(
                context: context,
                siteId: siteId).Rows)
            {
                siteDeptCollection.Add(dataRow["DeptId"].ToInt());
            }
            return siteDeptCollection;
        }

        private static List<int> GetSiteGroupHash(Context context, long siteId)
        {
            var siteGroupCollection = new List<int>();
            foreach (DataRow dataRow in SiteGroupDataTable(
                context: context,
                siteId: siteId).Rows)
            {
                siteGroupCollection.Add(dataRow["GroupId"].ToInt());
            }
            return siteGroupCollection;
        }

        private static List<int> GetSiteUserHash(Context context, long siteId)
        {
            var siteUserCollection = new List<int>();
            foreach (DataRow dataRow in SiteUserDataTable(
                context: context,
                siteId: siteId).Rows)
            {
                siteUserCollection.Add(dataRow["UserId"].ToInt());
            }
            return siteUserCollection;
        }

        private static DataTable SiteDeptDataTable(Context context, long siteId)
        {
            var deptRaw = "\"Depts\".\"DeptId\" and \"Depts\".\"DeptId\">0";
            return Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectDepts(
                    distinct: true,
                    column: Rds.DeptsColumn().DeptId(),
                    where: Rds.DeptsWhere()
                        .TenantId(context.TenantId)
                        .Add(
                            subLeft: Rds.SelectPermissions(
                                column: Rds.PermissionsColumn()
                                    .PermissionType(function: Sqls.Functions.Max),
                                where: Rds.PermissionsWhere()
                                    .ReferenceId(siteId)
                                    .DeptId(raw: deptRaw)),
                            _operator: ">0")));
        }

        private static DataTable SiteGroupDataTable(Context context, long siteId)
        {
            var groupRaw = "\"Groups\".\"GroupId\" and \"Groups\".\"GroupId\">0";
            return Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectGroups(
                    distinct: true,
                    column: Rds.GroupsColumn().GroupId(),
                    where: Rds.GroupsWhere()
                        .TenantId(context.TenantId)
                        .Add(
                            subLeft: Rds.SelectPermissions(
                                column: Rds.PermissionsColumn()
                                    .PermissionType(function: Sqls.Functions.Max),
                                where: Rds.PermissionsWhere()
                                    .ReferenceId(siteId)
                                    .GroupId(raw: groupRaw)),
                            _operator: ">0")));
        }

        private static DataTable SiteUserDataTable(Context context, long siteId)
        {
            return Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectUsers(
                    distinct: true,
                    column: Rds.UsersColumn().UserId(),
                    where: Rds.UsersWhere()
                        .TenantId(context.TenantId)
                        .SiteUserWhere(
                            context: context,
                            siteId: siteId)));
        }

        public static string Name(Context context, int id, Settings.Column.Types type)
        {
            switch (type)
            {
                case Settings.Column.Types.Dept:
                    return Dept(
                        tenantId: context.TenantId,
                        deptId: id).Name;
                case Settings.Column.Types.Group:
                    return Group(
                        tenantId: context.TenantId,
                        groupId: id).Name;
                case Settings.Column.Types.User:
                    return UserName(
                        context: context,
                        userId: id);
                default:
                    return string.Empty;
            }
        }

        public static Dept Dept(int tenantId, int deptId)
        {
            if (tenantId == 0 || deptId == 0)
            {
                return new Dept();
            }
            return TenantCaches.Get(tenantId)?.DeptHash?
                .Where(o => o.Key == deptId)
                .Select(o => o.Value)
                .FirstOrDefault() ?? new Dept();
        }

        public static Group Group(int tenantId, int groupId)
        {
            if (tenantId == 0 || groupId == 0)
            {
                return new Group();
            }
            return TenantCaches.Get(tenantId)?.GroupHash?
                .Where(o => o.Key == groupId)
                .Select(o => o.Value)
                .FirstOrDefault() ?? new Group();
        }

        public static User User(Context context, int userId)
        {
            if (context.TenantId == 0)
            {
                return new User();
            }
            if (userId == -1)
            {
                return new User()
                {
                    TenantId = context.TenantId,
                    Id = -1,
                    Name = Displays.AllUsers(context: context)
                };
            }
            return TenantCaches.Get(context.TenantId)?.UserHash?.Get(userId)
                ?? Anonymous(context: context);
        }

        private static User Anonymous(Context context)
        {
            return new User(
                context: context,
                userId: AnonymousId.ToInt());
        }

        public static string UserName(
            Context context, int userId, bool notSet = true, bool showDeptName = false)
        {
            var user = User(
                context: context,
                userId: userId);
            var name = user?.Name != null
                ? showDeptName
                    ? user.Dept.Name + ")" + user.Name
                    : user.Name
                : null;
            return name != null
                ? name
                : notSet
                    ? Displays.NotSet(context: context)
                    : string.Empty;
        }

        public static Dictionary<long, DataRow> Sites(Context context)
        {
            return TenantCaches.Get(context.TenantId).Sites;
        }

        public static LinkKeyValues Links(Context context)
        {
            return TenantCaches.Get(context.TenantId).Links;
        }

        private static void SetSites(Context context, TenantCache tenantCache)
        {
            var sitesUpdatedTime = tenantCache.SitesUpdatedTime.ToDateTime();
            var dataRows = Rds.ExecuteTable(
                context: context,
                statements: Rds.SelectSites(
                    column: Rds.SitesColumn()
                        .SiteId()
                        .Title()
                        .Body()
                        .SiteName()
                        .SiteGroupName()
                        .GridGuide()
                        .EditorGuide()
                        .ReferenceType()
                        .ParentId()
                        .InheritPermission()
                        .SiteSettings()
                        .UpdatedTime(),
                    where: Rds.SitesWhere()
                        .TenantId(context.TenantId)
                        .UpdatedTime(sitesUpdatedTime, _operator: ">")))
                            .AsEnumerable();
            SetSites(
                tenantCache: tenantCache,
                dataRows: dataRows);
            SetSiteMenu(
                context: context,
                tenantCache: tenantCache,
                dataRows: dataRows);
            SetLinks(
                context: context,
                tenantCache: tenantCache,
                sitesUpdatedTime: sitesUpdatedTime);
            if (dataRows.Any())
            {
                tenantCache.SitesUpdatedTime = dataRows
                    .Max(o => o.Field<DateTime>("UpdatedTime"))
                    .ToString("yyyy/M/d H:m:s.fff");
            }
        }

        private static void SetSites(TenantCache tenantCache, EnumerableRowCollection<DataRow> dataRows)
        {
            var sites = new Dictionary<long, DataRow>();
            tenantCache.Sites?.ForEach(data =>
                sites.Add(data.Key, data.Value));
            foreach (var dataRow in dataRows)
            {
                sites.AddOrUpdate(dataRow.Long("SiteId"), dataRow);
            }
            tenantCache.Sites = sites;
        }

        private static void SetSiteMenu(Context context, TenantCache tenantCache, EnumerableRowCollection<DataRow> dataRows)
        {
            var siteMenu = new SiteMenu();
            tenantCache.SiteMenu?.ForEach(data =>
                siteMenu.Add(data.Key, data.Value));
            foreach (var dataRow in dataRows)
            {
                siteMenu.AddOrUpdate(
                    dataRow.Long("SiteId"),
                    new SiteMenuElement(
                        context: context,
                        siteId: dataRow.Long("SiteId"),
                        referenceType: dataRow.String("ReferenceType"),
                        parentId: dataRow.Long("ParentId"),
                        title: dataRow.String("Title")));
            }
            tenantCache.SiteMenu = siteMenu;
        }

        private static void SetLinks(
            Context context,
            TenantCache tenantCache,
            DateTime sitesUpdatedTime)
        {
            var dataRows = Rds.ExecuteTable(
                context: context,
                statements: new SqlStatement[]
                {
                    SelectLinks(
                        context: context,
                        sitesUpdatedTime: sitesUpdatedTime,
                        tableName: "DestinationSites"),
                    SelectLinks(
                        context: context,
                        sitesUpdatedTime: sitesUpdatedTime,
                        tableName: "SourceSites",
                        unionType: Sqls.UnionTypes.UnionAll)
                })
                    .AsEnumerable();
            if (dataRows.Any())
            {
                var destinationKeyValues = new Dictionary<long, List<long>>();
                var sourceKeyValues = new Dictionary<long, List<long>>();
                tenantCache.Links?.DestinationKeyValues.ForEach(data =>
                    destinationKeyValues.Add(data.Key, data.Value));
                tenantCache.Links?.SourceKeyValues.ForEach(data =>
                    sourceKeyValues.Add(data.Key, data.Value));
                foreach (var data in dataRows.GroupBy(dataRow => dataRow.Long("DestinationId")))
                {
                    destinationKeyValues.AddOrUpdate(
                        data.Key,
                        (destinationKeyValues.Get(data.Key) ?? new List<long>())
                            .Concat(data.Select(dataRow => dataRow.Long("SourceId")))
                            .Distinct()
                            .ToList());
                }
                foreach (var data in dataRows.GroupBy(dataRow => dataRow.Long("SourceId")))
                {
                    sourceKeyValues.AddOrUpdate(
                        data.Key,
                        (sourceKeyValues.Get(data.Key) ?? new List<long>())
                            .Concat(data.Select(dataRow => dataRow.Long("DestinationId")))
                            .Distinct()
                            .ToList());
                }
                if (tenantCache.Links == null)
                {
                    tenantCache.Links = new LinkKeyValues();
                }
                tenantCache.Links.DestinationKeyValues = destinationKeyValues;
                tenantCache.Links.SourceKeyValues = sourceKeyValues;
            }
        }

        private static SqlSelect SelectLinks(
            Context context,
            DateTime sitesUpdatedTime,
            string tableName,
            Sqls.UnionTypes unionType = Sqls.UnionTypes.None)
        {
            return Rds.SelectLinks(
                column: Rds.LinksColumn()
                    .DestinationId()
                    .SourceId(),
                join: new SqlJoinCollection(
                    new SqlJoin(
                        tableBracket: "\"Sites\"",
                        joinType: SqlJoin.JoinTypes.Inner,
                        joinExpression: "\"DestinationSites\".\"SiteId\"=\"Links\".\"DestinationId\"",
                        _as: "DestinationSites"),
                    new SqlJoin(
                        tableBracket: "\"Sites\"",
                        joinType: SqlJoin.JoinTypes.Inner,
                        joinExpression: "\"SourceSites\".\"SiteId\"=\"Links\".\"SourceId\"",
                        _as: "SourceSites")),
                where: Rds.SitesWhere()
                    .TenantId(context.TenantId, tableName: "DestinationSites")
                    .TenantId(context.TenantId, tableName: "SourceSites")
                    .ReferenceType("Wikis", tableName: "DestinationSites", _operator: "<>")
                    .ReferenceType("Wikis", tableName: "SourceSites", _operator: "<>")
                    .UpdatedTime(sitesUpdatedTime, tableName: tableName, _operator: ">"),
                unionType: unionType);
        }

        public static void DeleteSiteCaches(Context context, List<long> siteIds)
        {
            var tenantCache = TenantCaches.Get(context.TenantId);
            DeleteSites(
                siteIds: siteIds,
                tenantCache: tenantCache);
            DeleteSiteMenu(
                siteIds: siteIds,
                tenantCache: tenantCache);
            DeleteLinks(
                siteIds: siteIds,
                tenantCache: tenantCache);
        }

        private static void DeleteSites(TenantCache tenantCache, List<long> siteIds)
        {
            var sites = new Dictionary<long, DataRow>();
            tenantCache.Sites?.ForEach(data =>
                sites.Add(data.Key, data.Value));
            sites.RemoveAll((key, value) => siteIds.Contains(key));
            tenantCache.Sites = sites;
        }

        private static void DeleteSiteMenu(TenantCache tenantCache, List<long> siteIds)
        {
            var siteMenu = new SiteMenu();
            tenantCache.SiteMenu?.ForEach(data =>
                siteMenu.Add(data.Key, data.Value));
            siteMenu.RemoveAll((key, value) => siteIds.Contains(key));
            tenantCache.SiteMenu = siteMenu;
        }

        private static void DeleteLinks(TenantCache tenantCache, List<long> siteIds)
        {
            var destinationKeyValues = new Dictionary<long, List<long>>();
            var sourceKeyValues = new Dictionary<long, List<long>>();
            if (tenantCache.Links != null)
            {
                tenantCache.Links?.DestinationKeyValues.ForEach(data =>
                    destinationKeyValues.Add(data.Key, data.Value));
                tenantCache.Links?.SourceKeyValues.ForEach(data =>
                    sourceKeyValues.Add(data.Key, data.Value));
                destinationKeyValues.RemoveAll((key, value) => siteIds.Contains(key));
                sourceKeyValues.RemoveAll((key, value) => siteIds.Contains(key));
                tenantCache.Links.DestinationKeyValues = destinationKeyValues;
                tenantCache.Links.SourceKeyValues = sourceKeyValues;
            }
        }

        public static void SetAnonymousId(Context context)
        {
            if (AnonymousId == null)
            {
                AnonymousId = Rds.ExecuteScalar_int(
                    context: context,
                    statements: Rds.SelectUsers(
                        column: Rds.UsersColumn().UserId(),
                        where: Rds.UsersWhere()
                            .TenantId(0)
                            .UserId(2)));
            }
        }
    }
}