using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.XPath;

namespace WCM.API.ApiService.Infrastructure.OpenApi;

/// <summary>
/// OpenAPI document transformer that enriches schemas with XML documentation comments.
/// </summary>
internal sealed class XmlCommentsTransformer : IOpenApiDocumentTransformer
{
    private readonly XDocument? _xmlDoc;

    public XmlCommentsTransformer()
    {
        string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

        if (File.Exists(xmlPath))
        {
            _xmlDoc = XDocument.Load(xmlPath);
        }
    }

    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        if (_xmlDoc == null)
        {
            return Task.CompletedTask;
        }

        if (document.Components?.Schemas != null)
        {
            foreach (KeyValuePair<string, IOpenApiSchema> schema in document.Components.Schemas)
            {
                string typeName = schema.Key;

                XElement? xmlNode = _xmlDoc.XPathSelectElement($"//member[starts-with(@name, 'T:') and contains(@name, '{typeName}')]");

                if (xmlNode != null)
                {
                    string? summary = xmlNode.Element("summary")?.Value.Trim();
                    if (!string.IsNullOrEmpty(summary))
                    {
                        schema.Value.Description = summary;
                    }
                }

                if (schema.Value.Properties != null)
                {
                    foreach (KeyValuePair<string, IOpenApiSchema> property in schema.Value.Properties)
                    {
                        string propertyName = property.Key;

                        XElement? propertyNode = _xmlDoc.XPathSelectElement(
                            $"//member[starts-with(@name, 'P:') and contains(@name, '{typeName}.{propertyName}')]");

                        if (propertyNode != null)
                        {
                            string? propertySummary = propertyNode.Element("summary")?.Value.Trim();
                            if (!string.IsNullOrEmpty(propertySummary))
                            {
                                property.Value.Description = propertySummary;
                            }
                        }
                    }
                }
            }
        }

        return Task.CompletedTask;
    }
}
