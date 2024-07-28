using Nop.Core.Configuration;

namespace Satyanam.Nop.Plugin.Misc.MenuManager
{
    public class MenuManagerSettings : ISettings
    {
        //public bool IncludeCategories { get; set; }

        //public bool ShowCategoryInSingleMenu { get; set; }

        //public bool IncludeManufacturers { get; set; }

        public bool IncludeTopics { get; set; }

        public bool ShowTopicContentHover { get; set; }

        public bool DisplayWithPicture { get; set; }

        public int SubMenuPictureSize { get; set; }

        public bool DisplayTitle { get; set; }

        public bool StickMenuOnTop { get; set; }

        public bool HiddenCategoryIfNoProduct { get; set; }

        public string MainMenuClass { get; set; }

        public string SubMenuContainerClass { get; set; }

        public string SubMenuClass { get; set; }

        public string MenuItemClass { get; set; }

        public string BackgroundColor { get; set; }

        public string FontSize { get; set; }

        public string FontStyle { get; set; }

        public string FontColor { get; set; }

        public string MobMenuBackHeader { get; set; }

        public int BgPictureId { get; set; }

        public string CategoryMenuTitle {get;set;}

        public string ManufacturerName { get; set; }

        public bool IncludeCategories { get; set; }

    }
}