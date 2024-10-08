using Microsoft.Extensions.FileProviders;

namespace System.Blog.Web.Configurations;

public static class StaticFilesConfiguration
{
    public static void AddStaticFilesConfiguration(this IApplicationBuilder app)
    {
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(uploadsFolder),
            RequestPath = "/uploads"
        });
    }
}
