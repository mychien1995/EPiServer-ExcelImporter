using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPiServer.ExcelImporter.Attributes
{
    public class EpiContentChildrenLoaderAttribute : Attribute
    {
        public string Name { get; set; }
        public EpiContentChildrenLoaderAttribute(string name)
        {
            this.Name = name;
        }
    }
}