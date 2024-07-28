using FluentValidation;
using Satyanam.Nop.Plugin.Misc.MenuManager.Models;
using Nop.Services.Localization;
using System.Threading.Tasks;

namespace Satyanam.Nop.Plugin.Misc.MenuManager.Validator
{
    public class MenuValidator : AbstractValidator<ManageMenuModel>
    {
        public async Task MenuValidatorAsync(ILocalizationService localizationService)
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage(await localizationService.GetResourceAsync("MenuManager.MenuTitleRequired"));
           
        }
    }
}
