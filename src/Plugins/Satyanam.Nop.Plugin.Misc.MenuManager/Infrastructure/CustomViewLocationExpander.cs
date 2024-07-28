using Microsoft.AspNetCore.Mvc.Razor;
using Nop.Web.Framework;
using Nop.Web.Framework.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satyanam.Nop.Plugin.Misc.MenuManager.Infrastructure
{
    public class CustomViewLocationExpander : IViewLocationExpander
    {
        private const string THEME_KEY = "nop.themename";

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            if (context.Values.TryGetValue(THEME_KEY, out string theme))
            {
                if (context.ViewName == "Components/TopMenu/Default")
                {
                    viewLocations = new[] {
                        $"~/Plugins/Misc.MenuManager/Themes/{theme}/Views/Shared/Components/TopmenuOverride/Default.cshtml",
                    }
                        .Concat(viewLocations);
                }
            }

            return viewLocations;
        }

        public void PopulateValues(ViewLocationExpanderContext context)
        {
            ////no need to add the themeable view locations at all as the administration should not be themeable anyway
            //if (context.AreaName?.Equals(AreaNames.Admin) ?? false)
            //    return;

            //var themeContext = (IThemeContext)context.ActionContext.HttpContext.RequestServices.GetService(typeof(IThemeContext));
            //context.Values[THEME_KEY] = themeContext.WorkingThemeName;
        }
    }
}
