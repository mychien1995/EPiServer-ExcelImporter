using EPiServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPiServer.ExcelImporter.Models
{
    public class RowImportResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public IContent Content { get; set; }
    }
}