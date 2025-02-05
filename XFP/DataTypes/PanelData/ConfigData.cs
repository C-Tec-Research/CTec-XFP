using Xfp.UI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Xfp.DataTypes.PanelData
{
    public class ConfigData
    {
        /// <summary>
        /// Returns an initialised PanelConfigData object.
        /// </summary>
        internal static ConfigData InitialisedNew() => throw new NotImplementedException("ConfigData.InitialisedNew()");


        public virtual bool Equals(ConfigData otherData) => throw new NotImplementedException("ConfigData.Equals()");


        public virtual bool Validate() => throw new NotImplementedException("ConfigData.Validate()");


        protected ConfigErrorPage _pageErrorOrWarningDetails = null;
        public ConfigErrorPage GetPageErrorDetails() => _pageErrorOrWarningDetails;

        protected ConfigErrorPageItems _errorItems = new(0, "");
        internal ConfigErrorPageItems GetErrorItems() => _errorItems;


        /// <summary>Clear down the page error details and validation codes</summary>
        public void ClearErrors() => _pageErrorOrWarningDetails.Items.Clear();


        internal bool HasErrorsOrWarnings(List<ValidationCodes> codes) => codes?.Count > 0;

        internal virtual bool HasErrors(List<ValidationCodes> codes)
        {
            if (codes is not null)
                foreach (var c in codes)
                    if (GetErrorLevel(c) == ErrorLevels.Error)
                        return true;
            return false;
        }

        internal virtual bool HasWarnings(List<ValidationCodes> codes)
        {
            if (codes is not null)
                foreach (var c in codes)
                    if (GetErrorLevel(c) == ErrorLevels.Warning)
                        return true;
            return false;
        }

        public bool HasErrorsOrWarnings()
        {
            foreach (var i in _pageErrorOrWarningDetails.Items)
                if (HasErrorsOrWarnings(i.ValidationCodes))
                    return true;
            return false;
        }

        public bool HasErrors()
        {
            foreach (var i in _pageErrorOrWarningDetails.Items)
                if (HasErrors(i.ValidationCodes))
                    return true;
            return false;
        }

        public bool HasWarnings()
        {
            foreach (var i in _pageErrorOrWarningDetails.Items)
                if (HasWarnings(i.ValidationCodes))
                    return true;
            return false;
        }


        internal static ErrorLevels GetErrorLevel(ValidationCodes code)
            => code switch
            {
                ValidationCodes.Ok => ErrorLevels.OK,

                ValidationCodes.DeviceNamesDataBlankEntry or
                ValidationCodes.SiteConfigNoSystemNameWarning or
                ValidationCodes.SiteConfigNoClientName or
                ValidationCodes.SiteConfigNoClientAddress or
                ValidationCodes.SiteConfigNoClientTel or
                ValidationCodes.SiteConfigNoPanelLocation or
                ValidationCodes.SiteConfigNoInstallerName or 
                ValidationCodes.SiteConfigNoInstallerAddress or 
                ValidationCodes.SiteConfigNoInstallerTel or 
                ValidationCodes.SiteConfigNoEngineerName or 
                ValidationCodes.SiteConfigNoInstallDate or 
                ValidationCodes.SiteConfigNoCommissionDate => ErrorLevels.Warning,

                _ => ErrorLevels.Error                               
            };
    }
}
