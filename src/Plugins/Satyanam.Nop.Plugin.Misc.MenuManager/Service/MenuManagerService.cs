using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Catalog;
using Satyanam.Nop.Plugin.Misc.MenuManager.Domain;
using Nop.Core;
using Nop.Services.Customers;
using Nop.Data;
using LinqToDB.DataProvider;
using System.Threading.Tasks;

namespace Satyanam.Nop.Plugin.Misc.MenuManager.Service
{
    /// <summary>
    /// Category service
    /// </summary>
    public partial class MenuManagerService : IMenuManagerService
    {

        #region Fields
        private readonly IRepository<Menu> _menuRepository;
        private readonly CatalogSettings _catalogSettings;
        private readonly IWorkContext _workContext;
       // private readonly IDataProvider _dataProvider;
        private readonly IStoreContext _storeContext;
        private readonly ICustomerService _customerService;
        private readonly INopDataProvider _nopDataProvider;
        #endregion

        #region Ctor

        public MenuManagerService(IRepository<Menu> menuRepository,
            CatalogSettings catalogSettings,
            IWorkContext workContext,
          //  IDataProvider dataProvider,
            IStoreContext storeContext,
            ICustomerService customerService,
            INopDataProvider nopDataProvider)
        {
            _menuRepository = menuRepository;
            _catalogSettings = catalogSettings;
            _workContext = workContext;
           // _dataProvider = dataProvider;
            _storeContext = storeContext;
            _customerService = customerService;
            _nopDataProvider = nopDataProvider;
        }

        #endregion

        #region Methods


        public virtual void DeleteMenu(Menu menu)
        {
            if (menu == null)
                throw new ArgumentNullException("menu");

            _menuRepository.Delete(menu);
        }

        public virtual async Task<IList<Menu>> GetAllMenusAsync(int storeScope = 0, int parentId = 0, bool showHidden = false)
        {
            var allowedCustomerRolesIds = showHidden ? new List<int>().ToArray() :(await _customerService.GetCustomerRoleIdsAsync(await _workContext.GetCurrentCustomerAsync()));
            
            //pass customer role identifiers as comma-delimited string
            var commaSeparatedAllowedCustomerRoleIds = string.Join(",", allowedCustomerRolesIds);

            //prepare input parameters
            //var pStoreId = _dataProvider.GetInt32Parameter("StoreId", storeScope);
            //var pStoreId = await _storeContext.GetCurrentStoreAsync();
            //var pAllowedCustomerRoleIds = SqlParameterHelper.GetStringParameter("allowedcustomerroleids", !_catalogSettings.IgnoreAcl ? commaSeparatedAllowedCustomerRoleIds : "");
            var customer = await _workContext.GetCurrentCustomerAsync();
            var customerRoleIds = await _customerService.GetCustomerRoleIdsAsync(customer);
            var customerRoleIdsString = string.Join(",", customerRoleIds);           

            //invoke stored procedure
            var menuList = await _nopDataProvider.QueryProcAsync<Menu>(
                "GetAllMenu",
                new LinqToDB.Data.DataParameter("ParentId", parentId),
                new LinqToDB.Data.DataParameter("StoreId",storeScope),
                new LinqToDB.Data.DataParameter("ShowHidden", showHidden),
                new LinqToDB.Data.DataParameter("AllowedCustomerRoleIds", customerRoleIdsString));          

            return menuList.ToList();
        }

        public virtual Menu GetById(int id)
        {
            if (id == 0)
                return null;

            var query = _menuRepository.GetById(id);

            return query;
        }

        public virtual void InsertMenu(Menu menu)
        {
            if (menu == null)
                throw new ArgumentNullException("menu");

            _menuRepository.Insert(menu);

        }

        /// <summary>
        /// Updates the category
        /// </summary>
        /// <param name="category">Category</param>
        public virtual void UpdateMenu(Menu menu)
        {
            if (menu == null)
                throw new ArgumentNullException("category");

            _menuRepository.Update(menu);

        }

        #endregion
    }
}
