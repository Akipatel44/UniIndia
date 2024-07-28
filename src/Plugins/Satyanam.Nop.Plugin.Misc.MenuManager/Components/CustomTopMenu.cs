using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Forums;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Services.Topics;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Themes;
using Satyanam.Nop.Plugin.Misc.MenuManager.Domain;
using Satyanam.Nop.Plugin.Misc.MenuManager.Models;
using Satyanam.Nop.Plugin.Misc.MenuManager.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satyanam.Nop.Plugin.Misc.MenuManager.Components
{
    public class CustomTopMenuViewComponent : NopViewComponent
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
        private readonly IForumService _forumService;
        #endregion

        #region Constructors
        public CustomTopMenuViewComponent(IMenuManagerService menuManagerService
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
                                  , IUrlRecordService urlRecordService
                                  , IForumService forumService)
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
            _forumService = forumService;
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
            var languageId = _workContext.GetWorkingLanguageAsync().Id;
            Menu.PermanentRedirectUrl = hostname + await _urlRecordService.GetSeNameAsync(item, languageId);
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
                menuChild.PermanentRedirectUrl = hostname + await _urlRecordService.GetSeNameAsync(subCat, languageId);
                menuChild.StoreId = storeScope;
                menuChild.IsActive = true;
                menuChild.IsDeleted = false;
                menuChild.ParentMenuId = Menu.Id;
                _menuManagerService.InsertMenu(menuChild);

                j++;
            }
        }

        private async Task InsertMenuAsync(int storeScope, string categoryName, string hostname, int i, Category item)
        {
            var languageId = _workContext.GetWorkingLanguageAsync().Id;
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
            childMenu.PermanentRedirectUrl = hostname + await _urlRecordService.GetSeNameAsync(item, languageId);
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

            var manufacturerName = setting.ManufacturerName;

            Menu menu = new Menu();
            var menuManufacture = (await _menuManagerService.GetAllMenusAsync(storeScope)).Where(x => x.MenuName == manufacturerName).FirstOrDefault();
            if (menuManufacture == null)
            {
                menu.MenuName = manufacturerName;
                menu.Title = manufacturerName;
                menu.MenuOrder = 20;
                menu.CreatedDate = DateTime.UtcNow;
                menu.PermanentRedirect = true;
                menu.PermanentRedirectUrl = hostname + Url.RouteUrl("ManufacturerList");
                menu.IsActive = true;
                menu.CategoryId = 0;
                menu.ManufacturerId = 0;
                menu.TopicId = 0;
                menu.IsDeleted = false;
                menu.StoreId = storeScope;
                _menuManagerService.InsertMenu(menu);

                int i = 0;
                var manufacturer = await _manufacturerService.GetAllManufacturersAsync();
                foreach (var item in manufacturer)
                {
                    if (item.LimitedToStores)
                    {
                        var storeMapping = await _storeMappingService.GetStoreMappingsAsync(item);
                        if (storeMapping != null)
                        {
                            if (storeMapping.Where(x => x.StoreId == storeScope).Count() > 0)
                            {
                                await IncludeManufacturer(storeScope, hostname, manufacturerName, i, item);
                            }
                        }
                    }
                    else
                    {
                        await IncludeManufacturer(storeScope, hostname, manufacturerName, i, item);
                    }

                    i++;
                }
            }
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
            int pictureSize = _menuManagerSettings.SubMenuPictureSize;
            //bool displayTitle = _menuManagerSettings.DisplayTitle;
            bool displayTitle = true;

            var Picture = await _pictureService.GetPictureByIdAsync(tv.PictureId);
            var PictureModel = new PictureModel();

            PictureModel.ImageUrl = (await _pictureService.GetPictureUrlAsync(Picture, pictureSize)).Url;
            PictureModel.Title = string.Format(await _localizationService.GetResourceAsync("Media.Custom.ImageLinkTitleFormat"), await _localizationService.GetLocalizedAsync(tv, x => x.Title));
            PictureModel.AlternateText = string.Format(await _localizationService.GetResourceAsync("Media.Custom.ImageAlternateTextFormat"), await _localizationService.GetLocalizedAsync(tv, x => x.Title));

            var ChildMenus = await _menuManagerService.GetAllMenusAsync(storeScope, tv.Id);


            if (ChildMenus != null && ChildMenus.Count() > 0)
            {
                if (displayPicture)
                {
                    if (tv.PictureId > 0)
                    {

                        sbMenus.AppendFormat("<li class=\"{0}\"><a href=\"{1}\"><img src=\"{2}\">{3}</a>", menuItemClass, await GetURLForMenu(tv), PictureModel.ImageUrl, await _localizationService.GetLocalizedAsync(tv, x => x.Title));
                    }
                    else
                    {
                        sbMenus.AppendFormat("<li class=\"{0}\"><a href=\"{1}\">{2}</a>", menuItemClass, await GetURLForMenu(tv), await _localizationService.GetLocalizedAsync(tv, x => x.Title));
                    }
                }
                else
                {
                    sbMenus.AppendFormat("<li class=\"{0}\"><a href=\"{1}\">{2}</a>", menuItemClass, await GetURLForMenu(tv), await _localizationService.GetLocalizedAsync(tv, x => x.Title));
                }

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

                                    if (displayTitle)
                                    {
                                        if (tvChild.PictureId > 0)
                                        {
                                            sbMenus.AppendFormat(urlFrmt, await GetURLForMenu(tvChild), "<img src=\"" + pictureModel.ImageUrl + "\">" + "<div class=\"Title\">" + await _localizationService.GetLocalizedAsync(tvChild, x => x.Title) + "</div>");
                                        }
                                        else
                                        {
                                            sbMenus.AppendFormat(urlFrmt, await GetURLForMenu(tvChild), "<div class=\"Title\">" + await _localizationService.GetLocalizedAsync(tvChild, x => x.Title) + "</div>");
                                        }
                                    }
                                    else
                                    {
                                        if (tvChild.PictureId > 0)
                                        {
                                            sbMenus.AppendFormat(urlFrmt, await GetURLForMenu(tvChild), "<img src=\"" + pictureModel.ImageUrl + "\">");
                                        }
                                        else
                                        {
                                            sbMenus.AppendFormat(urlFrmt, await GetURLForMenu(tvChild));
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

                                    if (displayTitle)
                                    {
                                        if (tvChild.PictureId > 0)
                                        {
                                            sbMenus.AppendFormat(urlFrmt, await GetURLForMenu(tvChild), "<img src=\"" + pictureModel.ImageUrl + "\">" + "<div class=\"Title\">" + await _localizationService.GetLocalizedAsync(tvChild, x => x.Title) + "</div>");
                                        }
                                        else
                                        {
                                            sbMenus.AppendFormat(urlFrmt, await GetURLForMenu(tvChild), "<div class=\"Title\">" + await _localizationService.GetLocalizedAsync(tvChild, x => x.Title) + "</div>");
                                        }
                                    }
                                    else
                                    {
                                        if (tvChild.PictureId > 0)
                                        {
                                            sbMenus.AppendFormat(urlFrmt, await GetURLForMenu(tvChild), "<img src=\"" + pictureModel.ImageUrl + "\">");
                                        }
                                        else
                                        {
                                            sbMenus.AppendFormat(urlFrmt, await GetURLForMenu(tvChild));
                                        }
                                    }
                                }
                                else
                                {
                                    sbMenus.AppendFormat(urlFrmt, await GetURLForMenu(tvChild), "<div class=\"Title\">" + await _localizationService.GetLocalizedAsync(tvChild, x => x.Title) + "</div>");
                                }
                            }
                        }


                        if ((tvChild.ManufacturerId != null && tvChild.ManufacturerId != 0) && (tvChild.CategoryId == 0 || tvChild.CategoryId == null))
                        {
                            var activeManufacturer = await _manufacturerService.GetManufacturerByIdAsync(tvChild.ManufacturerId ?? default(int));
                            if (activeManufacturer != null)
                            {
                                var picture = await _pictureService.GetPictureByIdAsync(tvChild.PictureId);
                                var pictureModel = new PictureModel();

                                pictureModel.ImageUrl = (await _pictureService.GetPictureUrlAsync(picture, pictureSize)).Url;
                                pictureModel.Title = string.Format(await _localizationService.GetResourceAsync("Media.Manufacturer.ImageLinkTitleFormat"), await _localizationService.GetLocalizedAsync(tvChild, x => x.Title));
                                pictureModel.AlternateText = string.Format(await _localizationService.GetResourceAsync("Media.Manufacturer.ImageAlternateTextFormat"), await _localizationService.GetLocalizedAsync(tvChild, x => x.Title));

                                if (displayTitle)
                                {
                                    if (tvChild.PictureId > 0)
                                    {
                                        sbMenus.AppendFormat(urlFrmt, await GetURLForMenu(tvChild), "<img src=\"" + pictureModel.ImageUrl + "\">" + "<div class=\"Title\">" + await _localizationService.GetLocalizedAsync(tvChild, x => x.Title) + "</div>");
                                    }
                                    else
                                    {
                                        sbMenus.AppendFormat(urlFrmt, await GetURLForMenu(tvChild), "<div class=\"Title\">" + await _localizationService.GetLocalizedAsync(tvChild, x => x.Title) + "</div>");
                                    }
                                }
                                else
                                {
                                    if (tvChild.PictureId > 0)
                                    {
                                        sbMenus.AppendFormat(urlFrmt, await GetURLForMenu(tvChild), "<img src=\"" + pictureModel.ImageUrl + "\">");
                                    }
                                    else
                                    {
                                        sbMenus.AppendFormat(urlFrmt, await GetURLForMenu(tvChild));
                                    }
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
                                sbMenus.AppendFormat(urlFrmt, await GetURLForMenu(tvChild), "<img src=\"" + pictureModel.ImageUrl + "\">" + "<div class=\"Title\">" + await _localizationService.GetLocalizedAsync(tvChild, x => x.Title) + "</div>");
                            }
                            else
                            {
                                sbMenus.AppendFormat(urlFrmt, await GetURLForMenu(tvChild), "<img src=\"" + pictureModel.ImageUrl + "\">");
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
                                if (tvChild.PictureId > 0)
                                {
                                    sbMenus.AppendFormat(urlFrmt, await GetURLForMenu(tvChild), "<img src=\"" + pictureModel.ImageUrl + "\">" + "<div class=\"Title\">" + await _localizationService.GetLocalizedAsync(tvChild, x => x.Title) + "</div>");
                                }
                                else
                                {
                                    sbMenus.AppendFormat(urlFrmt, await GetURLForMenu(tvChild), "<div class=\"Title\">" + await _localizationService.GetLocalizedAsync(tvChild, x => x.Title) + "</div>");
                                }
                            }
                            else
                            {
                                if (tvChild.PictureId > 0)
                                {
                                    sbMenus.AppendFormat(urlFrmt, await GetURLForMenu(tvChild), "<img src=\"" + pictureModel.ImageUrl + "\">");
                                }
                                else
                                {
                                    sbMenus.AppendFormat(urlFrmt, await GetURLForMenu(tvChild));
                                }
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
                                    sbMenus.AppendFormat(urlFrmt, await GetURLForMenu(tvChild), await _localizationService.GetLocalizedAsync(tvChild, x => x.Title));
                                }
                            }
                            else
                            {
                                sbMenus.AppendFormat(urlFrmt, await GetURLForMenu(tvChild), await _localizationService.GetLocalizedAsync(tvChild, x => x.Title));
                            }
                        }
                        //If its not any category or manufacture menu and its just custom child menu

                        if ((!tvChild.CategoryId.HasValue || (tvChild.CategoryId.HasValue && tvChild.CategoryId == 0)) && (!tvChild.ManufacturerId.HasValue || (tvChild.ManufacturerId.HasValue && tvChild.ManufacturerId == 0)))
                        {

                            sbMenus.AppendFormat(urlFrmt, await GetURLForMenu(tvChild), await _localizationService.GetLocalizedAsync(tvChild, x => x.Title));
                        }
                    }
                }
                sbMenus.Append("</ul></li>");
            }
            else
            {
                if(displayPicture)
                {
                    if (tv.PictureId > 0)
                    {
                        sbMenus.AppendFormat(urlFrmt, await GetURLForMenu(tv), "<img src=\"" + PictureModel.ImageUrl + "\">" + await _localizationService.GetLocalizedAsync(tv, x => x.Title));
                    }
                    else
                    {
                        sbMenus.AppendFormat(urlFrmt, await GetURLForMenu(tv), await _localizationService.GetLocalizedAsync(tv, x => x.Title));
                    }
                }
                else
                {
                    sbMenus.AppendFormat(urlFrmt, await GetURLForMenu(tv), await _localizationService.GetLocalizedAsync(tv, x => x.Title));
                }
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

            bool displayPicture = _menuManagerSettings.DisplayWithPicture;
            bool hiddenCategory = _menuManagerSettings.HiddenCategoryIfNoProduct;
            int pictureSize = _menuManagerSettings.SubMenuPictureSize;
            bool displayTitle = _menuManagerSettings.DisplayTitle;

            var languageId = _workContext.GetWorkingLanguageAsync().Id;
            bool showTopicContentOnHover = setting.ShowTopicContentHover;

            var picture = await _pictureService.GetPictureByIdAsync(tv.PictureId);
            var pictureModel = new PictureModel();

            pictureModel.ImageUrl = (await _pictureService.GetPictureUrlAsync(picture, pictureSize)).Url;
            pictureModel.Title = string.Format(await _localizationService.GetResourceAsync("Media.Custom.ImageLinkTitleFormat"), await _localizationService.GetLocalizedAsync(tv, x => x.Title));
            pictureModel.AlternateText = string.Format(await _localizationService.GetResourceAsync("Media.Custom.ImageAlternateTextFormat"), await _localizationService.GetLocalizedAsync(tv, x => x.Title));

            if (tv.TopicId > 0)
            {
                var topic = await _topics.GetTopicByIdAsync(tv.TopicId ?? default(int));

                var topicUrl = "";
                if (tv.PermanentRedirectUrl == null)
                {
                    topicUrl = Url.RouteUrl(nameof(topic), new
                    {
                        SeName = await _urlRecordService.GetSeNameAsync(topic, languageId)
                    });
                }
                else
                {
                    topicUrl = tv.PermanentRedirectUrl;
                }

                if (showTopicContentOnHover)
                {
                    if (displayPicture)
                    {
                        if (tv.PictureId > 0)
                        {
                            sbMenus.AppendFormat("<li class=\"{0}\"><a href=\"{1}\"><img src=\"{2}\">{3}</a>", menuItemClass, topicUrl, pictureModel.ImageUrl, await _localizationService.GetLocalizedAsync(tv, x => x.Title));
                            sbMenus.AppendFormat("<div class=\"" + subMenuContainerClass + "\"></div><div class=\"" + subMenuClass + "\"><p>" + await _localizationService.GetLocalizedAsync(topic, x => x.Body) + "</p></div>");
                        }
                    }
                    else
                    {
                        sbMenus.AppendFormat("<li class=\"{0}\"><a href=\"{1}\">{2}</a>", menuItemClass, topicUrl, await _localizationService.GetLocalizedAsync(tv, x => x.Title));
                        sbMenus.AppendFormat("<div class=\"" + subMenuContainerClass + "\"></div><div class=\"" + subMenuClass + "\"><p>" + await _localizationService.GetLocalizedAsync(topic, x => x.Body) + "</p></div>");
                    }
                }
                else
                {
                    sbMenus.AppendFormat("<li class=\"{0}\"><a href=\"{1}\"><img src=\"{2}\">{3}</a>", menuItemClass, topicUrl, pictureModel.ImageUrl, await _localizationService.GetLocalizedAsync(tv, x => x.Title));
                }
            }
            else
            {
                if(displayPicture)
                {
                    if (tv.PictureId > 0)
                    {
                        sbMenus.AppendFormat("<li class=\"{0}\"><a href=\"{1}\"><img src=\"{2}\">{3}</a>", menuItemClass, await GetURLForMenu(tv), pictureModel.ImageUrl, await _localizationService.GetLocalizedAsync(tv, x => x.Title));
                    }
                }
                else
                {
                    sbMenus.AppendFormat("<li class=\"{0}\"><a href=\"{1}\">{2}</a>", menuItemClass, await GetURLForMenu(tv), await _localizationService.GetLocalizedAsync(tv, x => x.Title));
                }
            }

            if (tv.CategoryId > 0)
            {
                var category = await _categoryService.GetCategoryByIdAsync(tv.CategoryId ?? default(int));
                var categoryUrl = "";
                if (tv.PermanentRedirectUrl == null)
                {
                    categoryUrl = Url.RouteUrl("Category", new { SeName = await _urlRecordService.GetSeNameAsync(category, languageId) });

                    if (displayPicture)
                    {
                        if (tv.PictureId > 0)
                        {
                            sbMenus.AppendFormat("<li class=\"{0}\"><a href=\"{1}\"><img src=\"{2}\">{3}</a>", menuItemClass, categoryUrl, pictureModel.ImageUrl, await _localizationService.GetLocalizedAsync(tv, x => x.Title));
                        }
                        else
                        {
                            sbMenus.AppendFormat("<li class=\"{0}\"><a href=\"{1}\">{2}</a>", menuItemClass, categoryUrl, await _localizationService.GetLocalizedAsync(tv, x => x.Title));
                        }
                    }
                    else
                    {
                        sbMenus.AppendFormat("<li class=\"{0}\"><a href=\"{1}\">{2}</a>", menuItemClass, categoryUrl, await _localizationService.GetLocalizedAsync(tv, x => x.Title));
                    }
                }
            }

            if (tv.ManufacturerId > 0)
            {
                var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(tv.ManufacturerId ?? default(int));
                var manufacturerUrl = "";
                if (tv.PermanentRedirectUrl == null)
                {
                    manufacturerUrl = Url.RouteUrl("Manufacturer", new { SeName = await _urlRecordService.GetSeNameAsync(manufacturer, languageId) });

                    if (displayPicture)
                    {
                        if (tv.PictureId > 0)
                        {
                            sbMenus.AppendFormat("<li class=\"{0}\"><a href=\"{1}\"><img src=\"{2}\">{3}</a>", menuItemClass, manufacturerUrl, pictureModel.ImageUrl, await _localizationService.GetLocalizedAsync(tv, x => x.Title));
                        }
                        else
                        {
                            sbMenus.AppendFormat("<li class=\"{0}\"><a href=\"{1}\">{2}</a>", menuItemClass, manufacturerUrl, await _localizationService.GetLocalizedAsync(tv, x => x.Title));
                        }
                    }
                    else
                    {
                        sbMenus.AppendFormat("<li class=\"{0}\"><a href=\"{1}\">{2}</a>", menuItemClass, manufacturerUrl, pictureModel.ImageUrl, await _localizationService.GetLocalizedAsync(tv, x => x.Title));
                    }
                }
            }
        }

        private async Task<string> GetURLForMenu(Menu tv)
        {

            string urlScheme = "http://" + HttpContext.Request.Host;

            if (tv.TopicId.HasValue && tv.TopicId > 0)
            {
                return urlScheme + "/" + tv.PermanentRedirectUrl;
            }
            else if (tv.PermanentRedirect && !string.IsNullOrWhiteSpace(tv.PermanentRedirectUrl))
            {
                string ss = tv.PermanentRedirectUrl.ToLower().StartsWith("http") ? tv.PermanentRedirectUrl : urlScheme + "/" + tv.PermanentRedirectUrl;

                return tv.PermanentRedirectUrl.ToLower().StartsWith("http") ? tv.PermanentRedirectUrl : urlScheme + "/" + tv.PermanentRedirectUrl;
            }
            else if (tv.CategoryId.HasValue && tv.CategoryId.Value > 0)
            {
                var category = await _categoryService.GetCategoryByIdAsync(tv.CategoryId.Value);
                if (category != null)
                    return await _urlRecordService.GetSeNameAsync(category);

                else
                    return urlScheme;
            }
            else
            {
                return urlScheme;
            }
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
        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        //public async Task<IViewComponentResult> InvokeAsync()
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
            if (setting.BgPictureId > 0)
            {
                model.BgPictureUrl = await _pictureService.GetPictureUrlAsync(setting.BgPictureId);
            }

            ViewBag.MainMenuClass = mainMenuClass;
            ViewBag.SubMenuContainerClass = subMenuContainerClass;

            string urlFrmt = "<li><a href=\"{0}\">{1}</a></li>";
            bool supportResponsive = true;

            StringBuilder sbMenus = new StringBuilder();

            #region Get Menu

            List<Menu> Menus = new List<Menu>();
            Menus = (await _menuManagerService.GetAllMenusAsync(storeScope)).OrderBy(x => x.MenuOrder).ToList();

            if (!includeTopics)
            {
                Menus = Menus.Where(x => x.TopicId.HasValue == false || (x.TopicId.HasValue && x.TopicId == 0)).ToList();
            }

            //Add all menu by menu order
            foreach (Menu tv in Menus)
            {
                if (tv.CategoryId > 0)
                {
                    await GetTopicDescription(urlFrmt, sbMenus, tv);
                }
                else if (tv.ManufacturerId > 0)
                {
                    await GetTopicDescription(urlFrmt, sbMenus, tv);
                }
                else if (tv.TopicId.HasValue && tv.TopicId > 0)
                {
                    await GetTopicDescription(urlFrmt, sbMenus, tv);
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

            string ViewPath = "~/Plugins/Misc.MenuManager/Views/Shared/Components/CustomTopMenu/Default.cshtml";

            if (!string.IsNullOrEmpty(currentThemeName))
            {
                ViewPath = "~/Plugins/Misc.MenuManager/Themes/" + currentThemeName + "/Views/Shared/Components/CustomTopMenu/Default.cshtml";
            }
            return View(ViewPath, model);
        }
        #endregion

    }
}
