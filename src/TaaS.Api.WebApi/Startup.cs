using System;
using System.IO;
using System.Reflection;
using AspNetCoreRateLimit;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Polly;
using TaaS.Api.WebApi.Configuration.Swagger;
using TaaS.Api.WebApi.Hosted;
using TaaS.Core.Domain.Gratitude.Query.GetGratitudeById;
using TaaS.Infrastructure.Contract.Client;
using TaaS.Infrastructure.Contract.Service;
using TaaS.Infrastructure.Implementation.Client;
using TaaS.Infrastructure.Implementation.Service;
using TaaS.Persistence.Context;

namespace TaaS.Api.WebApi
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
            services.AddHttpContextAccessor();
            services.AddControllers();
            services.AddMemoryCache();

            services.AddHostedService<ImportHostedService>();
            
            services.AddMediatR(typeof(GetGratitudeByIdQuery).Assembly);
            
            #region Persistence

            services.AddDbContext<TaaSDbContext>(builder =>
            { 
                var connectionString = Configuration["DB_CONNECTION_STRING"];
                builder.UseNpgsql(connectionString);
            });

            #endregion

            #region Infrastructure

            services.AddHttpClient<IImporterClient, ImporterClient>()
                .AddTransientHttpErrorPolicy(builder =>
                    builder.WaitAndRetryAsync(3, retryCount =>
                        TimeSpan.FromSeconds(Math.Pow(2, retryCount))));

            services.AddScoped<IImporterService, ImporterService>();

            #endregion
            
            #region IpRateLimiting

            services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"));
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            #endregion

            #region Api Versioning
            
            services.AddApiVersioning(o =>
            {
                o.ReportApiVersions = true;
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new ApiVersion(1, 0);
            });
            
            #endregion

            #region SwaggerGen

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Thanks as a Service", Version = "v1" });
               
                c.OperationFilter<RemoveVersionFromParameter>();
                c.DocumentFilter<ReplaceVersionWithExactValueInPath>();
                
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            }); 

            #endregion
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
                app.UseHttpsRedirection();
            }
            
            app.UseStaticFiles();
            
            app.UseSwagger();
            
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "TaaS V1");
                c.RoutePrefix = string.Empty;
            }); 
            
            app.UseRouting();

            app.UseIpRateLimiting();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}