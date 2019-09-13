using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EPiServer.ServiceLocation;
using EPiServer.ExcelImporter.Configurations;
using EPiServer.ExcelImporter.Excels;

namespace EPiServer.ExcelImporter.Services.Readers
{
    public class DefaultExcelReader : IExcelReader
    {
        public List<ExcelRange> Read(ExcelWorkbook workbook, ExcelImporterConfiguration configuration)
        {
            var result = new List<ExcelRange>();
            var xmlSheets = workbook.GetSheets();
            foreach (var sheet in xmlSheets)
            {
                if (sheet.Worksheet.Visibility == ClosedXML.Excel.XLWorksheetVisibility.Visible)
                {
                    var colTotal = sheet.Worksheet.ColumnsUsed().Count();
                    var rowTotal = sheet.Worksheet.LastRowUsed().RowNumber();
                    for (int i = 0; i < rowTotal - configuration.RowStart + 1; i++)
                    {
                        var row = i + configuration.RowStart;
                        var xlRange = sheet.GetRange(row, 1, rowTotal, colTotal);
                        if (xlRange.HasData(row))
                        {
                            xlRange.IndexInSheet = row;
                            xlRange.SheetName = sheet.Worksheet.Name;
                            result.Add(xlRange);
                        }
                    }
                }
            }
            return result;
        }
    }
}