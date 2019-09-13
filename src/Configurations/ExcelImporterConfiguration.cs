using EPiServer.Core;
using EPiServer.ExcelImporter.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPiServer.ExcelImporter.Configurations
{
    public class ExcelImporterConfiguration
    {
        public IExcelReader ExcelReader { get; set; }

        public List<IExcelValidator> ExcelValidators { get; set; }

        public IRowToContentIndicator RowIndicators { get; set; }

        public ExcelContentRoot ContentRoots { get; set; }

        public List<ExcelColumnMapping> ColumnMappings { get; set; }
        public List<ExcelDefaultValueMapping> DefaultValueMappings { get; set; }
        public Type TargetPageType { get; set; }

        public Dictionary<string, string> ExtraConfigurations { get; set; }
        public int RowStart { get; set; }
        public bool SaveWithoutPublish { get; set; }
    }
}