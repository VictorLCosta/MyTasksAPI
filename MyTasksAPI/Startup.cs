using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using MyTasksAPI.Database;
using Microsoft.EntityFrameworkCore;
using MyTasksAPI.V1.Repositories;
using MyTasksAPI.V1.Repositories.Contracts;
using MyTasksAPI.V1.Models;
using MyTasksAPI.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.Formatters;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace MyTasksAPI
{
    public class Startup
    {
        public IConfiguration _conf;

        public Startup(IConfiguration conf)
        {
            _conf = conf;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(op => {
                op.SuppressModelStateInvalidFilter = true;
            });

            services.AddDbContext<MyTasksContext>(op => 
            {
                op.UseSqlite(_conf.GetConnectionString("MyTasksContext"));
            });

            services.AddMvc(config => { 
                config.ReturnHttpNotAcceptable = true;
                config.InputFormatters.Add(new XmlSerializerInputFormatter(config));
                config.OutputFormatters.Add(new XmlSerializerOutputFormatter()); 
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddJsonOptions(opt =>
                    opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                );

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<MyTasksContext>()
                .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(opt => {
                opt.Events.OnRedirectToLogin = context => {
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };
            });
            
            #region Versioning
            services.AddApiVersioning(cfg =>{
                cfg.ReportApiVersions = true;
                cfg.AssumeDefaultVersionWhenUnspecified = true;
                cfg.DefaultApiVersion = new ApiVersion (1, 0);
            });

            services.AddSwaggerGen(cfg => {
                cfg.AddSecurityDefinition("Bearer", new ApiKeyScheme()
                {
                    In = "header",
                    Type = "apiKey",
                    Description = "Adicione o JSON Web Token (JWT) para autenticar",
                    Name = "Authorization"
                });

                var security = new Dictionary<string, IEnumerable<string>>()
                {
                    { "Bearer", new string[] {} }
                };

                cfg.AddSecurityRequirement(security);

                cfg.ResolveConflictingActions(apiDescription => apiDescription.First());
                cfg.SwaggerDoc("v1.0", new Info() { 
                    Title = "MyTasksAPI - V1.0", 
                    Version = "v1.0"});

                cfg.DocInclusionPredicate((docName, apiDesc) =>
                {
                    var actionApiVersionModel = apiDesc.ActionDescriptor?.GetApiVersion();
                    if (actionApiVersionModel == null)
                    {
                        return true;
                    }
                    if (actionApiVersionModel.DeclaredApiVersions.Any())
                    {
                        return actionApiVersionModel.DeclaredApiVersions.Any(v => $"v{v.ToString()}" == docName);
                    }
                    return actionApiVersionModel.ImplementedApiVersions.Any(v => $"v{v.ToString()}" == docName);
                });

                cfg.OperationFilter<ApiVersionOperationFilter>();
            });
            #endregion

            #region JWT Config
            services.AddAuthentication(opt => {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(opt => {
                opt.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_conf.GetValue<string>("Key")))
                };
            });

            services.AddAuthorization(opt => {
                opt.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
                                            .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                                            .RequireAuthenticatedUser().Build()
                );
            });
            #endregion

            //REPOSITORIES
            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITokenRepository, TokenRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseStatusCodePages();

            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(opt => {
                opt.SwaggerEndpoint("/swagger/v1.0/swagger.json", "MyTasksAPI - V1.0");
                opt.RoutePrefix = String.Empty;
            });
        }
    }
}
