using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using API_BlogApplication.Profiles;
using DAL;
using Services;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using Utilities.JWT;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.IO;
using API_BlogApplication.Swagger.Filters;

namespace API_BlogApplication
{
    public class Startup
    {
        public static SymmetricSecurityKey signingKey;
        public static TokenProviderOption userTokenOption;
        public static TokenValidationParameters tokenValidationParameters;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = tokenValidationParameters;
            });

            services.AddControllers();

            services.AddCors(options =>
            {
                options.AddPolicy(name: "BlogReactClient_HTTPS",
                                  builder =>
                                  {
                                      builder.WithOrigins("*", "*");
                                      builder.AllowAnyHeader();
                                      builder.AllowAnyMethod();
                                      builder.AllowCredentials();
                                      builder.SetIsOriginAllowed(origin => true);
                                  });
            });

            services.AddDbContext<BlogApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("BlogDBConnection")));

            services.AddTransient<IPostService, PostService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<ICommentService, CommentService>();

            services.AddAutoMapper(typeof(UserProfile));
            services.AddAutoMapper(typeof(TokenProfile));
            services.AddAutoMapper(typeof(PostProfile));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Blog Application API",
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    In = ParameterLocation.Header,
                    BearerFormat = "JWT",
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },

                        Array.Empty<string>()
                    }
                });

                c.EnableAnnotations();
                //c.DocumentFilter<RemoveSchemaDocumentFilter>();
                c.SchemaFilter<UserWriteDTOSchemaFilter>();

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.AddSwaggerGenNewtonsoftSupport();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.DefaultModelsExpandDepth(-1);
                c.DefaultModelExpandDepth(-1);
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Blog Application V1");
                c.RoutePrefix = "";
                c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
            });

            signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Keys:UserAuthSecretKey"]));

            userTokenOption = new TokenProviderOption
            {
                Audience = "ConsumerUser",
                Issuer = "BackEnd",
                SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256),
            };

            tokenValidationParameters = new TokenValidationParameters
            {
                // The signing key must match!
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,

                // Validate the JWT Issuer (iss) claim
                ValidateIssuer = true,
                ValidIssuer = "BackEnd",

                // Validate the JWT Audience (aud) claim
                ValidateAudience = true,
                ValidAudience = "ConsumerUser",

                // Validate the token expiry
                ValidateLifetime = true,

                // If you want to allow a certain amount of clock drift, set that here:
                ClockSkew = TimeSpan.Zero
            };

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors("BlogReactClient_HTTPS");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.ApplicationServices.CreateScope().ServiceProvider.GetService<BlogApplicationDbContext>().Database.Migrate();
        }
    }
}
