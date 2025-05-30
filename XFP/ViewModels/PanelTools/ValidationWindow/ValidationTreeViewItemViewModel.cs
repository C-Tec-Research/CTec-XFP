using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CTecUtil;
using Xfp.DataTypes;
using Xfp.DataTypes.PanelData;

namespace Xfp.ViewModels.PanelTools.ValidationWindow
{
    /// <summary>
    /// Base class for all ViewModel classes displayed by TreeViewItems.<br/>
    /// This acts as an adapter between a raw data object and a TreeViewItem.
    /// </summary>
    public abstract class ValidationTreeViewItemViewModel : INotifyPropertyChanged
    {
        public ValidationTreeViewItemViewModel() { }
        public ValidationTreeViewItemViewModel(string name) { Name = name; }

        protected ValidationTreeViewItemViewModel(ValidationTreeViewItemViewModel parent) => _parent = parent;


        readonly AsyncObservableCollection<ValidationTreeViewItemViewModel> _children = new();
        readonly ValidationTreeViewItemViewModel _parent;

        bool _isExpanded;
        bool _isSelected;


        public AsyncObservableCollection<ValidationTreeViewItemViewModel> Children => _children;

        public bool Contains(string name) => ChildrenContains(this, name);
        public ValidationTreeViewItemViewModel Find(string name) => FindInChildren(this, name);
        public ValidationTreeViewItemViewModel Add(ValidationTreeViewItemViewModel item) => AddToChildren(this, item);
        public void Remove(string name) => RemoveFromChildren(this, name);


        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (value != _isExpanded)
                {
                    _isExpanded = value;
                    OnPropertyChanged(nameof(IsExpanded));
                }

                // Expand all the way up to the root.
                if (_isExpanded && _parent != null)
                    _parent.IsExpanded = true;
            }
        }


        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value != _isSelected)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }


        private string _name = "";
        private ErrorLevels _errorLevel = ErrorLevels.OK;

        public string Name { get => _name; set { _name = value; OnPropertyChanged(nameof(Name)); } }
        public virtual ErrorLevels ErrorLevel { get => _errorLevel; set { _errorLevel = value; OnPropertyChanged(nameof(ErrorLevel)); } }

        public List<ValidationCodeViewModel> ValidationCodes { get; set; }


        public virtual int TotalErrors
        {
            get
            {
                int result = 0;
                foreach (var c in Children)
                    result += c?.TotalErrors??0;
                return result;
            }
        }


        public ErrorLevels GetHighestErrorLevel() => GetHighestErrorLevel(ValidationCodes);

        public ErrorLevels GetHighestErrorLevel(List<ValidationCodeViewModel> validationCodes)
        {
            ErrorLevels e;
            var result = ErrorLevels.OK;

            try
            {
                if (validationCodes is not null)
                    foreach (var v in validationCodes)
                        if ((e = ConfigData.GetErrorLevel(v.ValidationCode)) > result)
                            result = e;
            } catch { }

            return result;
        }

        public ErrorLevels GetHighestErrorLevel(List<ValidationCodes> validationCodes)
        {
            ErrorLevels e;
            var result = ErrorLevels.OK;
            if (validationCodes is not null)
                foreach (var v in validationCodes)
                    if ((e = ConfigData.GetErrorLevel(v)) > result)
                        result = e;
            return result;
        }

        public ErrorLevels GetHighestErrorLevel(List<ConfigErrorPageItems> errorItems)
        {
            ErrorLevels e;
            ErrorLevels result = ErrorLevels.OK;
            if (errorItems is not null)
                foreach (var i in errorItems)
                    if ((e = GetHighestErrorLevel(i.ValidationCodes)) > result)
                        result = e;
            return result;
        }


        public ValidationTreeViewItemViewModel Parent
        {
            get { return _parent; }
        }


        public void SetChildren(List<ConfigErrorPageItems> items, ValidationTreeViewItemViewModel parentPage, bool expandThisBranch)
        {
            /*** 1. remove any errors that have been corrected ***/
            List<string> errorsToBeRemoved = new();
            foreach (var c in parentPage.Children)
            {
                if (c is null)
                    continue;

                try
                {
                    bool isStillAnError = false;

                    if (items is not null)
                    {
                        foreach (var i in items)
                        {
                            if (i.Name == c.Name)
                            {
                                isStillAnError = true;
                                break;
                            }
                        }
                    }

                    if (!isStillAnError)
                        if (!errorsToBeRemoved.Contains(c.Name))
                            errorsToBeRemoved.Add(c.Name);
                }
                catch { }
            }

            foreach (var f in errorsToBeRemoved)
                parentPage.Remove(f);


            /*** 2. update new errors ***/

            if (items is not null)
                foreach (var s in items)
                    parentPage.Add(new ValidationPageItemViewModel(s, parentPage));


            /*** 3. set the error level on this branch and its children according to the highest condition (OK<Warning<Error) on the level below ***/

            ErrorLevels elv;
            ErrorLevel = ErrorLevels.OK;

            foreach (var i in Children)
            {
                if (i is null)
                    continue;

                elv = i.GetHighestErrorLevel();
                if (elv > ErrorLevel)
                    ErrorLevel = elv;

                ErrorLevels elv2 = ErrorLevels.OK;
                if (i.ValidationCodes is not null)
                {
                    foreach (var j in i.ValidationCodes)
                    {
                        var el3 = j.ErrorLevel = ConfigData.GetErrorLevel(j.ValidationCode);
                        if (el3 > elv2)
                            elv2 = el3;
                    }
                }

                if (elv2 > ErrorLevel)
                    ErrorLevel = elv2;
            }


            /*** 4. ensure desired branch is expanded fully ***/

            if (expandThisBranch)
            {
                IsExpanded = true;
                foreach (var c in Children)
                    if (c is not null)
                        if (c.ErrorLevel == ErrorLevels.Error)
                            c.IsExpanded = true;
            }

            OnPropertyChanged(nameof(TotalErrors));
        }


        public static bool ChildrenContains(ValidationTreeViewItemViewModel branch, string name)
        {
            if (branch is not null && branch.Children is not null)
                foreach (var _ in from p in branch.Children where p.Name == name select new { })
                    return true;
            return false;
        }

        public static ValidationTreeViewItemViewModel FindInChildren(ObservableCollection<ValidationTreeViewItemViewModel> branch, string name)
        {
            if (branch is not null)
                foreach (var c in from c in branch where c.Name == name select c)
                    return c;
            return null;
        }

        public static ValidationPageViewModel FindInChildren(ObservableCollection<ValidationPageViewModel> branch, string name)
        {
            if (branch is not null)
                foreach (var c in from c in branch where c.Name == name select c)
                    return c;
            return null;
        }

        public static ValidationTreeViewItemViewModel FindInChildren(ValidationTreeViewItemViewModel branch, string name)
        {
            try
            {
                if (branch is not null && branch.Children is not null)
                    foreach (var c in from c in branch.Children select c)
                        if (c is not null)
                            return c;
            }
            catch { }

            return null;
        }

        public static ValidationTreeViewItemViewModel AddToChildren(ValidationTreeViewItemViewModel branch, ValidationTreeViewItemViewModel item)
        {
            if (branch is not null && branch.Children is not null)
            {
                var existingItem = FindInChildren(branch, item.Name) as ValidationPageItemViewModel;
                if (existingItem is not null)
                {
                    existingItem.ValidationCodes.Clear();
                    foreach (var v in ((ValidationPageItemViewModel)item).ValidationCodes)
                        existingItem.AddValidationCode(v);
                    existingItem.OnPropertyChanged(nameof(ErrorLevel));
                    existingItem.OnPropertyChanged(nameof(ErrorLevel));
                    existingItem.OnPropertyChanged(nameof(ValidationCodes));
                    return existingItem;
                }
                else
                    branch.Children.Add(item);

                branch.OnPropertyChanged(nameof(ErrorLevel));
                branch.OnPropertyChanged(nameof(Children));
            }
            return item;
        }

        public static void RemoveFromChildren(ValidationTreeViewItemViewModel branch, string name)
        {
            if (branch is not null && branch.Children is not null)
            {
                for (int i = 0; i < branch.Children.Count; i++)
                {
                    if (branch.Children[i].Name == name)
                    {
                        branch.Children.RemoveAt(i);
                        return;
                    }
                }

                branch.OnPropertyChanged(nameof(ErrorLevel));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}