using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using CaseFile.Api.Core.Extensions;
using CaseFile.Api.Extensions;
using CaseFile.Entities;
using CaseFile.Api.Auth.Services;
using CaseFile.Api.Core.Services;

namespace CaseFile.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                                  builder =>
                                  {
                                      builder.WithOrigins("http://localhost:4200", "https://app-casefile-frontend-test.azurewebsites.net", "https://app-casefile-frontend-prod.azurewebsites.net")
                                            .AllowAnyMethod()
                                            .AllowAnyHeader()
                                            .AllowCredentials();
                                  });
            });

            services.AddControllers();
            services.ConfigureCustomOptions(Configuration);
            services.AddHashService(Configuration);
            services.AddFileService(Configuration);
            services.AddCaseFileAuthentication(Configuration);
            
            services.AddDbContext<CaseFileContext>(o => o.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddFirebase(Configuration);
            services.AddAutoMapper(GetAssemblies());
            services.AddMediatR(GetAssemblies().ToArray());

            services.ConfigureSwagger();
            services.AddApplicationInsightsTelemetry();
            services.AddCachingService(Configuration);
                        
            services.AddHttpClient();
            services.AddSingleton<IAuthy, Authy>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IEmailService, EmailService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            
            app.UseStaticFiles();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(MyAllowSpecificOrigins);

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSwaggerAndUi();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private IEnumerable<Assembly> GetAssemblies()
        {
            yield return Assembly.GetAssembly(typeof(Startup));

            yield return typeof(Answer.Controllers.AnswersController).GetTypeInfo().Assembly;
            yield return typeof(Auth.Controllers.Authorization).GetTypeInfo().Assembly;
            yield return typeof(County.Controllers.CountyController).GetTypeInfo().Assembly;            
            yield return typeof(Form.Controllers.FormController).GetTypeInfo().Assembly;            
            yield return typeof(Note.Controllers.NoteController).GetTypeInfo().Assembly;                        
            yield return typeof(Business.Controllers.UserController).GetTypeInfo().Assembly;            
            yield return typeof(Core.Handlers.UploadFileHandler).GetTypeInfo().Assembly;

        }
    }
}
