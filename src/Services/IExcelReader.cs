using EPiServer.ExcelImporter.Configurations;
using EPiServer.ExcelImporter.Excels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPiServer.ExcelImporter.Services
{
    public interface IExcelReader
    {
        List<ExcelRange> Read(ExcelWorkbook workbook, ExcelImporterConfiguration configuration);
    }
}