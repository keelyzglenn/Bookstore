using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Bookstore.Models;

namespace Bookstore
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

     
        public void ConfigureServices(IServiceCollection services)
        {
            
            services.AddMvc();
            services.AddEntityFramework()
              .AddDbContext<BookstoreDbContext>(options =>
                  options.UseSqlServer(Configuration["ConnectionStrings:DefaultConnection"]));
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            var context = app.ApplicationServices.GetService<BookstoreDbContext>();
            AddTestData(context);

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions {
                    HotModuleReplacement = true
                });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });
            app.Run(async (context1) =>
            {
                await context1.Response.WriteAsync("Hello World!");
            });
        }

        private static void AddTestData(BookstoreDbContext context)
        {
            context.Database.ExecuteSqlCommand("Delete From Books");

            var book1 = new Book();
            book1.Title = "Anna Karenina";
            book1.Author = "Leo Tolstoy";
            context.Books.Add(book1);

            var book2 = new Book();
            book2.Title = "Into the Wildnerness";
            book2.Author = "Sarah Donati";
            context.Books.Add(book2);

            var book3 = new Book();
            book3.Title = "Harry Potter, Chamber of Secrets";
            book3.Author = "JK Rowling";
            context.Books.Add(book3);

            context.SaveChanges();
        }
    }
}
