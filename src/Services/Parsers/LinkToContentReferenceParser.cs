using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using EPiServer.ExcelImporter.Attributes;
using EPiServer.ExcelImporter.Configurations;

namespace EPiServer.ExcelImporter.Services.Parsers
{
    [ExcelColumnParser("LinkToContentReference")]
    public class LinkToContentReferenceParser : IColumnDataParser
    {
        public object Parse(IContent currentContent, object excelData, ExcelImporterConfiguration configuration)
        {
            var urlResolver = ServiceLocator.Current.GetInstance<IUrlResolver>();
            if (excelData == null) return null;
            var link = excelData.ToString();
            var content = urlResolver.Route(new UrlBuilder(link));
            if (content != null) return content.ContentLink;
            return null;
        }
    }
}