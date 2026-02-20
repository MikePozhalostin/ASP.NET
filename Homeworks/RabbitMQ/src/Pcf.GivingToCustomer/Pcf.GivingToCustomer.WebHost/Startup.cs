using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pcf.GivingToCustomer.Core.Abstractions.Gateways;
using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.DataAccess;
using Pcf.GivingToCustomer.DataAccess.Data;
using Pcf.GivingToCustomer.DataAccess.Repositories;
using Pcf.GivingToCustomer.Integration;
using Pcf.GivingToCustomer.WebHost.Configurations;
using Pcf.GivingToCustomer.WebHost.Consumers;
using Pcf.GivingToCustomer.WebHost.Grpc;
using System;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;
using Query = Pcf.GivingToCustomer.WebHost.GraphQL.Query;

namespace Pcf.GivingToCustomer.WebHost
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddMvcOptions(x =>
                x.SuppressAsyncSuffixInActionNames = false);
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            services.AddScoped<INotificationGateway, NotificationGateway>();
            services.AddScoped<IDbInitializer, EfDbInitializer>();
            services.AddDbContext<DataContext>(x =>
            {
                x.UseSqlite("Filename=PromocodeFactoryGivingToCustomerDb.sqlite");
                //x.UseNpgsql(Configuration.GetConnectionString("PromocodeFactoryGivingToCustomerDb"));
                x.UseSnakeCaseNamingConvention();
                x.UseLazyLoadingProxies();
            });

            services.AddMassTransit(x =>
            {
                x.AddConsumer<PromoConsumer>();
                x.UsingRabbitMq((context, cfg) =>
                {
                    ConfigureRmq(cfg, Configuration);
                    RegisterEndPoints(cfg, context);
                });
            });

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            services.AddOpenApiDocument(options =>
            {
                options.Title = "PromoCode Factory Giving To Customer API Doc";
                options.Version = "1.0";
            });

            services.AddGrpc();
            services
                .AddGraphQLServer()
                .AddQueryType<Query>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IDbInitializer dbInitializer)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseOpenApi();
            app.UseSwaggerUi(x =>
            {
                x.DocExpansion = "list";
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGrpcService<CustomerGrpcService>();
                endpoints.MapGraphQL();
            });

            dbInitializer.InitializeDb();
        }

        /// <summary>
        /// Конфигурирование RMQ.
        /// </summary>
        /// <param name="configurator"> Конфигуратор RMQ. </param>
        /// <param name="configuration"> Конфигурация приложения. </param>
        private static void ConfigureRmq(IRabbitMqBusFactoryConfigurator configurator, IConfiguration configuration)
        {
            var rmqSettings = configuration.GetSection("RabbitMqConfiguration").Get<RabbitMqConfiguration>();
            configurator.Host(rmqSettings.Host,
                rmqSettings.VHost,
                h =>
                {
                    h.Username(rmqSettings.Login);
                    h.Password(rmqSettings.Password);
                });
        }

        /// <summary>
        /// регистрация эндпоинтов
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="context"></param>
        private static void RegisterEndPoints(IRabbitMqBusFactoryConfigurator configurator, IBusRegistrationContext context)
        {
            configurator.ReceiveEndpoint($"promoCodes", e =>
            {
                e.ConfigureConsumer<PromoConsumer>(context);
                e.UseMessageRetry(r =>
                {
                    r.Incremental(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
                });
            });
        }
    }
}