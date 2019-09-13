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
    [ExcelColumnParser("LinkListToContentReferenceList")]
    public class LinkListToContentReferenceListParser : IColumnDataParser
    {
        public object Parse(IContent currentContent, object excelData, ExcelImporterConfiguration configuration)
        {
            var result = new List<ContentReference>();
            var urlResolver = ServiceLocator.Current.GetInstance<IUrlResolver>();
            if (excelData == null) return null;
            var links = excelData.ToString().Split(
                new[] { "\r\n", "\r", "\n" },
                StringSplitOptions.None
            );
            foreach(var link in links)
            {
                var content = urlResolver.Route(new UrlBuilder(link));
                if (content != null) result.Add(content.ContentLink);
            }
            return result;
        }
    }
}