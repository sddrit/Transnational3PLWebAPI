using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Audit.Core;
using Audit.WebApi;
using DevExpress.AspNetCore;
using DevExpress.XtraReports.Services;
using Hangfire;
using Hangfire.SqlServer;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TransnationalLanka.ThreePL.Core.Environment;
using TransnationalLanka.ThreePL.Dal;
using TransnationalLanka.ThreePL.Dal.Entities;
using TransnationalLanka.ThreePL.Integration.Tracker;
using TransnationalLanka.ThreePL.Services.Account;
using TransnationalLanka.ThreePL.Services.Application;
using TransnationalLanka.ThreePL.Services.Delivery;
using TransnationalLanka.ThreePL.Services.Grn;
using TransnationalLanka.ThreePL.Services.Invoice;
using TransnationalLanka.ThreePL.Services.Metadata;
using TransnationalLanka.ThreePL.Services.Product;
using TransnationalLanka.ThreePL.Services.PurchaseOrder;
using TransnationalLanka.ThreePL.Services.Report;
using TransnationalLanka.ThreePL.Services.Setting;
using TransnationalLanka.ThreePL.Services.Stock;
using TransnationalLanka.ThreePL.Services.Supplier;
using TransnationalLanka.ThreePL.Services.Util;
using TransnationalLanka.ThreePL.Services.WareHouse;
using TransnationalLanka.ThreePL.WebApi.Util;
using TransnationalLanka.ThreePL.WebApi.Util.Enviroment;
using TransnationalLanka.ThreePL.WebApi.Util.Filters;
using TransnationalLanka.ThreePL.WebApi.Util.Middlewares;
using TransnationalLanka.ThreePL.WebApi.Util.Options;
using TransnationalLanka.ThreePL.WebApi.Util.Report;
using TransnationalLanka.ThreePL.WebApi.Util.Swagger;

namespace TransnationalLanka.ThreePL.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            Configuration = configuration;
            WebHostEnvironment = webHostEnvironment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment WebHostEnvironment { get; }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMediatR(typeof(ApplicationService).GetTypeInfo().Assembly);

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
            services.AddScoped<IEnvironment, WebEnvirnoment>();
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
            services.AddScoped(_ => new TrackerApiService(!WebHostEnvironment.IsProduction()));
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IInvoiceService, InvoiceService>();
            services.AddScoped<IReportProvider, ThreePlReportProvider>();
            services.AddScoped<ISettingService, SettingService>();
            services.AddScoped<ILogService, LogService>();

            // Add Hangfire services.
            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(Configuration.GetConnectionString("DbConnection"), new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                }));

            // Add the processing server as IHostedService
            services.AddHangfireServer();

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

            if (env.IsDevelopment()|| env.IsEnvironment("Uat"))
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TransnationalLanka.ThreePL.WebApi v1"));
            }

            Audit.Core.Configuration.Setup()
                .JsonAdapter<JsonNewtonsoftAdapter>()
                .UseSqlServer(config => config
                    .ConnectionString(Configuration.GetConnectionString("DbConnection"))
                    .Schema("dbo")
                    .TableName("Events")
                    .IdColumnName("Id")
                    .JsonColumnName("JsonData")
                    .CustomColumn("Updated", ev => DateTimeOffset.UtcNow)
                    .CustomColumn("Created", ev => DateTimeOffset.UtcNow)
                    .CustomColumn("EventType", ev => ev.EventType)
                    .CustomColumn("User", ev => ev.Environment.UserName));

            app.UseAuditMiddleware(_ => _
                .WithEventType("{verb}:{url}")
                .IncludeHeaders()
                .IncludeResponseHeaders()
                .IncludeResponseBody());

            app.Use(async (context, next) =>
            {
                context.Request.EnableBuffering();
                await next();
            });


            app.UseCors(builder =>
                builder.WithOrigins(Configuration["Origins"].Split(","))
                    .AllowAnyHeader()
                    .AllowAnyMethod()
            );

            app.UseDevExpressControls();

            app.UseMiddleware(typeof(ErrorHandlingMiddleware));

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseHangfireDashboard();

            RecurringJob.AddOrUpdate("invoice-generate-job",(IInvoiceService invoiceService) => invoiceService.GenerateInvoices(), Cron.Daily);

            //Initial the application
            var applicationService = app.ApplicationServices.GetService<IApplicationService>();
            applicationService?.Initial().Wait();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
