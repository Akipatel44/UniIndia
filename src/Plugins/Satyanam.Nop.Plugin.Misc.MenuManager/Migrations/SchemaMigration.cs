using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Satyanam.Nop.Plugin.Misc.MenuManager.Domain;

namespace Satyanam.Nop.Plugin.Misc.MenuManager.Migrations
{

    [NopMigration("2024/02/03 08:40:55:1687541", "Misc.MenuManager base schema", MigrationProcessType.Installation)]
    public class SchemaMigration : AutoReversingMigration
    {
        public override void Up()
        {
            Create.TableFor<Menu>();
            Create.TableFor<MenuTv>();
        }
    }
}
