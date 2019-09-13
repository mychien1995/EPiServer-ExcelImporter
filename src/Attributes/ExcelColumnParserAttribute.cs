using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPiServer.ExcelImporter.Attributes
{
    public class ExcelColumnParserAttribute : Attribute
    {
        public string Name { get; set; }
        public ExcelColumnParserAttribute(string name)
        {
            this.Name = name;
        }
    }
}