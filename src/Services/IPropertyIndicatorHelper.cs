using EPiServer.Core;
using EPiServer.ExcelImporter.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPiServer.ExcelImporter.Services
{
    public interface IPropertyIndicatorHelper
    {
        void SaveProperty(IContent content, ExcelDefaultValueMapping columnMapping, object value);
    }
}