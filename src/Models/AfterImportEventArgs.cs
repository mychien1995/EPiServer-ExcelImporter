using EPiServer.Core;
using EPiServer.ExcelImporter.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPiServer.ExcelImporter.Models
{
    public class AfterImportEventArgs : EventArgs
    {
        public ExcelImporterConfiguration Configuration { get; set; }
        public List<IContent> UpdatedContents { get; set; }
    }
}