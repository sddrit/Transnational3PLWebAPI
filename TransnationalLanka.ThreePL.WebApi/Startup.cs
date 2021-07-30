using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using DevExpress.AspNetCore;
using DevExpress.XtraReports.Services;
using DevExpress.XtraReports.Web.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TransnationalLanka.ThreePL.Dal;
using TransnationalLanka.ThreePL.Dal.Entities;
using TransnationalLanka.ThreePL.Integration.Tracker;
using TransnationalLanka.ThreePL.Services.Account;
using TransnationalLanka.ThreePL.Services.Application;
using TransnationalLanka.ThreePL.Services.Delivery;
using TransnationalLanka.ThreePL.Services.Grn;
using TransnationalLanka.ThreePL.Services.Metadata;
using TransnationalLanka.ThreePL.Services.Product;
using TransnationalLanka.ThreePL.Services.PurchaseOrder;
using TransnationalLanka.ThreePL.Services.Report;
using TransnationalLanka.ThreePL.Services.Stock;
using TransnationalLanka.ThreePL.Services.Supplier;
using TransnationalLanka.ThreePL.Services.Util;
using TransnationalLanka.ThreePL.Services.WareHouse;
using TransnationalLanka.ThreePL.WebApi.Util;
using TransnationalLanka.ThreePL.WebApi.Util.Filters;
using TransnationalLanka.ThreePL.WebApi.Util.Middlewares;
using TransnationalLanka.ThreePL.WebApi.Util.Options;
using TransnationalLanka.ThreePL.WebApi.Util.Report;
using TransnationalLanka.ThreePL.WebApi.Util.Swagger;

namespace TransnationalLanka.ThreePL.WebApi
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
            var authorizationConfiguration = Configuration.GetSection("Token");
            services.Configure<TokenConfiguration>(Configuration.GetSection("Token"));

            services.AddCors();
            services.AddAutoMapper(typeof(Startup));

            services.AddOptions();

            services.AddDbContextPool<ThreePlDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DbConnection")));

            services.AddScoped<ModelValidationAttribute>();
            services.AddScoped<ThreePlDbContext>();

            services.AddIdentity<User, Role>(options =>
                {
                    options.SignIn.RequireConfirmedEmail = false;
                    options.SignIn.RequireConfirmedPhoneNumber = false;
                })
                .AddEntityFrameworkStores<ThreePlDbContext>()
                .AddDefaultTokenProviders()
                .AddTokenProvider<TokenProvider>("Default");

            services.AddAuthentication()
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;

                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidIssuer = authorizationConfiguration["Issuer"],
                        ValidAudience = authorizationConfiguration["Issuer"],
                        IssuerSigningKey =
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authorizationConfiguration["Key"]))
                    };
                });

            services.AddDevExpressControls();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ICityService, CityService>();
            services.AddScoped<IMetadataService, MetadataService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IApplicationService, ApplicationService>();
            services.AddScoped<ISupplierService, SupplierService>();
            services.AddScoped<IWareHouseService, WareHouseService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
            services.AddScoped<IStockService, StockService>();
            services.AddScoped<IGrnService, GrnService>();
            services.AddScoped<IStockTransferService, StockTransferService>();
            services.AddScoped<IDeliveryService, DeliveryService>();
            services.AddScoped(_ => new TrackerApiService(true));
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IReportProvider, ThreePlReportProvider>();

            services.AddControllers()
                .ConfigureApplicationPartManager(appPart => {
                    var parts = appPart.ApplicationParts;
                    var aspNetCoreReportingAssemblyName = 
                        typeof(DevExpress.AspNetCore.Reporting.WebDocumentViewer.WebDocumentViewerController).Assembly.GetName().Name;
                    var reportingPart = parts.FirstOrDefault(part => part.Name == aspNetCoreReportingAssemblyName);
                    if (reportingPart != null)
                    {
                        parts.Remove(reportingPart);
                    }
                })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Transnational Lanka 3PL API Documentation",
                    Version = "v1",
                    Contact = new OpenApiContact
                    {
                        Name = "Sajith Lakjaya",
                        Email = "slakjaya@gmail.com"
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Apache 2.0",
                        Url = new Uri("http://www.apache.org/licenses/LICENSE-2.0.html")
                    }
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Scheme = "Bearer",
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,

                        },
                        new List<string>()
                    }
                });

                c.EnableAnnotations();

                c.DocumentFilter<SwaggerAddEnumDescriptions>();

            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TransnationalLanka.ThreePL.WebApi v1"));
            }

            app.UseCors(builder =>
                builder.WithOrigins(Configuration["Origins"].Split(","))
                    .AllowAnyHeader()
                    .AllowAnyMethod()
            );

            app.UseDevExpressControls();

            app.UseMiddleware(typeof(ErrorHandlingMiddleware));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            //Initial the application
            var applicationService = app.ApplicationServices.GetService<IApplicationService>();
            applicationService?.Initial().Wait();
        }
    }
}
