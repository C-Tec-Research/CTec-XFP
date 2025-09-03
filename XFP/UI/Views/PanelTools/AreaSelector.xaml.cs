using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Xfp.ViewModels.PanelTools;

namespace Xfp.UI.Views.PanelTools
{
    /// <summary>
    /// Interaction logic for AreaSelector.xaml
    /// </summary>
    public partial class AreaSelector : UserControl
    {
        public AreaSelector()
        {
            InitializeComponent();
            DataContext = _context = new AreaSelectorViewModel();
        }


        private AreaSelectorViewModel _context;


        public delegate void SelectionChangedHandler(string area);

        /// <summary>Event raised when the selected area has changed</summary>
        public event SelectionChangedHandler SelectionChanged;


        public char CurrentlySelectedArea { get => _context.CurrentlySelectedArea; set => _context.CurrentlySelectedArea = value; } 


        public void SetValidation(List<bool> values) => _context.AreaErrors = values;
        

        private void button_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key is Key.Left or Key.Up)
            {
                move(e, e.Key);
                e.Handled = true;
            }
            else if (e.Key is Key.Right or Key.Down)
            {
                move(e, e.Key);
                e.Handled = true;
            }
        }

        private void radioButton_GotFocus(object sender, RoutedEventArgs e) => ((RadioButton)sender).IsChecked = true;
        private void areaButtonClick(object sender, RoutedEventArgs e)      => SelectionChanged?.Invoke((string)((RadioButton)sender).Content);
        private void leftButton_Click(object sender, RoutedEventArgs e)     => SelectionChanged?.Invoke((string)moveLeft(e).Content);
        private void rightButton_Click(object sender, RoutedEventArgs e)    => SelectionChanged?.Invoke((string)moveRight(e).Content);


        private void move(KeyEventArgs e, Key k) => SelectionChanged?.Invoke((string)(e.Key is Key.Left or Key.Up ? moveLeft(e) : moveRight(e)).Content);

        
        private static List<RadioButton> _areaButtons;
        private static List<RadioButton> Buttons {  get => _areaButtons ?? initAreaButtonsList(_source); }
        private static DependencyObject _source;


        /// <summary>
        /// Move leftwards along the area buttons in response to the Left Button click
        /// </summary>
        /// <param name="e">The RoutedEventArgs parameter received by the Left Button click that initiated the move</param>
        /// <returns>Returns the newly-clicked button</returns>
        private static RadioButton moveLeft(RoutedEventArgs e) => move(e, -1);


        /// <summary>
        /// Move rightwards along the area buttons in response to the Right Button click
        /// </summary>
        /// <param name="e">The RoutedEventArgs parameter received by the Right Button click that initiated the move</param>
        /// <returns>Returns the newly-clicked button</returns>
        private static RadioButton moveRight(RoutedEventArgs e) => move(e, +1);


        /// <summary>
        /// Move rightwards along the area buttons in response to the Right Button click
        /// </summary>
        /// <param name="e">The RoutedEventArgs parameter received by the Right Button click that initiated the move</param>
        /// <returns>Returns the newly-clicked button</returns>
        private static RadioButton move(RoutedEventArgs e, int direction)
        { 
            _source = (DependencyObject)e.OriginalSource;

            for (var i = direction > 0 ? 0 : Buttons.Count-1;  i < Buttons.Count && i >= 0; i += direction)
            {
                if (Buttons[i].IsChecked == true)
                {
                    var idx = i + direction;
                    if (idx < 0) idx = Buttons.Count-1;
                    if (idx >= Buttons.Count) idx = 0;
                    var b = Buttons[idx];
                    b.Focus();
                    return b;
                }
            }
            return null;
        }


        /// <summary>
        /// Initialise the RadioButton list starting from the control that was clicked
        /// </summary>
        private static List<RadioButton> initAreaButtonsList(DependencyObject dep)
        {
            //traverse up the visual tree to find cell that was clicked (if any)
            while ((dep != null) && !(dep is Grid))
                dep = VisualTreeHelper.GetParent(dep);

            if (!(dep is Grid panel))
                return null;

            _areaButtons = new();

            foreach (var c in panel.Children)
                if (c is RadioButton)
                    _areaButtons.Add(c as RadioButton);

            return _areaButtons;
        }
    }
}
