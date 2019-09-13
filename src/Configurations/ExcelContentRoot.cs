using EPiServer.Core;
using EPiServer.ExcelImporter.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPiServer.ExcelImporter.Configurations
{
    public class ExcelContentRoot
    {
        public ExcelContentRoot()
        {
            ContentRoots = new List<IContent>();
        }
        public List<IContent> ContentRoots { get; set; }
        public IContentChildrenLoader ChildrenLoader { get; set; }
    }
}