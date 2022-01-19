using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using TransnationalLanka.ThreePL.ClientApi.Util.Authorization;
using TransnationalLanka.ThreePL.ClientApi.Util.Enviroment;
using TransnationalLanka.ThreePL.Core.Environment;
using TransnationalLanka.ThreePL.Dal;
using TransnationalLanka.ThreePL.Dal.Entities;
using TransnationalLanka.ThreePL.Integration.Tracker;
using TransnationalLanka.ThreePL.Services.Account;
using TransnationalLanka.ThreePL.Services.Api;
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

namespace TransnationalLanka.ThreePL.ClientApi
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

            services.AddCors();
            services.AddAutoMapper(typeof(Startup));

            services.AddOptions();

            services.AddDbContextPool<ThreePlDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DbConnection")));

            services.AddScoped<ThreePlDbContext>();

            services.AddIdentity<User, Role>(options =>
                {
                    options.SignIn.RequireConfirmedEmail = false;
                    options.SignIn.RequireConfirmedPhoneNumber = false;
                })
                .AddEntityFrameworkStores<ThreePlDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(SupplierAuthOptions.DefaultScemeName)
                .AddScheme<SupplierAuthOptions, SupplierAuthHandler>(
                    SupplierAuthOptions.DefaultScemeName,
                    opts =>
                    {

                    }
                );

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IEnvironment, ClientApiEnviroment>();
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
            services.AddScoped<ISettingService, SettingService>();
            services.AddScoped<IApiAccountService, ApiAccountService>();
            services.AddScoped<ILogService, LogService>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TransnationalLanka.ThreePL.ClientApi", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseReDoc(c =>
            {
                c.DocumentTitle = "Transnational Lanka 3PL Api Documentation";
                c.SpecUrl = "/swagger/v1/swagger.json";
                c.RoutePrefix = "docs";
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
