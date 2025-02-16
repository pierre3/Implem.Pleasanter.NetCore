﻿using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
namespace Implem.Pleasanter.Models
{
    public static class GroupValidators
    {
        /// <summary>
        /// Fixed:
        /// </summary>
        public static ErrorData OnEntry(Context context, SiteSettings ss, bool api = false)
        {
            if (api)
            {
                if ((!Parameters.Api.Enabled
                    || context.ContractSettings.Api == false
                    || context.UserSettings?.AllowApi(context: context) == false))
                {
                    return new ErrorData(type: Error.Types.InvalidRequest);
                }
                if (context.InvalidJsonData)
                {
                    return new ErrorData(type: Error.Types.InvalidJsonData);
                }
                return new ErrorData(type: Error.Types.None);
            }
            return context.UserSettings?.AllowGroupAdministration(context: context) == true
                || Permissions.CanManageTenant(context: context)
                    ? new ErrorData(type: Error.Types.None)
                    : new ErrorData(type: Error.Types.HasNotPermission);
        }

        public static ErrorData OnEditing(
            Context context, SiteSettings ss, GroupModel groupModel, bool api = false)
        {
            if (api)
            {
                if ((!Parameters.Api.Enabled
                    || context.ContractSettings.Api == false
                    || context.UserSettings?.AllowApi(context: context) == false))
                {
                    return new ErrorData(type: Error.Types.InvalidRequest);
                }
                if (context.InvalidJsonData)
                {
                    return new ErrorData(type: Error.Types.InvalidJsonData);
                }
            }
            if (ss.GetNoDisplayIfReadOnly())
            {
                return new ErrorData(type: Error.Types.NotFound);
            }
            switch (groupModel.MethodType)
            {
                case BaseModel.MethodTypes.Edit:
                    return
                        context.CanRead(ss: ss)
                        && groupModel.AccessStatus != Databases.AccessStatuses.NotFound
                            ? new ErrorData(type: Error.Types.None)
                            : new ErrorData(type: Error.Types.NotFound);
                case BaseModel.MethodTypes.New:
                    return context.CanCreate(ss: ss)
                        ? new ErrorData(type: Error.Types.None)
                        : !context.CanRead(ss: ss)
                            ? new ErrorData(type: Error.Types.NotFound)
                            : new ErrorData(type: Error.Types.HasNotPermission);
                default:
                    return new ErrorData(type: Error.Types.NotFound);
            }
        }

        public static ErrorData OnCreating(
            Context context, SiteSettings ss, GroupModel groupModel, bool api = false)
        {
            if (api)
            {
                if ((!Parameters.Api.Enabled
                    || context.ContractSettings.Api == false
                    || context.UserSettings?.AllowApi(context: context) == false))
                {
                    return new ErrorData(type: Error.Types.InvalidRequest);
                }
                if (context.InvalidJsonData)
                {
                    return new ErrorData(type: Error.Types.InvalidJsonData);
                }
            }
            if (!context.CanCreate(ss: ss) || groupModel.ReadOnly)
            {
                return !context.CanRead(ss: ss)
                    ? new ErrorData(type: Error.Types.NotFound)
                    : new ErrorData(type: Error.Types.HasNotPermission);
            }
            foreach (var column in ss.Columns
                .Where(o => !o.CanCreate(
                    context: context,
                    ss: ss,
                    mine: groupModel.Mine(context: context)))
                .Where(o => !ss.FormulaTarget(o.ColumnName))
                .Where(o => !o.Linking))
            {
                switch (column.ColumnName)
                {
                    case "TenantId":
                        if (groupModel.TenantId_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "GroupName":
                        if (groupModel.GroupName_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Body":
                        if (groupModel.Body_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Disabled":
                        if (groupModel.Disabled_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Comments":
                        if (groupModel.Comments_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    default:
                        switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                        {
                            case "Class":
                                if (groupModel.Class_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Num":
                                if (groupModel.Num_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Date":
                                if (groupModel.Date_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Description":
                                if (groupModel.Description_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Check":
                                if (groupModel.Check_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Attachments":
                                if (groupModel.Attachments_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                        }
                        break;
                }
            }
            return new ErrorData(type: Error.Types.None);
        }

        public static ErrorData OnUpdating(
            Context context, SiteSettings ss, GroupModel groupModel, bool api = false)
        {
            if (api)
            {
                if ((!Parameters.Api.Enabled
                    || context.ContractSettings.Api == false
                    || context.UserSettings?.AllowApi(context: context) == false))
                {
                    return new ErrorData(type: Error.Types.InvalidRequest);
                }
                if (context.InvalidJsonData)
                {
                    return new ErrorData(type: Error.Types.InvalidJsonData);
                }
            }
            if (!context.CanUpdate(ss: ss) || groupModel.ReadOnly)
            {
                return !context.CanRead(ss: ss)
                    ? new ErrorData(type: Error.Types.NotFound)
                    : new ErrorData(type: Error.Types.HasNotPermission);
            }
            foreach (var column in ss.Columns
                .Where(o => !o.CanUpdate(
                    context: context,
                    ss: ss,
                    mine: groupModel.Mine(context: context)))
                .Where(o => !ss.FormulaTarget(o.ColumnName)))
            {
                switch (column.ColumnName)
                {
                    case "TenantId":
                        if (groupModel.TenantId_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "GroupName":
                        if (groupModel.GroupName_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Body":
                        if (groupModel.Body_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Disabled":
                        if (groupModel.Disabled_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Comments":
                        if (groupModel.Comments_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    default:
                        switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                        {
                            case "Class":
                                if (groupModel.Class_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Num":
                                if (groupModel.Num_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Date":
                                if (groupModel.Date_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Description":
                                if (groupModel.Description_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Check":
                                if (groupModel.Check_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Attachments":
                                if (groupModel.Attachments_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                        }
                        break;
                }
            }
            return new ErrorData(type: Error.Types.None);
        }

        public static ErrorData OnDeleting(
            Context context, SiteSettings ss, GroupModel groupModel, bool api = false)
        {
            if (api)
            {
                if ((!Parameters.Api.Enabled
                    || context.ContractSettings.Api == false
                    || context.UserSettings?.AllowApi(context: context) == false))
                {
                    return new ErrorData(type: Error.Types.InvalidRequest);
                }
                if (context.InvalidJsonData)
                {
                    return new ErrorData(type: Error.Types.InvalidJsonData);
                }
            }
            return context.CanDelete(ss: ss) && !groupModel.ReadOnly
                ? new ErrorData(type: Error.Types.None)
                : !context.CanRead(ss: ss)
                    ? new ErrorData(type: Error.Types.NotFound)
                    : new ErrorData(type: Error.Types.HasNotPermission);
        }

        public static ErrorData OnRestoring(Context context, SiteSettings ss, bool api = false)
        {
            if (api)
            {
                if ((!Parameters.Api.Enabled
                    || context.ContractSettings.Api == false
                    || context.UserSettings?.AllowApi(context: context) == false))
                {
                    return new ErrorData(type: Error.Types.InvalidRequest);
                }
                if (context.InvalidJsonData)
                {
                    return new ErrorData(type: Error.Types.InvalidJsonData);
                }
            }
            return Permissions.CanManageTenant(context: context)
                ? new ErrorData(type: Error.Types.None)
                : new ErrorData(type: Error.Types.HasNotPermission);
        }

        public static ErrorData OnImporting(Context context, SiteSettings ss, bool api = false)
        {
            if (api)
            {
                if ((!Parameters.Api.Enabled
                    || context.ContractSettings.Api == false
                    || context.UserSettings?.AllowApi(context: context) == false))
                {
                    return new ErrorData(type: Error.Types.InvalidRequest);
                }
                if (context.InvalidJsonData)
                {
                    return new ErrorData(type: Error.Types.InvalidJsonData);
                }
            }
            return context.CanImport(ss: ss)
                ? new ErrorData(type: Error.Types.None)
                : !context.CanRead(ss: ss)
                    ? new ErrorData(type: Error.Types.NotFound)
                    : new ErrorData(type: Error.Types.HasNotPermission);
        }

        public static ErrorData OnExporting(Context context, SiteSettings ss, bool api = false)
        {
            if (api)
            {
                if ((!Parameters.Api.Enabled
                    || context.ContractSettings.Api == false
                    || context.UserSettings?.AllowApi(context: context) == false))
                {
                    return new ErrorData(type: Error.Types.InvalidRequest);
                }
                if (context.InvalidJsonData)
                {
                    return new ErrorData(type: Error.Types.InvalidJsonData);
                }
            }
            return context.CanExport(ss: ss)
                ? new ErrorData(type: Error.Types.None)
                : !context.CanRead(ss: ss)
                    ? new ErrorData(type: Error.Types.NotFound)
                    : new ErrorData(type: Error.Types.HasNotPermission);
        }
    }
}
