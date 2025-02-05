using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xfp.DataTypes;
using Xfp.DataTypes.PanelData;

namespace Xfp.ViewModels.PanelTools.ValidationWindow
{
    public class ValidationCodeViewModel : INotifyPropertyChanged
    {
        public ValidationCodeViewModel(ValidationCodes code) => ValidationCode = code;

        private ValidationCodes _validationCode { get; set; }
        public ValidationCodes ValidationCode { get => _validationCode; set { _validationCode = value; OnPropertyChanged(nameof(ValidationCode)); } }

        public ErrorLevels ErrorLevel { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
