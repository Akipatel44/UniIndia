using Satyanam.Nop.Plugin.Misc.MenuManager.Domain;
using Nop.Services.Configuration;
using Nop.Web.Framework.Menu;
using System.Linq;
using Nop.Services.Common;
using Microsoft.AspNetCore.Routing;
using Satyanam.Nop.Plugin.Misc.MenuManager.Service;
using Nop.Services.Plugins;
using Nop.Data;
using System.Threading.Tasks;

namespace Satyanam.Nop.Plugin.Misc.MenuManager
{
    public class MenuManagerPlugin : BasePlugin, IAdminMenuPlugin, IMiscPlugin
    {
        private readonly IRepository<Menu> _MenuRepo;
        private readonly ISettingService _settingService;
        private readonly MenuManagerSettings _menuManagerSettings;
        private readonly LocalizationImportService _localizationImportService;

        public MenuManagerPlugin(IRepository<Menu> MenuRepo, ISettingService settingService, MenuManagerSettings menuManagerSettings, LocalizationImportService localizationImportService)
        {
            _MenuRepo = MenuRepo;
            _settingService = settingService;
            _menuManagerSettings = menuManagerSettings;
            _localizationImportService = localizationImportService;
        }


        public async Task ManageSiteMapAsync(SiteMapNode rootNode)
        {
            var menuItem = new SiteMapNode()
            {
                Title = "Nop Menu Manager",
                Visible = true
            };

            var menuFirstItem = new SiteMapNode()
            {
                Title = "Configure",
                ActionName = "Configure",
                ControllerName = "MenuManager",
                Visible = true,
                RouteValues = new RouteValueDictionary() { { "area", "Admin" } },
                //Url = "/MenuManager/Configure",

            };
            menuItem.ChildNodes.Add(menuFirstItem);

            var menuSecondItem = new SiteMapNode()
            {
                Title = "Manage Menus",
                Visible = true,                     
                ActionName = "ManageMenus",
                ControllerName = "MenuManager",
                RouteValues = new RouteValueDictionary() { { "area", "Admin" } },
            };
            menuItem.ChildNodes.Add(menuSecondItem);

            var pluginNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Nopsites");
            if (pluginNode != null)
                pluginNode.ChildNodes.Add(menuItem);
            else
                rootNode.ChildNodes.Add(menuItem);
        }

        public override async Task InstallAsync()
        {

            var settings = new MenuManagerSettings()
            {

                //IncludeCategories = false,
                //ShowCategoryInSingleMenu = false,
                //IncludeManufacturers = false,
                IncludeTopics = false,
                ShowTopicContentHover = false,
                DisplayWithPicture = false,
                StickMenuOnTop = false,
                HiddenCategoryIfNoProduct = false,
                MainMenuClass = "top-menu",
                SubMenuContainerClass = "top-menu-triangle",
                SubMenuClass = "sublist firstLevel",
                MenuItemClass = "",
                CategoryMenuTitle = "Products",
                ManufacturerName = "Manufacturers",
                IncludeCategories = true

            };
            _settingService.SaveSetting(settings);

           await _localizationImportService.AddLocalizationsAsync();

            await base.InstallAsync();
        }

        public override async Task UninstallAsync()
        {
          
           await _localizationImportService.DeleteLocalizationsAsync();

            await base.UninstallAsync();
        }

        //public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        //{
        //    actionName = "Configure";
        //    controllerName = "MenuManager";
        //    routeValues = new RouteValueDictionary() { { "area", "Admin" } };
        //}
        
    }
}
