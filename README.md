# EPiServer-ExcelImporter
Excel Import tool for EpiServer content. 

Based on EPiServer CMS Core and the ClosedXMl library.

Rich set of configuration allow developer to be as flexible as possible.

I used a json file as configuration mapping between Excel data and EpiServer content.

The configuration can be described as below

```javascript
{
    "excelReader": { 
    	//The type indicator of the excel reader class, must inherit from IExcelReader
    	"type" : "dll.namespace.classname, dll"
    }
    "excelValidators": [
    	//The type indicator of the excel validators class, must inherit from IExcelValidator
    	{
    		"type" : "dll.namespace.classname, dll"
    	},
    	{
    		"type" : "dll.namespace.classname, dll"
    	}
    ],
    "rowToContentIndicator":
    {
    	//The row indicator to map from content to row, must inherit from IRowToContentIndicator
        "type": "dll.namespace.classname, dll"
    },
    "contentRoot":
    {
        "contentRootType": "ID", //ID or function, if ID then Value must be a number, if function then value is the type indicator of that class
        "value": "1678940",
        "getChildren": "DescendantsContentChildrenLoader" //loader method to get the children of content roots, default support Descendants and DirectChildren, you can extend this
    },
    "targetPageType": "dll.namespace.classname, dll",
    "columnMappings": [ //Mapping between excel column and epi's properties
    {
        "colNumber": 2,  //Excel col number
        "propertyIndicator": "MainBody.BannerBlock.BackgroundImage", //This can be <PropertyName> or <ContentAreaPropertyName>.<TargetBlock>.<TargetBlockPropertyName> or <LocalBlock>.<PropertyName>
        "parser": "ImageLinkToMedia", //Excel value to epi value parse, you can write your own parser
        "defaultValue": null,
        "useDefaultValue": false,
        "ignoreIfNull": false
    },
    {
        "colNumber": 3,
        "propertyIndicator": "MainBody.RichTextBlock.Description",
        "parser": "StringToRichText",
    },
    {
        "colNumber": 4,
        "propertyIndicator": "MainBody.LinkListBlock.Services",
        "parser": "LinkListToContentReferenceList",
    }],
    "defaultValuesMappings": [
    {
        "propertyIndicator": "MainBody.SomeBlock.Title",
        "defaultValue": "Here is the title",
        "useDefaultValue": false,
        "ignoreIfNull": false
    }],
    "extraConf":
    {
    },
    "rowStart": 2,
    "saveWithoutPublish": false
}
```
