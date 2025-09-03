using System.Globalization;
using System.Windows.Data;

namespace Xfp.Cultures
{
    /// <summary>
    /// Wraps up XAML access to instance of WPFLocalize.Properties.Resources, list of available cultures, and method to change culture
    /// </summary>
    public class CultureResources
    {
        /// <summary>
        /// The Resources ObjectDataProvider uses this method to get an instance of the WPFLocalize.Properties.Resources class
        /// </summary>
        /// <returns></returns>
        public Cultures.Resources GetResourceInstance()
        {
            return new Cultures.Resources();
        }

        private static ObjectDataProvider _provider;
        public static ObjectDataProvider ResourceProvider
        {
            get
            {
                if (_provider == null)
                    _provider = (ObjectDataProvider)App.Current.FindResource("Resources");
                return _provider;
            }
        }

        /// <summary>
        /// Change the current culture used in the application.
        /// If the desired culture is available all localized elements are updated.
        /// </summary>
        /// <param name="culture">Culture to change to</param>
        public static void ChangeCulture(CultureInfo culture)
        {
            //remain on the current culture if the desired culture cannot be found
            // - otherwise it would revert to the default resources set, which may or may not be what is wanted.
            if (CTecUtil.Cultures.CultureResources.SupportedCultures.Contains(culture))
            {
                Cultures.Resources.Culture = culture;
                CTecUtil.Cultures.CultureResources.ChangeCulture(culture);
                CTecControls.Cultures.CultureResources.ChangeCulture(culture);
                CTecDevices.Cultures.CultureResources.ChangeCulture(culture);
            }
            else
            {
                Cultures.Resources.Culture = CultureInfo.CurrentCulture;
                CTecUtil.Cultures.CultureResources.ChangeCulture(CultureInfo.CurrentCulture);
                CTecControls.Cultures.CultureResources.ChangeCulture(CultureInfo.CurrentCulture);
                CTecDevices.Cultures.CultureResources.ChangeCulture(CultureInfo.CurrentCulture);
            }
            ResourceProvider.Refresh();
        }
    }
}
