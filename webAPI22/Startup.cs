using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace webAPI22
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
			services.AddSignalR();
			services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
					{
						builder
					.WithOrigins("http://localhost:4200")
					.AllowAnyMethod()
					.AllowAnyHeader()
					.AllowCredentials();
					}));

			services.AddDbContext<ApplicationContext>();

			services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			   .AddJwtBearer(options =>
			   {
				   options.RequireHttpsMetadata = false;
				   options.TokenValidationParameters = new TokenValidationParameters
				   {
					   ValidateIssuer = true,
					   ValidIssuer = AuthOptions.ISSUER,
					   ValidateAudience = true,
					   ValidAudience = AuthOptions.AUDIENCE,
					   ValidateLifetime = true,
					   IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
					   ValidateIssuerSigningKey = true,
				   };
				   options.Events = new JwtBearerEvents
				   {
					   OnMessageReceived = context =>
					   {
						   var accessToken = context.Request.Query["access_token"];

							   // если запрос направлен хабу
							   var path = context.HttpContext.Request.Path;
						   if (!string.IsNullOrEmpty(accessToken) &&
								(path.StartsWithSegments("/chat")))
						   {
								   // получаем токен из строки запроса
								   context.Token = accessToken;
						   }
						   return Task.CompletedTask;
					   }
				   };
			   });

			services.AddMvc();


		}


		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
				app.UseHsts();
			}
			app.UseCors("CorsPolicy");
			app.UseAuthentication();

			app.UseSignalR(routes =>
			{
				routes.MapHub<ChatHub>("/chat");
			});
			app.UseHttpsRedirection();
			app.UseDefaultFiles();
			app.UseStaticFiles();
			app.UseAuthentication();
			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");
			});
		}
	}	
}
