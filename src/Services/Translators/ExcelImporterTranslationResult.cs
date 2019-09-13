using EPiServer.ExcelImporter.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPiServer.ExcelImporter.Services.Translators
{
    public class ExcelImporterTranslationResult
    {
        public ExcelImporterTranslationResult()
        {
            Configuration = new ExcelImporterConfiguration();
        }
        public ExcelImporterConfiguration Configuration { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}