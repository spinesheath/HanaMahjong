// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spines.Hana.Blame.Data;
using Spines.Hana.Blame.Models;
using Spines.Hana.Blame.Options;
using Spines.Hana.Blame.Services;
using Spines.Hana.Blame.Services.ReplayManager;

namespace Spines.Hana.Blame
{
  public class Startup
  {
    public Startup(IHostingEnvironment env)
    {
      var builder = new ConfigurationBuilder()
        .SetBasePath(env.ContentRootPath)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

      if (env.IsDevelopment())
      {
        builder.AddUserSecrets<Startup>();
      }

      builder.AddEnvironmentVariables();
      Configuration = builder.Build();

      _env = env;
    }

    public IConfigurationRoot Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    // ReSharper disable once UnusedMember.Global
    public void ConfigureServices(IServiceCollection services)
    {
      // Add framework services.
      services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(GetConnectionString()));

      services.AddIdentity<ApplicationUser, IdentityRole>(config => { config.SignIn.RequireConfirmedEmail = true; })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

      services.AddMvc();

      // Add application services.
      services.AddTransient<IEmailSender, AuthMessageSender>();
      services.AddTransient<ISmsSender, AuthMessageSender>();
      services.AddTransient<IdentityInitializer>();
      services.AddTransient<RuleSetInitializer>();
      services.AddTransient<RoomInitializer>();
      services.AddTransient<ReplayManager>();

      if (_env.IsDevelopment())
      {
        services.AddTransient<IStorage, MemoryStorage>();
      }
      else
      {
        services.AddTransient<IStorage, Storage>();
      }

      services.AddSingleton(CreateHttpClient);

      services.Configure<AuthMessageSenderOptions>(Configuration);
      services.Configure<InitializeIdentityOptions>(Configuration);
      services.Configure<CopyrightOptions>(Configuration);
      services.Configure<StorageOptions>(Configuration);
      services.Configure<SnitchOptions>(Configuration);
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    // ReSharper disable once UnusedMember.Global
    public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
    {
      loggerFactory.AddConsole(Configuration.GetSection("Logging"));
      loggerFactory.AddDebug();

      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
        app.UseDatabaseErrorPage();
        app.UseBrowserLink();
      }
      else
      {
        app.UseExceptionHandler("/Home/Error");
      }

      app.UseStaticFiles();

      app.UseAuthentication();

      app.UseMvc(routes =>
      {
        routes.MapRoute(
          name: "default",
          template: "{controller=Home}/{action=Index}/{id?}");
      });
    }

    private string GetConnectionString()
    {
      var connectionString = Configuration.GetConnectionString("SpinesHanaBlameDefaultConnectionString");
      return string.IsNullOrEmpty(connectionString) ? Configuration.GetValue<string>("SpinesHanaBlameDefaultConnectionString") : connectionString;
    }

    private static HttpClient CreateHttpClient(IServiceProvider arg)
    {
      var handler = new HttpClientHandler
      {
        AutomaticDecompression = DecompressionMethods.GZip
      };
      var httpClient = new HttpClient(handler);      
      httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
      return httpClient;
    }

    private readonly IHostingEnvironment _env;
  }
}