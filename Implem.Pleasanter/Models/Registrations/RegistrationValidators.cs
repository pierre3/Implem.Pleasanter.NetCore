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
    public static class RegistrationValidators
    {
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
            }
            if (!api && ss.GetNoDisplayIfReadOnly())
            {
                return new ErrorData(type: Error.Types.NotFound);
            }
            return context.HasPermission(ss: ss)
                ? new ErrorData(type: Error.Types.None)
                : !context.CanRead(ss: ss)
                    ? new ErrorData(type: Error.Types.NotFound)
                    : new ErrorData(type: Error.Types.HasNotPermission);
        }

        public static ErrorData OnEditing(
            Context context, SiteSettings ss, RegistrationModel registrationModel, bool api = false)
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
            switch (registrationModel.MethodType)
            {
                case BaseModel.MethodTypes.Edit:
                    return
                        context.CanRead(ss: ss)
                        && registrationModel.AccessStatus != Databases.AccessStatuses.NotFound
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
            Context context, SiteSettings ss, RegistrationModel registrationModel, bool api = false)
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
            if (!context.CanCreate(ss: ss) || registrationModel.ReadOnly)
            {
                return !context.CanRead(ss: ss)
                    ? new ErrorData(type: Error.Types.NotFound)
                    : new ErrorData(type: Error.Types.HasNotPermission);
            }
            foreach (var column in ss.Columns
                .Where(o => !o.CanCreate(
                    context: context,
                    ss: ss,
                    mine: registrationModel.Mine(context: context)))
                .Where(o => !ss.FormulaTarget(o.ColumnName))
                .Where(o => !o.Linking))
            {
                switch (column.ColumnName)
                {
                    case "MailAddress":
                        if (registrationModel.MailAddress_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Invitee":
                        if (registrationModel.Invitee_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "InviteeName":
                        if (registrationModel.InviteeName_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "LoginId":
                        if (registrationModel.LoginId_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Name":
                        if (registrationModel.Name_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Password":
                        if (registrationModel.Password_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Language":
                        if (registrationModel.Language_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Passphrase":
                        if (registrationModel.Passphrase_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Invitingflg":
                        if (registrationModel.Invitingflg_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "UserId":
                        if (registrationModel.UserId_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "DeptId":
                        if (registrationModel.DeptId_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "GroupId":
                        if (registrationModel.GroupId_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Comments":
                        if (registrationModel.Comments_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    default:
                        switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                        {
                            case "Class":
                                if (registrationModel.Class_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Num":
                                if (registrationModel.Num_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Date":
                                if (registrationModel.Date_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Description":
                                if (registrationModel.Description_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Check":
                                if (registrationModel.Check_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Attachments":
                                if (registrationModel.Attachments_Updated(
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
            Context context, SiteSettings ss, RegistrationModel registrationModel, bool api = false)
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
            if (!context.CanUpdate(ss: ss) || registrationModel.ReadOnly)
            {
                return !context.CanRead(ss: ss)
                    ? new ErrorData(type: Error.Types.NotFound)
                    : new ErrorData(type: Error.Types.HasNotPermission);
            }
            foreach (var column in ss.Columns
                .Where(o => !o.CanUpdate(
                    context: context,
                    ss: ss,
                    mine: registrationModel.Mine(context: context)))
                .Where(o => !ss.FormulaTarget(o.ColumnName)))
            {
                switch (column.ColumnName)
                {
                    case "MailAddress":
                        if (registrationModel.MailAddress_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Invitee":
                        if (registrationModel.Invitee_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "InviteeName":
                        if (registrationModel.InviteeName_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "LoginId":
                        if (registrationModel.LoginId_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Name":
                        if (registrationModel.Name_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Password":
                        if (registrationModel.Password_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Language":
                        if (registrationModel.Language_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Passphrase":
                        if (registrationModel.Passphrase_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Invitingflg":
                        if (registrationModel.Invitingflg_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "UserId":
                        if (registrationModel.UserId_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "DeptId":
                        if (registrationModel.DeptId_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "GroupId":
                        if (registrationModel.GroupId_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Comments":
                        if (registrationModel.Comments_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    default:
                        switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                        {
                            case "Class":
                                if (registrationModel.Class_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Num":
                                if (registrationModel.Num_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Date":
                                if (registrationModel.Date_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Description":
                                if (registrationModel.Description_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Check":
                                if (registrationModel.Check_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Attachments":
                                if (registrationModel.Attachments_Updated(
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
            Context context, SiteSettings ss, RegistrationModel registrationModel, bool api = false)
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
            return context.CanDelete(ss: ss) && !registrationModel.ReadOnly
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

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ErrorData OnEntry(Context context, SiteSettings ss)
        {
            return context.CanRead(ss: ss)
                ? new ErrorData(type: Error.Types.None)
                : new ErrorData(type: Error.Types.HasNotPermission);
        }
    }
}
