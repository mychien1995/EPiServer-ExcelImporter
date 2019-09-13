using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using Newtonsoft.Json;
using EPiServer.ExcelImporter.Attributes;
using EPiServer.ExcelImporter.Configurations;
using EPiServer.ExcelImporter.JsonConfigurations;
using EPiServer.ExcelImporter.Services.Loaders;
using EPiServer.ExcelImporter.Services.Readers;

namespace EPiServer.ExcelImporter.Services.Translators
{
    [ServiceConfiguration(typeof(IJsonToConfigTranslator))]
    public class DefaultJsonToConfigTranslator : IJsonToConfigTranslator
    {
        private readonly IContentRepository _contentRepository;
        public DefaultJsonToConfigTranslator(IContentRepository contentRepository)
        {
            _contentRepository = contentRepository;
        }

        public void FillDefaultValue(string input, out JsonImporterConfiguration json)
        {
            JsonImporterConfiguration rawData = JsonConvert.DeserializeObject<JsonImporterConfiguration>(input);
            if (rawData.ExcelReader == null) rawData.ExcelReader = new JsonLibraryClass(typeof(DefaultExcelReader));
            if (rawData.ContentRoot != null)
            {
                if (string.IsNullOrEmpty(rawData.ContentRoot.ContentRootType)) rawData.ContentRoot.ContentRootType = ContentRootTypes.IDS;
                if (string.IsNullOrEmpty(rawData.ContentRoot.GetChildren)) rawData.ContentRoot.GetChildren = "DefaultContentChildrenLoader";
            }
            if (rawData.ColumnMappings == null) rawData.ColumnMappings = new JsonColumnMapping[0];
            if (rawData.DefaultValueMappings == null) rawData.DefaultValueMappings = new JsonColumnMapping[0];
            if (rawData.ExcelValidators == null) rawData.ExcelValidators = new JsonLibraryClass[0];
            if (rawData.RowStart == null) rawData.RowStart = 2;
            json = rawData;
        }

        public virtual ExcelImporterTranslationResult Translate(JsonImporterConfiguration json)
        {
            var result = new ExcelImporterTranslationResult();
            result.IsSuccess = true;
            string validationMsg = string.Empty;
            if (!IsValidJsonData(json, out validationMsg))
            {
                result.IsSuccess = false;
                result.Message = validationMsg;
                return result;
            }
            var excelReader = LoadInstance<IExcelReader>(json.ExcelReader.Type);
            if (excelReader == null)
            {
                result.IsSuccess = false;
                result.Message = "Invalid Excel Reader";
                return result;
            }
            var excelValidators = new List<IExcelValidator>();
            foreach (var validatorType in json.ExcelValidators)
            {
                var validator = LoadInstance<IExcelValidator>(validatorType.Type);
                if (validator == null)
                {
                    result.IsSuccess = false;
                    result.Message = "Invalid Excel Validator";
                    return result;
                }
                excelValidators.Add(validator);
            }
            var rowToContentIndicator = LoadInstance<IRowToContentIndicator>(json.RowToContentIndicator.Type);
            if (rowToContentIndicator == null)
            {
                result.IsSuccess = false;
                result.Message = "Invalid row to content indicator";
                return result;
            }
            var contentRoots = TranslateContentRoots(json.ContentRoot);
            if (contentRoots == null || !contentRoots.ContentRoots.Any())
            {
                result.IsSuccess = false;
                result.Message = "Content roots cannot be null or empty";
                return result;
            }
            var targetPageType = LoadType(json.TargetPageType);
            var customParsers = GetColumnDataParsers();
            var columnMappings = new List<ExcelColumnMapping>();
            foreach (var columnMapping in json.ColumnMappings)
            {
                IColumnDataParser parser = null;
                if (columnMapping.ParserType != null) customParsers.TryGetValue(columnMapping.ParserType, out parser);
                columnMappings.Add(new ExcelColumnMapping()
                {
                    ExcelColumnNumber = columnMapping.ExcelColumnNumber,
                    Parser = parser,
                    PropertyIndicator = columnMapping.PropertyIndicator,
                    DefaultValue = columnMapping.DefaultValue,
                    UseDefaultValue = columnMapping.UseDefaultValue,
                    IgnoreIfNull = columnMapping.IgnoreIfNull,
                    Configuration = result.Configuration
                });
            }
            var defaultValueMappings = new List<ExcelDefaultValueMapping>();
            foreach (var defaultValueMapping in json.DefaultValueMappings)
            {
                IColumnDataParser parser = null;
                if (defaultValueMapping.ParserType != null) customParsers.TryGetValue(defaultValueMapping.ParserType, out parser);
                defaultValueMappings.Add(new ExcelDefaultValueMapping()
                {
                    Parser = parser,
                    PropertyIndicator = defaultValueMapping.PropertyIndicator,
                    DefaultValue = defaultValueMapping.DefaultValue,
                    UseDefaultValue = defaultValueMapping.UseDefaultValue,
                    IgnoreIfNull = defaultValueMapping.IgnoreIfNull,
                    Configuration = result.Configuration
                });
            }
            result.Configuration.ColumnMappings = columnMappings;
            result.Configuration.ContentRoots = contentRoots;
            result.Configuration.ExcelReader = excelReader;
            result.Configuration.ExcelValidators = excelValidators;
            result.Configuration.ExtraConfigurations = json.ExtraConfigurations;
            result.Configuration.RowIndicators = rowToContentIndicator;
            result.Configuration.DefaultValueMappings = defaultValueMappings;
            result.Configuration.TargetPageType = targetPageType;
            result.Configuration.RowStart = json.RowStart.Value;
            result.Configuration.SaveWithoutPublish = json.SaveWithoutPublish;
            return result;
        }

        public virtual ExcelContentRoot TranslateContentRoots(JsonContentRoot jsonContentRoot)
        {
            var result = new ExcelContentRoot();
            if (string.IsNullOrEmpty(jsonContentRoot.Value)) return null;
            if (jsonContentRoot.ContentRootType == ContentRootTypes.IDS)
            {
                var contentIds = jsonContentRoot.Value.Split(',');
                foreach (var id in contentIds)
                {
                    int contentId;
                    IContent content;
                    if (int.TryParse(id, out contentId) && _contentRepository.TryGet(new ContentReference(contentId), out content))
                    {
                        result.ContentRoots.Add(content);
                    }
                    else
                    {
                        result.ContentRoots = new List<IContent>();
                        break;
                    }
                }
            }
            else if (jsonContentRoot.ContentRootType == ContentRootTypes.FUNCTION)
            {
                var rootLoader = LoadInstance<IContentRootLoader>(jsonContentRoot.Value);
                if (rootLoader == null) return null;
                result.ContentRoots = rootLoader.GetRoots();
            }
            var dictChildrenLoaders = GetContentChildrenLoaders();
            IContentChildrenLoader childrenLoader;
            if (dictChildrenLoaders.TryGetValue(jsonContentRoot.GetChildren, out childrenLoader))
            {
                result.ChildrenLoader = childrenLoader;

            }
            return result;
        }

        public virtual bool IsValidJsonData(JsonImporterConfiguration json, out string message)
        {
            if (json == null)
            {
                message = "Json configuration cannot be empty";
                return false;
            }
            if (string.IsNullOrEmpty(json?.ExcelReader?.Type))
            {
                message = "Excel reader cannot be empty";
                return false;
            }
            if (string.IsNullOrEmpty(json?.ContentRoot?.Value))
            {
                message = "Content Root cannot be empty";
                return false;
            }
            if (string.IsNullOrEmpty(json?.RowToContentIndicator?.Type))
            {
                message = "Row To Content Indicator cannot be empty";
                return false;
            }
            if (json?.ColumnMappings == null || !json.ColumnMappings.Any())
            {
                message = "Column Mappings cannot be empty";
                return false;
            }
            if (string.IsNullOrEmpty(json?.TargetPageType))
            {
                message = "Target Page Type cannot be empty";
                return false;
            }
            message = string.Empty;
            return true;
        }

        private TAbstract LoadInstance<TAbstract>(string typeName)
        {
            try
            {
                var type = LoadType(typeName);
                if (!typeof(TAbstract).IsAssignableFrom(type)) return default(TAbstract);
                return (TAbstract)Activator.CreateInstance(type);
            }
            catch
            {
                return default(TAbstract);
            }
        }

        private Type LoadType(string typeName)
        {
            var type = Type.GetType(typeName, true, true);
            return type;
        }

        private Dictionary<string, IColumnDataParser> GetColumnDataParsers()
        {
            var result = new Dictionary<string, IColumnDataParser>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var parserTypes = assemblies.SelectMany(x => x.GetTypes()
            .Where(t => typeof(IColumnDataParser).IsAssignableFrom(t) && !t.IsAbstract));
            foreach (var instanceType in parserTypes)
            {
                var key = instanceType.Name;
                var attribute = (ExcelColumnParserAttribute)instanceType.GetCustomAttributes(typeof(ExcelColumnParserAttribute), false).FirstOrDefault();
                if (attribute != null && !string.IsNullOrEmpty(attribute.Name))
                {
                    key = attribute.Name;
                }
                if (!result.ContainsKey(key))
                {
                    result.Add(key, (IColumnDataParser)Activator.CreateInstance(instanceType));
                }
            }
            return result;
        }

        private Dictionary<string, IContentChildrenLoader> GetContentChildrenLoaders()
        {
            var result = new Dictionary<string, IContentChildrenLoader>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var parserTypes = assemblies.SelectMany(x => x.GetTypes()
            .Where(t => typeof(IContentChildrenLoader).IsAssignableFrom(t) && !t.IsAbstract));
            foreach (var instanceType in parserTypes)
            {
                var key = instanceType.Name;
                var attribute = (EpiContentChildrenLoaderAttribute)instanceType.GetCustomAttributes(typeof(EpiContentChildrenLoaderAttribute), false).FirstOrDefault();
                if (attribute != null && !string.IsNullOrEmpty(attribute.Name))
                {
                    key = attribute.Name;
                }
                if (!result.ContainsKey(key))
                {
                    result.Add(key, (IContentChildrenLoader)Activator.CreateInstance(instanceType));
                }
            }
            return result;
        }
    }
}