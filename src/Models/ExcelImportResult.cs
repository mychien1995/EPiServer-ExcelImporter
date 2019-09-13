using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPiServer.ExcelImporter.Models
{
    public class ExcelImportResult
    {
        public ExcelImportResult()
        {
            Message = new List<string>();
            NoOfContentUpdated = 0;
            NoOfContentInserted = 0;
            NoOfContentErrors = 0;
            NoOfContentIgnored = 0;
        }

        public void AddMessage(string msg)
        {
            Message.Add(msg);
        }

        public void SetConfigurationFailureMessage(string msg)
        {
            Message = new List<string>();
            Message.Add(msg);
        }

        public List<string> Message { get; set; }
        public bool IsSuccess { get; set; }
        public int ExcelRowCount { get; set; }
        public int NoOfContentErrors { get; set; }
        public int NoOfContentUpdated { get; set; }
        public int NoOfContentIgnored { get; set; }
        public int NoOfContentInserted { get; set; }
    }
}