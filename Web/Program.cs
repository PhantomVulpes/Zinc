using Vulpes.Zinc.Domain.Extensions;
using Vulpes.Zinc.External.Extensions;
using Vulpes.Zinc.External.Mongo;

namespace Vulpes.Zinc.Web;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        MongoConfigurator.Configure();

        // Add services to the container.
        _ = builder.Services.AddRazorPages();

        _ = builder.Services
            .InjectDomain()
            .InjectExternal()
            ;

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            _ = app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            _ = app.UseHsts();
        }

        _ = app.UseHttpsRedirection();
        _ = app.UseStaticFiles();

        _ = app.UseRouting();

        _ = app.UseAuthorization();

        _ = app.MapRazorPages();

        app.Run();
    }
}