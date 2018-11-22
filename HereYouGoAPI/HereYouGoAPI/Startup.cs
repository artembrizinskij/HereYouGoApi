using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using DAl.Sql;
using DAl.Sql.Services;
using Domain.Entities;
using Domain.ViewModels;
using Infastructure.Extensions;
using Logic.AutoMapperProfiles;
using Logic.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Security.Providers;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace HereYouGoAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            SetConnectionString(services);
            AddTransients(services);
            AuthConfigure(services);
            services.AddSwaggerDocumentation();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseAuthentication();
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseHsts();

            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<CommonContext>();
                context.Database.Migrate();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
            Mapper.Initialize(cfg => {});
            app.UseStaticFiles();
            app.UseSwagger();
            app.UseSwaggerDocumentation();
        }

        private void AuthConfigure(IServiceCollection services)
        {
            var tokenProvider = new JwtTokenProvider("HereYouGoApi", "http://localhost:8888");
            services.AddSingleton<ITokenProvider>(tokenProvider);
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = tokenProvider.GetValidationParameters();
                });

            services.AddAuthorization(auth => {
                auth.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();
            });
        }

        private void AddTransients(IServiceCollection services)
        {
            services.AddTransient<ICommonDbService, CommonDbService>();
            services.AddTransient<IAccountService, AccountService>();
            services.AddTransient<IBlockchainService, Web3Service>();
            services.AddSingleton<IContextProvider, ContextProvider>();
        }

        private void SetConnectionString(IServiceCollection services) =>
                services.AddDbContext<CommonContext>(options => options.UseSqlServer(Configuration.GetConnectionString("SqlConnection")));

    }
}
