using EPiServer.ExcelImporter.Excels;
using EPiServer.ExcelImporter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPiServer.ExcelImporter.Services
{
    public interface IExcelValidator
    {
        ExcelDataValidationResult Validate(ExcelRange row);
    }
}