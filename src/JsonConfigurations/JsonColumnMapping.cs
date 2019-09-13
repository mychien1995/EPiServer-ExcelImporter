using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPiServer.ExcelImporter.JsonConfigurations
{
    public class JsonColumnMapping : JsonDefaultValueMapping
    {

        [JsonProperty("colNumber")]
        public int ExcelColumnNumber { get; set; }
    }

    public class JsonDefaultValueMapping
    {

        [JsonProperty("propertyIndicator")]
        public string PropertyIndicator { get; set; }
        [JsonProperty("parser")]
        public string ParserType { get; set; }

        [JsonProperty("defaultValue")]
        public object DefaultValue { get; set; }

        [JsonProperty("useDefaultValue")]
        public bool UseDefaultValue { get; set; }

        [JsonProperty("ignoreIfNull")]
        public bool IgnoreIfNull { get; set; }
    }
}