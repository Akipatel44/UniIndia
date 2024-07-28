using Satyanam.Nop.Plugin.Misc.MenuManager.Domain;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core.Infrastructure;
using Nop.Core;
using Satyanam.Nop.Plugin.Misc.MenuManager.Models;
using Nop.Services.Topics;
using Nop.Data;
using Nop.Services.Media;
using Nop.Services.Localization;
using Nop.Core.Domain.Catalog;
using Satyanam.Nop.Plugin.Misc.MenuManager.Service;
using Nop.Services.Stores;
using Nop.Web.Framework.Themes;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Nop.Services.Seo;

namespace Satyanam.Nop.Plugin.Misc.MenuManager.Controllers
{
    public class CatalogController : BasePluginController
    {
        #region Fields
        private readonly IMenuManagerService _menuManagerService;
        private IRepository<ProductCategory> _productCategoryRepository;
        private IProductService _products;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;
        private ISettingService _settings;
        private ITopicService _topics;
        private readonly ICategoryService _categoryService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IPictureService _pictureService;
        private readonly ILocalizationService _localizationService;
        private readonly MenuManagerSettings _menuManagerSettings;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IStoreContext _storeContext;
        private readonly IUrlRecordService _urlRecordService;
        #endregion

        #region Constructors
        public CatalogController(IMenuManagerService menuManagerService
                          , IRepository<ProductCategory> productCategoryRepository
                                  , IProductService productService
                                  , IStoreService storeService
                                  , IWorkContext workContext
                                  , ISettingService settingService
                                  , ITopicService topicService
                                  , ICategoryService categoryService
                                  , IManufacturerService manufacturerService
                                  , IPictureService pictureService
                                  , ILocalizationService localizationService
                                  , MenuManagerSettings menuManagerSettings
                                  , ILocalizedEntityService localizedEntityService
                                  , IStoreMappingService storeMappingService
                                  , IStoreContext storeContext 
                                  , IUrlRecordService urlRecordService )
        {
            _products = productService;
            _menuManagerService = menuManagerService;
            _productCategoryRepository = productCategoryRepository;
            _storeService = storeService;
            _workContext = workContext;
            _settings = settingService;
            _topics = topicService;
            _categoryService = categoryService;
            _manufacturerService = manufacturerService;
            _pictureService = pictureService;
            _localizationService = localizationService;
            _menuManagerSettings = menuManagerSettings;
            _localizedEntityService = localizedEntityService;
            _storeMappingService = storeMappingService;
            _storeContext = storeContext;
            _urlRecordService = urlRecordService;
        }
        #endregion

        #region Utilities

        private async Task InsertParticularCateogry(int storeScope, string hostname, IPagedList<Category> category, int i, Category item)
        {
            Menu Menu = new Menu();
            Menu.MenuName = item.Name;
            Menu.Title = item.Name;
            Menu.MenuOrder = i;
            Menu.CategoryId = item.Id;
            Menu.ManufacturerId = 0;
            Menu.TopicId = 0;
            Menu.CreatedDate = DateTime.UtcNow;
            Menu.PermanentRedirect = true;
            Menu.PermanentRedirectUrl = hostname + await _urlRecordService.GetSeNameAsync(item);
            Menu.StoreId = storeScope;
            Menu.IsActive = true;
            Menu.IsDeleted = false;
            _menuManagerService.InsertMenu(Menu);

            var subCategory = category.Where(x => x.ParentCategoryId == item.Id);
            int j = 0;
            foreach (var subCat in subCategory)
            {
                Menu menuChild = new Menu();
                menuChild.MenuName = subCat.Name;
                menuChild.Title = subCat.Name;
                menuChild.MenuOrder = j;
                menuChild.CategoryId = subCat.Id;
                menuChild.ManufacturerId = 0;
                menuChild.TopicId = 0;
                menuChild.CreatedDate = DateTime.UtcNow;
                menuChild.PermanentRedirect = true;
                menuChild.PermanentRedirectUrl = hostname + await _urlRecordService.GetSeNameAsync(subCat);
                menuChild.StoreId = storeScope;
                menuChild.IsActive = true;
                menuChild.IsDeleted = false;
                menuChild.ParentMenuId = Menu.Id;
                _menuManagerService.InsertMenu(menuChild);

                j++;
            }
        }

        private async Task InsertMenu(int storeScope, string categoryName, string hostname, int i, Category item)
        {
            Menu childMenu = new Menu();
            childMenu.MenuName = item.Name;
            childMenu.Title = item.Name;
            childMenu.MenuOrder = i;
            childMenu.CategoryId = item.Id;
            childMenu.ManufacturerId = 0;
            childMenu.TopicId = 0;
            childMenu.CreatedDate = DateTime.UtcNow;
            var query = (await _menuManagerService.GetAllMenusAsync(storeScope)).Where(x => x.MenuName == categoryName).Select(x => x.Id);
            childMenu.ParentMenuId = query.FirstOrDefault();
            childMenu.PermanentRedirect = true;
            childMenu.PermanentRedirectUrl = hostname + await _urlRecordService.GetSeNameAsync(item);
            childMenu.IsActive = true;
            childMenu.IsDeleted = false;
            childMenu.StoreId = storeScope;
            _menuManagerService.InsertMenu(childMenu);
        }

        private async Task GetManufacturer()
        {
            var storeScope = _storeContext.GetCurrentStoreAsync().Result.Id;
            var setting = _settings.LoadSetting<MenuManagerSettings>(storeScope);
            var getStore = await _storeService.GetStoreByIdAsync(storeScope);
            var hostname = getStore.Url;

            //bool includeManufacturer = setting.IncludeManufacturers;

            //if (includeManufacturer)
            //{
            //    var manufacturerName = _settings.GetSettingByKey<string>(key: "MenuManager.ManufacturerName", storeId: storeScope);

            //    Menu menu = new Menu();
            //    var menuManufacture = _menuManagerService.GetAllMenus(storeScope).Where(x => x.MenuName == manufacturerName).FirstOrDefault();
            //    if (menuManufacture == null)
            //    {
            //        menu.MenuName = manufacturerName;
            //        menu.Title = manufacturerName;
            //        menu.MenuOrder = 20;
            //        menu.CreatedDate = DateTime.UtcNow;
            //        menu.PermanentRedirect = true;
            //        menu.PermanentRedirectUrl = hostname + Url.RouteUrl("ManufacturerList");
            //        menu.IsActive = true;
            //        menu.CategoryId = 0;
            //        menu.ManufacturerId = 0;
            //        menu.TopicId = 0;
            //        menu.IsDeleted = false;
            //        menu.StoreId = storeScope;
            //        _menuManagerService.InsertMenu(menu);

            //        int i = 0;
            //        var manufacturer = _manufacturerService.GetAllManufacturers();
            //        foreach (var item in manufacturer)
            //        {
            //            if (item.LimitedToStores)
            //            {
            //                var storeMapping = _storeMappingService.GetStoreMappings(item);
            //                if (storeMapping != null)
            //                {
            //                    if (storeMapping.Where(x => x.StoreId == storeScope).Count() > 0)
            //                    {
            //                        IncludeManufacturer(storeScope, hostname, manufacturerName, i, item);
            //                    }
            //                }
            //            }
            //            else
            //            {
            //                IncludeManufacturer(storeScope, hostname, manufacturerName, i, item);
            //            }

            //            i++;
            //        }
            //    }

            //}
        }

        private async Task IncludeManufacturer(int storeScope, string hostname, string manufacturerName, int i, Manufacturer item)
        {
            Menu menuChild = new Menu();
            menuChild.MenuName = item.Name;
            menuChild.Title = item.Name;
            var query = (await _menuManagerService.GetAllMenusAsync(storeScope)).Where(x => x.MenuName == manufacturerName).Select(x => x.Id);
            menuChild.ParentMenuId = query.FirstOrDefault();
            menuChild.MenuOrder = i;
            menuChild.ManufacturerId = item.Id;
            menuChild.CategoryId = 0;
            menuChild.TopicId = 0;
            menuChild.CreatedDate = DateTime.UtcNow;
            menuChild.PermanentRedirect = true;
            menuChild.PermanentRedirectUrl = hostname + await _urlRecordService.GetSeNameAsync(item);
            menuChild.IsActive = true;
            menuChild.IsDeleted = false;
            menuChild.StoreId = storeScope;
            _menuManagerService.InsertMenu(menuChild);
        }

        private async Task GetChildMenuItem(string urlFrmt, StringBuilder sbMenus, Menu tv)
        {
            var storeScope = _storeContext.GetCurrentStoreAsync().Result.Id;
            var setting = _settings.LoadSetting<MenuManagerSettings>(storeScope);

            var mainMenuClass = setting.MainMenuClass;
            var subMenuContainerClass = setting.SubMenuContainerClass;
            var subMenuClass = setting.SubMenuClass;
            var menuItemClass = setting.MenuItemClass;

            bool displayPicture = _menuManagerSettings.DisplayWithPicture;
            bool hiddenCategory = _menuManagerSettings.HiddenCategoryIfNoProduct;
            //bool showCategorySingleMenu = setting.ShowCategoryInSingleMenu;
            int pictureSize = _menuManagerSettings.SubMenuPictureSize;
            bool displayTitle = _menuManagerSettings.DisplayTitle;

            var ChildMenus = await _menuManagerService.GetAllMenusAsync(storeScope, tv.Id);


            if (ChildMenus != null && ChildMenus.Count() > 0)
            {
                sbMenus.AppendFormat("<li class=\"{0}\"><a href=\"{1}\">{2}</a>", menuItemClass, GetURLForMenu(tv), await _localizationService.GetLocalizedAsync(tv, x => x.Title));

                if (displayPicture)
                {
                    sbMenus.AppendFormat("<div class=\"" + subMenuContainerClass + "\"></div><ul class=\"" + subMenuClass + "\"><li class=\"back-button\"><span>Back</span></li> ");
                }
                else
                {
                    sbMenus.AppendFormat("<div class=\"" + subMenuContainerClass + "\"></div><ul class=\"subNoPic\"><li class=\"back-button\"><span>Back</span></li>");
                }


                foreach (Menu tvChild in ChildMenus)
                {

                    bool hasProduct = true;

                    if (displayPicture)
                    {

                        if (tvChild.CategoryId != null && tvChild.CategoryId > 0)
                        {

                            if (hiddenCategory)
                            {
                                if (hasProduct)
                                {
                                    var activeCategory = await _categoryService.GetCategoryByIdAsync(tvChild.CategoryId.HasValue ? tvChild.CategoryId.Value : 0);
                                    var picture = await _pictureService.GetPictureByIdAsync(tvChild.PictureId);
                                    var pictureModel = new PictureModel();

                                    pictureModel.ImageUrl = (await _pictureService.GetPictureUrlAsync(picture, pictureSize)).Url;
                                    pictureModel.Title = string.Format(await _localizationService.GetResourceAsync("Media.Category.ImageLinkTitleFormat"), await _localizationService.GetLocalizedAsync(tvChild, x => x.Title));
                                    pictureModel.AlternateText = string.Format(await _localizationService.GetResourceAsync("Media.Category.ImageAlternateTextFormat"), await _localizationService.GetLocalizedAsync(tvChild, x => x.Title));

                                    var getChildOfParent = await _categoryService.GetAllCategoriesByParentCategoryIdAsync(tvChild.CategoryId ?? default(int), true);

                                    if (getChildOfParent.Count > 0)
                                    {
                                        if (displayTitle)
                                        {
                                            sbMenus.AppendFormat(urlFrmt, GetURLForMenu(tvChild), "<div class=\"Title\">" + await _localizationService.GetLocalizedAsync(tvChild, x => x.Title) + "</div> <img src=\"" + pictureModel.ImageUrl + "\"></a><div class=\"top-menu-triangle\"></div><ul class=\"subcategories\">");
                                        }
                                        else
                                        {
                                            sbMenus.AppendFormat(urlFrmt, GetURLForMenu(tvChild), "<img src=\"" + pictureModel.ImageUrl + "\"></a><div class=\"top-menu-triangle\"></div><ul class=\"subcategories\">");
                                        }
                                        foreach (var item in getChildOfParent)
                                        {
                                            hasProduct = _productCategoryRepository.Table.Where(x => x.CategoryId == item.Id).Count() > 0 ? true : false;
                                            if (hasProduct)
                                            {
                                                sbMenus.AppendFormat("<li><a href=\"{0}\">{1}</a></li>", Url.Action("Category", new { SeName = await _urlRecordService.GetSeNameAsync(item) }), item.Name);

                                            }
                                        }
                                        sbMenus.AppendFormat("</ul>");
                                    }
                                    else
                                    {
                                        if (displayTitle)
                                        {
                                            sbMenus.AppendFormat(urlFrmt, GetURLForMenu(tvChild), "<div class=\"Title\">" + await _localizationService.GetLocalizedAsync(tvChild, x => x.Title) + "</div> <img src=\"" + pictureModel.ImageUrl + "\">");
                                        }
                                        else
                                        {
                                            sbMenus.AppendFormat(urlFrmt, GetURLForMenu(tvChild), "<img src=\"" + pictureModel.ImageUrl + "\">");
                                        }
                                    }

                                }
                            }
                            else
                            {
                                var activeCategory = await _categoryService.GetCategoryByIdAsync(tvChild.CategoryId.HasValue ? tvChild.CategoryId.Value : 0);

                                if (activeCategory != null)
                                {
                                    var picture = await _pictureService.GetPictureByIdAsync(tvChild.PictureId);
                                    var pictureModel = new PictureModel();

                                    pictureModel.ImageUrl = (await _pictureService.GetPictureUrlAsync(picture, pictureSize)).Url;
                                    pictureModel.Title = string.Format(await _localizationService.GetResourceAsync("Media.Category.ImageLinkTitleFormat"), await _localizationService.GetLocalizedAsync(tvChild, x => x.Title));
                                    pictureModel.AlternateText = string.Format(await _localizationService.GetResourceAsync("Media.Category.ImageAlternateTextFormat"), await _localizationService.GetLocalizedAsync(tvChild, x => x.Title));

                                    var getChildOfParent = await _categoryService.GetAllCategoriesByParentCategoryIdAsync(tvChild.CategoryId ?? default(int), true);
                                    if (getChildOfParent.Count > 0)
                                    {
                                        if (displayTitle)
                                        {
                                            sbMenus.AppendFormat(urlFrmt, GetURLForMenu(tvChild), "<div class=\"Title\">" + await _localizationService.GetLocalizedAsync(tvChild, x => x.Title) + "</div> <img src=\"" + pictureModel.ImageUrl + "\"></a><div class=\"top-menu-triangle\"></div><ul class=\"subcategories\">");
                                        }
                                        else
                                        {
                                            sbMenus.AppendFormat(urlFrmt, GetURLForMenu(tvChild), "<img src=\"" + pictureModel.ImageUrl + "\"></a><div class=\"top-menu-triangle\"></div><ul class=\"subcategories\">");
                                        }
                                        foreach (var item in getChildOfParent)
                                        {
                                            sbMenus.AppendFormat("<li><a href=\"{0}\">{1}</a></li>", Url.Action("Category", new { SeName = await _urlRecordService.GetSeNameAsync(item) }), item.Name);
                                        }
                                        sbMenus.AppendFormat("</ul>");
                                    }
                                    else
                                    {
                                        if (displayTitle)
                                        {
                                            sbMenus.AppendFormat(urlFrmt, GetURLForMenu(tvChild), "<div class=\"Title\">" + await _localizationService.GetLocalizedAsync(tvChild, x => x.Title) + "</div> <img src=\"" + pictureModel.ImageUrl + "\">");
                                        }
                                        else
                                        {
                                            sbMenus.AppendFormat(urlFrmt, GetURLForMenu(tvChild), "<img src=\"" + pictureModel.ImageUrl + "\">");
                                        }
                                    }
                                }
                                else
                                {
                                    sbMenus.AppendFormat(urlFrmt, GetURLForMenu(tvChild), "<div class=\"Title\">" + await _localizationService.GetLocalizedAsync(tvChild, x => x.Title) + "</div>");
                                }

                            }
                        }


                        if ((tvChild.ManufacturerId != null && tvChild.ManufacturerId != 0) && (tvChild.CategoryId == 0 || tvChild.CategoryId == null))
                        {
                            var activeManufacturer = _manufacturerService.GetManufacturerByIdAsync(tvChild.ManufacturerId ?? default(int));
                            if (activeManufacturer != null)
                            {
                                var picture = await _pictureService.GetPictureByIdAsync(tvChild.PictureId);
                                var pictureModel = new PictureModel();

                                pictureModel.ImageUrl = (await _pictureService.GetPictureUrlAsync(picture, pictureSize)).Url;
                                pictureModel.Title = string.Format(await _localizationService.GetResourceAsync("Media.Manufacturer.ImageLinkTitleFormat"), await _localizationService.GetLocalizedAsync(tvChild, x => x.Title));
                                pictureModel.AlternateText = string.Format(await _localizationService.GetResourceAsync("Media.Manufacturer.ImageAlternateTextFormat"), await _localizationService.GetLocalizedAsync(tvChild, x => x.Title));

                                if (displayTitle)
                                {
                                    sbMenus.AppendFormat(urlFrmt, GetURLForMenu(tvChild), "<div class=\"Title\">" + await _localizationService.GetLocalizedAsync(tvChild, x => x.Title) + "</div> <img src=\"" + pictureModel.ImageUrl + "\">");
                                }
                                else
                                {
                                    sbMenus.AppendFormat(urlFrmt, GetURLForMenu(tvChild), "<img src=\"" + pictureModel.ImageUrl + "\">");
                                }
                            }
                        }

                        if ((tvChild.TopicId != null && tvChild.TopicId != 0) && (tvChild.ManufacturerId == 0 || tvChild.ManufacturerId == null) && (tvChild.CategoryId == 0 || tvChild.CategoryId == null))
                        {
                            var picture = await _pictureService.GetPictureByIdAsync(tvChild.PictureId);
                            var pictureModel = new PictureModel();

                            pictureModel.ImageUrl = (await _pictureService.GetPictureUrlAsync(picture, pictureSize)).Url;
                            pictureModel.Title = string.Format(await _localizationService.GetResourceAsync("Media.Custom.ImageLinkTitleFormat"), await _localizationService.GetLocalizedAsync(tvChild, x => x.Title));
                            pictureModel.AlternateText = string.Format(await _localizationService.GetResourceAsync("Media.Custom.ImageAlternateTextFormat"), await _localizationService.GetLocalizedAsync(tvChild, x => x.Title));

                            if (displayTitle)
                            {
                                sbMenus.AppendFormat(urlFrmt, GetURLForMenu(tvChild), "<div class=\"Title\">" + await _localizationService.GetLocalizedAsync(tvChild, x => x.Title) + "</div><img src=\"" + pictureModel.ImageUrl + "\">");
                            }
                            else
                            {
                                sbMenus.AppendFormat(urlFrmt, GetURLForMenu(tvChild), "<img src=\"" + pictureModel.ImageUrl + "\">");
                            }
                        }

                        if ((tvChild.ManufacturerId == null || tvChild.ManufacturerId == 0) && (tvChild.CategoryId == null || tvChild.CategoryId == 0) && (tvChild.TopicId == null || tvChild.TopicId == 0))
                        {
                            var picture = await _pictureService.GetPictureByIdAsync(tvChild.PictureId);
                            var pictureModel = new PictureModel();

                            pictureModel.ImageUrl = (await _pictureService.GetPictureUrlAsync(picture, pictureSize)).Url;
                            pictureModel.Title = string.Format(await _localizationService.GetResourceAsync("Media.Custom.ImageLinkTitleFormat"), await _localizationService.GetLocalizedAsync(tvChild, x => x.Title));
                            pictureModel.AlternateText = string.Format(await _localizationService.GetResourceAsync("Media.Custom.ImageAlternateTextFormat"), await _localizationService.GetLocalizedAsync(tvChild, x => x.Title));

                            if (displayTitle)
                            {
                                sbMenus.AppendFormat(urlFrmt, GetURLForMenu(tvChild), "<div class=\"Title\">" + await _localizationService.GetLocalizedAsync(tvChild, x => x.Title) + "</div><img src=\"" + pictureModel.ImageUrl + "\">");
                            }
                            else
                            {
                                sbMenus.AppendFormat(urlFrmt, GetURLForMenu(tvChild), "<img src=\"" + pictureModel.ImageUrl + "\">");
                            }
                        }
                    }
                    else
                    {
                        if ((tvChild.CategoryId != null && tvChild.CategoryId > 0) || (tvChild.ManufacturerId != null && tvChild.ManufacturerId > 0))
                        {
                            if (hiddenCategory)
                            {
                                if (hasProduct)
                                {
                                    sbMenus.AppendFormat(urlFrmt, GetURLForMenu(tvChild), await _localizationService.GetLocalizedAsync(tvChild, x => x.Title));
                                }
                            }
                            else
                            {
                                sbMenus.AppendFormat(urlFrmt, GetURLForMenu(tvChild), await _localizationService.GetLocalizedAsync(tvChild, x => x.Title));
                            }
                        }
                        //If its not any category or manufacture menu and its just custom child menu

                        if ((!tvChild.CategoryId.HasValue || (tvChild.CategoryId.HasValue && tvChild.CategoryId == 0)) && (!tvChild.ManufacturerId.HasValue || (tvChild.ManufacturerId.HasValue && tvChild.ManufacturerId == 0)))
                        {

                            sbMenus.AppendFormat(urlFrmt, GetURLForMenu(tvChild), await _localizationService.GetLocalizedAsync(tvChild, x => x.Title));
                        }
                    }
                }
                sbMenus.Append("</ul></li>");
            }
            else
            {
                sbMenus.AppendFormat(urlFrmt, GetURLForMenu(tv), await _localizationService.GetLocalizedAsync(tv, x => x.Title));
            }
        }

        private async Task GetTopicDescription(string urlFrmt, StringBuilder sbMenus, Menu tv)
        {
            var storeScope = _storeContext.GetCurrentStoreAsync().Result.Id;
            var setting = _settings.LoadSetting<MenuManagerSettings>(storeScope);

            var mainMenuClass = setting.MainMenuClass;
            var subMenuContainerClass = setting.SubMenuContainerClass;
            var subMenuClass = setting.SubMenuClass;
            var menuItemClass = setting.MenuItemClass;

            bool showTopicContentOnHover = setting.ShowTopicContentHover;

            if (tv.TopicId > 0)
            {
                var topic = await _topics.GetTopicByIdAsync(tv.TopicId ?? default(int));

                var topicUrl = "";
                if (tv.PermanentRedirectUrl == null)
                {
                    topicUrl = Url.RouteUrl("Topic", new { SeName = await _urlRecordService.GetSeNameAsync(topic) });
                }
                else
                {
                    topicUrl = tv.PermanentRedirectUrl;
                }

                if (showTopicContentOnHover)
                {
                    sbMenus.AppendFormat("<li class=\"{0}\"><a href=\"{1}\">{2}</a>", menuItemClass, topicUrl, await _localizationService.GetLocalizedAsync(tv, x => x.Title));
                    sbMenus.AppendFormat("<div class=\"" + subMenuContainerClass + "\"></div><div class=\"" + subMenuClass + "\"><p>" + await _localizationService.GetLocalizedAsync(topic, x => x.Body)
                    + "</p></div>");
                }
                else
                {
                    sbMenus.AppendFormat("<li class=\"{0}\"><a href=\"{1}\">{2}</a>", menuItemClass, topicUrl, await _localizationService.GetLocalizedAsync(tv, x => x.Title));
                }
            }

            if (tv.CategoryId > 0)
            {
                var category = await _categoryService.GetCategoryByIdAsync(tv.CategoryId ?? default(int));
                var categoryUrl = "";
                if (tv.PermanentRedirectUrl == null)
                {
                    categoryUrl = Url.RouteUrl("Category", new { SeName = await _urlRecordService.GetSeNameAsync(category) });
                }
                else
                {
                    categoryUrl = tv.PermanentRedirectUrl;
                }

                sbMenus.AppendFormat("<li class=\"{0}\"><a href=\"{1}\">{2}</a>", menuItemClass, categoryUrl, await _localizationService.GetLocalizedAsync(tv, x => x.Title));
            }

            if (tv.ManufacturerId > 0)
            {
                var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(tv.ManufacturerId ?? default(int));
                var manufacturerUrl = "";
                if (tv.PermanentRedirectUrl == null)
                {
                    manufacturerUrl = Url.RouteUrl("Manufacturer", new { SeName = await _urlRecordService.GetSeNameAsync(manufacturer) });

                }
                else
                {
                    manufacturerUrl = tv.PermanentRedirectUrl;
                }

                sbMenus.AppendFormat("<li class=\"{0}\"><a href=\"{1}\">{2}</a>", menuItemClass, manufacturerUrl, await _localizationService.GetLocalizedAsync(tv, x => x.Title));
            }
        }

        private string GetURLForMenu(Menu tv)
        {

            string urlScheme = "http://" + HttpContext.Request.Host;

            if (tv.TopicId.HasValue && tv.TopicId > 0)
                return urlScheme + "/" + tv.PermanentRedirectUrl;
            else if (tv.PermanentRedirect && !string.IsNullOrWhiteSpace(tv.PermanentRedirectUrl))
                return tv.PermanentRedirectUrl.ToLower().StartsWith("http") ? tv.PermanentRedirectUrl : urlScheme + "/" + tv.PermanentRedirectUrl;
            else
                return urlScheme;
        }

        protected virtual void UpdateLocales(Menu menu, ManageMenuModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValueAsync(menu,
                                                               x => x.Title,
                                                               localized.Title,
                                                               localized.LanguageId);
            }
        }

        #endregion

        #region Topmenu
        public async Task<IActionResult> TopMenu()
        {
            MenuSettingsViewModel model = new MenuSettingsViewModel();

            var storeScope = _storeContext.GetCurrentStoreAsync().Result.Id;
            var setting = _settings.LoadSetting<MenuManagerSettings>(storeScope);
            var currentThemeName = await EngineContext.Current.Resolve<IThemeContext>().GetWorkingThemeNameAsync();

            bool includeTopics = setting.IncludeTopics;

            string categoryName = setting.CategoryMenuTitle;
            string manufacturerName = setting.ManufacturerName;

            var mainMenuClass = setting.MainMenuClass;
            var subMenuContainerClass = setting.SubMenuContainerClass;
            var subMenuClass = setting.SubMenuClass;
            var menuItemClass = setting.MenuItemClass;

            model.StickMenuOnTop = setting.StickMenuOnTop;

            ViewBag.MainMenuClass = mainMenuClass;
            ViewBag.SubMenuContainerClass = subMenuContainerClass;

            string urlFrmt = "<li><a href=\"{0}\">{1}</a> </li>";

            bool supportResponsive = true;

            StringBuilder sbMenus = new StringBuilder();

            #region Get Menu

            List<Menu> Menus = new List<Menu>();

            Menus =(await _menuManagerService.GetAllMenusAsync(storeScope)).OrderBy(x => x.MenuOrder).ToList();

            if (!includeTopics)
            {
                Menus = Menus.Where(x => x.TopicId.HasValue == false || (x.TopicId.HasValue && x.TopicId == 0)).ToList();
            }

            //Add all menu by menu order
            foreach (Menu tv in Menus)
            {
                //For Topic
                if (tv.CategoryId > 0)
                {
                    GetTopicDescription(urlFrmt, sbMenus, tv);
                }

                else if (tv.ManufacturerId > 0)
                {
                    GetTopicDescription(urlFrmt, sbMenus, tv);
                }

                else if (tv.TopicId.HasValue && tv.TopicId > 0)
                {
                    GetTopicDescription(urlFrmt, sbMenus, tv);
                }
                else
                {
                    //All other pending menu like custom menu
                   await GetChildMenuItem(urlFrmt, sbMenus, tv);
                }
            }

            #endregion


            ViewBag.MenuItems = sbMenus.ToString();
            ViewBag.SupportResponsive = supportResponsive;

            sbMenus = null;

            string ViewPath = "~/Plugins/Misc.MenuManager/Views/Catalog/TopMenu.cshtml";

            if (!string.IsNullOrEmpty(currentThemeName))
            {
                ViewPath = "~/Plugins/Misc.MenuManager/Themes/" + currentThemeName + "/Views/Catalog/TopMenu.cshtml";
            }
            return PartialView(ViewPath, model);
        }

        #endregion

    }
}
