using Nop.Core;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Security;
using System;

namespace Satyanam.Nop.Plugin.Misc.MenuManager.Domain
{
    
    public class Menu : BaseEntity , ILocalizedEntity, IAclSupported
    {
        public string MenuName { get; set; }

        public string Title { get; set; }
        public int MenuOrder { get; set; }

        public int? ParentMenuId { get; set; }

        public bool PermanentRedirect { get; set; }
        public string PermanentRedirectUrl { get; set; }

        public int MenuLevel { get; set; }

        public int? StoreId { get; set; }

        public int? TopicId { get; set; }
        public int? CategoryId { get; set; }
        public int? ManufacturerId { get; set; }

        public bool HasChildren { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public DateTime CreatedDate { get; set; }
        public int CreatedUserId { get; set; }

        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedUserId { get; set; }

        public int PictureId { get; set; }

        public bool SubjectToAcl { get; set; }
    }
}
