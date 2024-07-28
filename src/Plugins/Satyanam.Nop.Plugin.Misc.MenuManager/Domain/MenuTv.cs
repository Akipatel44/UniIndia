using System.Collections.Generic;
using Nop.Core;

namespace Satyanam.Nop.Plugin.Misc.MenuManager.Domain
{
    public class MenuTv:BaseEntity
    {
        public string MenuName { get; set; }
        public string Title { get; set; }
        public int? ParentMenuId { get; set; }
        public int? TopicId { get; set; }
        public bool HasChildren { get; set; }
        public int? CategoryId { get; set; }
        public int MenuOrder { get; set; }
        public List<MenuTv> ChildMenus { get; set; }
    }
}
