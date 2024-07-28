using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Satyanam.Nop.Plugin.Misc.MenuManager.Service;

namespace Satyanam.Nop.Plugin.Misc.MenuManager.Infrastructure
{
    public class NopStartup : INopStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IMenuManagerService, MenuManagerService>();
            services.AddScoped<LocalizationImportService>();

            //services.AddScoped<IMenuManagerService, MenuManagerService>();

            //services.Configure<RazorViewEngineOptions>(options =>
            //{
            //    options.ViewLocationExpanders.Add(new CustomViewLocationExpander());
            //});
        }

        public void Configure(IApplicationBuilder application)
        {
        }

        public int Order
        {
            get { return int.MaxValue; } //add after nopcommerce is done
        }

    }
}
