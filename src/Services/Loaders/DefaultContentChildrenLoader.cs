using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;

namespace EPiServer.ExcelImporter.Services.Loaders
{
    public class DefaultContentChildrenLoader : IContentChildrenLoader
    {
        public DefaultContentChildrenLoader()
        {
        }
        public List<IContent> GetChildren(IContent root, Type childrenType)
        {
            var _contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();
            return _contentRepository.GetChildren<IContent>(root.ContentLink).Where(x => childrenType.IsAssignableFrom(childrenType)).ToList();
        }
    }
}