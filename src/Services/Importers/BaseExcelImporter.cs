using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using EPiServer;
using EPiServer.Core;
using EPiServer.Logging;
using EPiServer.ServiceLocation;
using EPiServer.ExcelImporter.Configurations;
using EPiServer.ExcelImporter.Excels;
using EPiServer.ExcelImporter.JsonConfigurations;
using EPiServer.ExcelImporter.Models;

namespace EPiServer.ExcelImporter.Services.Importers
{

    public delegate void AfterImportHandler(object sender, EventArgs e);
    public class BaseExcelImporter : IExcelImporter
    {
        protected readonly IJsonToConfigTranslator _configTranslator;
        protected readonly IContentRepository _contentRepository;
        protected static readonly ILogger _logger = LogManager.GetLogger(typeof(BaseExcelImporter));

        public event AfterImportHandler AfterImport;

        public BaseExcelImporter(IJsonToConfigTranslator configTranslator, IContentRepository contentRepository)
        {
            _configTranslator = configTranslator;
            _contentRepository = contentRepository;
        }

        public virtual ExcelImportResult Import(string jsonContent, ExcelWorkbook workbook)
        {
            JsonImporterConfiguration jsonConfiguration = new JsonImporterConfiguration();
            _configTranslator.FillDefaultValue(jsonContent, out jsonConfiguration);
            return Import(jsonConfiguration, workbook);
        }

        /// <summary>
        /// Public interface for the core DoImport function
        /// </summary>
        /// <param name="jsonConfiguration"></param>
        /// <param name="workbook"></param>
        /// <returns></returns>
        public virtual ExcelImportResult Import(JsonImporterConfiguration jsonConfiguration, ExcelWorkbook workbook)
        {
            ExcelImportResult importResult = new ExcelImportResult();
            ExcelImporterConfiguration importerConfiguration;
            var configTranslationResult = _configTranslator.Translate(jsonConfiguration);
            if (!configTranslationResult.IsSuccess)
            {
                _logger.Error(configTranslationResult.Message);
                importResult.AddMessage("Import Configuration Invalid: " + configTranslationResult.Message);
                return importResult;
            }
            importerConfiguration = configTranslationResult.Configuration;
            return DoImport(workbook, importerConfiguration);
        }


        /// <summary>
        /// Step by step import.
        /// 1. Read the excel file with IExcelReader
        /// 2. Validate all rows wih IExcelValidators
        /// 3. Get target contents based on content roots and target paget type
        /// 4. Loop through all rows, find matching content using IRowToContentIndicator, update / insert content
        /// </summary>
        /// <param name="workbook"></param>
        /// <param name="importerConfiguration"></param>
        /// <returns></returns>
        public virtual ExcelImportResult DoImport(ExcelWorkbook workbook, ExcelImporterConfiguration importerConfiguration)
        {
            ExcelImportResult importResult = new ExcelImportResult();
            List<IContent> updatedContents = new List<IContent>();
            try
            {
                importResult.IsSuccess = true;
                var excelReader = importerConfiguration.ExcelReader;
                var rawExcelData = excelReader.Read(workbook, importerConfiguration);
                importResult.ExcelRowCount = rawExcelData.Count;
                var validationResults = ValidateData(rawExcelData, importerConfiguration.ExcelValidators);
                if (validationResults.Any())
                {
                    importResult.IsSuccess = false;
                    foreach (var validationResult in validationResults)
                    {
                        importResult.AddMessage(validationResult.Message);
                    }
                    return importResult;
                }

                var targetPageType = importerConfiguration.TargetPageType;
                var targetContents = GetTargetContents(importerConfiguration.ContentRoots, targetPageType);
                var rowToContentIndicator = importerConfiguration.RowIndicators;
                for (int i = 0; i < rawExcelData.Count; i++)
                {
                    RowImportResult rowImportResult = null;
                    var row = rawExcelData[i];
                    var rowNumber = importerConfiguration.RowStart + i;
                    IContent contentToModify = null;
                    var decision = rowToContentIndicator.FindContent(row, rowNumber, targetContents, out contentToModify);
                    if (contentToModify == null)
                    {
                        decision = RowToContentFinderResult.ShouldIgnore;
                    }
                    switch (decision)
                    {
                        case RowToContentFinderResult.ShouldIgnore:
                            importResult.AddMessage($"Sheet: {rawExcelData[i].SheetName} - Row {rawExcelData[i].IndexInSheet} ignored. No matching content found");
                            importResult.NoOfContentIgnored++;
                            continue;
                        case RowToContentFinderResult.ShouldUpdate:
                            rowImportResult = DoUpdate(row, rowNumber, contentToModify, importerConfiguration);
                            break;
                        case RowToContentFinderResult.ShouldInsert:
                            rowImportResult = DoInsert(row, rowNumber, contentToModify, importerConfiguration);
                            break;
                        default:
                            break;
                    }
                    if (rowImportResult != null)
                    {
                        importResult.AddMessage(rowImportResult.Message);
                        if (rowImportResult.IsSuccess)
                        {
                            switch (decision)
                            {
                                case RowToContentFinderResult.ShouldUpdate:
                                    updatedContents.Add(rowImportResult.Content);
                                    importResult.NoOfContentUpdated++;
                                    break;
                                case RowToContentFinderResult.ShouldInsert:
                                    importResult.NoOfContentInserted++;
                                    break;
                                default:
                                    continue;
                            }
                        }
                        else
                        {
                            importResult.NoOfContentErrors++;
                        }
                    }
                }
                AfterImport?.Invoke(this, new AfterImportEventArgs()
                {
                    UpdatedContents = updatedContents,
                    Configuration = importerConfiguration
                });
            }
            catch (Exception ex)
            {
                importResult.IsSuccess = false;
                importResult.Message.Add(ex.Message);
                _logger.Error("Office Excel Import Error", ex);
            }
            return importResult;
        }

        public virtual RowImportResult DoUpdate(ExcelRange row, int rowNumber, IContent content, ExcelImporterConfiguration configuration)
        {
            return null;
        }

        public virtual RowImportResult DoInsert(ExcelRange row, int rowNumber, IContent content, ExcelImporterConfiguration configuration)
        {
            return null;
        }

        private List<ExcelDataValidationResult> ValidateData(List<ExcelRange> rows, List<IExcelValidator> validators)
        {
            return rows.SelectMany(row => validators.Select(x => x.Validate(row))).Where(x => !x.IsValid).ToList();
        }

        private List<IContent> GetTargetContents(ExcelContentRoot contentRoots, Type targetPageType)
        {
            var childrenLoader = contentRoots.ChildrenLoader;
            return contentRoots.ContentRoots.SelectMany(x => childrenLoader.GetChildren(x, targetPageType)).ToList();
        }
    }
}