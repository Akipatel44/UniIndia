using Nop.Web.Framework.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;

namespace Nop.Plugin.Misc.MenuManager
{
    public class RouteProvider : IRouteProvider
    {

        public int Priority
        {
            get { return int.MaxValue; }
        }

        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute("Satyanam.Nop.Plugin.Misc.MenuManager.ManageMenus",
                "MenuManager/{action}",
                new { controller = "MenuManager", action = "ManageMenus", area = "Admin" });


            endpointRouteBuilder.MapControllerRoute("Satyanam.Nop.Plugin.Misc.MenuManager.Configure",
              "MenuManager/{action}",
              new { controller = "MenuManager", action = "Configure", area = "Admin" });

            endpointRouteBuilder.MapControllerRoute("Satyanam.Nop.Plugin.Misc.MenuManager.TopMenu",
              "Catalog/TopMenu",
              new { controller = "Catalog", action = "Catalog" });

        }
    }
}
