using Satyanam.Nop.Plugin.Misc.MenuManager.Validator;
using Nop.Web.Framework.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Satyanam.Nop.Plugin.Misc.MenuManager.Models
{
    //[Validator(typeof(MenuValidator))]
    public partial record ManageMenuModel : BaseNopEntityModel, ILocalizedModel<MenuLocalizedModel>
    {
        public ManageMenuModel()
        {
            Locales = new List<MenuLocalizedModel>();
            AvailableCategories = new List<SelectListItem>();
            AvailablePerentMenu = new List<SelectListItem>();
            AvailableManufacturer = new List<SelectListItem>();
            AvailableTopics = new List<SelectListItem>();
            AvailableCustomerRoles = new List<SelectListItem>();
            SelectedCustomerRoleIds = new List<int>();
        }

        public string MenuName { get; set; }
        public string Data { get; set; }
        public int Total { get; set; }

        public bool Flag { get; set; }

        public bool LogOnRequired { get; set; }

        [NopResourceDisplayName("MenuManager.Title")]
        public string Title { get; set; }

        [NopResourceDisplayName("MenuManager.MenuOrder")]
        public int MenuOrder { get; set; }

        [NopResourceDisplayName("MenuManager.ParentMenu")]
        public int? ParentMenuId { get; set; }

        [NopResourceDisplayName("MenuManager.ParentMenu")]
        public int? ParentId { get; set; }

        public bool PermanentRedirect { get; set; }

        [NopResourceDisplayName("MenuManager.PermanentRedirectUrl")]
        public string PermanentRedirectUrl { get; set; }

        public bool DisableLink { get; set; }
        public int MenuLevel { get; set; }

        public int? StoreId { get; set; }

        [NopResourceDisplayName("MenuManager.Category")]
        public int? CategoryId { get; set; }

        [NopResourceDisplayName("MenuManager.Manufacturer")]
        public int? ManufacturerId { get; set; }

        [NopResourceDisplayName("MenuManager.Topics")]
        public int? TopicId { get; set; }

        public bool HasChildren { get; set; }

        [NopResourceDisplayName("MenuManager.IsActive")]
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public DateTime CreatedDate { get; set; }
        public int CreatedUserId { get; set; }

        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedUserId { get; set; }

        public IList<MenuLocalizedModel> Locales { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Manufacturers.Fields.AclCustomerRoles")]
        public IList<int> SelectedCustomerRoleIds { get; set; }
        public IList<SelectListItem> AvailableCustomerRoles { get; set; }
        public bool IncludeTopics { get; set; }

        public bool DisplayWithPicture { get; set; }

        [NopResourceDisplayName("MenuManager.Picture")]
        [UIHint("Picture")]
        public int PictureId { get; set; }

        [NopResourceDisplayName("MenuManager.currentPicture")]
        public string PictureUrl { get; set; }

        public IList<SelectListItem> AvailableCategories { get; set; }
        public IList<SelectListItem> AvailablePerentMenu { get; set; }
        public IList<SelectListItem> AvailableManufacturer { get; set; }
        public IList<SelectListItem> AvailableTopics { get; set; }
    }


    public partial class MenuLocalizedModel : ILocalizedLocaleModel
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("MenuManager.Title")]
        public string Title { get; set; }
    }
}