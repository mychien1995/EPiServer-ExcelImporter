using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPiServer.ExcelImporter.Services.Loaders
{
    public class DescendantsContentChildrenLoader : IContentChildrenLoader
    {
        public DescendantsContentChildrenLoader()
        {
        }
        public List<IContent> GetChildren(IContent root, Type childrenType)
        {
            var _contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();
            return _contentRepository.GetDescendents(root.ContentLink)
                .Select(x => _contentRepository.Get<IContent>(x))
                .Where(type => childrenType.IsAssignableFrom(type.GetType())).ToList();
        }
    }
}