using EPiServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPiServer.ExcelImporter.Services
{
    public interface IContentChildrenLoader
    {
        List<IContent> GetChildren(IContent root, Type childrenType);
    }
}