using Swashbuckle.AspNetCore.SwaggerGen;

namespace PostService.Config
{
    public static class SwaggerExtensions
    {
        public static void AddSwaggerGenMultipartSupport(this SwaggerGenOptions options)
        {
            options.OperationFilter<FileUploadOperationFilter>();
        }
    }
}
