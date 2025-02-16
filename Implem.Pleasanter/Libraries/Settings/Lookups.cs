﻿using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Models;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Settings
{
    public class Lookups : List<Lookup>
    {
        public Lookups GetRecordingData()
        {
            var lookups = new Lookups();
            ForEach(lookup =>
                lookups.Add(lookup.GetRecordingData()));
            return lookups;
        }

        public Dictionary<string, string> LookupData(
            Context context,
            SiteSettings ss,
            Link link,
            long id)
        {
            var currentSs = ss.Destinations.Get(link.SiteId);
            var canRead = false;
            if (currentSs == null)
            {
                canRead = true;
                switch (link?.TableName)
                {
                    case "Depts":
                        currentSs = SiteSettingsUtilities.DeptsSiteSettings(context: context);
                        break;
                    case "Users":
                        currentSs = SiteSettingsUtilities.UsersSiteSettings(context: context);
                        break;
                    case "Groups":
                        currentSs = SiteSettingsUtilities.GroupsSiteSettings(context: context);
                        break;
                    default:
                        canRead = false;
                        break;
                }
            }
            else
            {
                canRead = Permissions.CanRead(
                    context: context,
                    siteId: currentSs.SiteId,
                    id: id);
            }
            var formData = link.Lookups.ToDictionary(
                lookup => $"{ss.ReferenceType}_{lookup.To}",
                lookup => string.Empty);
            if (id > 0
                && currentSs != null
                && canRead)
            {
                link.Lookups.ForEach(lookup =>
                {
                    formData.AddOrUpdate(
                        $"{ss.ReferenceType}_{lookup.To}",
                        string.Empty);
                });
                switch (currentSs.ReferenceType)
                {
                    case "Issues":
                        var issueModel = new IssueModel(
                            context: context,
                            ss: currentSs,
                            issueId: id);
                        link.Lookups.ForEach(lookup =>
                            formData.AddOrUpdate(
                                $"{ss.ReferenceType}_{lookup.To}",
                                lookup.Data(
                                    context: context,
                                    ss: currentSs,
                                    issueModel: issueModel)));
                        break;
                    case "Results":
                        var resultModel = new ResultModel(
                            context: context,
                            ss: currentSs,
                            resultId: id);
                        link.Lookups.ForEach(lookup =>
                            formData.AddOrUpdate(
                                $"{ss.ReferenceType}_{lookup.To}",
                                lookup.Data(
                                    context: context,
                                    ss: currentSs,
                                    resultModel: resultModel)));
                        break;
                    case "Depts":
                        var deptModel = new DeptModel(
                            context: context,
                            ss: currentSs,
                            deptId: id.ToInt());
                        link.Lookups.ForEach(lookup =>
                            formData.AddOrUpdate(
                                $"{ss.ReferenceType}_{lookup.To}",
                                lookup.Data(
                                    context: context,
                                    ss: currentSs,
                                    deptModel: deptModel)));
                        break;
                    case "Users":
                        var userModel = new UserModel(
                            context: context,
                            ss: currentSs,
                            userId: id.ToInt());
                        link.Lookups.ForEach(lookup =>
                            formData.AddOrUpdate(
                                $"{ss.ReferenceType}_{lookup.To}",
                                lookup.Data(
                                    context: context,
                                    ss: currentSs,
                                    userModel: userModel)));
                        break;
                    case "Groups":
                        var groupModel = new GroupModel(
                            context: context,
                            ss: currentSs,
                            groupId: id.ToInt());
                        link.Lookups.ForEach(lookup =>
                            formData.AddOrUpdate(
                                $"{ss.ReferenceType}_{lookup.To}",
                                lookup.Data(
                                    context: context,
                                    ss: currentSs,
                                    groupModel: groupModel)));
                        break;
                }
            }
            return formData;
        }
    }
}