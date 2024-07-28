using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Satyanam.Nop.Plugin.Misc.MenuManager.Domain;

namespace Satyanam.Nop.Plugin.Misc.MenuManager.Data
{
    public class MenuBuilder: NopEntityBuilder<Menu>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            
        }
    }
}
