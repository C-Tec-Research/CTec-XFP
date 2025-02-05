using CTecControls;

namespace Xfp.ViewModels
{
    class AboutNotesViewModel
    {
        public string ComponentName  => Credits.Components[0].Name;
        public string ComponentNotes => Credits.Components[0].Notes;
    }
}
