using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPiServer.ExcelImporter.Models
{
    public enum RowToContentFinderResult
    {
        ShouldInsert,
        ShouldUpdate,
        ShouldIgnore
    }
}