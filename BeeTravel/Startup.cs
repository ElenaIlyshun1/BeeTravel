using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using BeeTravel.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BeeTravel.Entities;

using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using BeeTravel.Service;
using Microsoft.AspNetCore.Authentication.Cookies;
using BeeTravel.Interfaces;
using BeeTravel.Data.Repository;
using BeeTravel.Data.Entities;
using Microsoft.Extensions.FileProviders;
using System.IO;
using BeeTravel.SMTPSend;

namespace BeeTravel
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Configuration.Bind("Project", new Config());
            //services.AddDbContext<ApplicationDbContext>(options =>
            //    options.UseSqlServer(
            //        Configuration.GetConnectionString("DefaultConnection")));
            var con = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));
            services.AddTransient<ITourRepository, TourRepository>();
            services.AddIdentity<DbUser, DbRole>(options => {
                options.Stores.MaxLengthForKeys = 128;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 5;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
            services.AddControllersWithViews();
            services.AddRazorPages();

            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.Configure<RequestLocalizationOptions>(
                opt =>
                {
                    var supportedCulteres = new List<CultureInfo>
                    {
                       new CultureInfo("en"),
                       new CultureInfo("ru"),
                       new CultureInfo("kr"),
                       new CultureInfo("uk")
                    };
                    opt.DefaultRequestCulture = new RequestCulture("en");
                    opt.SupportedCultures = supportedCulteres;
                    opt.SupportedUICultures = supportedCulteres;

                });
            //відправка листів
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddControllersWithViews()
                .AddViewLocalization();
            services.AddSession();
            services.AddAuthentication().AddGoogle(options =>
            {
                options.ClientId = "277214799085-bi5ji74kedv0m653mnfvirqd6sabrdct.apps.googleusercontent.com";
                options.ClientSecret = "qPLofxG5--rnI__T5sPiRUFE";
            }).AddFacebook(options =>
            {
                options.AppId = "902807517171429";
                options.AppSecret = "82fa380ab080cfc770dc357c7a2fd9c9";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseStatusCodePagesWithRedirects("/error/{0}");
                app.UseStatusCodePagesWithReExecute("/error/{0}");
            }
            app.UseStaticFiles();

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                 Path.Combine(env.ContentRootPath, "Uploads")),
                RequestPath = "/Files"
            });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();
            app.UseUserDestroyer();

            app.UseRequestLocalization(app.ApplicationServices.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
            using (var scope = app.ApplicationServices.CreateScope())
            {
                ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
               // SeederDB.SeedData(app.ApplicationServices, env, this.Configuration);

            }
        }
    }
}
