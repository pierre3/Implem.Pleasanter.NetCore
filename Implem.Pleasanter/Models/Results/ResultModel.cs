﻿using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Extensions;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.ServerScripts;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using static Implem.Pleasanter.Libraries.ServerScripts.ServerScriptModel;
namespace Implem.Pleasanter.Models
{
    [Serializable]
    public class ResultModel : BaseItemModel
    {
        public long ResultId = 0;
        public Status Status = new Status();
        public User Manager = new User();
        public User Owner = new User();
        public bool Locked = false;

        public TitleBody TitleBody
        {
            get
            {
                return new TitleBody(ResultId, Ver, VerType == Versions.VerTypes.History, Title.Value, Title.DisplayValue, Body);
            }
        }

        public SiteTitle SiteTitle
        {
            get
            {
                return new SiteTitle(SiteId);
            }
        }

        public long SavedResultId = 0;
        public int SavedStatus = 0;
        public int SavedManager = 0;
        public int SavedOwner = 0;
        public bool SavedLocked = false;

        public bool Status_Updated(Context context, Column column = null)
        {
            return Status.Value != SavedStatus &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToInt() != Status.Value);
        }

        public bool Manager_Updated(Context context, Column column = null)
        {
            return Manager.Id != SavedManager &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToInt() != Manager.Id);
        }

        public bool Owner_Updated(Context context, Column column = null)
        {
            return Owner.Id != SavedOwner &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToInt() != Owner.Id);
        }

        public bool Locked_Updated(Context context, Column column = null)
        {
            return Locked != SavedLocked &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToBool() != Locked);
        }

        public string PropertyValue(Context context, Column column)
        {
            switch (column?.ColumnName)
            {
                case "SiteId": return SiteId.ToString();
                case "UpdatedTime": return UpdatedTime.Value.ToString();
                case "ResultId": return ResultId.ToString();
                case "Ver": return Ver.ToString();
                case "Title": return Title.Value;
                case "Body": return Body;
                case "TitleBody": return TitleBody.ToString();
                case "Status": return Status.Value.ToString();
                case "Manager": return Manager.Id.ToString();
                case "Owner": return Owner.Id.ToString();
                case "Locked": return Locked.ToString();
                case "SiteTitle": return SiteTitle.SiteId.ToString();
                case "Comments": return Comments.ToJson();
                case "Creator": return Creator.Id.ToString();
                case "Updator": return Updator.Id.ToString();
                case "CreatedTime": return CreatedTime.Value.ToString();
                case "VerUp": return VerUp.ToString();
                case "Timestamp": return Timestamp;
                default: return GetValue(
                    context: context,
                    column: column);
            }
        }

        public Dictionary<string, string> PropertyValues(Context context, List<Column> columns)
        {
            var hash = new Dictionary<string, string>();
            columns?
                .Where(column => column != null)
                .ForEach(column =>
                {
                    switch (column.ColumnName)
                    {
                        case "SiteId":
                            hash.Add("SiteId", SiteId.ToString());
                            break;
                        case "UpdatedTime":
                            hash.Add("UpdatedTime", UpdatedTime.Value.ToString());
                            break;
                        case "ResultId":
                            hash.Add("ResultId", ResultId.ToString());
                            break;
                        case "Ver":
                            hash.Add("Ver", Ver.ToString());
                            break;
                        case "Title":
                            hash.Add("Title", Title.Value);
                            break;
                        case "Body":
                            hash.Add("Body", Body);
                            break;
                        case "TitleBody":
                            hash.Add("TitleBody", TitleBody.ToString());
                            break;
                        case "Status":
                            hash.Add("Status", Status.Value.ToString());
                            break;
                        case "Manager":
                            hash.Add("Manager", Manager.Id.ToString());
                            break;
                        case "Owner":
                            hash.Add("Owner", Owner.Id.ToString());
                            break;
                        case "Locked":
                            hash.Add("Locked", Locked.ToString());
                            break;
                        case "SiteTitle":
                            hash.Add("SiteTitle", SiteTitle.SiteId.ToString());
                            break;
                        case "Comments":
                            hash.Add("Comments", Comments.ToJson());
                            break;
                        case "Creator":
                            hash.Add("Creator", Creator.Id.ToString());
                            break;
                        case "Updator":
                            hash.Add("Updator", Updator.Id.ToString());
                            break;
                        case "CreatedTime":
                            hash.Add("CreatedTime", CreatedTime.Value.ToString());
                            break;
                        case "VerUp":
                            hash.Add("VerUp", VerUp.ToString());
                            break;
                        case "Timestamp":
                            hash.Add("Timestamp", Timestamp);
                            break;
                        default:
                            hash.Add(column.ColumnName, GetValue(
                                context: context,
                                column: column));
                            break;
                    }
                });
            return hash;
        }

        public bool PropertyUpdated(Context context, string name)
        {
            switch (name)
            {
                case "SiteId": return SiteId_Updated(context: context);
                case "Ver": return Ver_Updated(context: context);
                case "Title": return Title_Updated(context: context);
                case "Body": return Body_Updated(context: context);
                case "Status": return Status_Updated(context: context);
                case "Manager": return Manager_Updated(context: context);
                case "Owner": return Owner_Updated(context: context);
                case "Locked": return Locked_Updated(context: context);
                case "Comments": return Comments_Updated(context: context);
                case "Creator": return Creator_Updated(context: context);
                case "Updator": return Updator_Updated(context: context);
                default: 
                    switch (Def.ExtendedColumnTypes.Get(name ?? string.Empty))
                    {
                        case "Class": return Class_Updated(name);
                        case "Num": return Num_Updated(name);
                        case "Date": return Date_Updated(name);
                        case "Description": return Description_Updated(name);
                        case "Check": return Check_Updated(name);
                        case "Attachments": return Attachments_Updated(name);
                    }
                    break;
            }
            return false;
        }

        public string CsvData(
            Context context,
            SiteSettings ss,
            Column column,
            ExportColumn exportColumn,
            List<string> mine,
            bool? encloseDoubleQuotes)
        {
            var value = string.Empty;
            switch (column.Name)
            {
                case "SiteId":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? SiteId.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "UpdatedTime":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? UpdatedTime.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "ResultId":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? ResultId.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "Ver":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? Ver.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "Title":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? Title.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "Body":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? Body.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "TitleBody":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? TitleBody.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "Status":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? Status.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "Manager":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? Manager.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "Owner":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? Owner.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "Locked":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? Locked.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "SiteTitle":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? SiteTitle.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "Comments":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? Comments.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "Creator":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? Creator.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "Updator":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? Updator.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "CreatedTime":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? CreatedTime.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                default:
                    switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                    {
                        case "Class":
                            value = ss.ReadColumnAccessControls.Allowed(
                                context: context,
                                ss: ss,
                                column: column,
                                mine: mine)
                                    ? GetClass(columnName: column.Name).ToExport(
                                        context: context,
                                        column: column,
                                        exportColumn: exportColumn)
                                    : string.Empty;
                            break;
                        case "Num":
                            value = ss.ReadColumnAccessControls.Allowed(
                                context: context,
                                ss: ss,
                                column: column,
                                mine: mine)
                                    ? GetNum(columnName: column.Name).ToExport(
                                        context: context,
                                        column: column,
                                        exportColumn: exportColumn)
                                    : string.Empty;
                            break;
                        case "Date":
                            value = ss.ReadColumnAccessControls.Allowed(
                                context: context,
                                ss: ss,
                                column: column,
                                mine: mine)
                                    ? GetDate(columnName: column.Name).ToExport(
                                        context: context,
                                        column: column,
                                        exportColumn: exportColumn)
                                    : string.Empty;
                            break;
                        case "Description":
                            value = ss.ReadColumnAccessControls.Allowed(
                                context: context,
                                ss: ss,
                                column: column,
                                mine: mine)
                                    ? GetDescription(columnName: column.Name).ToExport(
                                        context: context,
                                        column: column,
                                        exportColumn: exportColumn)
                                    : string.Empty;
                            break;
                        case "Check":
                            value = ss.ReadColumnAccessControls.Allowed(
                                context: context,
                                ss: ss,
                                column: column,
                                mine: mine)
                                    ? GetCheck(columnName: column.Name).ToExport(
                                        context: context,
                                        column: column,
                                        exportColumn: exportColumn)
                                    : string.Empty;
                            break;
                        case "Attachments":
                            value = ss.ReadColumnAccessControls.Allowed(
                                context: context,
                                ss: ss,
                                column: column,
                                mine: mine)
                                    ? GetAttachments(columnName: column.Name).ToExport(
                                        context: context,
                                        column: column,
                                        exportColumn: exportColumn)
                                    : string.Empty;
                            break;
                        default: return string.Empty;
                    }
                    break;
            }
            if (encloseDoubleQuotes != false)
            {
                return "\"" + value?.Replace("\"", "\"\"") + "\"";
            }
            else
            {
                return value;
            }
        }

        public List<long> SwitchTargets;

        public ResultModel()
        {
        }

        public ResultModel(
            Context context,
            SiteSettings ss,
            Dictionary<string, string> formData = null,
            bool setByApi = false,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing(context: context);
            Context = context;
            SiteId = ss.SiteId;
            if (ResultId == 0) SetDefault(context: context, ss: ss);
            if (formData != null)
            {
                SetByForm(
                    context: context,
                    ss: ss,
                    formData: formData);
            }
            if (setByApi) SetByApi(context: context, ss: ss);
            if (formData != null || setByApi)
            {
                SetByLookups(
                    context: context,
                    ss: ss,
                    requestFormData: formData);
            }
            MethodType = methodType;
            OnConstructed(context: context);
        }

        public ResultModel(
            Context context,
            SiteSettings ss,
            long resultId,
            Dictionary<string, string> formData = null,
            bool setByApi = false,
            bool clearSessions = false,
            List<long> switchTargets = null,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing(context: context);
            Context = context;
            ResultId = resultId;
            SiteId = ss.SiteId;
            if (context.QueryStrings.ContainsKey("ver"))
            {
                Get(context: context,
                    tableType: Sqls.TableTypes.NormalAndHistory,
                    where: Rds.ResultsWhereDefault(
                        context: context,
                        resultModel: this)
                            .Results_Ver(context.QueryStrings.Int("ver")), ss: ss);
            }
            else
            {
                Get(context: context, ss: ss);
            }
            if (clearSessions) ClearSessions(context: context);
            if (ResultId == 0) SetDefault(context: context, ss: ss);
            if (formData != null)
            {
                SetByForm(
                    context: context,
                    ss: ss,
                    formData: formData);
            }
            if (setByApi) SetByApi(context: context, ss: ss);
            if (formData != null || setByApi)
            {
                SetByLookups(
                    context: context,
                    ss: ss,
                    requestFormData: formData);
            }
            if (SavedLocked)
            {
                ss.SetLockedRecord(
                    context: context,
                    time: UpdatedTime,
                    user: Updator);
            }
            SwitchTargets = switchTargets;
            MethodType = methodType;
            OnConstructed(context: context);
        }

        public ResultModel(
            Context context,
            SiteSettings ss,
            DataRow dataRow,
            Dictionary<string, string> formData = null,
            string tableAlias = null)
        {
            OnConstructing(context: context);
            Context = context;
            if (dataRow != null)
            {
                Set(
                    context: context,
                    ss: ss,
                    dataRow: dataRow,
                    tableAlias: tableAlias);
            }
            if (formData != null)
            {
                SetByForm(
                    context: context,
                    ss: ss,
                    formData: formData);
            }
            if (formData != null)
            {
                SetByLookups(
                    context: context,
                    ss: ss,
                    requestFormData: formData);
            }
            OnConstructed(context: context);
        }

        private void OnConstructing(Context context)
        {
        }

        private void OnConstructed(Context context)
        {
        }

        public void ClearSessions(Context context)
        {
        }

        public ResultModel Get(
            Context context,
            SiteSettings ss,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlColumnCollection column = null,
            SqlJoinCollection join = null,
            SqlWhereCollection where = null,
            SqlOrderByCollection orderBy = null,
            SqlParamCollection param = null,
            bool distinct = false,
            int top = 0)
        {
            where = where ?? Rds.ResultsWhereDefault(
                context: context,
                resultModel: this);
            var view = new View();
            view.SetColumnsWhere(
                context: context,
                ss: ss,
                where: where,
                siteId: SiteId,
                id: ResultId,
                timestamp: Timestamp.ToDateTime());
            column = (column ?? Rds.ResultsEditorColumns(ss))?.SetExtendedSqlSelectingColumn(context: context, ss: ss, view: view);
            join = join ?? Rds.ResultsJoinDefault();
            if (ss?.TableType == Sqls.TableTypes.Normal)
            {
                join = ss.Join(
                    context: context,
                    join: new Implem.Libraries.DataSources.Interfaces.IJoin[]
                    {
                        column,
                        where,
                        orderBy
                    });
            }
            Set(context, ss, Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectResults(
                    tableType: tableType,
                    column: column,
                    join: join,
                    where: where,
                    orderBy: orderBy,
                    param: param,
                    distinct: distinct,
                    top: top)));
            return this;
        }

        public ResultApiModel GetByApi(Context context, SiteSettings ss)
        {
            var data = new ResultApiModel()
            {
                ApiVersion = context.ApiVersion
            };
            ss.ReadableColumns(context: context, noJoined: true).ForEach(column =>
            {
                switch (column.ColumnName)
                {
                    case "SiteId": data.SiteId = SiteId; break;
                    case "UpdatedTime": data.UpdatedTime = UpdatedTime.Value.ToLocal(context: context); break;
                    case "ResultId": data.ResultId = ResultId; break;
                    case "Ver": data.Ver = Ver; break;
                    case "Title": data.Title = Title.Value; break;
                    case "Body": data.Body = Body; break;
                    case "Status": data.Status = Status.Value; break;
                    case "Manager": data.Manager = Manager.Id; break;
                    case "Owner": data.Owner = Owner.Id; break;
                    case "Locked": data.Locked = Locked; break;
                    case "Creator": data.Creator = Creator.Id; break;
                    case "Updator": data.Updator = Updator.Id; break;
                    case "CreatedTime": data.CreatedTime = CreatedTime.Value.ToLocal(context: context); break;
                    case "Comments": data.Comments = Comments.ToLocal(context: context).ToJson(); break;
                    default: 
                        data.Value(
                            context: context,
                            column: column,
                            value: GetValue(
                                context: context,
                                column: column,
                                toLocal: true));
                        break;
                }
            });
            data.ItemTitle = Title.DisplayValue;
            return data;
        }

        public string ToDisplay(Context context, SiteSettings ss, Column column, List<string> mine)
        {
            if (!ss.ReadColumnAccessControls.Allowed(
                context: context,
                ss: ss,
                column: column,
                mine: mine))
            {
                return string.Empty;
            }
            switch (column.Name)
            {
                case "ResultId":
                    return ResultId.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "Title":
                    return Title.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "Body":
                    return Body.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "Status":
                    return Status.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "Manager":
                    return Manager.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "Owner":
                    return Owner.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "Locked":
                    return Locked.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "Timestamp":
                    return Timestamp.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                default:
                    switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                    {
                        case "Class":
                            return GetClass(columnName: column.Name).ToDisplay(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Num":
                            return GetNum(columnName: column.Name).ToDisplay(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Date":
                            return GetDate(columnName: column.Name).ToDisplay(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Description":
                            return GetDescription(columnName: column.Name).ToDisplay(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Check":
                            return GetCheck(columnName: column.Name).ToDisplay(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Attachments":
                            return GetAttachments(columnName: column.Name).ToDisplay(
                                context: context,
                                ss: ss,
                                column: column);
                        default:
                            return string.Empty;
                    }
            }
        }

        public object ToApiDisplayValue(Context context, SiteSettings ss, Column column, List<string> mine)
        {
            if (!ss.ReadColumnAccessControls.Allowed(
                context: context,
                ss: ss,
                column: column,
                mine: mine))
            {
                return string.Empty;
            }
            switch (column.Name)
            {
                case "SiteId":
                    return SiteId.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "UpdatedTime":
                    return UpdatedTime.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "ResultId":
                    return ResultId.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Ver":
                    return Ver.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Title":
                    return Title.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Body":
                    return Body.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "TitleBody":
                    return TitleBody.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Status":
                    return Status.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Manager":
                    return Manager.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Owner":
                    return Owner.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Locked":
                    return Locked.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "SiteTitle":
                    return SiteTitle.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Comments":
                    return Comments.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Creator":
                    return Creator.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Updator":
                    return Updator.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "CreatedTime":
                    return CreatedTime.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "VerUp":
                    return VerUp.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Timestamp":
                    return Timestamp.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                default:
                    switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                    {
                        case "Class":
                            return GetClass(columnName: column.Name).ToApiDisplayValue(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Num":
                            return GetNum(columnName: column.Name).ToApiDisplayValue(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Date":
                            return GetDate(columnName: column.Name).ToApiDisplayValue(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Description":
                            return GetDescription(columnName: column.Name).ToApiDisplayValue(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Check":
                            return GetCheck(columnName: column.Name).ToApiDisplayValue(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Attachments":
                            return GetAttachments(columnName: column.Name).ToApiDisplayValue(
                                context: context,
                                ss: ss,
                                column: column);
                        default:
                            return string.Empty;
                    }
            }
        }

        public object ToApiValue(Context context, SiteSettings ss, Column column, List<string> mine)
        {
            if (!ss.ReadColumnAccessControls.Allowed(
                context: context,
                ss: ss,
                column: column,
                mine: mine))
            {
                return string.Empty;
            }
            switch (column.Name)
            {
                case "SiteId":
                    return SiteId.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "UpdatedTime":
                    return UpdatedTime.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "ResultId":
                    return ResultId.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Ver":
                    return Ver.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Title":
                    return Title.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Body":
                    return Body.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "TitleBody":
                    return TitleBody.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Status":
                    return Status.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Manager":
                    return Manager.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Owner":
                    return Owner.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Locked":
                    return Locked.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "SiteTitle":
                    return SiteTitle.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Comments":
                    return Comments.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Creator":
                    return Creator.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Updator":
                    return Updator.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "CreatedTime":
                    return CreatedTime.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "VerUp":
                    return VerUp.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Timestamp":
                    return Timestamp.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                default:
                    switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                    {
                        case "Class":
                            return GetClass(columnName: column.Name).ToApiValue(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Num":
                            return GetNum(columnName: column.Name).ToApiValue(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Date":
                            return GetDate(columnName: column.Name).ToApiValue(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Description":
                            return GetDescription(columnName: column.Name).ToApiValue(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Check":
                            return GetCheck(columnName: column.Name).ToApiValue(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Attachments":
                            return GetAttachments(columnName: column.Name).ToApiValue(
                                context: context,
                                ss: ss,
                                column: column);
                        default:
                            return string.Empty;
                    }
            }
        }

        public string FullText(
            Context context,
            SiteSettings ss,
            bool backgroundTask = false,
            bool onCreating = false)
        {
            if (!Parameters.Search.CreateIndexes && !backgroundTask) return null;
            if (AccessStatus == Databases.AccessStatuses.NotFound) return null;
            var fullText = new System.Text.StringBuilder();
            if (ss.FullTextIncludeBreadcrumb == true)
            {
                SiteInfo.TenantCaches
                    .Get(context.TenantId)?
                    .SiteMenu.Breadcrumb(
                        context: context,
                        siteId: SiteId)
                    .FullText(
                        context: context,
                        fullText: fullText);
            }
            if (ss.FullTextIncludeSiteId == true)
            {
                fullText.Append($" {ss.SiteId}");
            }
            if (ss.FullTextIncludeSiteTitle == true)
            {
                fullText.Append($" {ss.Title}");
            }
            ss.GetEditorColumnNames(
                context: context,
                columnOnly: true)
                    .Select(columnName => ss.GetColumn(
                        context: context,
                        columnName: columnName))
                    .ForEach(column =>
                    {
                        switch (column.ColumnName)
                        {
                            case "ResultId":
                                ResultId.FullText(
                                    context: context,
                                    column: column,
                                    fullText: fullText);
                                break;
                            case "Title":
                                Title.FullText(
                                    context: context,
                                    column: column,
                                    fullText: fullText);
                                break;
                            case "Body":
                                Body.FullText(
                                    context: context,
                                    column: column,
                                    fullText: fullText);
                                break;
                            case "Status":
                                Status.FullText(
                                    context: context,
                                    column: column,
                                    fullText: fullText);
                                break;
                            case "Manager":
                                Manager.FullText(
                                    context: context,
                                    column: column,
                                    fullText: fullText);
                                break;
                            case "Owner":
                                Owner.FullText(
                                    context: context,
                                    column: column,
                                    fullText: fullText);
                                break;
                            case "Comments":
                                Comments.FullText(
                                    context: context,
                                    column: column,
                                    fullText: fullText);
                                break;
                            default:
                                BaseFullText(
                                    context: context,
                                    column: column,
                                    fullText: fullText);
                                break;
                        }
                    });
            Creator.FullText(
                context,
                column: ss.GetColumn(
                    context: context,
                    columnName: "Creator"),
                fullText);
            Updator.FullText(
                context,
                column: ss.GetColumn(
                    context: context,
                    columnName: "Updator"),
                fullText);
            CreatedTime.FullText(
                context,
                column: ss.GetColumn(
                    context: context,
                    columnName: "CreatedTime"),
                fullText);
            UpdatedTime.FullText(
                context,
                column: ss.GetColumn(
                    context: context,
                    columnName: "UpdatedTime"),
                fullText);
            if (!onCreating)
            {
                FullTextExtensions.OutgoingMailsFullText(
                    context: context,
                    ss: ss,
                    fullText: fullText,
                    referenceType: "Results",
                    referenceId: ResultId);
            }
            return fullText
                .ToString()
                .Replace("　", " ")
                .Replace("\r", " ")
                .Replace("\n", " ")
                .Split(' ')
                .Select(o => o.Trim())
                .Where(o => o != string.Empty)
                .Distinct()
                .Join(" ");
        }

        public ErrorData Create(
            Context context,
            SiteSettings ss,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlParamCollection param = null,
            long copyFrom = 0,
            bool extendedSqls = true,
            bool synchronizeSummary = true,
            bool forceSynchronizeSourceSummary = false,
            bool notice = false,
            string noticeType = "Created",
            bool otherInitValue = false,
            bool get = true)
        {
            SetByBeforeCreateServerScript(
                context: context,
                ss: ss);
            if (context.ErrorData.Type != Error.Types.None)
            {
                return context.ErrorData;
            }
            var statements = new List<SqlStatement>();
            if (extendedSqls)
            {
                statements.OnCreatingExtendedSqls(
                    context: context,
                    siteId: SiteId);
            }
            statements.AddRange(CreateStatements(
                context: context,
                ss: ss,
                tableType: tableType,
                param: param,
                otherInitValue: otherInitValue));
            var response = Repository.ExecuteScalar_response(
                context: context,
                transactional: true,
                selectIdentity: true,
                statements: statements.ToArray());
            if (response.Event == "Duplicated")
            {
                return new ErrorData(
                    type: Error.Types.Duplicated,
                    id: ResultId,
                    columnName: response.ColumnName);
            }
            WriteAttachments(
                context: context,
                ss: ss);
            ResultId = (response.Id ?? ResultId).ToLong();
            if (synchronizeSummary)
            {
                SynchronizeSummary(
                    context: context,
                    ss: ss,
                    force: forceSynchronizeSourceSummary);
            }
            ExecuteAutomaticNumbering(
                context: context,
                ss: ss);
            if (context.ContractSettings.Notice != false && notice)
            {
                SetTitle(
                    context: context,
                    ss: ss);
                Notice(
                    context: context,
                    ss: ss,
                    notifications: GetNotifications(
                        context: context,
                        ss: ss,
                        notice: notice),
                    type: noticeType);
            }
            if (get) Get(context: context, ss: ss);
            if (ss.PermissionForCreating != null)
            {
                ss.SetPermissions(
                    context: context,
                    referenceId: ResultId);
            }
            var fullText = FullText(context, ss: ss, onCreating: true);
            statements = new List<SqlStatement>();
            statements.Add(Rds.UpdateItems(
                param: Rds.ItemsParam()
                    .Title(Title.DisplayValue)
                    .FullText(fullText, _using: fullText != null)
                    .SearchIndexCreatedTime(DateTime.Now, _using: fullText != null),
                where: Rds.ItemsWhere().ReferenceId(ResultId)));
            statements.Add(BinaryUtilities.UpdateReferenceId(
                context: context,
                ss: ss,
                referenceId: ResultId,
                values: fullText));
            if (extendedSqls)
            {
                statements.OnCreatedExtendedSqls(
                    context: context,
                    siteId: SiteId,
                    id: ResultId);
            }
            Repository.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: statements.ToArray());
            if (get && Rds.ExtendedSqls(
                context: context,
                siteId: SiteId,
                id: ResultId)
                    ?.Any(o => o.OnCreated) == true)
            {
                Get(
                    context: context,
                    ss: ss);
            }
            if (copyFrom > 0)
            {
                ss.LinkActions(
                    context: context,
                    type: "CopyWithLinks",
                    data: new Dictionary<string, string>()
                    {
                        { "From", copyFrom.ToString() },
                        { "To", ResultId.ToString() }
                    });
            }
            SetByAfterCreateServerScript(
                context: context,
                ss: ss);
            return new ErrorData(type: Error.Types.None);
        }

        public List<SqlStatement> CreateStatements(
            Context context,
            SiteSettings ss,
            string dataTableName = null,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlParamCollection param = null,
            bool otherInitValue = false)
        {
            var statements = new List<SqlStatement>();
            statements.AddRange(IfDuplicatedStatements(ss: ss));
            statements.AddRange(new List<SqlStatement>
            {
                Rds.InsertItems(
                    dataTableName: dataTableName,
                    selectIdentity: true,
                    param: Rds.ItemsParam()
                        .ReferenceType("Results")
                        .SiteId(SiteId)
                        .Title(Title.DisplayValue)),
                Rds.InsertResults(
                    dataTableName: dataTableName,
                    tableType: tableType,
                    param: param ?? Rds.ResultsParamDefault(
                        context: context,
                        ss: ss,
                        resultModel: this,
                        setDefault: true,
                        otherInitValue: otherInitValue)),
                InsertLinks(
                    context: context,
                    ss: ss,
                    setIdentity: true),
            });
            statements.AddRange(UpdateAttachmentsStatements(context: context, ss: ss));
            statements.AddRange(PermissionUtilities.InsertStatements(
                context: context,
                ss: ss,
                users: ss.Columns
                    .Where(o => o.Type == Column.Types.User)
                    .ToDictionary(o =>
                        o.ColumnName,
                        o => SiteInfo.User(
                            context: context,
                            userId: PropertyValue(
                                context: context,
                                column: o).ToInt()))));
            return statements;
        }

        public void ExecuteAutomaticNumbering(
            Context context,
            SiteSettings ss)
        {
            ss.Columns
                .Where(column => !column.AutoNumberingFormat.IsNullOrEmpty())
                .Where(column => !column.Joined)
                .ForEach(column => SetByForm(
                    context: context,
                    ss: ss,
                    formData: new Dictionary<string, string>()
                    {
                        {
                            $"Results_{column.ColumnName}",
                            AutoNumberingUtilities.ExecuteAutomaticNumbering(
                                context: context,
                                ss: ss,
                                column: column,
                                data: ss.IncludedColumns(value: column.AutoNumberingFormat)
                                    .ToDictionary(
                                        o => o.ColumnName,
                                        o => ToDisplay(
                                            context: context,
                                            ss: ss,
                                            column: o,
                                            mine: Mine(context: context))),
                                updateModel: Rds.UpdateResults(
                                    where: Rds.ResultsWhere()
                                        .SiteId(SiteId)
                                        .ResultId(ResultId)))
                        }
                    }));
        }

        public ErrorData Update(
            Context context,
            SiteSettings ss,
            Process process = null,
            bool extendedSqls = true,
            bool synchronizeSummary = true,
            bool forceSynchronizeSourceSummary = false,
            bool notice = false,
            string previousTitle = null,
            SqlParamCollection param = null,
            List<SqlStatement> additionalStatements = null,
            bool otherInitValue = false,
            bool setBySession = true,
            bool get = true)
        {
            SetByBeforeUpdateServerScript(
                context: context,
                ss: ss);
            if (context.ErrorData.Type != Error.Types.None)
            {
                return context.ErrorData;
            }
            var notifications = GetNotifications(
                context: context,
                ss: ss,
                notice: notice,
                before: true);
            if (setBySession)
            {
                SetBySession(context: context);
            }
            var statements = new List<SqlStatement>();
            if (extendedSqls)
            {
                statements.OnUpdatingExtendedSqls(
                    context: context,
                    siteId: SiteId,
                    id: ResultId,
                    timestamp: Timestamp.ToDateTime());
            }
            statements.AddRange(UpdateStatements(
                context: context,
                ss: ss,
                param: param,
                otherInitValue: otherInitValue,
                additionalStatements: additionalStatements));
            var response = Repository.ExecuteScalar_response(
                context: context,
                transactional: true,
                statements: statements.ToArray());
            if (response.Event == "Duplicated")
            {
                return new ErrorData(
                    type: Error.Types.Duplicated,
                    id: ResultId,
                    columnName: response.ColumnName);
            }
            if (response.Event == "Conflicted")
            {
                return new ErrorData(
                    type: Error.Types.UpdateConflicts,
                    id: ResultId);
            }
            WriteAttachments(
                context: context,
                ss: ss);
            if (synchronizeSummary)
            {
                SynchronizeSummary(
                    context: context,
                    ss: ss,
                    force: forceSynchronizeSourceSummary);
            }
            if (context.ContractSettings.Notice != false && notice)
            {
                Notice(
                    context: context,
                    ss: ss,
                    notifications: NotificationUtilities.MeetConditions(
                        ss: ss,
                        before: notifications,
                        after: GetNotifications(
                            context: context,
                            ss: ss,
                            notice: notice)),
                    type: "Updated");
                process?.Notifications?.ForEach(notification =>
                    notification.Send(
                        context: context,
                        ss: ss,
                        title: ReplacedDisplayValues(
                            context: context,
                            ss: ss,
                            value: notification.Subject),
                        body: ReplacedDisplayValues(
                            context: context,
                            ss: ss,
                            value: notification.Body),
                        values: ss.IncludedColumns(notification.Address)
                            .ToDictionary(
                                column => column,
                                column => PropertyValue(
                                    context: context,
                                    column: column))));
            }
            if (get)
            {
                Get(context: context, ss: ss);
            }
            UpdateRelatedRecords(
                context: context,
                ss: ss,
                extendedSqls: extendedSqls,
                previousTitle: previousTitle,
                get: get,
                addUpdatedTimeParam: true,
                addUpdatorParam: true,
                updateItems: true);
            SetByAfterUpdateServerScript(
                context: context,
                ss: ss);
            return new ErrorData(type: Error.Types.None);
        }

        public List<SqlStatement> UpdateStatements(
            Context context,
            SiteSettings ss,
            string dataTableName = null,
            SqlParamCollection param = null,
            bool otherInitValue = false,
            List<SqlStatement> additionalStatements = null)
        {
            ss.Columns
                .Where(column => column.ColumnName.StartsWith("Attachments"))
                .ForEach(column => GetAttachments(column.ColumnName).SetData(column: column));
            var timestamp = Timestamp.ToDateTime();
            var statements = new List<SqlStatement>();
            var where = Rds.ResultsWhereDefault(
                context: context,
                resultModel: this)
                    .UpdatedTime(timestamp, _using: timestamp.InRange());
            statements.AddRange(IfDuplicatedStatements(ss: ss));
            if (Versions.VerUp(
                context: context,
                ss: ss,
                verUp: VerUp))
            {
                statements.Add(Rds.ResultsCopyToStatement(
                    where: where,
                    tableType: Sqls.TableTypes.History,
                    ColumnNames()));
                Ver++;
            }
            statements.AddRange(UpdateStatements(
                context: context,
                ss: ss,
                dataTableName: dataTableName,
                where: where,
                param: param,
                otherInitValue: otherInitValue));
            statements.AddRange(UpdateAttachmentsStatements(context: context, ss: ss));
            if (RecordPermissions != null)
            {
                statements.UpdatePermissions(context, ss, ResultId, RecordPermissions);
            }
            if (additionalStatements?.Any() == true)
            {
                statements.AddRange(additionalStatements);
            }
            return statements;
        }

        private List<SqlStatement> UpdateStatements(
            Context context,
            SiteSettings ss,
            string dataTableName = null,
            SqlWhereCollection where = null,
            SqlParamCollection param = null,
            bool otherInitValue = false)
        {
            return new List<SqlStatement>
            {
                Rds.UpdateResults(
                    dataTableName: dataTableName,
                    where: where,
                    param: param ?? Rds.ResultsParamDefault(
                        context: context,
                        ss: ss,
                        resultModel: this,
                        otherInitValue: otherInitValue)),
                new SqlStatement(Def.Sql.IfConflicted.Params(ResultId))
                {
                    DataTableName = dataTableName,
                    IfConflicted = true,
                    Id = ResultId
                }
            };
        }

        private List<SqlStatement> UpdateAttachmentsStatements(Context context, SiteSettings ss)
        {
            var statements = new List<SqlStatement>();
            ColumnNames()
                .Where(columnName => columnName.StartsWith("Attachments"))
                .Where(columnName => Attachments_Updated(columnName: columnName))
                .ForEach(columnName =>
                    GetAttachments(columnName: columnName).Statements(
                        context: context,
                        statements: statements,
                        referenceId: ResultId,
                        column: ss.GetColumn(
                            context: context,
                            columnName: columnName)));
            return statements;
        }

        private void WriteAttachments(Context context, SiteSettings ss)
        {
            ColumnNames()
                .Where(columnName => columnName.StartsWith("Attachments"))
                .Where(columnName => Attachments_Updated(columnName: columnName))
                .ForEach(columnName =>
                    GetAttachments(columnName: columnName).Write(
                        context: context,
                        column: ss.GetColumn(
                            context: context,
                            columnName: columnName)));
        }

        public void UpdateRelatedRecords(
            Context context,
            SiteSettings ss,
            bool extendedSqls = false,
            bool get = false,
            string previousTitle = null,
            bool addUpdatedTimeParam = true,
            bool addUpdatorParam = true,
            bool updateItems = true)
        {
            Repository.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: UpdateRelatedRecordsStatements(
                    context: context,
                    ss: ss,
                    extendedSqls: extendedSqls,
                    addUpdatedTimeParam: addUpdatedTimeParam,
                    addUpdatorParam: addUpdatorParam,
                    updateItems: updateItems)
                        .ToArray());
            var titleUpdated = Title_Updated(context: context);
            if (get && Rds.ExtendedSqls(
                context: context,
                siteId: SiteId,
                id: ResultId)
                    ?.Any(o => o.OnUpdated) == true)
            {
                Get(
                    context: context,
                    ss: ss);
            }
            if (previousTitle != null
                && previousTitle != Title.DisplayValue
                && ss.Sources?.Any() == true)
            {
                ItemUtilities.UpdateSourceTitles(
                    context: context,
                    ss: ss,
                    idList: ResultId.ToSingleList());
            }
        }

        public List<SqlStatement> UpdateRelatedRecordsStatements(
            Context context,
            SiteSettings ss,
            bool extendedSqls = false,
            bool addUpdatedTimeParam = true,
            bool addUpdatorParam = true,
            bool updateItems = true)
        {
            var fullText = FullText(context, ss: ss);
            var statements = new List<SqlStatement>();
            statements.Add(Rds.UpdateItems(
                where: Rds.ItemsWhere().ReferenceId(ResultId),
                param: Rds.ItemsParam()
                    .SiteId(SiteId)
                    .Title(Title.DisplayValue)
                    .FullText(fullText, _using: fullText != null)
                    .SearchIndexCreatedTime(DateTime.Now, _using: fullText != null),
                addUpdatedTimeParam: addUpdatedTimeParam,
                addUpdatorParam: addUpdatorParam,
                _using: updateItems));
            statements.Add(Rds.PhysicalDeleteLinks(
                where: Rds.LinksWhere().SourceId(ResultId)));
            statements.Add(InsertLinks(
                context: context,
                ss: ss));
            if (extendedSqls)
            {
                statements.OnUpdatedExtendedSqls(
                    context: context,
                    siteId: SiteId,
                    id: ResultId);
            }
            return statements;
        }

        private SqlInsert InsertLinks(Context context, SiteSettings ss, bool setIdentity = false)
        {
            var link = ss.Links
                ?.Where(o => o.SiteId > 0)
                .Where(o => ss.Destinations.ContainsKey(o.SiteId))
                .Select(o => ss.GetColumn(
                    context: context,
                    columnName: o.ColumnName))
                .Where(o => o != null)
                .SelectMany(column => column.MultipleSelections == true
                    ? GetClass(column).Deserialize<List<long>>()
                        ?? new List<long>()
                    : GetClass(column).ToLong().ToSingleList())
                .Where(id => id > 0)
                .Distinct()
                .ToDictionary(id => id, id => ResultId);
            return LinkUtilities.Insert(link, setIdentity);
        }

        public ErrorData UpdateOrCreate(
            Context context,
            SiteSettings ss,
            string dataTableName = null,
            SqlWhereCollection where = null,
            SqlParamCollection param = null)
        {
            SetBySession(context: context);
            var statements = new List<SqlStatement>
            {
                Rds.InsertItems(
                    dataTableName: dataTableName,
                    selectIdentity: true,
                    param: Rds.ItemsParam()
                        .ReferenceType("Results")
                        .SiteId(SiteId)
                        .Title(Title.DisplayValue)),
                Rds.UpdateOrInsertResults(
                    where: where ?? Rds.ResultsWhereDefault(
                        context: context,
                        resultModel: this),
                    param: param ?? Rds.ResultsParamDefault(
                        context: context,
                        ss: ss,
                        resultModel: this,
                        setDefault: true))
            };
            var response = Repository.ExecuteScalar_response(
                context: context,
                transactional: true,
                selectIdentity: true,
                statements: statements.ToArray());
            ResultId = (response.Id ?? ResultId).ToLong();
            Get(context: context, ss: ss);
            return new ErrorData(type: Error.Types.None);
        }

        public ErrorData Move(Context context, SiteSettings ss, SiteSettings targetSs)
        {
            SiteId = targetSs.SiteId;
            var statements = new List<SqlStatement>();
            var fullText = FullText(
                context: context,
                ss: targetSs);
            statements.AddRange(IfDuplicatedStatements(ss: targetSs));
            statements.AddRange(new List<SqlStatement>
            {
                Rds.UpdateItems(
                    where: Rds.ItemsWhere().ReferenceId(ResultId),
                    param: Rds.ItemsParam()
                        .SiteId(SiteId)
                        .FullText(fullText, _using: fullText != null)),
                Rds.UpdateResults(
                    where: Rds.ResultsWhere().ResultId(ResultId),
                    param: Rds.ResultsParam().SiteId(SiteId))
            });
            var response = Repository.ExecuteScalar_response(
                context: context,
                transactional: true,
                statements: statements.ToArray());
            if (response?.Event == "Duplicated")
            {
                return new ErrorData(
                    type: Error.Types.Duplicated,
                    id: ResultId,
                    columnName: response.ColumnName);
            }
            SynchronizeSummary(
                context: context,
                ss: ss);
            Get(
                context: context,
                ss: targetSs);
            return new ErrorData(type: Error.Types.None);
        }

        public ErrorData Delete(Context context, SiteSettings ss, bool notice = false)
        {
            SetByBeforeDeleteServerScript(
                context: context,
                ss: ss);
            if (context.ErrorData.Type != Error.Types.None)
            {
                return context.ErrorData;
            }
            var notifications = context.ContractSettings.Notice != false && notice
                ? GetNotifications(
                    context: context,
                    ss: ss,
                    notice: notice)
                : null;
            ss.LinkActions(
                context: context,
                type: "DeleteWithLinks",
                sub: Rds.SelectResults(
                    column: Rds.ResultsColumn().ResultId(),
                    where: Rds.ResultsWhere()
                        .SiteId(ss.SiteId)
                        .ResultId(ResultId)));
            var statements = new List<SqlStatement>();
            var where = Rds.ResultsWhere().SiteId(SiteId).ResultId(ResultId);
            statements.OnDeletingExtendedSqls(
                context: context,
                siteId: SiteId,
                id: ResultId);
            statements.AddRange(new List<SqlStatement>
            {
                Rds.DeleteItems(
                    factory: context,
                    where: Rds.ItemsWhere().ReferenceId(ResultId)),
                Rds.DeleteBinaries(
                    factory: context,
                    where: Rds.BinariesWhere()
                        .TenantId(context.TenantId)
                        .ReferenceId(ResultId)),
                Rds.DeleteResults(
                    factory: context,
                    where: where)
            });
            if (Parameters.BinaryStorage.RestoreLocalFiles == false)
            {
                ColumnNames()
                    .Where(columnName => columnName.StartsWith("Attachments"))
                    .ForEach(columnName =>
                    {
                        var attachments = GetAttachments(columnName: columnName);
                        attachments.ForEach(attachment =>
                            attachment.Deleted = true);
                        attachments.Statements(
                            context: context,
                            statements: statements,
                            referenceId: ResultId,
                            ss.GetColumn(
                                context: context,
                                columnName: columnName));
                    });
            }
            statements.OnDeletedExtendedSqls(
                context: context,
                siteId: SiteId,
                id: ResultId);
            Repository.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: statements.ToArray());
            WriteAttachments(
                context: context,
                ss: ss);
            SynchronizeSummary(context, ss);
            if (context.ContractSettings.Notice != false && notice)
            {
                Notice(
                    context: context,
                    ss: ss,
                    notifications: notifications,
                    type: "Deleted");
            }
            SetByAfterDeleteServerScript(
                context: context,
                ss: ss);
            return new ErrorData(type: Error.Types.None);
        }

        public ErrorData Restore(Context context, SiteSettings ss,long resultId)
        {
            ResultId = resultId;
            Repository.ExecuteNonQuery(
                context: context,
                connectionString: Parameters.Rds.OwnerConnectionString,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.RestoreItems(
                        factory: context,
                        where: Rds.ItemsWhere().ReferenceId(ResultId)),
                    Rds.RestoreResults(
                        factory: context,
                        where: Rds.ResultsWhere().ResultId(ResultId))
                });
            return new ErrorData(type: Error.Types.None);
        }

        public ErrorData PhysicalDelete(
            Context context, SiteSettings ss,Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            Repository.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: Rds.PhysicalDeleteResults(
                    tableType: tableType,
                    param: Rds.ResultsParam().SiteId(SiteId).ResultId(ResultId)));
            return new ErrorData(type: Error.Types.None);
        }

        private List<SqlStatement> IfDuplicatedStatements(SiteSettings ss)
        {
            var statements = new List<SqlStatement>();
            var param = new Rds.ResultsParamCollection();
            ss.Columns
                .Where(column => column.NoDuplication == true)
                .ForEach(column =>
                {
                    switch (column.ColumnName)
                    {
                        case "Title":
                            if (Title.Value != string.Empty)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.Title(Title.Value.MaxLength(1024)), SiteId, ResultId));
                            break;
                        case "Body":
                            if (Body != string.Empty)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.Body(Body), SiteId, ResultId));
                            break;
                        case "Status":
                            if (Status.Value != 0)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.Status(Status.Value), SiteId, ResultId));
                            break;
                        case "Manager":
                            if (Manager.Id != 0)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.Manager(Manager.Id), SiteId, ResultId));
                            break;
                        case "Owner":
                            if (Owner.Id != 0)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.Owner(Owner.Id), SiteId, ResultId));
                            break;
                        default:
                            switch (Def.ExtendedColumnTypes.Get(column?.ColumnName ?? string.Empty))
                            {
                                case "Class":
                                    if (!GetClass(column: column).IsNullOrEmpty())
                                        statements.Add(column.IfDuplicatedStatement(
                                            param: param.Add(
                                                columnBracket: $"\"{column.ColumnName}\"",
                                                name: column.ColumnName,
                                                value: GetClass(column: column).MaxLength(1024)),
                                            siteId: SiteId,
                                            referenceId: ResultId));
                                    break;
                                case "Num":
                                    var num = GetNum(column: column);
                                    if (column.Nullable == true)
                                    {
                                        if (num?.Value != null)
                                            statements.Add(column.IfDuplicatedStatement(
                                                param: param.Add(
                                                    columnBracket: $"\"{column.ColumnName}\"",
                                                    name: column.ColumnName,
                                                    value: num.Value),
                                                siteId: SiteId,
                                                referenceId: ResultId));
                                    }
                                    else
                                    {
                                        if (num?.Value != null && num?.Value != 0)
                                            statements.Add(column.IfDuplicatedStatement(
                                                param: param.Add(
                                                    columnBracket: $"\"{column.ColumnName}\"",
                                                    name: column.ColumnName,
                                                    value: num.Value),
                                                siteId: SiteId,
                                                referenceId: ResultId));
                                    }
                                    break;
                                case "Date":
                                    if (GetDate(column: column) != 0.ToDateTime())
                                        statements.Add(column.IfDuplicatedStatement(
                                            param: param.Add(
                                                columnBracket: $"\"{column.ColumnName}\"",
                                                name: column.ColumnName,
                                                value: GetDate(column: column)),
                                            siteId: SiteId,
                                            referenceId: ResultId));
                                    break;
                                case "Description":
                                    if (!GetDescription(column: column).IsNullOrEmpty())
                                        statements.Add(column.IfDuplicatedStatement(
                                            param: param.Add(
                                                columnBracket: $"\"{column.ColumnName}\"",
                                                name: column.ColumnName,
                                                value: GetDescription(column: column)),
                                            siteId: SiteId,
                                            referenceId: ResultId));
                                    break;
                            }
                            break;
                    }
                });
            return statements;
        }

        public void SetDefault(Context context, SiteSettings ss)
        {
            ss.Columns
                .Where(o => !o.DefaultInput.IsNullOrEmpty())
                .ForEach(column => SetDefault(context: context, ss: ss, column: column));
        }

        public void SetCopyDefault(Context context, SiteSettings ss)
        {
            ss.Columns
                .Where(column => column.CopyByDefault == true
                    || column.TypeCs == "Attachments"
                    || !column.CanRead(
                        context: context,
                        ss: ss,
                        mine: Mine(context: context)))
                .ForEach(column => SetDefault(
                    context: context,
                    ss: ss,
                    column: column));
        }

        public void SetDefault(Context context, SiteSettings ss, Column column)
        {
            var defaultInput = column.GetDefaultInput(context: context);
            switch (column.ColumnName)
            {
                case "ResultId":
                    ResultId = defaultInput.ToLong();
                    break;
                case "Title":
                    Title.Value = defaultInput.ToString();
                    break;
                case "Body":
                    Body = defaultInput.ToString();
                    break;
                case "Status":
                    Status.Value = defaultInput.ToInt();
                    break;
                case "Locked":
                    Locked = defaultInput.ToBool();
                    break;
                case "Timestamp":
                    Timestamp = defaultInput.ToString();
                    break;
                case "Manager":
                    Manager = SiteInfo.User(
                        context: context,
                        userId: column.GetDefaultInput(context: context).ToInt());
                    break;
                case "Owner":
                    Owner = SiteInfo.User(
                        context: context,
                        userId: column.GetDefaultInput(context: context).ToInt());
                    break;
                default:
                    switch (Def.ExtendedColumnTypes.Get(column?.ColumnName ?? string.Empty))
                    {
                        case "Class":
                            GetClass(
                                column: column,
                                value: defaultInput);
                            break;
                        case "Num":
                            GetNum(
                                column: column,
                                value: new Num(
                                    context: context,
                                    column: column,
                                    value: defaultInput));
                            break;
                        case "Date":
                            GetDate(
                                column: column,
                                value: column.DefaultTime(context: context));
                            break;
                        case "Description":
                            GetDescription(
                                column: column,
                                value: defaultInput.ToString());
                            break;
                        case "Check":
                            GetCheck(
                                column: column,
                                value: defaultInput.ToBool());
                            break;
                        case "Attachments":
                            GetAttachments(
                                column: column,
                                value: new Attachments());
                            break;
                    }
                    break;
            }
        }

        public void SetByForm(
            Context context,
            SiteSettings ss,
            Dictionary<string, string> formData)
        {
            formData.ForEach(data =>
            {
                var key = data.Key;
                var value = data.Value ?? string.Empty;
                switch (key)
                {
                    case "Results_Title": Title = new Title(ResultId, value); break;
                    case "Results_Body": Body = value.ToString(); break;
                    case "Results_Status": Status = new Status(value.ToInt());; break;
                    case "Results_Manager": Manager = SiteInfo.User(context: context, userId: value.ToInt()); break;
                    case "Results_Owner": Owner = SiteInfo.User(context: context, userId: value.ToInt()); break;
                    case "Results_Locked": Locked = value.ToBool(); break;
                    case "Results_Timestamp": Timestamp = value.ToString(); break;
                    case "Comments": Comments.Prepend(
                        context: context,
                        ss: ss,
                        body: value); break;
                    case "VerUp": VerUp = value.ToBool(); break;
                    case "CurrentPermissionsAll":
                        RecordPermissions = context.Forms.List("CurrentPermissionsAll");
                        break;
                    default:
                        if (key.RegexExists("Comment[0-9]+"))
                        {
                            Comments.Update(
                                context: context,
                                ss: ss,
                                commentId: key.Substring("Comment".Length).ToInt(),
                                body: value);
                        }
                        else
                        {
                            var column = ss.GetColumn(
                                context: context,
                                columnName: key.Split_2nd('_'));
                            switch (Def.ExtendedColumnTypes.Get(column?.ColumnName ?? string.Empty))
                            {
                                case "Class":
                                    GetClass(
                                        columnName: column.ColumnName,
                                        value: value);
                                    break;
                                case "Num":
                                    GetNum(
                                        columnName: column.ColumnName,
                                        value: new Num(
                                            context: context,
                                            column: column,
                                            value: value));
                                    break;
                                case "Date":
                                    GetDate(
                                        columnName: column.ColumnName,
                                        value: value.ToDateTime().ToUniversal(context: context));
                                    break;
                                case "Description":
                                    GetDescription(
                                        columnName: column.ColumnName,
                                        value: value);
                                    break;
                                case "Check":
                                    GetCheck(
                                        columnName: column.ColumnName,
                                        value: value.ToBool());
                                    break;
                                case "Attachments":
                                    GetAttachments(
                                        columnName: column.ColumnName,
                                        value: value.Deserialize<Attachments>());
                                    break;
                            }
                        }
                        break;
                }
            });
            if (context.QueryStrings.ContainsKey("ver"))
            {
                Ver = context.QueryStrings.Int("ver");
            }
            var formsSiteId = context.Forms.Long("FromSiteId");
            if (formsSiteId > 0)
            {
                var column = ss.GetColumn(
                    context: context,
                    columnName: ss.Links
                        ?.Where(o => o.SiteId > 0)
                        .FirstOrDefault(o => o.SiteId == formsSiteId).ColumnName);
                if (column != null)
                {
                    var value = PropertyValue(
                        context: context,
                        column: column);
                    column.Linking = column.MultipleSelections == true
                        ? value.Deserialize<List<string>>()?.Contains(context.Forms.Data("LinkId")) == true
                        : value == context.Forms.Data("LinkId");
                }
            }
            var queryStringsSiteId = context.QueryStrings.Long("FromSiteId");
            if (queryStringsSiteId > 0)
            {
                var id = context.QueryStrings.Data("LinkId");
                ss.Links
                    ?.Where(link => link.SiteId == queryStringsSiteId)
                    .Select(link => ss.GetColumn(
                        context: context,
                        columnName: link.ColumnName))
                    .Where(column => column != null)
                    .ForEach(column =>
                    {
                        id = column.MultipleSelections == true
                            ? id.ToSingleList().ToJson()
                            : id;
                        GetClass(column.ColumnName, id);
                        column.ControlCss += " always-send";
                    });
            }
            SetByFormula(context: context, ss: ss);
            SetChoiceHash(context: context, ss: ss);
            if (context.Action == "deletecomment")
            {
                DeleteCommentId = formData.Get("ControlId")?
                    .Split(',')
                    ._2nd()
                    .ToInt() ?? 0;
                Comments.RemoveAll(o => o.CommentId == DeleteCommentId);
            }
        }

        public void SetProcessMatchConditions(
            Context context,
            SiteSettings ss)
        {
            ss.Processes?.ForEach(process =>
                process.MatchConditions = GetProcessMatchConditions(
                    context: context,
                    ss: ss,
                    process: process));
        }

        public bool GetProcessMatchConditions(
            Context context,
            SiteSettings ss,
            Process process)
        {
            return Matched(
                context: context,
                ss: ss,
                view: process.View)
                   && (process.CurrentStatus == -1
                        || Status.Value == process.CurrentStatus);
        }

        public void SetByModel(ResultModel resultModel)
        {
            SiteId = resultModel.SiteId;
            UpdatedTime = resultModel.UpdatedTime;
            Title = resultModel.Title;
            Body = resultModel.Body;
            Status = resultModel.Status;
            Manager = resultModel.Manager;
            Owner = resultModel.Owner;
            Locked = resultModel.Locked;
            Comments = resultModel.Comments;
            Creator = resultModel.Creator;
            Updator = resultModel.Updator;
            CreatedTime = resultModel.CreatedTime;
            VerUp = resultModel.VerUp;
            Comments = resultModel.Comments;
            ClassHash = resultModel.ClassHash;
            NumHash = resultModel.NumHash;
            DateHash = resultModel.DateHash;
            DescriptionHash = resultModel.DescriptionHash;
            CheckHash = resultModel.CheckHash;
            AttachmentsHash = resultModel.AttachmentsHash;
        }

        public void SetByApi(Context context, SiteSettings ss)
        {
            var data = context.RequestDataString.Deserialize<ResultApiModel>();
            if (data == null)
            {
                context.InvalidJsonData = !context.RequestDataString.IsNullOrEmpty();
                return;
            }
            if (data.Title != null) Title = new Title(data.ResultId.ToLong(), data.Title);
            if (data.Body != null) Body = data.Body.ToString().ToString();
            if (data.Status != null) Status = new Status(data.Status.ToInt());;
            if (data.Manager != null) Manager = SiteInfo.User(context: context, userId: data.Manager.ToInt());
            if (data.Owner != null) Owner = SiteInfo.User(context: context, userId: data.Owner.ToInt());
            if (data.Locked != null) Locked = data.Locked.ToBool().ToBool();
            if (data.Comments != null) Comments.Prepend(context: context, ss: ss, body: data.Comments);
            if (data.VerUp != null) VerUp = data.VerUp.ToBool();
            data.ClassHash?.ForEach(o => GetClass(
                columnName: o.Key,
                value: o.Value));
            data.NumHash?.ForEach(o => GetNum(
                columnName: o.Key,
                value: new Num(
                    context: context,
                    column: ss.GetColumn(
                        context: context,
                        columnName: o.Key),
                    value: o.Value.ToString())));
            data.DateHash?.ForEach(o => GetDate(
                columnName: o.Key,
                value: o.Value.ToDateTime().ToUniversal(context: context)));
            data.DescriptionHash?.ForEach(o => GetDescription(
                columnName: o.Key,
                value: o.Value));
            data.CheckHash?.ForEach(o => GetCheck(
                columnName: o.Key,
                value: o.Value));
            data.AttachmentsHash?.ForEach(o =>
            {
                string columnName = o.Key;
                Attachments newAttachments = o.Value;
                Attachments oldAttachments;
                if (columnName == "Attachments#Uploading")
                {
                    var kvp = AttachmentsHash
                        .FirstOrDefault(x => x.Value
                            .Any(att => att.Guid == newAttachments.FirstOrDefault()?.Guid?.Split_1st()));
                    columnName = kvp.Key;
                    oldAttachments = kvp.Value;
                    var column = ss.GetColumn(
                        context: context,
                        columnName: columnName);
                    if (column.OverwriteSameFileName == true)
                    {
                        var oldAtt = oldAttachments
                            .FirstOrDefault(att => att.Guid == newAttachments.FirstOrDefault()?.Guid?.Split_1st());
                        if (oldAtt != null)
                        {
                            oldAtt.Deleted = true;
                            oldAtt.Overwritten = true;
                        }
                    }
                    newAttachments.ForEach(att => att.Guid = att.Guid.Split_2nd());
                }
                else
                {
                    oldAttachments = AttachmentsHash.Get(columnName);
                }
                if (oldAttachments != null)
                {
                    var column = ss.GetColumn(
                        context: context,
                        columnName: columnName);
                    var newGuidSet = new HashSet<string>(newAttachments.Select(x => x.Guid).Distinct());
                    var newNameSet = new HashSet<string>(newAttachments.Select(x => x.Name).Distinct());
                    newAttachments.ForEach(newAttachment =>
                    {
                        newAttachment.AttachmentAction(
                            context: context,
                            column: column,
                            oldAttachments: oldAttachments);
                    });
                    if (column.OverwriteSameFileName == true)
                    {
                        newAttachments.AddRange(oldAttachments.
                            Where((oldvalue) =>
                                !newGuidSet.Contains(oldvalue.Guid) &&
                                !newNameSet.Contains(oldvalue.Name)));
                    }
                    else
                    {
                        newAttachments.AddRange(oldAttachments.
                            Where((oldvalue) => !newGuidSet.Contains(oldvalue.Guid)));
                    }
                }
                GetAttachments(columnName: columnName, value: newAttachments);
            });
            RecordPermissions = data.RecordPermissions;
            SetByFormula(context: context, ss: ss);
            SetChoiceHash(context: context, ss: ss);
        }

        public void SetByLookups(Context context, SiteSettings ss, Dictionary<string,string> requestFormData = null)
        {
            var formData = new Dictionary<string, string>();
            ss.Links
                .Where(link => link.Lookups?.Any() == true)
                .Where(link => PropertyUpdated(
                    context: context,
                    name: link.ColumnName))
                .ForEach(link => link.Lookups.LookupData(
                    context: context,
                    ss: ss,
                    link: link,
                    id: GetClass(link.ColumnName).ToLong())
                        .Where(data => requestFormData == null
                            || !requestFormData.ContainsKey(data.Key))
                        .ForEach(data =>
                            formData.AddOrUpdate(data.Key, data.Value)));
            if (formData.Any())
            {
                SetByForm(
                    context: context,
                    ss: ss,
                    formData: formData);
            }
        }

        public void SynchronizeSummary(Context context, SiteSettings ss, bool force = false)
        {
            ss.Summaries.ForEach(summary =>
            {
                var id = SynchronizeSummaryDestinationId(linkColumn: summary.LinkColumn);
                var savedId = SynchronizeSummaryDestinationId(
                    linkColumn: summary.LinkColumn,
                    saved: true);
                if (id != 0)
                {
                    SynchronizeSummary(
                        context: context,
                        ss: ss,
                        summary: summary,
                        id: id);
                }
                if (savedId != 0 && id != savedId)
                {
                    SynchronizeSummary(
                        context: context,
                        ss: ss,
                        summary: summary,
                        id: savedId);
                }
            });
            SynchronizeSourceSummary(
                context: context,
                ss: ss,
                force: force);
        }

        private void SynchronizeSummary(
            Context context, SiteSettings ss, Summary summary, long id)
        {
            var destinationSs = SiteSettingsUtilities.Get(
                context: context, siteId: summary.SiteId);
            if (destinationSs != null)
            {
                Summaries.Synchronize(
                    context: context,
                    ss: ss,
                    destinationSs: destinationSs,
                    destinationSiteId: summary.SiteId,
                    destinationColumn: summary.DestinationColumn,
                    destinationCondition: destinationSs.Views?.Get(summary.DestinationCondition),
                    setZeroWhenOutOfCondition: summary.SetZeroWhenOutOfCondition == true,
                    sourceSiteId: SiteId,
                    sourceReferenceType: "Results",
                    linkColumn: summary.LinkColumn,
                    type: summary.Type,
                    sourceColumn: summary.SourceColumn,
                    sourceCondition: ss.Views?.Get(summary.SourceCondition),
                    id: id);
            }
        }

        private void SynchronizeSourceSummary(
            Context context, SiteSettings ss, bool force = false)
        {
            ss.Sources.Values.ForEach(sourceSs =>
                sourceSs.Summaries
                    .Where(o => ss.Views?.Get(o.DestinationCondition) != null || force)
                    .ForEach(summary =>
                        Summaries.Synchronize(
                            context: context,
                            ss: sourceSs,
                            destinationSs: ss,
                            destinationSiteId: summary.SiteId,
                            destinationColumn: summary.DestinationColumn,
                            destinationCondition: ss.Views?.Get(summary.DestinationCondition),
                            setZeroWhenOutOfCondition: summary.SetZeroWhenOutOfCondition == true,
                            sourceSiteId: sourceSs.SiteId,
                            sourceReferenceType: sourceSs.ReferenceType,
                            linkColumn: summary.LinkColumn,
                            type: summary.Type,
                            sourceColumn: summary.SourceColumn,
                            sourceCondition: sourceSs.Views?.Get(summary.SourceCondition),
                            id: ResultId)));
        }

        private long SynchronizeSummaryDestinationId(string linkColumn, bool saved = false)
        {
            return saved
                ? GetSavedClass(linkColumn).ToLong()
                : GetClass(linkColumn).ToLong();
        }

        public void UpdateFormulaColumns(
            Context context, SiteSettings ss, IEnumerable<int> selected = null)
        {
            SetByFormula(context: context, ss: ss);
            var param = Rds.ResultsParam();
            ss.Formulas?
                .Where(o => selected == null || selected.Contains(o.Id))
                .ForEach(formulaSet =>
                {
                    switch (formulaSet.Target)
                    {
                        default:
                            if (Def.ExtendedColumnTypes.ContainsKey(formulaSet.Target ?? string.Empty))
                            {
                                param.Add(
                                    columnBracket: $"\"{formulaSet.Target}\"",
                                    name: formulaSet.Target,
                                    value: GetNum(formulaSet.Target).Value);
                            }
                            break;
                    }
                });
            Repository.ExecuteNonQuery(
                context: context,
                statements: Rds.UpdateResults(
                    param: param,
                    where: Rds.ResultsWhereDefault(
                        context: context,
                        resultModel: this),
                    addUpdatedTimeParam: false,
                    addUpdatorParam: false));
        }

        public void SetByFormula(Context context, SiteSettings ss)
        {
            SetByBeforeFormulaServerScript(
                context: context,
                ss: ss);
            ss.Formulas?.ForEach(formulaSet =>
            {
                var columnName = formulaSet.Target;
                var formula = formulaSet.Formula;
                var view = ss.Views?.Get(formulaSet.Condition);
                if (view != null && !Matched(context: context, ss: ss, view: view))
                {
                    if (formulaSet.OutOfCondition != null)
                    {
                        formula = formulaSet.OutOfCondition;
                    }
                    else
                    {
                        return;
                    }
                }
                var data = new Dictionary<string, decimal>
                {
                };
                data.AddRange(NumHash.ToDictionary(
                    o => o.Key,
                    o => o.Value?.Value?.ToDecimal() ?? 0));
                var value = formula?.GetResult(
                    data: data,
                    column: ss.GetColumn(
                        context: context,
                        columnName: columnName)) ?? 0;
                switch (columnName)
                {
                    default:
                        GetNum(
                            columnName: columnName,
                            value: new Num(value));
                        break;
                }
                if (ss.OutputFormulaLogs == true)
                {
                    context.LogBuilder?.AppendLine($"formulaSet: {formulaSet.GetRecordingData().ToJson()}");
                    context.LogBuilder?.AppendLine($"formulaSource: {data.ToJson()}");
                    context.LogBuilder?.AppendLine($"formulaResult: {{\"{columnName}\":{value}}}");
                }
            });
            SetByAfterFormulaServerScript(
                context: context,
                ss: ss);
        }

        public void SetTitle(Context context, SiteSettings ss)
        {
            if (Title?.ItemTitle != true)
            {
                Title = new Title(
                    context: context,
                    ss: ss,
                    id: ResultId,
                    ver: Ver,
                    isHistory: VerType == Versions.VerTypes.History,
                    data: PropertyValues(
                        context: context,
                        columns: ss.GetTitleColumns(context: context)));
            }
        }

        private bool Matched(Context context, SiteSettings ss, View view)
        {
            var userId = context.UserId;
            if (view.Own == true && !(Manager.Id == userId || Owner.Id == userId))
            {
                return false;
            }
            if (view.ColumnFilterHash != null)
            {
                foreach (var filter in view.ColumnFilterHash)
                {
                    var match = true;
                    var column = ss.GetColumn(context: context, columnName: filter.Key);
                    switch (filter.Key)
                    {
                        case "UpdatedTime":
                            match = UpdatedTime.Value.Matched(
                                column: column,
                                condition: filter.Value);
                            break;
                        case "ResultId":
                            match = ResultId.Matched(
                                column: column,
                                condition: filter.Value);
                            break;
                        case "Ver":
                            match = Ver.Matched(
                                column: column,
                                condition: filter.Value);
                            break;
                        case "Title":
                            match = Title.Value.Matched(
                                column: column,
                                condition: filter.Value);
                            break;
                        case "Body":
                            match = Body.Matched(
                                column: column,
                                condition: filter.Value);
                            break;
                        case "Status":
                            match = Status.Value.Matched(
                                column: column,
                                condition: filter.Value);
                            break;
                        case "Manager":
                            match = Manager.Id.Matched(
                                column: column,
                                condition: filter.Value);
                            break;
                        case "Owner":
                            match = Owner.Id.Matched(
                                column: column,
                                condition: filter.Value);
                            break;
                        case "Locked":
                            match = Locked.Matched(
                                column: column,
                                condition: filter.Value);
                            break;
                        case "SiteTitle":
                            match = SiteTitle.SiteId.Matched(
                                column: column,
                                condition: filter.Value);
                            break;
                        case "Creator":
                            match = Creator.Id.Matched(
                                column: column,
                                condition: filter.Value);
                            break;
                        case "Updator":
                            match = Updator.Id.Matched(
                                column: column,
                                condition: filter.Value);
                            break;
                        case "CreatedTime":
                            match = CreatedTime.Value.Matched(
                                column: column,
                                condition: filter.Value);
                            break;
                        default:
                            switch (Def.ExtendedColumnTypes.Get(filter.Key ?? string.Empty))
                            {
                                case "Class":
                                    match = GetClass(column: column).Matched(
                                        column: column,
                                        condition: filter.Value);
                                    break;
                                case "Num":
                                    match = GetNum(column: column).Matched(
                                        column: column,
                                        condition: filter.Value);
                                    break;
                                case "Date":
                                    match = GetDate(column: column).Matched(
                                        column: column,
                                        condition: filter.Value);
                                    break;
                                case "Description":
                                    match = GetDescription(column: column).Matched(
                                        column: column,
                                        condition: filter.Value);
                                    break;
                                case "Check":
                                    match = GetCheck(column: column).Matched(
                                        column: column,
                                        condition: filter.Value);
                                    break;
                            }
                            break;
                    }
                    if (!match) return false;
                }
            }
            return true;
        }

        public string ReplacedDisplayValues(
            Context context,
            SiteSettings ss,
            string value)
        {
            ss.IncludedColumns(value: value).ForEach(column =>
                value = value.Replace(
                    $"[{column.ColumnName}]",
                    ToDisplay(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: Mine(context: context))));
            value = ReplacedContextValues(context, value);
            return value;
        }

        private string ReplacedContextValues(Context context, string value)
        {
            var url = Locations.ItemEditAbsoluteUri(
                context: context,
                id: ResultId);
            var mailAddress = MailAddressUtilities.Get(
                context: context,
                userId: context.UserId);
            value = value
                .Replace("{Url}", url)
                .Replace("{LoginId}", context.User.LoginId)
                .Replace("{UserName}", context.User.Name)
                .Replace("{MailAddress}", mailAddress);
            return value;
        }

        public List<Notification> GetNotifications(
            Context context,
            SiteSettings ss,
            bool notice,
            bool before = false,
            Sqls.TableTypes tableTypes = Sqls.TableTypes.Normal)
        {
            if (context.ContractSettings.Notice == false || !notice)
            {
                return null;
            }
            var notifications = NotificationUtilities.Get(
                context: context,
                ss: ss);
            if (notifications?.Any() == true)
            {
                var dataSet = Repository.ExecuteDataSet(
                    context: context,
                    statements: notifications.Select(notification =>
                    {
                        var where = ss.Views?.Get(before
                            ? notification.BeforeCondition
                            : notification.AfterCondition)
                                ?.Where(
                                    context: context,
                                    ss: ss,
                                    where: Rds.ResultsWhere().ResultId(ResultId))
                                        ?? Rds.ResultsWhere().ResultId(ResultId);
                        return Rds.SelectResults(
                            dataTableName: notification.Index.ToString(),
                            tableType: tableTypes,
                            column: Rds.ResultsColumn().ResultId(),
                            join: ss.Join(
                                context: context,
                                join: where),
                            where: where);
                    }).ToArray());
                return notifications
                    .Where(notification =>
                        dataSet.Tables[notification.Index.ToString()].Rows.Count == 1)
                    .ToList();
            }
            else
            {
                return null;
            }
        }

        public void Notice(
            Context context,
            SiteSettings ss,
            List<Notification> notifications,
            string type)
        {
            notifications?.ForEach(notification =>
            {
                if (notification.HasRelatedUsers())
                {
                    var users = new List<int>();
                    Repository.ExecuteTable(
                        context: context,
                        statements: Rds.SelectResults(
                            tableType: Sqls.TableTypes.All,
                            distinct: true,
                            column: Rds.ResultsColumn()
                                .Manager()
                                .Owner()
                                .Creator()
                                .Updator(),
                            where: Rds.ResultsWhere().ResultId(ResultId)))
                                .AsEnumerable()
                                .ForEach(dataRow =>
                                {
                                    users.Add(dataRow.Int("Manager"));
                                    users.Add(dataRow.Int("Owner"));
                                    users.Add(dataRow.Int("Creator"));
                                    users.Add(dataRow.Int("Updator"));
                                });
                    notification.ReplaceRelatedUsers(
                        context: context,
                        users: users);
                }
                var values = ss.IncludedColumns(notification.Address)
                    .ToDictionary(
                        column => column,
                        column => PropertyValue(
                            context: context,
                            column: column));
                switch (type)
                {
                    case "Created":
                    case "Copied":
                        if ((type == "Created" && notification.AfterCreate != false)
                            || (type == "Copied" && notification.AfterCopy != false))
                        {
                            notification.Send(
                                context: context,
                                ss: ss,
                                title: Displays.Created(
                                    context: context,
                                    data: Title.DisplayValue).ToString(),
                                body: NoticeBody(
                                    context: context,
                                    ss: ss,
                                    notification: notification),
                                values: values);
                        }
                        break;
                    case "Updated":
                        if (notification.AfterUpdate != false
                            && notification.MonitorChangesColumns.Any(columnName => PropertyUpdated(
                                context: context,
                                name: columnName)))
                        {
                            var body = NoticeBody(
                                context: context,
                                ss: ss,
                                notification: notification,
                                update: true);
                            notification.Send(
                                context: context,
                                ss: ss,
                                title: Displays.Updated(
                                    context: context,
                                    data: Title.DisplayValue).ToString(),
                                body: body,
                                values: values);
                        }
                        break;
                    case "Deleted":
                        if (notification.AfterDelete != false)
                        {
                            notification.Send(
                                context: context,
                                ss: ss,
                                title: Displays.Deleted(
                                    context: context,
                                    data: Title.DisplayValue).ToString(),
                                body: NoticeBody(
                                    context: context,
                                    ss: ss,
                                    notification: notification),
                                values: values);
                        }
                        break;
                }
            });
        }

        private string NoticeBody(
            Context context,
            SiteSettings ss,
            Notification notification,
            bool update = false)
        {
            var body = new System.Text.StringBuilder();
            notification.GetFormat(
                context: context,
                ss: ss)
                    .Split('\n')
                    .Select(line => new
                    {
                        Line = line.Trim(),
                        Format = line.Trim().Deserialize<NotificationColumnFormat>()
                    })
                    .ForEach(data =>
                    {
                        var column = ss.IncludedColumns(data.Format?.Name)?.FirstOrDefault();
                        if (column == null)
                        {
                            body.Append(ReplacedContextValues(
                                context: context,
                                value: data.Line));
                            body.Append("\n");
                        }
                        else
                        {
                            switch (column.Name)
                            {
                                case "Title":
                                    body.Append(Title.ToNotice(
                                        context: context,
                                        saved: SavedTitle,
                                        column: column,
                                        notificationColumnFormat: data.Format,
                                        updated: Title_Updated(context: context),
                                        update: update));
                                    break;
                                case "Body":
                                    body.Append(Body.ToNotice(
                                        context: context,
                                        saved: SavedBody,
                                        column: column,
                                        notificationColumnFormat: data.Format,
                                        updated: Body_Updated(context: context),
                                        update: update));
                                    break;
                                case "Status":
                                    body.Append(Status.ToNotice(
                                        context: context,
                                        saved: SavedStatus,
                                        column: column,
                                        notificationColumnFormat: data.Format,
                                        updated: Status_Updated(context: context),
                                        update: update));
                                    break;
                                case "Manager":
                                    body.Append(Manager.ToNotice(
                                        context: context,
                                        saved: SavedManager,
                                        column: column,
                                        notificationColumnFormat: data.Format,
                                        updated: Manager_Updated(context: context),
                                        update: update));
                                    break;
                                case "Owner":
                                    body.Append(Owner.ToNotice(
                                        context: context,
                                        saved: SavedOwner,
                                        column: column,
                                        notificationColumnFormat: data.Format,
                                        updated: Owner_Updated(context: context),
                                        update: update));
                                    break;
                                case "Locked":
                                    body.Append(Locked.ToNotice(
                                        context: context,
                                        saved: SavedLocked,
                                        column: column,
                                        notificationColumnFormat: data.Format,
                                        updated: Locked_Updated(context: context),
                                        update: update));
                                    break;
                                case "Comments":
                                    body.Append(Comments.ToNotice(
                                        context: context,
                                        saved: SavedComments,
                                        column: column,
                                        notificationColumnFormat: data.Format,
                                        updated: Comments_Updated(context: context),
                                        update: update));
                                    break;
                                case "Creator":
                                    body.Append(Creator.ToNotice(
                                        context: context,
                                        saved: SavedCreator,
                                        column: column,
                                        notificationColumnFormat: data.Format,
                                        updated: Creator_Updated(context: context),
                                        update: update));
                                    break;
                                case "Updator":
                                    body.Append(Updator.ToNotice(
                                        context: context,
                                        saved: SavedUpdator,
                                        column: column,
                                        notificationColumnFormat: data.Format,
                                        updated: Updator_Updated(context: context),
                                        update: update));
                                    break;
                                default:
                                    switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                                    {
                                        case "Class":
                                            body.Append(GetClass(columnName: column.Name).ToNotice(
                                                context: context,
                                                saved: GetSavedClass(columnName: column.Name),
                                                column: column,
                                                notificationColumnFormat: data.Format,
                                                updated: Class_Updated(columnName: column.Name),
                                                update: update));
                                            break;
                                        case "Num":
                                            body.Append(GetNum(columnName: column.Name).ToNotice(
                                                context: context,
                                                saved: GetSavedNum(columnName: column.Name),
                                                column: column,
                                                notificationColumnFormat: data.Format,
                                                updated: Num_Updated(columnName: column.Name),
                                                update: update));
                                            break;
                                        case "Date":
                                            body.Append(GetDate(columnName: column.Name).ToNotice(
                                                context: context,
                                                saved: GetSavedDate(columnName: column.Name),
                                                column: column,
                                                notificationColumnFormat: data.Format,
                                                updated: Date_Updated(columnName: column.Name),
                                                update: update));
                                            break;
                                        case "Description":
                                            body.Append(GetDescription(columnName: column.Name).ToNotice(
                                                context: context,
                                                saved: GetSavedDescription(columnName: column.Name),
                                                column: column,
                                                notificationColumnFormat: data.Format,
                                                updated: Description_Updated(columnName: column.Name),
                                                update: update));
                                            break;
                                        case "Check":
                                            body.Append(GetCheck(columnName: column.Name).ToNotice(
                                                context: context,
                                                saved: GetSavedCheck(columnName: column.Name),
                                                column: column,
                                                notificationColumnFormat: data.Format,
                                                updated: Check_Updated(columnName: column.Name),
                                                update: update));
                                            break;
                                        case "Attachments":
                                            body.Append(GetAttachments(columnName: column.Name).ToNotice(
                                                context: context,
                                                saved: GetSavedAttachments(columnName: column.Name),
                                                column: column,
                                                notificationColumnFormat: data.Format,
                                                updated: Attachments_Updated(columnName: column.Name),
                                                update: update));
                                            break;
                                    }
                                    break;
                            }
                        }
                    });
            return body.ToString();
        }

        private void SetBySession(Context context)
        {
        }

        private void Set(Context context, SiteSettings ss, DataTable dataTable)
        {
            switch (dataTable.Rows.Count)
            {
                case 1: Set(context, ss, dataTable.Rows[0]); break;
                case 0: AccessStatus = Databases.AccessStatuses.NotFound; break;
                default: AccessStatus = Databases.AccessStatuses.Overlap; break;
            }
            SetChoiceHash(context: context, ss: ss);
        }

        public void SetChoiceHash(Context context, SiteSettings ss)
        {
            if (!ss.SetAllChoices)
            {
                ss.GetUseSearchLinks(context: context).ForEach(link =>
                {
                    var column = ss.GetColumn(
                        context: context,
                        columnName: link.ColumnName);
                    var value = PropertyValue(
                        context: context,
                        column: column);
                    if (!value.IsNullOrEmpty() 
                        && column?.ChoiceHash?.Any(o => o.Value.Value == value) != true)
                    {
                        ss.SetChoiceHash(
                            context: context,
                            columnName: column.ColumnName,
                            selectedValues: value.ToSingleList());
                    }
                });
            }
            SetTitle(context: context, ss: ss);
        }

        private void Set(Context context, SiteSettings ss, DataRow dataRow, string tableAlias = null)
        {
            AccessStatus = Databases.AccessStatuses.Selected;
            foreach (DataColumn dataColumn in dataRow.Table.Columns)
            {
                var column = new ColumnNameInfo(dataColumn.ColumnName);
                if (column.TableAlias == tableAlias)
                {
                    switch (column.Name)
                    {
                        case "SiteId":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                SiteId = dataRow[column.ColumnName].ToLong();
                                SavedSiteId = SiteId;
                            }
                            break;
                        case "UpdatedTime":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                UpdatedTime = new Time(context, dataRow, column.ColumnName); Timestamp = dataRow.Field<DateTime>(column.ColumnName).ToString("yyyy/M/d H:m:s.fff");
                                SavedUpdatedTime = UpdatedTime.Value;
                            }
                            break;
                        case "ResultId":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                ResultId = dataRow[column.ColumnName].ToLong();
                                SavedResultId = ResultId;
                            }
                            break;
                        case "Ver":
                            Ver = dataRow[column.ColumnName].ToInt();
                            SavedVer = Ver;
                            break;
                        case "Title":
                            Title = new Title(context: context, ss: ss, dataRow: dataRow, column: column);
                            SavedTitle = Title.Value;
                            break;
                        case "Body":
                            Body = dataRow[column.ColumnName].ToString();
                            SavedBody = Body;
                            break;
                        case "Status":
                            Status = new Status(dataRow, column);
                            SavedStatus = Status.Value;
                            break;
                        case "Manager":
                            Manager = SiteInfo.User(context: context, userId: dataRow.Int(column.ColumnName));
                            SavedManager = Manager.Id;
                            break;
                        case "Owner":
                            Owner = SiteInfo.User(context: context, userId: dataRow.Int(column.ColumnName));
                            SavedOwner = Owner.Id;
                            break;
                        case "Locked":
                            Locked = dataRow[column.ColumnName].ToBool();
                            SavedLocked = Locked;
                            break;
                        case "Comments":
                            Comments = dataRow[column.ColumnName].ToString().Deserialize<Comments>() ?? new Comments();
                            SavedComments = Comments.ToJson();
                            break;
                        case "Creator":
                            Creator = SiteInfo.User(context: context, userId: dataRow.Int(column.ColumnName));
                            SavedCreator = Creator.Id;
                            break;
                        case "Updator":
                            Updator = SiteInfo.User(context: context, userId: dataRow.Int(column.ColumnName));
                            SavedUpdator = Updator.Id;
                            break;
                        case "CreatedTime":
                            CreatedTime = new Time(context, dataRow, column.ColumnName);
                            SavedCreatedTime = CreatedTime.Value;
                            break;
                        case "IsHistory":
                            VerType = dataRow.Bool(column.ColumnName)
                                ? Versions.VerTypes.History
                                : Versions.VerTypes.Latest; break;
                        default:
                            switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                            {
                                case "Class":
                                    GetClass(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToString());
                                    GetSavedClass(
                                        columnName: column.Name,
                                        value: GetClass(columnName: column.Name));
                                    break;
                                case "Num":
                                    GetNum(
                                        columnName: column.Name,
                                        value: new Num(
                                            dataRow: dataRow,
                                            name: column.ColumnName));
                                    GetSavedNum(
                                        columnName: column.Name,
                                        value: GetNum(columnName: column.Name).Value);
                                    break;
                                case "Date":
                                    GetDate(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToDateTime());
                                    GetSavedDate(
                                        columnName: column.Name,
                                        value: GetDate(columnName: column.Name));
                                    break;
                                case "Description":
                                    GetDescription(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToString());
                                    GetSavedDescription(
                                        columnName: column.Name,
                                        value: GetDescription(columnName: column.Name));
                                    break;
                                case "Check":
                                    GetCheck(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToBool());
                                    GetSavedCheck(
                                        columnName: column.Name,
                                        value: GetCheck(columnName: column.Name));
                                    break;
                                case "Attachments":
                                    GetAttachments(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToString()
                                            .Deserialize<Attachments>() ?? new Attachments());
                                    GetSavedAttachments(
                                        columnName: column.Name,
                                        value: GetAttachments(columnName: column.Name).ToJson());
                                    break;
                            }
                            break;
                    }
                }
            }
            SetTitle(context: context, ss: ss);
            SetByWhenloadingRecordServerScript(
                context: context,
                ss: ss);
        }

        public bool Updated(Context context)
        {
            return Updated()
                || SiteId_Updated(context: context)
                || Ver_Updated(context: context)
                || Title_Updated(context: context)
                || Body_Updated(context: context)
                || Status_Updated(context: context)
                || Manager_Updated(context: context)
                || Owner_Updated(context: context)
                || Locked_Updated(context: context)
                || Comments_Updated(context: context)
                || Creator_Updated(context: context)
                || Updator_Updated(context: context);
        }

        public override List<string> Mine(Context context)
        {
            if (MineCache == null)
            {
                var mine = new List<string>();
                var userId = context.UserId;
                if (SavedManager == userId) mine.Add("Manager");
                if (SavedOwner == userId) mine.Add("Owner");
                if (SavedCreator == userId) mine.Add("Creator");
                if (SavedUpdator == userId) mine.Add("Updator");
                MineCache = mine;
            }
            return MineCache;
        }

        public string IdSuffix()
        {
            return $"_{SiteId}_{(ResultId == 0 ? -1 : ResultId)}";
        }
    }
}
