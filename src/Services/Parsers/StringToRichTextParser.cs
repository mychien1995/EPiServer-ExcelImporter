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
    [ExcelColumnParser("StringToRichText")]
    public class StringToRichTextParser : IColumnDataParser
    {
        public object Parse(IContent currentContent, object excelData, ExcelImporterConfiguration configuration)
        {
            if (excelData == null) return null;
            var strData = excelData + "";
            if (string.IsNullOrWhiteSpace(strData)) return null;
            if (strData.StartsWith("<p")) return strData;
            strData = strData.Replace(System.Environment.NewLine, "<br />");
            strData = strData.Replace("\n", "<br />");
            return "<p>" + strData + "</p>";
        }
    }
}