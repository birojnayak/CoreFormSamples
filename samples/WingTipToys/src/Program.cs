﻿using System.Runtime.Loader;
using System.Web.Optimization;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Web.Routing;
using Microsoft.Extensions.FileProviders;

namespace WingtipToys
{
    public class Program
    {
        static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDataProtection();

            builder.Services.AddSession();
            builder.Services.AddDistributedMemoryCache();


            builder.Services.AddSystemWebAdapters()
                 .AddJsonSessionSerializer(options =>
                 {
                     // Serialization/deserialization requires each session key to be registered to a type
                     options.RegisterKey<string>("CartId");
                 })
                .AddPreApplicationStartMethod(false)
                .AddJsonSessionSerializer()
                .AddHttpApplication<Global>()
                .AddWrappedAspNetCoreSession()
                .AddRouting()
                .AddWebForms()
                .AddScriptManager()
                .AddOptimization()
                .AddDynamicPages();


            var app = builder.Build();

            foreach (var staticPath in new[] { "Content", "Images", "Catalog", "fonts", "Scripts" })
            {
                app.UseStaticFiles(new StaticFileOptions()
                {
                    FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath, staticPath)),
                    RequestPath = "/" + staticPath,
                });
            }
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseSession();
            app.UseSystemWebAdapters();

            app.Services.GetRequiredService<IHostApplicationLifetime>().ApplicationStarted.Register(() =>
            {
                RouteTable.Routes.MapPageRoute("Index", "Index", "~/Default.aspx");
            });

            app.MapHttpHandlers();
            app.MapScriptManager();
            app.MapBundleTable();

            app.Run();
        }
    }
}
