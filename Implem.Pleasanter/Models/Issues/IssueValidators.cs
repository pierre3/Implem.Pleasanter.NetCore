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
    public static class IssueValidators
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
            if (api && !ss.IsSite(context: context) && !context.CanRead(ss: ss))
            {
                return new ErrorData(type: Error.Types.NotFound);
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
            Context context, SiteSettings ss, IssueModel issueModel, bool api = false)
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
            switch (issueModel.MethodType)
            {
                case BaseModel.MethodTypes.Edit:
                    return
                        context.CanRead(ss: ss)
                        && issueModel.AccessStatus != Databases.AccessStatuses.NotFound
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
            Context context, SiteSettings ss, IssueModel issueModel, bool api = false)
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
            if (ss.LockedTable())
            {
                return new ErrorData(
                    type: Error.Types.LockedTable,
                    data: new string[]
                    {
                        ss.LockedTableUser.Name,
                        ss.LockedTableTime.DisplayValue.ToString(context.CultureInfo())
                    });
            }
            if (!context.CanCreate(ss: ss) || issueModel.ReadOnly)
            {
                return !context.CanRead(ss: ss)
                    ? new ErrorData(type: Error.Types.NotFound)
                    : new ErrorData(type: Error.Types.HasNotPermission);
            }
            foreach (var column in ss.Columns
                .Where(o => !o.CanCreate(
                    context: context,
                    ss: ss,
                    mine: issueModel.Mine(context: context)))
                .Where(o => !ss.FormulaTarget(o.ColumnName))
                .Where(o => !o.Linking))
            {
                switch (column.ColumnName)
                {
                    case "Title":
                        if (issueModel.Title_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Body":
                        if (issueModel.Body_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "WorkValue":
                        if (issueModel.WorkValue_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "ProgressRate":
                        if (issueModel.ProgressRate_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Status":
                        if (issueModel.Status_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Manager":
                        if (issueModel.Manager_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Owner":
                        if (issueModel.Owner_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Locked":
                        if (issueModel.Locked_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "StartTime":
                        if (issueModel.StartTime_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "CompletionTime":
                        if (issueModel.CompletionTime_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Comments":
                        if (issueModel.Comments_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    default:
                        switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                        {
                            case "Class":
                                if (issueModel.Class_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Num":
                                if (issueModel.Num_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Date":
                                if (issueModel.Date_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Description":
                                if (issueModel.Description_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Check":
                                if (issueModel.Check_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Attachments":
                                if (issueModel.Attachments_Updated(
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
            var errorData = OnAttaching(
                context: context,
                ss: ss,
                issueModel: issueModel);
            if (errorData.Type != Error.Types.None)
            {
                return errorData;
            }
            var inputErrorData = OnInputValidating(
                context: context,
                ss: ss,
                issueModel: issueModel).FirstOrDefault();
            if (inputErrorData.Type != Error.Types.None)
            {
                return inputErrorData;
            }
            return new ErrorData(type: Error.Types.None);
        }

        public static ErrorData OnUpdating(
            Context context, SiteSettings ss, IssueModel issueModel, bool api = false)
        {
            if (issueModel.RecordPermissions != null && !context.CanManagePermission(ss: ss))
            {
                return new ErrorData(type: Error.Types.HasNotPermission);
            }
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
            if (ss.LockedTable())
            {
                return new ErrorData(
                    type: Error.Types.LockedTable,
                    data: new string[]
                    {
                        ss.LockedTableUser.Name,
                        ss.LockedTableTime.DisplayValue.ToString(context.CultureInfo())
                    });
            }
            if (ss.LockedRecord())
            {
                return new ErrorData(
                    type: Error.Types.LockedRecord,
                    data: new string[]
                    {
                        issueModel.IssueId.ToString(),
                        ss.LockedRecordUser.Name,
                        ss.LockedRecordTime.DisplayValue.ToString(context.CultureInfo())
                    });
            }
            if (!context.CanUpdate(ss: ss) || issueModel.ReadOnly)
            {
                return !context.CanRead(ss: ss)
                    ? new ErrorData(type: Error.Types.NotFound)
                    : new ErrorData(type: Error.Types.HasNotPermission);
            }
            foreach (var column in ss.Columns
                .Where(o => !o.CanUpdate(
                    context: context,
                    ss: ss,
                    mine: issueModel.Mine(context: context)))
                .Where(o => !ss.FormulaTarget(o.ColumnName)))
            {
                switch (column.ColumnName)
                {
                    case "Title":
                        if (issueModel.Title_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Body":
                        if (issueModel.Body_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "StartTime":
                        if (issueModel.StartTime_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "CompletionTime":
                        if (issueModel.CompletionTime_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "WorkValue":
                        if (issueModel.WorkValue_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "ProgressRate":
                        if (issueModel.ProgressRate_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Status":
                        if (issueModel.Status_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Manager":
                        if (issueModel.Manager_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Owner":
                        if (issueModel.Owner_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Locked":
                        if (issueModel.Locked_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Comments":
                        if (issueModel.Comments_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    default:
                        switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                        {
                            case "Class":
                                if (issueModel.Class_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Num":
                                if (issueModel.Num_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Date":
                                if (issueModel.Date_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Description":
                                if (issueModel.Description_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Check":
                                if (issueModel.Check_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Attachments":
                                if (issueModel.Attachments_Updated(
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
            var errorData = OnAttaching(
                context: context,
                ss: ss,
                issueModel: issueModel);
            if (errorData.Type != Error.Types.None)
            {
                return errorData;
            }
            var inputErrorData = OnInputValidating(
                context: context,
                ss: ss,
                issueModel: issueModel).FirstOrDefault();
            if (inputErrorData.Type != Error.Types.None)
            {
                return inputErrorData;
            }
            return new ErrorData(type: Error.Types.None);
        }

        public static ErrorData OnMoving(
            Context context,
            SiteSettings ss,
            SiteSettings destinationSs,
            IssueModel issueModel)
        {
            if (ss.LockedTable())
            {
                return new ErrorData(
                    type: Error.Types.LockedTable,
                    data: new string[]
                    {
                        ss.LockedTableUser.Name,
                        ss.LockedTableTime.DisplayValue.ToString(context.CultureInfo())
                    });
            }
            if (ss.LockedRecord())
            {
                return new ErrorData(
                    type: Error.Types.LockedRecord,
                    data: new string[]
                    {
                        issueModel.IssueId.ToString(),
                        ss.LockedRecordUser.Name,
                        ss.LockedRecordTime.DisplayValue.ToString(context.CultureInfo())
                    });
            }
            if (!Permissions.CanMove(
                context: context,
                source: ss,
                destination: destinationSs)
                    || issueModel.ReadOnly)
            {
                return new ErrorData(type: Error.Types.HasNotPermission);
            }
            return new ErrorData(type: Error.Types.None);
        }

        public static ErrorData OnDeleting(
            Context context, SiteSettings ss, IssueModel issueModel, bool api = false)
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
            if (ss.LockedTable())
            {
                return new ErrorData(
                    type: Error.Types.LockedTable,
                    data: new string[]
                    {
                        ss.LockedTableUser.Name,
                        ss.LockedTableTime.DisplayValue.ToString(context.CultureInfo())
                    });
            }
            if (ss.LockedRecord())
            {
                return new ErrorData(
                    type: Error.Types.LockedRecord,
                    data: new string[]
                    {
                        issueModel.IssueId.ToString(),
                        ss.LockedRecordUser.Name,
                        ss.LockedRecordTime.DisplayValue.ToString(context.CultureInfo())
                    });
            }
            return context.CanDelete(ss: ss) && !issueModel.ReadOnly
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
            if (ss.LockedTable())
            {
                return new ErrorData(
                    type: Error.Types.LockedTable,
                    data: new string[]
                    {
                        ss.LockedTableUser.Name,
                        ss.LockedTableTime.DisplayValue.ToString(context.CultureInfo())
                    });
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
            if (ss.LockedTable())
            {
                return new ErrorData(
                    type: Error.Types.LockedTable,
                    data: new string[]
                    {
                        ss.LockedTableUser.Name,
                        ss.LockedTableTime.DisplayValue.ToString(context.CultureInfo())
                    });
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

        public static ErrorData OnDeleteHistory(
            Context context,
            SiteSettings ss,
            IssueModel issueModel,
            bool api = false)
        {
            if (!Parameters.History.PhysicalDelete
                || ss.AllowPhysicalDeleteHistories == false)
            {
                return new ErrorData(type: Error.Types.InvalidRequest);
            }
            if (!context.CanManageSite(ss: ss) || issueModel.ReadOnly)
            {
                return new ErrorData(type: Error.Types.HasNotPermission);
            }
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
            if (ss.LockedRecord())
            {
                return new ErrorData(
                    type: Error.Types.LockedRecord,
                    data: new string[]
                    {
                        issueModel.IssueId.ToString(),
                        ss.LockedRecordUser.Name,
                        ss.LockedRecordTime.DisplayValue.ToString(context.CultureInfo())
                    });
            }
            return new ErrorData(type: Error.Types.None);
        }

        public static ErrorData OnUnlockRecord(
            Context context, SiteSettings ss, bool api = false)
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
            if (!ss.LockedRecord())
            {
                return new ErrorData(type: Error.Types.NotLockedRecord);
            }
            if (!context.HasPrivilege && ss.LockedRecordUser.Id != context.UserId)
            {
                return new ErrorData(type: Error.Types.HasNotPermission);
            }
            return new ErrorData(type: Error.Types.None);
        }

        private static ErrorData OnAttaching(
            Context context, SiteSettings ss, IssueModel issueModel)
        {
            foreach (var column in ss.Columns.Where(o => o.TypeCs == "Attachments"))
            {
                if (issueModel.Attachments_Updated(
                    columnName: column.Name,
                    context: context,
                    column: column))
                {
                    var invalid = BinaryValidators.OnUploading(
                        context: context,
                        ss: ss,
                        attachmentsHash: issueModel.AttachmentsHash);
                    switch (invalid)
                    {
                        case Error.Types.OverLimitQuantity:
                            return new ErrorData(
                                type: Error.Types.OverLimitQuantity,
                                data: column.LimitQuantity.ToInt().ToString());
                        case Error.Types.OverLimitSize:
                            return new ErrorData(
                                type: Error.Types.OverLimitSize,
                                data: column.LimitSize.ToInt().ToString());
                        case Error.Types.OverTotalLimitSize:
                            return new ErrorData(
                                type: Error.Types.OverTotalLimitSize,
                                data: column.TotalLimitSize.ToInt().ToString());
                        case Error.Types.OverLocalFolderLimitSize:
                            return new ErrorData(
                                type: Error.Types.OverLocalFolderLimitSize,
                                data: column.LocalFolderLimitSize.ToInt().ToString());
                        case Error.Types.OverLocalFolderTotalLimitSize:
                            return new ErrorData(
                                type: Error.Types.OverLocalFolderTotalLimitSize,
                                data: column.LocalFolderTotalLimitSize.ToInt().ToString());
                        case Error.Types.OverTenantStorageSize:
                            return new ErrorData(
                                type: Error.Types.OverTenantStorageSize,
                                data: context.ContractSettings.StorageSize.ToInt().ToString());
                    }
                }
            }
            return new ErrorData(type: Error.Types.None);
        }

        public static List<ErrorData> OnInputValidating(
            Context context,
            SiteSettings ss,
            Dictionary<int, IssueModel> issueHash)
        {
            var errors = issueHash
                ?.OrderBy(data => data.Key)
                .SelectMany((data, index) => OnInputValidating(
                    context: context,
                    ss: ss,
                    issueModel: data.Value,
                    rowNo: index + 1))
                .Where(data => data.Type != Error.Types.None).ToList()
                    ?? new List<ErrorData>();
            if (errors.Count == 0)
            {
                errors.Add(new ErrorData(type: Error.Types.None));
            }
            return errors;
        }

        private static List<ErrorData> OnInputValidating(
            Context context,
            SiteSettings ss,
            IssueModel issueModel,
            int rowNo = 0)
        {
            var errors = new List<ErrorData>();
            var editorColumns = ss.GetEditorColumns(context: context);
            editorColumns
                ?.Concat(ss
                    .Columns
                    ?.Where(o => !o.NotEditorSettings)
                    .Where(column => !editorColumns
                        .Any(editorColumn => editorColumn.ColumnName == column.ColumnName)))
                .ForEach(column =>
                {
                    var value = issueModel.PropertyValue(
                        context: context,
                        column: column);
                    if (column.TypeCs == "Comments")
                    {
                        var savedCommentId = issueModel
                            .SavedComments
                            ?.Deserialize<Comments>()
                            ?.Max(savedComment => (int?)savedComment.CommentId) ?? default(int);
                        var comment = value
                            ?.Deserialize<Comments>()
                            ?.FirstOrDefault();
                        value = comment?.CommentId > savedCommentId ? comment?.Body : null;
                    }
                    if (!value.IsNullOrEmpty())
                    {
                        Validators.ValidateMaxLength(
                            columnName: column.ColumnName,
                            maxLength: column.MaxLength,
                            errors: errors,
                            value: value);
                        Validators.ValidateRegex(
                            columnName: column.ColumnName,
                            serverRegexValidation: column.ServerRegexValidation,
                            regexValidationMessage: column.RegexValidationMessage,
                            errors: errors,
                            value: value);
                        ss.Processes
                            ?.FirstOrDefault(o => $"Process_{o.Id}" == context.Forms.ControlId())
                            ?.ValidateInputs
                            ?.Where(validateInputs => validateInputs.ColumnName == column.ColumnName)
                            ?.ForEach(validateInputs =>
                                Validators.ValidateRegex(
                                    columnName: validateInputs.ColumnName,
                                    serverRegexValidation: validateInputs.ServerRegexValidation,
                                    regexValidationMessage: validateInputs.RegexValidationMessage,
                                    errors: errors,
                                    value: value));
                    }
                });
            if (errors.Count == 0)
            {
                errors.Add(new ErrorData(type: Error.Types.None));
            }
            return errors;
        }
    }
}
