using System.Collections.Generic;
using CTecUtil.ViewModels;

namespace Xfp.ViewModels.PanelTools
{
    class AreaSelectorViewModel : ViewModelBase
    {
        public AreaSelectorViewModel() => CurrentlySelectedArea = 'A';


        private char _currentlySelectedArea;
        
        public char CurrentlySelectedArea { get => _currentlySelectedArea; set { _currentlySelectedArea = value; OnPropertyChanged(); } }


        private bool[] _areaErrors = new bool[26];

        public List<bool> AreaErrors
        {
            get => [.. _areaErrors];
            set
            {
                _areaErrors = new bool[26];

                if (value is not null)
                    for (int i = 0; i < value.Count && i < _areaErrors.Length; i++)
                        _areaErrors[i] = value[i];

                OnPropertyChanged();
                OnPropertyChanged(nameof(AreaAHasErrors));
                OnPropertyChanged(nameof(AreaBHasErrors));
                OnPropertyChanged(nameof(AreaCHasErrors));
                OnPropertyChanged(nameof(AreaDHasErrors));
                OnPropertyChanged(nameof(AreaEHasErrors));
                OnPropertyChanged(nameof(AreaFHasErrors));
                OnPropertyChanged(nameof(AreaGHasErrors));
                OnPropertyChanged(nameof(AreaHHasErrors));
                OnPropertyChanged(nameof(AreaIHasErrors));
                OnPropertyChanged(nameof(AreaJHasErrors));
                OnPropertyChanged(nameof(AreaKHasErrors));
                OnPropertyChanged(nameof(AreaLHasErrors));
                OnPropertyChanged(nameof(AreaMHasErrors));
                OnPropertyChanged(nameof(AreaNHasErrors));
                OnPropertyChanged(nameof(AreaOHasErrors));
                OnPropertyChanged(nameof(AreaPHasErrors));
                OnPropertyChanged(nameof(AreaQHasErrors));
                OnPropertyChanged(nameof(AreaRHasErrors));
                OnPropertyChanged(nameof(AreaSHasErrors));
                OnPropertyChanged(nameof(AreaTHasErrors));
                OnPropertyChanged(nameof(AreaUHasErrors));
                OnPropertyChanged(nameof(AreaVHasErrors));
                OnPropertyChanged(nameof(AreaWHasErrors));
                OnPropertyChanged(nameof(AreaXHasErrors));
                OnPropertyChanged(nameof(AreaYHasErrors));
                OnPropertyChanged(nameof(AreaZHasErrors));
            }
        }

        public bool AreaAHasErrors { get => _areaErrors[0]; }
        public bool AreaBHasErrors { get => _areaErrors[1]; }
        public bool AreaCHasErrors { get => _areaErrors[2]; }
        public bool AreaDHasErrors { get => _areaErrors[3]; }
        public bool AreaEHasErrors { get => _areaErrors[4]; }
        public bool AreaFHasErrors { get => _areaErrors[5]; }
        public bool AreaGHasErrors { get => _areaErrors[6]; }
        public bool AreaHHasErrors { get => _areaErrors[7]; }
        public bool AreaIHasErrors { get => _areaErrors[8]; }
        public bool AreaJHasErrors { get => _areaErrors[9]; }
        public bool AreaKHasErrors { get => _areaErrors[10]; }
        public bool AreaLHasErrors { get => _areaErrors[11]; }
        public bool AreaMHasErrors { get => _areaErrors[12]; }
        public bool AreaNHasErrors { get => _areaErrors[13]; }
        public bool AreaOHasErrors { get => _areaErrors[14]; }
        public bool AreaPHasErrors { get => _areaErrors[15]; }
        public bool AreaQHasErrors { get => _areaErrors[16]; }
        public bool AreaRHasErrors { get => _areaErrors[17]; }
        public bool AreaSHasErrors { get => _areaErrors[18]; }
        public bool AreaTHasErrors { get => _areaErrors[19]; }
        public bool AreaUHasErrors { get => _areaErrors[20]; }
        public bool AreaVHasErrors { get => _areaErrors[21]; }
        public bool AreaWHasErrors { get => _areaErrors[22]; }
        public bool AreaXHasErrors { get => _areaErrors[23]; }
        public bool AreaYHasErrors { get => _areaErrors[24]; }
        public bool AreaZHasErrors { get => _areaErrors[25]; }
    }
}
