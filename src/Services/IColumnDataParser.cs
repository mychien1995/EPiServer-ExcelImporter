using EPiServer.Core;
using EPiServer.ExcelImporter.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPiServer.ExcelImporter.Services
{
    public interface IColumnDataParser
    {
        object Parse(IContent currentContent, object excelData, ExcelImporterConfiguration configuration);
    }
}