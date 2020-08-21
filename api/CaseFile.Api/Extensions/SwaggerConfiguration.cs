﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CaseFile.Api.Extensions
{
    public static class SwaggerConfiguration
    {
        public static IServiceCollection ConfigureSwagger(this IServiceCollection services)
        {
            return services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v2", new OpenApiInfo
                {
                    Version = "v2",
                    Title = "CaseFile ",
                    Description = "API specs for NGO Admin and Email operations.",
                    TermsOfService =
                        new Uri("https://github.com/code4romania/monitorizare-vot/blob/refactor/major-wip/LICENSE"),
                    Contact =
                        new OpenApiContact
                        {
                            Email = "info@casefile.ro",
                            Name = "FDP",
                            Url = new Uri("https://CaseFile.org")
                        },
                });

                options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference {Type = ReferenceType.SecurityScheme, Id = "bearer"}
                            },
                            new[] {"readAccess", "writeAccess"}

                        }
                    }
                );
                //options.OperationFilter<AddFileUploadParams>();

                var baseDocPath = Directory.GetCurrentDirectory();

                foreach (var api in Directory.GetFiles(baseDocPath, "*.xml", SearchOption.AllDirectories))
                {
                    options.IncludeXmlComments(api);
                }
            });
        }

        public static IApplicationBuilder UseSwaggerAndUi(this IApplicationBuilder app)
        {
            app.UseSwagger();

            // Enable middleware to serve swagger-ui assets (HTML, JS, CSS etc.)
            app.UseSwaggerUI(o => o.SwaggerEndpoint("/swagger/v2/swagger.json", "CaseFile API v2"));

            return app;
        }
    }
}
