﻿using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Extensions;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.ServerScripts;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using static Implem.Pleasanter.Libraries.ServerScripts.ServerScriptModel;
namespace Implem.Pleasanter.Models
{
    public class BaseModel
    {
        public enum MethodTypes
        {
            NotSet,
            Index,
            New,
            Edit
        }

        public Context Context;
        public FormData FormData;
        public Databases.AccessStatuses AccessStatus = Databases.AccessStatuses.Initialized;
        public MethodTypes MethodType = MethodTypes.NotSet;
        public Versions.VerTypes VerType = Versions.VerTypes.Latest;
        public int Ver = 1;
        public Comments Comments = new Comments();
        public User Creator = new User();
        public User Updator = new User();
        public Time CreatedTime = null;
        public Time UpdatedTime = null;
        public bool VerUp = false;
        public string Timestamp = string.Empty;
        public int DeleteCommentId;
        public int SavedVer = 1;
        public int SavedCreator = 0;
        public int SavedUpdator = 0;
        public DateTime SavedCreatedTime = 0.ToDateTime();
        public DateTime SavedUpdatedTime = 0.ToDateTime();
        public bool SavedVerUp = false;
        public string SavedTimestamp = string.Empty;
        public string SavedComments = "[]";
        public ServerScriptModelRow ServerScriptModelRow = new ServerScriptModelRow();

        public bool Ver_Updated(Context context, Column column = null)
        {
            return Ver != SavedVer &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToInt() != Ver);
        }

        public bool Comments_Updated(Context context, Column column = null)
        {
            return Comments.ToJson() != SavedComments && Comments.ToJson() != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != Comments.ToJson());
        }

        public bool Creator_Updated(Context context, Column column = null)
        {
            return Creator.Id != SavedCreator &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToInt() != Creator.Id);
        }

        public bool Updator_Updated(Context context, Column column = null)
        {
            return Updator.Id != SavedUpdator &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToInt() != Updator.Id);
        }

        public Dictionary<string, string> ClassHash = new Dictionary<string, string>();
        public Dictionary<string, string> SavedClassHash = new Dictionary<string, string>();
        public Dictionary<string, Num> NumHash = new Dictionary<string, Num>();
        public Dictionary<string, decimal?> SavedNumHash = new Dictionary<string, decimal?>();
        public Dictionary<string, DateTime> DateHash = new Dictionary<string, DateTime>();
        public Dictionary<string, DateTime> SavedDateHash = new Dictionary<string, DateTime>();
        public Dictionary<string, string> DescriptionHash = new Dictionary<string, string>();
        public Dictionary<string, string> SavedDescriptionHash = new Dictionary<string, string>();
        public Dictionary<string, bool> CheckHash = new Dictionary<string, bool>();
        public Dictionary<string, bool> SavedCheckHash = new Dictionary<string, bool>();
        public Dictionary<string, Attachments> AttachmentsHash = new Dictionary<string, Attachments>();
        public Dictionary<string, string> SavedAttachmentsHash = new Dictionary<string, string>();
        public bool ReadOnly;
        public List<string> MineCache;

        public List<string> ColumnNames()
        {
            var data = new List<string>();
            data.AddRange(ClassHash.Keys);
            data.AddRange(NumHash.Keys);
            data.AddRange(DateHash.Keys);
            data.AddRange(DescriptionHash.Keys);
            data.AddRange(CheckHash.Keys);
            data.AddRange(AttachmentsHash.Keys);
            return data
                .Where(columnName => Def.ExtendedColumnTypes.ContainsKey(columnName ?? string.Empty))
                .ToList();
        }

        public string GetValue(
            Context context,
            Column column,
            bool toLocal = false)
        {
            switch (Def.ExtendedColumnTypes.Get(column?.ColumnName ?? string.Empty))
            {
                case "Class":
                    return GetClass(columnName: column.ColumnName);
                case "Num":
                    return column.Nullable != true
                        ? GetNum(columnName: column.ColumnName).Value?.ToString() ?? "0"
                        : GetNum(columnName: column.ColumnName).Value?.ToString() ?? string.Empty;
                case "Date":
                    return toLocal
                        ? GetDate(columnName: column.ColumnName)
                            .ToLocal(context: context)
                            .ToString()
                        : GetDate(columnName: column.ColumnName).ToString();
                case "Description":
                    return GetDescription(columnName: column.ColumnName);
                case "Check":
                    return GetCheck(columnName: column.ColumnName).ToString();
                case "Attachments":
                    return GetAttachments(columnName: column.ColumnName).ToJson();
                default:
                    return null;
            }
        }

        public void GetValue(
            Context context,
            Column column,
            string value,
            bool toUniversal = false)
        {
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
                        value: toUniversal
                            ? value.ToDateTime().ToUniversal(context: context)
                            : value.ToDateTime());
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

        public bool Updated()
        {
            return ClassHash.Any(o => Class_Updated(o.Key))
                || NumHash.Any(o => Num_Updated(o.Key))
                || DateHash.Any(o => Date_Updated(o.Key))
                || DescriptionHash.Any(o => Description_Updated(o.Key))
                || CheckHash.Any(o => Check_Updated(o.Key))
                || AttachmentsHash.Any(o => Attachments_Updated(o.Key));
        }

        public string GetClass(Column column)
        {
            return GetClass(columnName: column.ColumnName);
        }

        public string GetSavedClass(Column column)
        {
            return GetSavedClass(columnName: column.ColumnName);
        }

        public string GetClass(string columnName)
        {
            return ClassHash.Get(columnName) ?? string.Empty;
        }

        public string GetSavedClass(string columnName)
        {
            return SavedClassHash.Get(columnName) ?? string.Empty;
        }

        public void GetClass(Column column, string value)
        {
            GetClass(
                columnName: column.ColumnName,
                value: value);
        }

        public void GetSavedClass(Column column, string value)
        {
            GetSavedClass(
                columnName: column.ColumnName,
                value: value);
        }

        public void GetClass(string columnName, string value)
        {
            if (!ClassHash.ContainsKey(columnName))
            {
                ClassHash.Add(columnName, value);
            }
            else
            {
                ClassHash[columnName] = value;
            }
        }

        public void GetSavedClass(string columnName, string value)
        {
            if (!SavedClassHash.ContainsKey(columnName))
            {
                SavedClassHash.Add(columnName, value);
            }
            else
            {
                SavedClassHash[columnName] = value;
            }
        }

        public bool Class_Updated(
            string columnName,
            Context context = null,
            Column column = null)
        {
            var value = GetClass(columnName: columnName);
            return value != GetSavedClass(columnName: columnName)
                && (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context) != value);
        }

        public Num GetNum(Column column)
        {
            return GetNum(columnName: column.ColumnName);
        }

        public decimal? GetSavedNum(Column column)
        {
            return GetSavedNum(columnName: column.ColumnName);
        }

        public Num GetNum(string columnName)
        {
            return NumHash.Get(columnName) ?? new Num();
        }

        public decimal? GetSavedNum(string columnName)
        {
            return SavedNumHash.Get(columnName);
        }

        public void GetNum(Column column, Num value)
        {
            GetNum(
                columnName: column.ColumnName,
                value: value);
        }

        public void GetSavedNum(Column column, decimal? value)
        {
            GetSavedNum(
                columnName: column.ColumnName,
                value: value);
        }

        public void GetNum(string columnName, Num value)
        {
            if (!NumHash.ContainsKey(columnName))
            {
                NumHash.Add(columnName, value);
            }
            else
            {
                NumHash[columnName] = value;
            }
        }

        public void GetSavedNum(string columnName, decimal? value)
        {
            if (!SavedNumHash.ContainsKey(columnName))
            {
                SavedNumHash.Add(columnName, value);
            }
            else
            {
                SavedNumHash[columnName] = value;
            }
        }

        public bool Num_Updated(
            string columnName,
            Context context = null,
            Column column = null,
            bool paramDefault = false)
        {
            var value = GetNum(columnName: columnName)?.Value;
            var savedValue = GetSavedNum(columnName: columnName);
            if (column?.Nullable !=  true)
            {
                value = value ?? 0;
                savedValue = savedValue ?? 0;
            }
            return value != savedValue
                && (paramDefault
                    || column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToDecimal() != value);
        }

        public DateTime GetDate(Column column)
        {
            return GetDate(columnName: column.ColumnName);
        }

        public DateTime GetSavedDate(Column column)
        {
            return GetSavedDate(columnName: column.ColumnName);
        }

        public DateTime GetDate(string columnName)
        {
            return DateHash.ContainsKey(columnName)
                ? DateHash.Get(columnName)
                : 0.ToDateTime();
        }

        public DateTime GetSavedDate(string columnName)
        {
            return SavedDateHash.ContainsKey(columnName)
                ? SavedDateHash.Get(columnName)
                : 0.ToDateTime();
        }

        public void GetDate(Column column, DateTime value)
        {
            GetDate(
                columnName: column.ColumnName,
                value: value);
        }

        public void GetSavedDate(Column column, DateTime value)
        {
            GetSavedDate(
                columnName: column.ColumnName,
                value: value);
        }

        public void GetDate(string columnName, DateTime value)
        {
            if (!DateHash.ContainsKey(columnName))
            {
                DateHash.Add(columnName, value);
            }
            else
            {
                DateHash[columnName] = value;
            }
        }

        public void GetSavedDate(string columnName, DateTime value)
        {
            if (!SavedDateHash.ContainsKey(columnName))
            {
                SavedDateHash.Add(columnName, value);
            }
            else
            {
                SavedDateHash[columnName] = value;
            }
        }

        public bool Date_Updated(
            string columnName,
            Context context = null,
            Column column = null)
        {
            var value = GetDate(columnName: columnName);
            return value != GetSavedDate(columnName: columnName)
                && (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToDateTime() != value);
        }

        public string GetDescription(Column column)
        {
            return GetDescription(columnName: column.ColumnName);
        }

        public string GetSavedDescription(Column column)
        {
            return GetSavedDescription(columnName: column.ColumnName);
        }

        public string GetDescription(string columnName)
        {
            return DescriptionHash.Get(columnName) ?? string.Empty;
        }

        public string GetSavedDescription(string columnName)
        {
            return SavedDescriptionHash.Get(columnName) ?? string.Empty;
        }

        public void GetDescription(Column column, string value)
        {
            GetDescription(
                columnName: column.ColumnName,
                value: value);
        }

        public void GetSavedDescription(Column column, string value)
        {
            GetSavedDescription(
                columnName: column.ColumnName,
                value: value);
        }

        public void GetDescription(string columnName, string value)
        {
            if (!DescriptionHash.ContainsKey(columnName))
            {
                DescriptionHash.Add(columnName, value);
            }
            else
            {
                DescriptionHash[columnName] = value;
            }
        }

        public void GetSavedDescription(string columnName, string value)
        {
            if (!SavedDescriptionHash.ContainsKey(columnName))
            {
                SavedDescriptionHash.Add(columnName, value);
            }
            else
            {
                SavedDescriptionHash[columnName] = value;
            }
        }

        public bool Description_Updated(
            string columnName,
            Context context = null,
            Column column = null)
        {
            var value = GetDescription(columnName: columnName);
            return value != GetSavedDescription(columnName: columnName)
                && (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context) != value);
        }

        public bool GetCheck(Column column)
        {
            return GetCheck(columnName: column.ColumnName);
        }

        public bool GetSavedCheck(Column column)
        {
            return GetSavedCheck(columnName: column.ColumnName);
        }

        public bool GetCheck(string columnName)
        {
            return CheckHash.Get(columnName);
        }

        public bool GetSavedCheck(string columnName)
        {
            return SavedCheckHash.Get(columnName);
        }

        public void GetCheck(Column column, bool value)
        {
            GetCheck(
                columnName: column.ColumnName,
                value: value);
        }

        public void GetSavedCheck(Column column, bool value)
        {
            GetSavedCheck(
                columnName: column.ColumnName,
                value: value);
        }

        public void GetCheck(string columnName, bool value)
        {
            if (!CheckHash.ContainsKey(columnName))
            {
                CheckHash.Add(columnName, value);
            }
            else
            {
                CheckHash[columnName] = value;
            }
        }

        public void GetSavedCheck(string columnName, bool value)
        {
            if (!CheckHash.ContainsKey(columnName))
            {
                SavedCheckHash.Add(columnName, value);
            }
            else
            {
                SavedCheckHash[columnName] = value;
            }
        }

        public bool Check_Updated(
            string columnName,
            Context context = null,
            Column column = null)
        {
            var value = GetCheck(columnName: columnName);
            return value != GetSavedCheck(columnName: columnName)
                && (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToBool() != value);
        }

        public Attachments GetAttachments(Column column)
        {
            return GetAttachments(columnName: column.ColumnName);
        }

        public string GetSavedAttachments(Column column)
        {
            return GetSavedAttachments(columnName: column.ColumnName);
        }

        public Attachments GetAttachments(string columnName)
        {
            return AttachmentsHash.Get(columnName) ?? new Attachments();
        }

        public string GetSavedAttachments(string columnName)
        {
            return SavedAttachmentsHash.Get(columnName) ?? new Attachments().RecordingJson();
        }

        public void GetAttachments(Column column, Attachments value)
        {
            GetAttachments(
                columnName: column.ColumnName,
                value: value);
        }

        public void GetSavedAttachments(Column column, string value)
        {
            GetSavedAttachments(
                columnName: column.ColumnName,
                value: value);
        }

        public void GetAttachments(string columnName, Attachments value)
        {
            if (!AttachmentsHash.ContainsKey(columnName))
            {
                AttachmentsHash.Add(columnName, value);
            }
            else
            {
                AttachmentsHash[columnName] = value;
            }
        }

        public void GetSavedAttachments(string columnName, string value)
        {
            if (!AttachmentsHash.ContainsKey(columnName))
            {
                SavedAttachmentsHash.Add(columnName, value);
            }
            else
            {
                SavedAttachmentsHash[columnName] = value;
            }
        }

        public bool Attachments_Updated(
            string columnName,
            Context context = null,
            Column column = null)
        {
            var value = GetAttachments(columnName: columnName).RecordingJson();
            return value != GetSavedAttachments(columnName: columnName)
                && (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context) != value);
        }

        public void BaseFullText(
            Context context,
            Column column,
            System.Text.StringBuilder fullText)
        {
            if (column != null)
            {
                switch (Def.ExtendedColumnTypes.Get(column?.ColumnName ?? string.Empty))
                {
                    case "Class":
                        GetClass(column.ColumnName)?.FullText(
                            context: context,
                            column: column,
                            fullText: fullText);
                        break;
                    case "Num":
                        GetNum(column.ColumnName)?.FullText(
                            context: context,
                            column: column,
                            fullText: fullText);
                        break;
                    case "Date":
                        GetDate(column.ColumnName).FullText(
                            context: context,
                            column: column,
                            fullText: fullText);
                        break;
                    case "Description":
                        GetDescription(column.ColumnName)?.FullText(
                            context: context,
                            column: column,
                            fullText: fullText);
                        break;
                    case "Attachments":
                        GetAttachments(column.ColumnName)?.FullText(
                            context: context,
                            column: column,
                            fullText: fullText);
                        break;
                }
            }
        }

        public void SetByWhenloadingSiteSettingsServerScript(
            Context context,
            SiteSettings ss)
        {
            if (context.Id == ss.SiteId)
            {
                switch (context.Action)
                {
                    case "createbytemplate":
                    case "edit":
                    case "update":
                    case "copy":
                    case "delete":
                        return;
                }
            }
            var scriptValues = ServerScriptUtilities.Execute(
                context: context,
                ss: ss,
                itemModel: null,
                view: null,
                where: script => script.WhenloadingSiteSettings == true,
                condition: "WhenloadingSiteSettings");
            SetServerScriptModelColumns(context: context,
                ss: ss,
                scriptValues: scriptValues);
        }

        public virtual void SetByAfterFormulaServerScript(Context context, SiteSettings ss)
        {
        }

        public virtual ServerScriptModelRow SetByWhenloadingRecordServerScript(
            Context context,
            SiteSettings ss)
        {
            return null;
        }

       public virtual ServerScriptModelRow SetByBeforeOpeningRowServerScript(
            Context context,
            SiteSettings ss)
        {
            return null;
        }

        public virtual ServerScriptModelRow SetByBeforeOpeningPageServerScript(
            Context context,
            SiteSettings ss,
            View view = null)
        {
            var scriptValues = ServerScriptUtilities.Execute(
                context: context,
                ss: ss,
                itemModel: null,
                view: view,
                where: script => script.BeforeOpeningPage == true,
                condition: "BeforeOpeningPage");
            if (scriptValues != null)
            {
                SetServerScriptModelColumns(context: context,
                    ss: ss,
                    scriptValues: scriptValues);
                ServerScriptModelRow = scriptValues;
            }
            return scriptValues;
        }

        public virtual List<string> Mine(Context context)
        {
            return null;
        }

        public static void SetServerScriptModelColumns(Context context, SiteSettings ss, ServerScriptModelRow scriptValues)
        {
            scriptValues?.Columns.ForEach(scriptColumn =>
            {
                var column = ss.GetColumn(
                    context: context,
                    columnName: scriptColumn.Key);
                if (column != null)
                {
                    if (scriptColumn.Value.ChoiceHash != null)
                    {
                        var searchText = context.Forms.Data("DropDownSearchText");
                        var searchIndexes = searchText.SearchIndexes();
                        column.ChoiceHash = scriptColumn.Value
                            ?.ChoiceHash
                            ?.Where(o => searchIndexes?.Any() != true ||
                                searchIndexes.All(p =>
                                    o.Key.ToString() == p ||
                                    (o.Value?.ToString()).RegexLike(p).Any()))
                            ?.ToDictionary(
                                o => o.Key.ToString(),
                                o => new Choice(
                                    o.Key.ToString(),
                                    o.Value?.ToString()));
                        column.AddChoiceHashByServerScript = true;
                    }
                    column.ServerScriptModelColumn = scriptColumn.Value;
                }
            });
        }
    }

    public class BaseItemModel : BaseModel
    {
        public long SiteId = 0;
        public Title Title = new Title();
        public string Body = string.Empty;
        public long SavedSiteId = 0;
        public string SavedTitle = string.Empty;
        public string SavedBody = string.Empty;
        public List<string> RecordPermissions;

        public bool SiteId_Updated(Context context, Column column = null)
        {
            return SiteId != SavedSiteId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToLong() != SiteId);
        }

        public bool Title_Updated(Context context, Column column = null)
        {
            return Title.Value != SavedTitle && Title.Value != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != Title.Value);
        }

        public bool Body_Updated(Context context, Column column = null)
        {
            return Body != SavedBody && Body != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != Body);
        }

        public override ServerScriptModelRow SetByWhenloadingRecordServerScript(
            Context context,
            SiteSettings ss)
        {
            var scriptValues = ServerScriptUtilities.Execute(
                context: context,
                ss: ss,
                itemModel: this,
                view: null,
                where: script => script.WhenloadingRecord == true,
                condition: "WhenloadingRecord");
            if (scriptValues != null)
            {
                ServerScriptModelRow = scriptValues;
            }
            return scriptValues;
        }

        public void SetByBeforeFormulaServerScript(Context context, SiteSettings ss)
        {
            ServerScriptUtilities.Execute(
                context: context,
                ss: ss,
                itemModel: this,
                view: null,
                where: script => script.BeforeFormula == true,
                condition: "BeforeFormula");
        }

        public override void SetByAfterFormulaServerScript(Context context, SiteSettings ss)
        {
            ServerScriptUtilities.Execute(
                context: context,
                ss: ss,
                itemModel: this,
                view: null,
                where: script => script.AfterFormula == true,
                condition: "AfterFormula");
        }

        public void SetByAfterUpdateServerScript(
            Context context,
            SiteSettings ss)
        {
            ServerScriptUtilities.Execute(
                context: context,
                ss: ss,
                itemModel: this,
                view: null,
                where: script => script.AfterUpdate == true,
                condition: "AfterUpdate");
        }

        public void SetByBeforeUpdateServerScript(
            Context context,
            SiteSettings ss)
        {
            ServerScriptUtilities.Execute(
                context: context,
                ss: ss,
                itemModel: this,
                view: null,
                where: script => script.BeforeUpdate == true,
                condition: "BeforeUpdate");
        }

        public void SetByAfterCreateServerScript(
            Context context,
            SiteSettings ss)
        {
            ServerScriptUtilities.Execute(
                context: context,
                ss: ss,
                itemModel: this,
                view: null,
                where: script => script.AfterCreate == true,
                condition: "AfterCreate");
        }

        public void SetByBeforeCreateServerScript(
            Context context,
            SiteSettings ss)
        {
            ServerScriptUtilities.Execute(
                context: context,
                ss: ss,
                itemModel: this,
                view: null,
                where: script => script.BeforeCreate == true,
                condition: "BeforeCreate");
        }

        public void SetByAfterDeleteServerScript(
            Context context,
            SiteSettings ss)
        {
            ServerScriptUtilities.Execute(
                context: context,
                ss: ss,
                itemModel: this,
                view: null,
                where: script => script.AfterDelete == true,
                condition: "AfterDelete");
        }

        public void SetByBeforeDeleteServerScript(
            Context context,
            SiteSettings ss)
        {
            ServerScriptUtilities.Execute(
                context: context,
                ss: ss,
                itemModel: this,
                view: null,
                where: script => script.BeforeDelete == true,
                condition: "BeforeDelete");
        }

        public override ServerScriptModelRow SetByBeforeOpeningRowServerScript(
            Context context,
            SiteSettings ss)
        {
            var scriptValues = ServerScriptUtilities.Execute(
                context: context,
                ss: ss,
                itemModel: this,
                view: null,
                where: script => script.BeforeOpeningRow == true,
                condition: "BeforeOpeningRow");
            if (scriptValues != null)
            {
                SetServerScriptModelColumns(context: context,
                    ss: ss,
                    scriptValues: scriptValues);
                ServerScriptModelRow = scriptValues;
            }
            return scriptValues;
        }

        public override ServerScriptModelRow SetByBeforeOpeningPageServerScript(
            Context context,
            SiteSettings ss,
            View view = null)
        {
            var scriptValues = ServerScriptUtilities.Execute(
                context: context,
                ss: ss,
                itemModel: this,
                view: view,
                where: script => script.BeforeOpeningPage == true,
                condition: "BeforeOpeningPage");
            if (scriptValues != null)
            {
                SetServerScriptModelColumns(context: context,
                    ss: ss,
                    scriptValues: scriptValues);
                ServerScriptModelRow = scriptValues;
            }
            return scriptValues;
        }
    }
}
