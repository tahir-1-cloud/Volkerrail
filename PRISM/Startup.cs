using System;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using PRISM.Models;
using PRISM.Services.Interfaces;
using PRISM.Services;
using Microsoft.CodeAnalysis.RulesetToEditorconfig;
using PRISM.Filters;
using DinkToPdf.Contracts;
using DinkToPdf;

namespace active_directory_aspnetcore_webapp_openidconnect_v2
{
    public class Startup
	{
	//	private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _hostingEnvironment;


		public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
           // _hostingEnvironment = env;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var initialScopes = Configuration.GetValue<string>("DownstreamApi:Scopes")?.Split(' ');

            services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApp(Configuration.GetSection("AzureAd"))
                    .EnableTokenAcquisitionToCallDownstreamApi(initialScopes)
                        .AddMicrosoftGraph(Configuration.GetSection("DownstreamApi"))
                        .AddInMemoryTokenCaches();


			//var architectureFolder = (IntPtr.Size == 8) ? "64 bit" : "32 bit";
			//var wkHtmlToPdfPath = Path.Combine(_hostingEnvironment.ContentRootPath, $"wkhtmltox\\v0.12.4\\{architectureFolder}\\libwkhtmltox");
           // CustomAssemblyLoadContext context = new CustomAssemblyLoadContext();
           // context.LoadUnmanagedLibrary(wkHtmlToPdfPath);
            //services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));          //Other codes...


            services.AddControllersWithViews(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            }).AddJsonOptions(options =>
             {
                 options.JsonSerializerOptions.PropertyNamingPolicy = null;
             });

            services.AddDbContext<PRISMContext>(options =>
             options.UseSqlServer(Configuration.GetConnectionString("ConnStr")));

            services.AddRazorPages()
                .AddMicrosoftIdentityUI();


            services.AddDistributedMemoryCache(); // Use an in-memory cache for demo purposes, consider using a distributed cache in production
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(180); // Set the session timeout
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            services.AddScoped<ILookupServices, LookupServices>();
            services.AddScoped<IRoster, RosterServices>();
            services.AddScoped<IAbsances, Absanceservices>();
            services.AddScoped<IDistributions, DistributionsServices>();
            services.AddScoped<IContacts, ContactsServices>();
            services.AddScoped<IEmployees, EmployeesServices>();
            services.AddScoped<IMachines, MachinesServices>();
            services.AddScoped<IRoutes, RoutesServices>();
            services.AddScoped<IHomeServices, HomeServices>();
            services.AddTransient<IReportServices, ReportServices>();
            services.AddTransient<IApiServices, ApiServices>();
            services.AddTransient<IChangeLogServices, ChangeLogServices>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
