using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core;
using Satyanam.Nop.Plugin.Misc.MenuManager.Domain;

namespace Satyanam.Nop.Plugin.Misc.MenuManager.Service
{
    /// <summary>
    /// Category service interface
    /// </summary>
    public partial interface IMenuManagerService
    {

        void DeleteMenu(Menu menu);

        //IList<Menu> GetAllMenus(int storeScope = 0, int parentId = 0, bool showHidden = false);
        Task<IList<Menu>> GetAllMenusAsync(int storeScope = 0, int parentId = 0, bool showHidden = false);

        Menu GetById(int id);

        void InsertMenu(Menu menu);

        void UpdateMenu(Menu menu);


    }
}
