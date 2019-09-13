using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using EPiServer.ExcelImporter.Attributes;
using EPiServer.ExcelImporter.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPiServer.ExcelImporter.Services.Parsers
{
    [ExcelColumnParser("IdToContentReference")]
    public class IdToContentReferenceParser : IColumnDataParser
    {
        public object Parse(IContent currentContent, object excelData, ExcelImporterConfiguration configuration)
        {
            if (excelData == null) return null;
            int id;
            if (int.TryParse(excelData + "", out id))
            {
                var contentRef = new ContentReference(id);
                if (!ContentReference.IsNullOrEmpty(contentRef)) return contentRef;
            }
            return null;
        }
    }
}