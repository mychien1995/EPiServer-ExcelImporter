using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPiServer.ExcelImporter.JsonConfigurations
{
    public class JsonLibraryClass
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        public JsonLibraryClass()
        {

        }
        public JsonLibraryClass(Type type)
        {
            this.Type = type.FullName + ", " + type.Assembly.GetName().Name;
        }
    }
}