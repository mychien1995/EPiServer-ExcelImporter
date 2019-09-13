using EPiServer.ExcelImporter.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPiServer.ExcelImporter.Configurations
{
    public class ExcelColumnMapping : ExcelDefaultValueMapping
    {
        public int ExcelColumnNumber { get; set; }
    }

    public class ExcelDefaultValueMapping
    {
        public ExcelDefaultValueMapping()
        {
            IgnoreIfNull = true;
            UseDefaultValue = true;
        } 

        public ExcelImporterConfiguration Configuration { get; set; }
        public string PropertyIndicator { get; set; }
        public object DefaultValue { get; set; }
        public bool UseDefaultValue { get; set; }
        public bool IgnoreIfNull { get; set; }

        public IColumnDataParser Parser { get; set; }
    }
}