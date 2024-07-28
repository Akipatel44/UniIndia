using Satyanam.Nop.Plugin.Misc.MenuManager.Domain;
using Nop.Web.Framework.Controllers;
using System;
using System.Linq;
using Satyanam.Nop.Plugin.Misc.MenuManager.Models;
using Satyanam.Nop.Plugin.Misc.MenuManager.Service;
using Nop.Services.Stores;
using Nop.Core;
using Nop.Services.Configuration;
using Nop.Services.Catalog;
using Nop.Services.Topics;
using Nop.Services.Media;
using Nop.Services.Localization;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework;
using Nop.Services.Customers;
using Nop.Services.Security;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Web.Framework.Models.Extensions;
using Nop.Services.Messages;

namespace Satyanam.Nop.Plugin.Misc.MenuManager.Controllers
{
    public class MenuManagerController : BasePluginController
    {

        #region Fields
        private readonly IMenuManagerService _menuManagerService;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;
        private ISettingService _settingsSerivce;
        private ITopicService _topics;
        private readonly ICategoryService _categoryService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IPictureService _pictureService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ICustomerService _customerService;
        private readonly IAclService _aclService;
        private readonly IStoreContext _storeContext;
        private readonly INotificationService _notificationService;
        private readonly ILocalizationService _localizationService;
        #endregion

        #region Constructors
        public MenuManagerController(IMenuManagerService menuManagerService, IStoreService storeService
                                , IWorkContext workContext, ISettingService settingService, ITopicService topicService
                                , ICategoryService categoryService, IManufacturerService manufacturerService
                                , IPictureService pictureService, ILanguageService languageService
                                , ILocalizedEntityService localizedEntityService, ICustomerService customerService,
                                IAclService aclService, IStoreContext storeContext,
                                INotificationService notificationService, ILocalizationService localizationService)
        {
            _menuManagerService = menuManagerService;
            _storeService = storeService;
            _workContext = workContext;
            _settingsSerivce = settingService;
            _topics = topicService;
            _categoryService = categoryService;
            _manufacturerService = manufacturerService;
            _pictureService = pictureService;
            _languageService = languageService;
            _localizedEntityService = localizedEntityService;
            _aclService = aclService;
            _customerService = customerService;
            _storeContext = storeContext;
            _notificationService = notificationService;
            _localizationService = localizationService;
        }

        #endregion

        #region Utilities

        protected virtual async Task PrepareAclModel(ManageMenuModel model, Menu menu)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (menu != null)
                model.SelectedCustomerRoleIds = (await _aclService.GetCustomerRoleIdsWithAccessAsync(menu)).ToList();

            var allRoles = await _customerService.GetAllCustomerRolesAsync(true);
            foreach (var role in allRoles)
            {
                model.AvailableCustomerRoles.Add(new SelectListItem
                {
                    Text = role.Name,
                    Value = role.Id.ToString(),
                    Selected = model.SelectedCustomerRoleIds.Contains(role.Id)
                });
            }
        }

        protected virtual async Task SaveMenuAcl(Menu menu, ManageMenuModel model)
        {
            menu.SubjectToAcl = model.SelectedCustomerRoleIds.Any();

            var existingAclRecords = await _aclService.GetAclRecordsAsync(menu);
            var allCustomerRoles = await _customerService.GetAllCustomerRolesAsync(true);
            foreach (var customerRole in allCustomerRoles)
            {
                if (model.SelectedCustomerRoleIds.Contains(customerRole.Id))
                {
                    //new role
                    if (existingAclRecords.Count(acl => acl.CustomerRoleId == customerRole.Id) == 0)
                        await _aclService.InsertAclRecordAsync(menu, customerRole.Id);
                }
                else
                {
                    //remove role
                    var aclRecordToDelete = existingAclRecords.FirstOrDefault(acl => acl.CustomerRoleId == customerRole.Id);
                    if (aclRecordToDelete != null)
                        await _aclService.DeleteAclRecordAsync(aclRecordToDelete);
                }
            }
        }

        protected virtual async Task PrepareAllCategoriesModel(ManageMenuModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            model.AvailableCategories.Add(new SelectListItem
            {
                Text = "Select Category",
                Value = "0"
            });
            var categories = await _categoryService.GetAllCategoriesAsync(showHidden: true);
            foreach (var c in categories)
            {
                model.AvailableCategories.Add(new SelectListItem
                {
                    //Text = c.GetFormattedBreadCrumbAsync(categories),
                    Text = await _categoryService.GetFormattedBreadCrumbAsync(c, categories),
                    Value = c.Id.ToString()
                });
            }
        }

        protected virtual async Task PrepareAllPerentMenuModel(ManageMenuModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            model.AvailablePerentMenu.Add(new SelectListItem
            {
                Text = "Select PerentMenu",
                Value = null
            });
            var perentmenu = await _menuManagerService.GetAllMenusAsync();
            perentmenu = perentmenu.Where(x => x.Title != model.Title).ToList();
            foreach (var p in perentmenu)
            {
                model.AvailablePerentMenu.Add(new SelectListItem
                {
                    //Text = c.GetFormattedBreadCrumbAsync(categories),
                    Text = p.Title,
                    Value = p.Id.ToString()
                });
            }
        }

        protected virtual async Task PrepareAllManufacturerModel(ManageMenuModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            //var storeScope = GetActiveStoreScopeConfigurationAsync(_storeService, _workContext);

            model.AvailableManufacturer.Add(new SelectListItem
            {
                Text = "Select Manufacturer",
                Value = "0"
            });

            var manufacturer = await _manufacturerService.GetAllManufacturersAsync();

            foreach (var m in manufacturer)
            {
                model.AvailableManufacturer.Add(new SelectListItem
                {
                    Text = m.Name,
                    Value = m.Id.ToString()
                });
            }
        }

        protected virtual async Task PrepareAllTopicsModel(ManageMenuModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            //var storeScope = GetActiveStoreScopeConfiguration(_storeService, _workContext);

            model.AvailableTopics.Add(new SelectListItem
            {
                Text = "Select Topic",
                Value = "0"
            });

            var topics = await _topics.GetAllTopicsAsync(storeScope, true);
            foreach (var t in topics)
            {
                model.AvailableTopics.Add(new SelectListItem
                {
                    Text = t.SystemName,
                    Value = t.Id.ToString()
                });
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

        [HttpPost]
        [Area(AreaNames.Admin)]
        public async Task<JsonResult> GetMenus()
        {
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            //var storeScope = GetActiveStoreScopeConfiguration(_storeService, _workContext);

            /* var Menus = (from Menu in await _menuManagerService.GetAllMenusAsync(storeScope,0,true)
                          select (new MenuTv { Id = Menu.Id, ParentMenuId = Menu.ParentMenuId, Title = Menu.Title, MenuName = Menu.MenuName })).ToList();

             var gridModel = new DataSourceResult
             {
                 Data = Menus,
                 Total = Menus.Count
             };*/

            var searchModel = new ManageMenuSearchModel();

            searchModel.SetGridPageSize();
            var menus = (await _menuManagerService.GetAllMenusAsync(storeScope, 0, true)).ToPagedList(searchModel);

            var gridModel = await new ManageMenuListModel().PrepareToGridAsync(searchModel, menus, () =>
            {
                //fill in model values from the entity
                return menus.SelectAwait(async MenuTv =>
                {
                    //fill in model values from the entity
                    var manageMenuModel = new ManageMenuModel
                    {
                        Id = MenuTv.Id,
                        ParentMenuId = MenuTv.ParentMenuId,
                        //ParentId = MenuTv.ParentMenuId,
                        Title = MenuTv.Title,
                        MenuName = MenuTv.MenuName,
                    };
                    return manageMenuModel;
                });
            });
            return Json(gridModel);
        }

        [HttpPost]
        [Area(AreaNames.Admin)]
        public async Task<JsonResult> UpdateMenus(MenuTv[] Menus)
        {
            if (Menus != null && Menus.Length > 0)
            {
                for (int ictr = 0; ictr < Menus.Length; ictr++)
                {
                    if (Menus[ictr].Id > 0)
                    {
                        var Menu = _menuManagerService.GetById(Menus[ictr].Id);
                        if (Menu != null)
                        {
                            if (!Menu.ParentMenuId.HasValue)
                                Menu.MenuOrder = ictr + 1;

                            Menu.LastModifiedDate = DateTime.UtcNow;
                            Menu.LastModifiedUserId = _workContext.GetCurrentCustomerAsync().Id;
                            Menu.ParentMenuId = Menus[ictr].ParentMenuId;
                            Menu.MenuLevel = Menus[ictr].ParentMenuId == null ? 1 : 2;
                            Menu.HasChildren = (Menus.Where(s => s.ParentMenuId == s.Id).Count() > 0);

                            _menuManagerService.UpdateMenu(Menu);
                        }
                        Menu = null;
                    }
                }
            }

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Menu changes saved"));

            return Json("complete");
        }
        #endregion

        #region Configure / ManageMenu / Delete Menu
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> Configure()
        {
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            //var storeScope = GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var menuSettings = _settingsSerivce.LoadSetting<MenuManagerSettings>(storeScope);
            var model = new MenuSettingsViewModel();

            model.IncludeTopics = menuSettings.IncludeTopics;
            model.ShowTopicContentHover = menuSettings.ShowTopicContentHover;
            model.DisplayWithPicture = menuSettings.DisplayWithPicture;
            model.StickMenuOnTop = menuSettings.StickMenuOnTop;
            model.HiddenCategoryIfNoProduct = menuSettings.HiddenCategoryIfNoProduct;
            model.MainMenuClass = menuSettings.MainMenuClass;
            model.SubMenuContainerClass = menuSettings.SubMenuContainerClass;
            model.SubMenuClass = menuSettings.SubMenuClass;
            model.MenuItemClass = menuSettings.MenuItemClass;

            model.SubMenuPictureSize = menuSettings.SubMenuPictureSize;
            model.DisplayTitle = menuSettings.DisplayTitle;
            model.BackgroundColor = menuSettings.BackgroundColor;
            model.FontSize = menuSettings.FontSize;
            model.FontStyle = menuSettings.FontStyle;
            model.FontColor = menuSettings.FontColor;
            model.MobMenuBackHeader = menuSettings.MobMenuBackHeader;
            model.BgPictureId = menuSettings.BgPictureId;
            model.BgPictureUrl = await _pictureService.GetPictureUrlAsync(model.BgPictureId);
            model.ActiveStoreScopeConfiguration = storeScope;
            if (storeScope > 0)
            {

                model.IncludeTopics_OverrideForStore = _settingsSerivce.SettingExists(menuSettings, x => x.IncludeTopics, storeScope);
                model.ShowTopicContentHover_OverrideForStore = _settingsSerivce.SettingExists(menuSettings, x => x.ShowTopicContentHover, storeScope);
                model.DisplayWithPicture_OverrideForStore = _settingsSerivce.SettingExists(menuSettings, x => x.DisplayWithPicture, storeScope);
                model.StickMenuOnTop_OverrideForStore = _settingsSerivce.SettingExists(menuSettings, x => x.StickMenuOnTop, storeScope);
                model.HiddenCategoryIfNoProduct_OverrideForStore = _settingsSerivce.SettingExists(menuSettings, x => x.HiddenCategoryIfNoProduct, storeScope);

                model.SubMenuPictureSize_OverrideForStore = _settingsSerivce.SettingExists(menuSettings, x => x.SubMenuPictureSize, storeScope);
                model.DisplayTitle_OverrideForStore = _settingsSerivce.SettingExists(menuSettings, x => x.DisplayTitle, storeScope);
                model.BackgroundColor_OverrideForStore = _settingsSerivce.SettingExists(menuSettings, x => x.BackgroundColor, storeScope);
                model.FontSize_OverrideForStore = _settingsSerivce.SettingExists(menuSettings, x => x.FontSize, storeScope);
                model.FontStyle_OverrideForStore = _settingsSerivce.SettingExists(menuSettings, x => x.FontStyle, storeScope);
                model.FontColor_OverrideForStore = _settingsSerivce.SettingExists(menuSettings, x => x.FontColor, storeScope);
                model.MobMenuBackHeader_OverrideForStore = _settingsSerivce.SettingExists(menuSettings, x => x.MobMenuBackHeader, storeScope);
                model.BgPictureId_OverrideForStore = _settingsSerivce.SettingExists(menuSettings, x => x.BgPictureId, storeScope);
            }

            return View("~/Plugins/Misc.MenuManager/Views/MenuManager/Configure.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> Configure(MenuSettingsViewModel model)
        {
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            //var storeScope = GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var menuSettings = _settingsSerivce.LoadSetting<MenuManagerSettings>(storeScope);

            //get previous picture identifiers
            var previousPictureIds = new[]
            {
                menuSettings.BgPictureId
            };

            menuSettings.IncludeTopics = model.IncludeTopics;
            menuSettings.ShowTopicContentHover = model.ShowTopicContentHover;
            menuSettings.DisplayWithPicture = model.DisplayWithPicture;
            menuSettings.StickMenuOnTop = model.StickMenuOnTop;
            menuSettings.HiddenCategoryIfNoProduct = model.HiddenCategoryIfNoProduct;
            menuSettings.MainMenuClass = model.MainMenuClass;
            menuSettings.SubMenuContainerClass = model.SubMenuContainerClass;
            menuSettings.SubMenuClass = model.SubMenuClass;
            menuSettings.MenuItemClass = model.MenuItemClass;

            menuSettings.SubMenuPictureSize = model.SubMenuPictureSize;
            menuSettings.DisplayTitle = model.DisplayTitle;
            menuSettings.BackgroundColor = model.BackgroundColor;
            menuSettings.FontSize = model.FontSize;
            menuSettings.FontStyle = model.FontStyle;
            menuSettings.FontColor = model.FontColor;
            menuSettings.MobMenuBackHeader = model.MobMenuBackHeader;
            menuSettings.BgPictureId = model.BgPictureId;

            if (model.IncludeTopics_OverrideForStore || storeScope == 0)
                _settingsSerivce.SaveSetting(menuSettings, x => x.IncludeTopics, storeScope, false);
            else if (storeScope > 0)
                await _settingsSerivce.DeleteSettingAsync(menuSettings, x => x.IncludeTopics, storeScope);

            if (model.ShowTopicContentHover_OverrideForStore || storeScope == 0)
                _settingsSerivce.SaveSetting(menuSettings, x => x.ShowTopicContentHover, storeScope, false);
            else if (storeScope > 0)
                await _settingsSerivce.DeleteSettingAsync(menuSettings, x => x.ShowTopicContentHover, storeScope);

            if (model.DisplayWithPicture_OverrideForStore || storeScope == 0)
                _settingsSerivce.SaveSetting(menuSettings, x => x.DisplayWithPicture, storeScope, false);
            else if (storeScope > 0)
                await _settingsSerivce.DeleteSettingAsync(menuSettings, x => x.DisplayWithPicture, storeScope);

            if (model.StickMenuOnTop_OverrideForStore || storeScope == 0)
                _settingsSerivce.SaveSetting(menuSettings, x => x.StickMenuOnTop, storeScope, false);
            else if (storeScope > 0)
                await _settingsSerivce.DeleteSettingAsync(menuSettings, x => x.StickMenuOnTop, storeScope);

            if (model.HiddenCategoryIfNoProduct_OverrideForStore || storeScope == 0)
                _settingsSerivce.SaveSetting(menuSettings, x => x.HiddenCategoryIfNoProduct, storeScope, false);
            else if (storeScope > 0)
                await _settingsSerivce.DeleteSettingAsync(menuSettings, x => x.HiddenCategoryIfNoProduct, storeScope);


            if (model.SubMenuPictureSize_OverrideForStore || storeScope == 0)
                _settingsSerivce.SaveSetting(menuSettings, x => x.SubMenuPictureSize, storeScope, false);
            else if (storeScope > 0)
                await _settingsSerivce.DeleteSettingAsync(menuSettings, x => x.SubMenuPictureSize, storeScope);

            if (model.DisplayTitle_OverrideForStore || storeScope == 0)
                _settingsSerivce.SaveSetting(menuSettings, x => x.DisplayTitle, storeScope, false);
            else if (storeScope > 0)
                await _settingsSerivce.DeleteSettingAsync(menuSettings, x => x.DisplayTitle, storeScope);

            if (model.BackgroundColor_OverrideForStore || storeScope == 0)
                _settingsSerivce.SaveSetting(menuSettings, x => x.BackgroundColor, storeScope, false);
            else if (storeScope > 0)
                await _settingsSerivce.DeleteSettingAsync(menuSettings, x => x.BackgroundColor, storeScope);

            if (model.FontStyle_OverrideForStore || storeScope == 0)
                _settingsSerivce.SaveSetting(menuSettings, x => x.FontStyle, storeScope, false);
            else if (storeScope > 0)
                await _settingsSerivce.DeleteSettingAsync(menuSettings, x => x.FontStyle, storeScope);

            if (model.FontSize_OverrideForStore || storeScope == 0)
                _settingsSerivce.SaveSetting(menuSettings, x => x.FontSize, storeScope, false);
            else if (storeScope > 0)
                await _settingsSerivce.DeleteSettingAsync(menuSettings, x => x.FontSize, storeScope);

            if (model.FontColor_OverrideForStore || storeScope == 0)
                _settingsSerivce.SaveSetting(menuSettings, x => x.FontColor, storeScope, false);
            else if (storeScope > 0)
                await _settingsSerivce.DeleteSettingAsync(menuSettings, x => x.FontColor, storeScope);

            if (model.MobMenuBackHeader_OverrideForStore || storeScope == 0)
                _settingsSerivce.SaveSetting(menuSettings, x => x.MobMenuBackHeader, storeScope, false);
            else if (storeScope > 0)
                await _settingsSerivce.DeleteSettingAsync(menuSettings, x => x.MobMenuBackHeader, storeScope);

            if (model.BgPictureId_OverrideForStore || storeScope == 0)
                _settingsSerivce.SaveSetting(menuSettings, x => x.BgPictureId, storeScope, false);
            else if (storeScope > 0)
                await _settingsSerivce.DeleteSettingAsync(menuSettings, x => x.BgPictureId, storeScope);

            _settingsSerivce.ClearCache();

            var currentPictureIds = new[]
            {
                menuSettings.BgPictureId
            };

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }

        [HttpGet]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> ManageMenus()
        {
            Menu Menu = new Menu();
            ManageMenuModel model = new ManageMenuModel();

            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            //var storeScope = GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var setting = _settingsSerivce.LoadSetting<MenuManagerSettings>(storeScope);

            model.IncludeTopics = setting.IncludeTopics;
            model.DisplayWithPicture = setting.DisplayWithPicture;

            await PrepareAllCategoriesModel(model);
            await PrepareAllPerentMenuModel(model);
            await PrepareAllManufacturerModel(model);
            await PrepareAllTopicsModel(model);
            await PrepareAclModel(model, null);
            //locales
            await AddLocalesAsync(_languageService, model.Locales, async (locale, languageId) =>
            {
                locale.Title = await _localizationService.GetLocalizedAsync(Menu, x => x.Title, languageId, false, false);
            });

            return View("~/Plugins/Misc.MenuManager/Views/MenuManager/ManageMenus.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> ManageMenus(ManageMenuModel model, IFormFileCollection form)
        {
            List<Menu> Menus = new List<Menu>();
            Menus = (await _menuManagerService.GetAllMenusAsync()).OrderBy(x => x.MenuOrder).ToList();

            //Menus = Menus.Where(x => x.TopicId.HasValue == false || (x.TopicId.HasValue && x.TopicId == 0)).ToList();

            //Add all menu by menu order
            foreach (Menu tv in Menus)
            {
                if (model.Title == tv.Title)
                {
                    model.Flag = true;
                }
               
            }

            //if (model.Flag == false && model.Title != null)
            //{
                var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
                //var storeScope = GetActiveStoreScopeConfiguration(_storeService, _workContext);
                if (storeScope == 0)
                {
                    storeScope = _storeService.GetAllStores().FirstOrDefault() != null ? _storeService.GetAllStores().FirstOrDefault().Id : 0;
                }

                var setting = _settingsSerivce.LoadSetting<MenuManagerSettings>(storeScope);
                model.IncludeTopics = setting.IncludeTopics;
                model.DisplayWithPicture = setting.DisplayWithPicture;

                if (Request.Form.Keys.Count() > 0 && model.Id == 0)
                {
                  if (model.Flag == false && model.Title != null)
                  {

                    Menu menu = new Menu();
                    menu.MenuName = model.Title;
                    menu.Title = model.Title;
                    menu.CategoryId = model.CategoryId;
                    menu.TopicId = model.TopicId;
                    await PrepareAllCategoriesModel(model);
                    await PrepareAllPerentMenuModel(model);
                    await PrepareAllManufacturerModel(model);
                    await PrepareAllTopicsModel(model);
                    menu.ManufacturerId = model.ManufacturerId;
                    menu.ParentMenuId = model.ParentMenuId;
                    //menu.ParentMenuId = model.ParentId;
                    menu.StoreId = storeScope;
                    menu.MenuOrder = model.MenuOrder;
                    menu.IsActive = model.IsActive;
                    menu.PermanentRedirect = true;
                    menu.PermanentRedirectUrl = model.PermanentRedirectUrl;
                    menu.CreatedDate = DateTime.UtcNow;
                    menu.CreatedUserId = _workContext.GetCurrentCustomerAsync().Id;

                    menu.PictureId = model.PictureId;

                    _menuManagerService.InsertMenu(menu);

                    await SaveMenuAcl(menu, model);

                    ModelState.Clear();
                  }

                }
                else
                {
                    var menu = _menuManagerService.GetById(model.Id);

                    menu.HasChildren = model.HasChildren;
                    menu.Title = model.Title;
                    menu.CategoryId = model.CategoryId;
                    menu.TopicId = model.TopicId;
                    menu.ManufacturerId = model.ManufacturerId;
                    await PrepareAllCategoriesModel(model);
                    await PrepareAllManufacturerModel(model);
                    await PrepareAllTopicsModel(model);
                    //menu.ParentMenuId = model.ParentId;
                    menu.ParentMenuId = model.ParentMenuId;
                    menu.MenuOrder = model.MenuOrder;
                    menu.IsActive = model.IsActive;
                    menu.StoreId = storeScope;
                    menu.PermanentRedirectUrl = model.PermanentRedirectUrl;
                    menu.LastModifiedDate = DateTime.UtcNow;
                    menu.LastModifiedUserId = (await _workContext.GetCurrentCustomerAsync()).Id;

                    menu.PictureId = model.PictureId;
                    await SaveMenuAcl(menu, model);
                    _menuManagerService.UpdateMenu(menu);

                    ModelState.Clear();
                    UpdateLocales(menu, model);

                }

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugin.Misc.MenuManager.Save"));
                //SuccessNotification("Your menu item has been saved.");

            //}

            //ModelState.Clear();

            model.Title = "";
            model.MenuOrder = 0;
            //model.ParentId = null;
            model.ParentMenuId = null;
            model.IsActive = false;
            model.PermanentRedirectUrl = "";
            model.CategoryId = 0;
            model.TopicId = 0;
            model.PictureId = 0;
            await PrepareAclModel(model, null);
            return View("~/Plugins/Misc.MenuManager/Views/MenuManager/ManageMenus.cshtml", model);
        }

        [Area(AreaNames.Admin)]
        public async Task<IActionResult> Delete(int id)
        {

            var menu = _menuManagerService.GetById(id);
            if (menu == null)
                //No topic found with the specified id
                return RedirectToAction("ManageMenus");

            //menu.IsDeleted = true;
            var model = new ManageMenuModel();

            _menuManagerService.DeleteMenu(menu);
            ModelState.Clear();

            _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Plugin.Misc.MenuManager.Delete"));
            //SuccessNotification("Your menu item has been removed.");
            //return RedirectToAction("ManageMenus");
            return Content(await _localizationService.GetResourceAsync("Plugin.Misc.MenuManager.Delete"));
        }
    
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> GetMenu(int id)
        {
            var Menu = _menuManagerService.GetById(id);
            var model = new ManageMenuModel
            {
                Id = Menu.Id,
                Title = Menu.Title,
                MenuOrder = Menu.MenuOrder,
                //ParentId = Menu.ParentMenuId,
                ParentMenuId = Menu.ParentMenuId,
                CategoryId = Menu.CategoryId,
                ManufacturerId = Menu.ManufacturerId,
                TopicId = Menu.TopicId,
                PermanentRedirect = Menu.PermanentRedirect,
                PermanentRedirectUrl = Menu.PermanentRedirectUrl,
                IsActive = Menu.IsActive,
                PictureId = Menu.PictureId

            };
            model.PictureUrl = await _pictureService.GetPictureUrlAsync(model.PictureId, 100, true);

            await AddLocalesAsync(_languageService, model.Locales, async (locale, languageId) =>
            {
                locale.Title = await _localizationService.GetLocalizedAsync(Menu, x => x.Title, languageId, false, false);
            });

            await PrepareAclModel(model, Menu);

            return Json(model);
        }

        public async Task<JsonResult> GetMenusTv()
        {
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            //var storeScope = GetActiveStoreScopeConfiguration(_storeService, _workContext);
            if (storeScope == 0)
            {
                storeScope = _storeService.GetAllStores().FirstOrDefault() != null ? _storeService.GetAllStores().FirstOrDefault().Id : 0;
            }
            var setting = _settingsSerivce.LoadSetting<MenuManagerSettings>(storeScope);

            bool includeTopics = setting.IncludeTopics;
            bool hiddenCategory = setting.HiddenCategoryIfNoProduct;

            string categoryName = setting.CategoryMenuTitle;
            string manufacturerName = setting.ManufacturerName;

            List<MenuTv> Menus = new List<MenuTv>();

            Menus = (from Menu in (await _menuManagerService.GetAllMenusAsync(storeScope, 0, true)).Where(x => x.CreatedUserId > 0 || x.CategoryId > 0 || (x.TopicId.HasValue && x.TopicId > 0) || x.MenuName == manufacturerName)
                     where Menu.ParentMenuId == null && Menu.IsDeleted == false
                     orderby Menu.MenuOrder
                     select (new MenuTv { Id = Menu.Id, ParentMenuId = Menu.ParentMenuId, Title = Menu.Title, MenuName = Menu.MenuName, HasChildren = false, TopicId = Menu.TopicId, MenuOrder = Menu.MenuOrder })).ToList();

            List<MenuTv> CategoryMenus = new List<MenuTv>();

            // Bind Child menu of all parent menu
            foreach (MenuTv tv in Menus)
            {
                tv.ChildMenus = (from Menu in await _menuManagerService.GetAllMenusAsync(storeScope, tv.Id, true)
                                 orderby Menu.MenuOrder
                                 select (new MenuTv { Id = Menu.Id, ParentMenuId = Menu.ParentMenuId, Title = Menu.Title, MenuName = Menu.MenuName, CategoryId = Menu.CategoryId.HasValue ? Menu.CategoryId.Value : 0 })).ToList();

                tv.HasChildren = (tv.ChildMenus != null && tv.ChildMenus.Count > 0);
            }

            return Json(Menus.OrderBy(x => x.MenuOrder));
        }

        public async Task<JsonResult> GetMenuTv(int id)
        {
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            //var storeScope = GetActiveStoreScopeConfiguration(_storeService, _workContext);
            if (storeScope == 0)
            {
                storeScope = _storeService.GetAllStores().FirstOrDefault() != null ? _storeService.GetAllStores().FirstOrDefault().Id : 0;
            }
            var setting = _settingsSerivce.LoadSetting<MenuManagerSettings>(storeScope);

            var menu = _menuManagerService.GetById(id);

            bool includeTopics = setting.IncludeTopics;
            bool hiddenCategory = setting.HiddenCategoryIfNoProduct;

            string categoryName = setting.CategoryMenuTitle;
            string manufacturerName = setting.ManufacturerName;

            List<MenuTv> Menus = new List<MenuTv>();

            Menus = (from Menu in (await _menuManagerService.GetAllMenusAsync(storeScope, 0, true)).Where(x => x.Id != menu.Id && (x.CreatedUserId > 0 || x.CategoryId > 0 || (x.TopicId.HasValue && x.TopicId > 0) || x.MenuName == manufacturerName))
                     where Menu.ParentMenuId == null && Menu.IsDeleted == false
                     orderby Menu.MenuOrder
                     select (new MenuTv { Id = Menu.Id, ParentMenuId = Menu.ParentMenuId, Title = Menu.Title, MenuName = Menu.MenuName, HasChildren = false, TopicId = Menu.TopicId, MenuOrder = Menu.MenuOrder })).ToList();

            List<MenuTv> CategoryMenus = new List<MenuTv>();

            // Bind Child menu of all parent menu
            foreach (MenuTv tv in Menus)
            {
                tv.ChildMenus = (from Menu in await _menuManagerService.GetAllMenusAsync(storeScope, tv.Id, true)
                                 orderby Menu.MenuOrder
                                 select (new MenuTv { Id = Menu.Id, ParentMenuId = Menu.ParentMenuId, Title = Menu.Title, MenuName = Menu.MenuName, CategoryId = Menu.CategoryId.HasValue ? Menu.CategoryId.Value : 0 })).ToList();

                tv.HasChildren = (tv.ChildMenus != null && tv.ChildMenus.Count > 0);
            }

            return Json(Menus.OrderBy(x => x.MenuOrder));
        }
        #endregion
    }
}
