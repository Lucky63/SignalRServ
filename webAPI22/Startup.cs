using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace webAPI22
{
	public class Startup
	{
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
			
		}

		public void Configure(IApplicationBuilder app)
		{
			app.UseCors("CorsPolicy");
			app.UseSignalR(routes =>
			{
				routes.MapHub<ChatHub>("/chat");
			});

			app.UseDefaultFiles();
			app.UseStaticFiles();

			
		}
	}
}
