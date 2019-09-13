using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPiServer.ExcelImporter.JsonConfigurations
{
    public class JsonContentRoot
    {
        [JsonProperty("contentRootType")]
        public string ContentRootType { get; set; }
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("getChildren")]
        public string GetChildren { get; set; }
    }
}