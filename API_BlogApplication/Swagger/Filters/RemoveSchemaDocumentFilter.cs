using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_BlogApplication.Swagger.Filters
{
    public class RemoveSchemaDocumentFilter:IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {

            IDictionary<string, OpenApiSchema> _remove = swaggerDoc.Components.Schemas;

            foreach (KeyValuePair<string, OpenApiSchema> _item in _remove)
            {
                swaggerDoc.Components.Schemas.Remove(_item.Key);
            }
        }
    }
}
