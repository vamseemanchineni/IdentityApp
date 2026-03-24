using API.Utility;
using IdentityApp.Data;
using IdentityApp.Models;
using IdentityApp.Services;
using IdentityApp.Services.IServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Threading.Tasks;

namespace IdentityApp.Extensions
{
    public static class WebApplicationBuilderExtensions
    {
        public static WebApplicationBuilder AddApplicationServices(this WebApplicationBuilder builder)
        {
            // Add custom services here, e.g.:
            // builder.Services.AddScoped<IMyService, MyService>();
            builder.Services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            builder.Services.AddScoped<ITokenService, TokenService>();
            return builder;
        }
        public static WebApplicationBuilder AddAuthenticationServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddIdentity<AppUser, AppRole>(options => {
                options.Password.RequiredLength = StaticDetails.RequiredPaaswordLength;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.SignIn.RequireConfirmedEmail = true;
                options.SignIn.RequireConfirmedAccount = true;
                options.Lockout.AllowedForNewUsers = false;
                options.Lockout.MaxFailedAccessAttempts = StaticDetails.MaxFailedAccessAttempts;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(StaticDetails.RequiredPaaswordLength);
            }).AddEntityFrameworkStores<DataContext>().AddDefaultTokenProviders();
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddCookie(opt =>
            {
                opt.Cookie.Name = StaticDetails.IdentityAppCookie;
            }).AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])),
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["JWT:Issuer"],
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
                opt.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        context.Token = context.Request.Cookies[StaticDetails.IdentityAppCookie];
                        return Task.CompletedTask;
                    }
                };
            });
                // Add custom services here, e.g.:
                // builder.Services.AddScoped<IMyService, MyService>();
                return builder;
        }
    }
}
