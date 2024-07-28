using Nop.Web.Framework.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace Satyanam.Nop.Plugin.Misc.MenuManager.Models
{
    public class MenuSettingsViewModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        
        //[NopResourceDisplayName("MenuManager.IncludeCategories")]
        //public bool IncludeCategories { get; set; }
        //public bool IncludeCategories_OverrideForStore { get; set; }

        //[NopResourceDisplayName("MenuManager.ShowCategoryInSingleMenu")]
        //public bool ShowCategoryInSingleMenu { get; set; }
        //public bool ShowCategoryInSingleMenu_OverrideForStore { get; set; }

        //[NopResourceDisplayName("MenuManager.IncludeManufacturers")]
        //public bool IncludeManufacturers { get; set; }
        //public bool IncludeManufacturers_OverrideForStore { get; set; }

        [NopResourceDisplayName("MenuManager.IncludeTopics")]
        public bool IncludeTopics { get; set; }
        public bool IncludeTopics_OverrideForStore { get; set; }

        [NopResourceDisplayName("MenuManager.ShowTopicContentOnHover")]
        public bool ShowTopicContentHover { get; set; }
        public bool ShowTopicContentHover_OverrideForStore { get; set; }

        [NopResourceDisplayName("MenuManager.DisplayWithPicture")]
        public bool DisplayWithPicture { get; set; }
        public bool DisplayWithPicture_OverrideForStore { get; set; }

        [NopResourceDisplayName("MenuManager.SubMenuPictureSize")]
        public int SubMenuPictureSize { get; set; }
        public bool SubMenuPictureSize_OverrideForStore { get; set; }

        [NopResourceDisplayName("MenuManager.DisplayTitle")]
        public bool DisplayTitle { get; set; }
        public bool DisplayTitle_OverrideForStore { get; set; }

        [NopResourceDisplayName("MenuManager.HiddenCategoryIfNoProduct")]
        public bool HiddenCategoryIfNoProduct { get; set; }
        public bool HiddenCategoryIfNoProduct_OverrideForStore { get; set; }

        [NopResourceDisplayName("MenuManager.StickMenuOnTop")]
        public bool StickMenuOnTop { get; set; }
        public bool StickMenuOnTop_OverrideForStore { get; set; }

        [NopResourceDisplayName("MenuManager.MainMenuClass")]
        public string MainMenuClass { get; set; }

        [NopResourceDisplayName("MenuManager.SubMenuContainerClass")]
        public string SubMenuContainerClass { get; set; }
        
        [NopResourceDisplayName("MenuManager.SubMenuClass")]
        public string SubMenuClass { get; set; }

        [NopResourceDisplayName("MenuManager.MenuItemClass")]
        public string MenuItemClass { get; set; }

        [NopResourceDisplayName("MenuManager.BackgroundColor")]
        public string BackgroundColor { get; set; }
        public bool BackgroundColor_OverrideForStore { get; set; }

        [NopResourceDisplayName("MenuManager.FontSize")]
        public string FontSize { get; set; }
        public bool FontSize_OverrideForStore { get; set; }

        [NopResourceDisplayName("MenuManager.FontStyle")]
        public string FontStyle { get; set; }
        public bool FontStyle_OverrideForStore { get; set; }

        [NopResourceDisplayName("MenuManager.FontColor")]
        public string FontColor { get; set; }
        public bool FontColor_OverrideForStore { get; set; }

        [NopResourceDisplayName("MenuManager.MobMenuBackHeader")]
        public string MobMenuBackHeader { get; set; }
        public bool MobMenuBackHeader_OverrideForStore { get; set; }

        [NopResourceDisplayName("MenuManager.BackgroundPicture")]
        [UIHint("Picture")]
        public int BgPictureId { get; set; }
        public bool BgPictureId_OverrideForStore { get; set; }

        [NopResourceDisplayName("MenuManager.currentPicture")]
        public string BgPictureUrl { get; set; }

    }
}