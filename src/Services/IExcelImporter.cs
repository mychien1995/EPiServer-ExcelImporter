using EPiServer.ExcelImporter.Excels;
using EPiServer.ExcelImporter.JsonConfigurations;
using EPiServer.ExcelImporter.Models;
using EPiServer.ExcelImporter.Services.Importers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace EPiServer.ExcelImporter.Services
{
    public interface IExcelImporter
    {
        ExcelImportResult Import(string jsonContent, ExcelWorkbook workbook);

        ExcelImportResult Import(JsonImporterConfiguration jsonConfiguration, ExcelWorkbook workbook);

        event AfterImportHandler AfterImport;
    }
}