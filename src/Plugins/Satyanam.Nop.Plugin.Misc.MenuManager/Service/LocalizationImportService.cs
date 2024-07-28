using System.IO;
using System.Linq;
using System.Xml;
using Nop.Services.Localization;
using Nop.Core.Infrastructure;
using Nop.Core.Domain.Localization;
using Nop.Core;
using System.Threading.Tasks;
using Nop.Services.Themes;

namespace Satyanam.Nop.Plugin.Misc.MenuManager.Service
{
    public class LocalizationImportService
    {
        #region Fields

        private readonly INopFileProvider _fileProvider;

        #endregion

        #region Ctor

        public LocalizationImportService(INopFileProvider fileProvider)
        {
            _fileProvider = fileProvider;
        }

        #endregion


        public async Task AddLocalizationsAsync()
        {
            var folder = GetFolderPath();
            if (folder.Exists || (folder.Exists && folder.GetFiles("*.xml").Count() > 0))
            {
                var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
                var languageService = EngineContext.Current.Resolve<ILanguageService>();
                var languages = languageService.GetAllLanguages();

                foreach (var file in folder.GetFiles("*.xml"))
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(file.FullName);

                    XmlNode parentNode = xmlDoc.SelectSingleNode("/Language");
                    var language = languages.FirstOrDefault(x => x.Name == parentNode.Attributes["Name"].Value);
                    if (language != null)
                    {
                        XmlNodeList nodeList = xmlDoc.SelectNodes("/Language/LocaleResource");
                        foreach (XmlNode node in nodeList)
                        {
                            string resourceName = node.Attributes["Name"].Value;
                            var valueNode = node.SelectNodes("Value")[0];
                            string resourceValue = valueNode.InnerXml;
                            var lsr = localizationService.GetLocaleStringResourceByName(resourceName, language.Id, false);
                            if (lsr == null)
                            {
                                lsr = new LocaleStringResource()
                                {
                                    LanguageId = language.Id,
                                    ResourceName = resourceName,
                                    ResourceValue = resourceValue
                                };
                               await localizationService.InsertLocaleStringResourceAsync(lsr);
                            }
                            else
                            {
                                lsr.ResourceValue = resourceValue;
                               await localizationService.UpdateLocaleStringResourceAsync(lsr);
                            }
                        }
                    }
                }
            }

        }


        public async Task GetSupportedWidgetZonesAsync()
        {
            var folder = GetFolderPath();
            if (folder.Exists || (folder.Exists && folder.GetFiles("*.xml").Count() > 0))
            {
                var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
                var languageService = EngineContext.Current.Resolve<ILanguageService>();
                var languages = languageService.GetAllLanguages();

                foreach (var file in folder.GetFiles("*.xml"))
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(file.FullName);

                    XmlNode parentNode = xmlDoc.SelectSingleNode("/SupportedWidgetZones");

                    XmlNodeList nodeList = xmlDoc.SelectNodes("/SupportedWidgetZones/WidgetZone");
                    foreach (XmlNode node in nodeList)
                    {
                        string resourceName = node.Attributes["name"].Value;
                        var valueNode = node.SelectNodes("value")[0];
                        string resourceValue = valueNode.InnerXml;
                        
                    }
                }

            }

        }

        public async Task DeleteLocalizationsAsync()
        {
            var folder = GetFolderPath();
            if (folder.Exists || (folder.Exists && folder.GetFiles("*.xml").Count() > 0))
            {
                var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
                var languageService = EngineContext.Current.Resolve<ILanguageService>();
                var languages = languageService.GetAllLanguages();

                foreach (var file in folder.GetFiles("*.xml"))
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(file.FullName);

                    XmlNode parentNode = xmlDoc.SelectSingleNode("/Language");
                    var language = languages.FirstOrDefault(x => x.Name == parentNode.Attributes["Name"].Value);
                    if (language != null)
                    {
                        XmlNodeList nodeList = xmlDoc.SelectNodes("/Language/LocaleResource");
                        foreach (XmlNode node in nodeList)
                        {
                            string resourceName = node.Attributes["Name"].Value;
                            string resourceValue = node.InnerText;

                            var lsr = localizationService.GetLocaleStringResourceByName(resourceName, language.Id, false);
                            if (lsr != null)
                                await localizationService.DeleteLocaleStringResourceAsync(lsr);
                        }
                    }
                }
            }
        }

        private DirectoryInfo GetFolderPath()
        {
            return new DirectoryInfo(_fileProvider.MapPath("~/Plugins/Misc.MenuManager/Resources/"));
        }
    }
}
