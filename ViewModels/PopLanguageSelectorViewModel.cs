using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CTecControls.UI;
using CTecControls.ViewModels;
using Xfp.Cultures;
using Xfp.DataTypes.PanelData;
using Xfp.UI.Interfaces;

namespace Xfp.ViewModels
{
    public class PopLanguageSelectorViewModel : ViewModelBase
    {
        public PopLanguageSelectorViewModel()
        {
            foreach (CultureInfo c in CTecUtil.Cultures.CultureResources.SupportedCultures)
            {
                var itm = new LanguageSelectorItem() { Culture = c, Tag = c, IsCurrentCulture = false };
                itm.Click += LanguageSelected;
                AvailableLanguages.Add(itm);
            }

            OnPropertyChanged(nameof(AvailableLanguages));

            setCurrentLanguageCheckmark();
        }


        private void setCurrentLanguageCheckmark()
        {
            //try to match on full culture name - e.g. "en-GB"
            foreach (var i in AvailableLanguages)
            {
                if (CultureInfo.CurrentCulture.Name == i.Culture.Name)
                {
                    i.IsCurrentCulture = true;
                    return;
                }
            }

            //no match on cultures' full names so check prefixes - e.g. "en"
            string currentLang = getLanguagePrefix(CultureInfo.CurrentCulture.Name);
            if (!string.IsNullOrEmpty(currentLang))
            {
                foreach (LanguageSelectorItem i in AvailableLanguages)
                {
                    if (getLanguagePrefix(((CultureInfo)i.Tag).Name) == currentLang)
                    {
                        i.IsCurrentCulture = true;
                        return;
                    }
                }
            }
        }


        private string getLanguagePrefix(string cultureName)
        {
            var split = CultureInfo.CurrentCulture.Name.Split(new char[] { '-' });
            return split.Length > 0 ? split[0].ToLower() : "";
        }


        private bool _staysOpen;
        private bool _staysOpen2;
        private bool _isOpen;
        public bool LanguageSelectorClassicStaysOpen  { get => _staysOpen;  set { _staysOpen = value; OnPropertyChanged(); } }
        public bool LanguageSelectorStaysOpen { get => _staysOpen2; set { _staysOpen2 = value; OnPropertyChanged(); } }

        public bool LanguageSelectorIsOpen
        {
            get => _isOpen;
            set
            {
                //popup doesn't want to work without toggling StaysOpen along with IsOpen
                LanguageSelectorStaysOpen = value;
                if (_isOpen = value)
                    setCurrentLanguageCheckmark();
                OnPropertyChanged();
            }
        }


        public void LanguageSelected(object sender, EventArgs e)
        {
            LanguageSelectorIsOpen = false;
            var culture = ((LanguageSelectorItem)sender).Culture;
            if (culture != CultureInfo.CurrentCulture)
                OnCultureChanged?.Invoke(culture);
            
        }


        //public delegate void CultureChangedNotifier(CultureInfo culture);
        public CultureChangedNotifier OnCultureChanged;
        

        private List<LanguageSelectorItem> _availableLanguages = new();
        public List<LanguageSelectorItem> AvailableLanguages { get => _availableLanguages; set { _availableLanguages = value; OnPropertyChanged(); } }
    }
}
