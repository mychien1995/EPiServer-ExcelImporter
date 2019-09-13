using EPiServer.ExcelImporter.Configurations;
using EPiServer.ExcelImporter.JsonConfigurations;
using EPiServer.ExcelImporter.Services.Translators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPiServer.ExcelImporter.Services
{
    public interface IJsonToConfigTranslator
    {
        void FillDefaultValue(string input, out JsonImporterConfiguration json);
        ExcelImporterTranslationResult Translate(JsonImporterConfiguration json);

        bool IsValidJsonData(JsonImporterConfiguration json, out string message);
    }
}