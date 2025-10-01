using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.DataAccess.Data;
using PromoCodeFactory.DataAccess.Repositories;

namespace PromoCodeFactory.WebHost
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddDbContext<DataContext>(d =>
            {
                d.UseNpgsql(_configuration.GetConnectionString("DefaultConnection")
                .Replace("{USERNAME}", _configuration["mysecretconfig:postgres-username"])
                .Replace("{PASSWORD}", _configuration["mysecretconfig:postgres-password"]));

                //d.UseSqlite("Data Source=MyDatabase.db");
                d.UseLazyLoadingProxies();
                d.UseAsyncSeeding(async (context, _, ct) => await DataContextInitializer.SeedAsync((DataContext)context, ct));
                d.UseSeeding((context, _) => DataContextInitializer.Seed((DataContext)context));
            });

            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

            services.AddOpenApiDocument(options =>
            {
                options.Title = "PromoCode Factory API Doc";
                options.Version = "1.0";
            });
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
                app.UseHsts();
            }

            app.UseOpenApi();
            app.UseSwaggerUi(x =>
            {
                x.DocExpansion = "list";
            });

            using var scope = app.ApplicationServices.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<DataContext>();

            //db.Database.EnsureDeletedAsync();
            db.Database.EnsureCreatedAsync();
            db.Database.MigrateAsync();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}