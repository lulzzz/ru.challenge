using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace RU.Challenge.Presentation.API.Swagger
{
    public class AddAuthorizationHeaderParameter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (operation.OperationId != "ApiAuthRegisterPost")
            {
                if (operation.Parameters == null)
                    operation.Parameters = new List<IParameter>();

                operation.Parameters.Add(new NonBodyParameter()
                {
                    Name = "Authorization",
                    Required = true,
                    Type = "string",
                    In = "header",
                });
            }
        }
    }
}