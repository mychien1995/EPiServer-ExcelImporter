using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPiServer.ExcelImporter.Models
{
    public class ExcelDataValidationResult
    {
        public string Message { get; set; }
        public bool IsValid { get; set; }
    }
}