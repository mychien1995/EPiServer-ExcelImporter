using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.DataAccess;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using EPiServer.SpecializedProperties;
using EPiServer.ExcelImporter.Configurations;

namespace EPiServer.ExcelImporter.Services.Parsers
{
    [ServiceConfiguration(typeof(IPropertyIndicatorHelper))]
    public class DefaultPropertyIndicatorHelper : IPropertyIndicatorHelper
    {
        private readonly IContentRepository _contentRepository;
        private readonly ContentAssetHelper _contentAssetHelper;
        private readonly IContentTypeRepository _contentTypeRepository;
        private readonly IContentVersionRepository _contentVersionRepository;
        private readonly SaveAction saveBlockAction = SaveAction.Default | SaveAction.SkipValidation | SaveAction.Publish;
        public DefaultPropertyIndicatorHelper(IContentRepository contentRepository, ContentAssetHelper contentAssetHelper
            , IContentTypeRepository contentTypeRepository, IContentVersionRepository contentVersionRepository)
        {
            _contentRepository = contentRepository;
            _contentAssetHelper = contentAssetHelper;
            _contentTypeRepository = contentTypeRepository;
            _contentVersionRepository = contentVersionRepository;
        }

        /// <summary>
        /// Update content based on row values. Using property indicator to find matching property to update
        /// Syntax: propertyName.propertyName or contentAreaName.blockname.propertyname
        /// </summary>
        /// <param name="rowData"></param>
        /// <param name="rowNumber"></param>
        /// <param name="content"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public void SaveProperty(IContent content, ExcelDefaultValueMapping columnMapping, object excelValue)
        {
            var saveWithoutPublish = columnMapping.Configuration.SaveWithoutPublish;
            string propertyIndicator = columnMapping.PropertyIndicator;
            var processingQueue = new Queue<IContent>();
            processingQueue.Enqueue(content);
            var segments = propertyIndicator.Split('.').ToList();
            for (int i = 0; i < segments.Count; i++)
            {
                var processingContent = processingQueue.Peek();
                if (processingContent == null) continue;
                var segment = segments[i];
                if (!IsPropertyExist(processingContent, segment))
                {
                    if ((excelValue != null && !string.IsNullOrEmpty(excelValue + "")))
                    {
                        ContentArea contentArea;
                        PropertyInfo contentAreaProperty;
                        if (i > 0 && IsContainerProperty(processingContent, segments[i - 1], out contentArea, out contentAreaProperty))
                        {
                            var allowedBlocks = GetAllowedBlocks(contentAreaProperty);
                            var blockType = allowedBlocks.FirstOrDefault(c => c.Name.Equals(segment, StringComparison.OrdinalIgnoreCase));
                            if (blockType != null)
                            {
                                var contentType = _contentTypeRepository.Load(blockType.Name);
                                if (contentArea?.Items != null)
                                {
                                    var blocks = contentArea.Items.Select(x => _contentRepository.Get<IContent>(x.ContentLink.ToReferenceWithoutVersion())).ToList();
                                    var existingBlock = blocks.FirstOrDefault(x => x != null && x.ContentTypeID == contentType.ID);
                                    if (existingBlock != null)
                                    {
                                        processingQueue.Enqueue((IContent)(existingBlock as ContentData).CreateWritableClone());
                                        processingQueue.Dequeue();
                                        continue;
                                    }
                                }
                                var assetFolder = _contentAssetHelper.GetOrCreateAssetFolder(processingContent.ContentLink);
                                var newBlock = _contentRepository.GetDefault<IContent>(assetFolder.ContentLink, contentType.ID);
                                newBlock.Name = blockType.Name;
                                var savedNewBlock = _contentRepository.Save(newBlock, saveBlockAction, AccessLevel.NoAccess);
                                contentArea.Items.Add(new ContentAreaItem()
                                {
                                    ContentLink = savedNewBlock
                                });
                                SaveContent(processingContent, segments[i - 1], contentArea, saveWithoutPublish);
                                processingQueue.Enqueue(newBlock);
                                processingQueue.Dequeue();
                            }
                        }
                    }
                    continue;
                }
                else
                {
                    object epiValue = null;
                    if (columnMapping.Parser != null)
                        epiValue = columnMapping.Parser.Parse(processingContent, excelValue, columnMapping.Configuration);
                    else epiValue = excelValue;
                    ContentArea contentArea;
                    PropertyInfo contentAreaProperty;
                    if (IsContainerProperty(processingContent, segment, out contentArea, out contentAreaProperty))
                    {
                        if (contentArea == null) contentArea = new ContentArea();
                        if (i == segments.Count - 1 && epiValue is ContentReference)
                        {
                            contentArea.Items.Clear();
                            contentArea.Items.Add(new ContentAreaItem()
                            {
                                ContentLink = (ContentReference)epiValue
                            });
                        }
                        SaveContent(processingContent, segment, contentArea, saveWithoutPublish);
                        continue;
                    }
                    string blockContentTypeName;
                    if (IsLocalBlockProperty(processingContent, segment, out blockContentTypeName) && i == segments.Count - 2)
                    {
                        var localBlockObject = ((ContentData)processingContent)[segment];
                        PropertyInfo prop = localBlockObject.GetType().GetProperty(segments[i + 1], BindingFlags.Public | BindingFlags.Instance);
                        if (null != prop && prop.CanWrite)
                        {
                            if (epiValue != null || !columnMapping.IgnoreIfNull)
                            {
                                prop.SetValue(localBlockObject, epiValue, null);
                                SaveContent(processingContent, segment, localBlockObject, saveWithoutPublish);
                            }
                        }
                        break;
                    }
                    if (epiValue != null || !columnMapping.IgnoreIfNull)
                        SaveContent(processingContent, segment, epiValue, saveWithoutPublish);
                }
            }
        }

        private bool IsLocalBlockProperty(IContent content, string propertyIndicator, out string contentTypeName)
        {
            contentTypeName = null;
            var property = content.GetOriginalType().GetProperties().FirstOrDefault(x => x.Name == propertyIndicator);
            if (property == null)
            {
                return false;
            }
            if (typeof(BlockData).IsAssignableFrom(property.GetOriginalType()))
            {
                contentTypeName = property.GetOriginalType().Name;
                return true;
            }
            return false;
        }
        private void SaveContent(IContent content, string propertyIndicator, object value, bool saveWithoutPublish = false)
        {
            var contentData = content as ContentData;
            contentData[propertyIndicator] = value;
            var saveActions = SaveAction.Default | SaveAction.SkipValidation;
            var version = content as IVersionable;
            if (!saveWithoutPublish) saveActions = saveActions | SaveAction.Publish;
            _contentRepository.Save(content, saveActions, AccessLevel.NoAccess);

        }

        private IContent GetDraftedContent(ContentReference contentLink)
        {
            var versions = _contentVersionRepository.LoadCommonDraft(contentLink, "en");
            return _contentRepository.Get<IContent>(versions.ContentLink);
        }
        private List<Type> GetAllowedBlocks(PropertyInfo propertyInfo)
        {
            var allowedTypesProperty = (AllowedTypesAttribute)propertyInfo.GetCustomAttribute(typeof(AllowedTypesAttribute));
            if (allowedTypesProperty == null) return new List<Type>();
            return allowedTypesProperty.AllowedTypes.ToList();
        }

        private bool IsPropertyExist(IContent content, string segment)
        {
            var properties = content.Property;
            return properties.Any(c => c.Name.Equals(segment, StringComparison.OrdinalIgnoreCase));
        }
        private bool IsContainerProperty(IContent content, string segment, out ContentArea contentArea, out PropertyInfo propertyData)
        {
            propertyData = null;
            contentArea = null;
            var property = content.Property.FirstOrDefault(x => x.Name.Equals(segment, StringComparison.OrdinalIgnoreCase));
            if (property == null) return false;
            if (property.GetType() == typeof(PropertyContentArea))
            {
                propertyData = content.GetType().GetProperty(segment);
                contentArea = property.Value as ContentArea;
                return true;
            }
            return false;
        }
    }
}