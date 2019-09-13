using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPiServer.ExcelImporter.JsonConfigurations
{
    public class JsonCustomParser
    {

        [JsonProperty("type")]
        public string Type { get; set; }


        [JsonProperty("name")]
        public string ParserName { get; set; }
    }
}