using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPiServer.ExcelImporter.JsonConfigurations
{
    public class JsonImporterConfiguration
    {
        [JsonProperty("excelReader")]
        public JsonLibraryClass ExcelReader { get; set; }

        [JsonProperty("excelValidators")]
        public JsonLibraryClass[] ExcelValidators { get; set; }

        [JsonProperty("rowToContentIndicator")]
        public JsonLibraryClass RowToContentIndicator { get; set; }

        [JsonProperty("contentRoot")]
        public JsonContentRoot ContentRoot { get; set; }

        [JsonProperty("targetPageType")]
        public string TargetPageType { get; set; }


        [JsonProperty("columnMappings")]
        public JsonColumnMapping[] ColumnMappings { get; set; }

        [JsonProperty("defaultValuesMappings")]
        public JsonDefaultValueMapping[] DefaultValueMappings { get; set; }


        [JsonProperty("extraConf")]
        public Dictionary<string, string> ExtraConfigurations { get; set; }

        [JsonProperty("rowStart")]
        public int? RowStart { get; set; }

        [JsonProperty("saveWithoutPublish")]
        public bool SaveWithoutPublish { get; set; }
    }
}