using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace RU.Challenge.Presentation.API.Swagger
{
    public class FileUploadParameter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (operation.OperationId == "ApiReleasesByTitleByArtistIdByGenreIdPost")
            {
                var item = operation.Parameters.Single(e => e.Name == "coverArt");
                operation.Parameters.Remove(item);

                operation.Parameters.Add(new NonBodyParameter
                {
                    Name = "coverArt",
                    In = "formData",
                    Required = true,
                    Type = "file"
                });
                operation.Consumes.Add("multipart/form-data");
            }

            if (operation.OperationId == "ApiReleasesByReleaseIdTrackByNameByArtistIdByGenreIdPost")
            {
                var item = operation.Parameters.Single(e => e.Name == "song");
                operation.Parameters.Remove(item);

                operation.Parameters.Add(new NonBodyParameter
                {
                    Name = "song",
                    In = "formData",
                    Required = true,
                    Type = "file"
                });
                operation.Consumes.Add("multipart/form-data");
            }
        }
    }
}