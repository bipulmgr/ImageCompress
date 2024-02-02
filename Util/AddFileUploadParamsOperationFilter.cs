using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ImageCompressApi.Util;
public class AddFileUploadParamsOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.Parameters == null)
            return;

        var fileParams = operation.Parameters
            .Where(p => p.In == ParameterLocation.Query && p.Schema?.Type == "file")
            .ToList();

        foreach (var fileParam in fileParams)
        {
            operation.Parameters.Remove(fileParam);
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = fileParam.Name,
                In = ParameterLocation.Header,
                Required = true,
                Schema = new OpenApiSchema { Type = "array", Items = new OpenApiSchema { Type = "string", Format = "binary" } },
                Style = ParameterStyle.Form
            });
        }
    }
}
