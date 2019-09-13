using EPiServer;
using EPiServer.Core;
using EPiServer.DataAccess;
using EPiServer.ServiceLocation;
using EPiServer.Security;
using EPiServer.ExcelImporter.Configurations;
using EPiServer.ExcelImporter.Excels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EPiServer.ExcelImporter.Models;
using EPiServer.Logging;

namespace EPiServer.ExcelImporter.Services.Importers
{
    [ServiceConfiguration(typeof(IExcelImporter))]
    public class DefaultExcelImporter : BaseExcelImporter
    {
        private readonly IPropertyIndicatorHelper _propertyIndicatorHelper;
        public DefaultExcelImporter(IJsonToConfigTranslator configTranslator, IContentRepository contentRepository,
            IPropertyIndicatorHelper propertyIndicatorParser) : base(configTranslator, contentRepository)
        {
            _propertyIndicatorHelper = propertyIndicatorParser;
        }
        public override RowImportResult DoUpdate(ExcelRange rowData, int rowNumber, IContent content, ExcelImporterConfiguration configuration)
        {
            RowImportResult result = new RowImportResult();
            result.IsSuccess = true;
            result.Message = $"Update data from excel successfully. Sheet {rowData.SheetName} - row {rowData.IndexInSheet}. Content: {content.ContentLink} - {content.Name}";
            try
            {
                var contentData = content as ContentData;
                var contentToModify = (PageData)contentData.CreateWritableClone();
                var columnMappings = configuration.ColumnMappings;
                foreach (var columnMapping in columnMappings)
                {
                    var colNumber = columnMapping.ExcelColumnNumber;
                    var parser = columnMapping.Parser;
                    var excelValue = rowData.Range.Cell(1, colNumber).GetValue<object>();
                    if ((excelValue == null || string.IsNullOrEmpty(excelValue + "")) && columnMapping.UseDefaultValue)
                    {
                        excelValue = columnMapping.DefaultValue;
                    }
                    var propertyIndicator = columnMapping.PropertyIndicator;
                    _propertyIndicatorHelper.SaveProperty(contentToModify, columnMapping, excelValue);
                    //ReloadContent(contentToModify);
                }
                var defaultMappings = configuration.DefaultValueMappings;
                foreach (var defaultMapping in defaultMappings)
                {
                    var parser = defaultMapping.Parser;
                    var defaultValue = defaultMapping.DefaultValue;
                    var propertyIndicator = defaultMapping.PropertyIndicator;
                    _propertyIndicatorHelper.SaveProperty(contentToModify, defaultMapping, defaultValue);
                    //ReloadContent(contentToModify);
                }
                result.Content = contentToModify;
            }
            catch (Exception ex)
            {
                _logger.Error("Update data from excel error: ", ex);
                result.IsSuccess = false;
                result.Message = $"Update data from excel error: {ex.Message}. Issue at sheet {rowData.SheetName} - row {rowData.IndexInSheet}. Content Id: {content.ContentLink}";
            }
            return result;
        }

        private void ReloadContent(IContent content)
        {
            content = _contentRepository.Get<IContent>(content.ContentLink);
        }
    }
}