using CTecDevices.DataTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Xfp.ViewModels.PanelTools
{
    public class ModeSettingOption : INotifyPropertyChanged
    {
            public ModeSettingOption(ModeSetting mode) { _mode = mode; }

            public int Index { get => _mode.Index; set { _mode.Index = value; OnPropertyChanged(); } }
            public int Number => _mode.Number;
            public string Text { get => _mode.Text; set { _mode.Text = value; OnPropertyChanged(); } }
            public bool IsEnabled { get => _mode.IsEnabled; set { _mode.IsEnabled = value; OnPropertyChanged(); } }

            public void RefreshView()
            {
                OnPropertyChanged(nameof(Index));
                OnPropertyChanged(nameof(Number));
                OnPropertyChanged(nameof(Text));
                OnPropertyChanged(nameof(IsEnabled));
            }


            private ModeSetting _mode;
            public event PropertyChangedEventHandler PropertyChanged;

            private string _changedPropertyName = null;
            private int    _changedPropertyCount = 0;
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                try
                {
                    if (propertyName != "CurrentTime")
                    {
                        if (propertyName == _changedPropertyName)
                        {
                            //prevent stack overflow when changinging culture triggers multiple calls to OnPropertyChanged (arbitrary 5 count)
                            if (++_changedPropertyCount > 5)
                                return;
                        }
                        else
                        {
                            _changedPropertyName  = propertyName;
                            _changedPropertyCount = 0;
                        }
                    }

                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                }
                catch { }
            }
    }
}
