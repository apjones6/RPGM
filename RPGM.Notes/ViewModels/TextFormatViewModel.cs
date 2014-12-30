using Windows.UI.Text;

namespace RPGM.Notes.ViewModels
{
    public class TextFormatViewModel : ViewModel
    {
        private bool open;
        private ITextSelection selection;

        public bool IsBold
        {
            get { return selection.CharacterFormat.Bold == FormatEffect.On; }
            set { selection.CharacterFormat.Bold = value ? FormatEffect.On : FormatEffect.Off; }
        }

        public bool IsItalic
        {
            get { return selection.CharacterFormat.Italic == FormatEffect.On; }
            set { selection.CharacterFormat.Italic = value ? FormatEffect.On : FormatEffect.Off; }
        }

        public bool IsOpen
        {
            get { return open; }
            set
            {
                open = value;
                NotifyOfPropertyChange(() => IsOpen);
            }
        }

        public bool IsUnderline
        {
            get { return selection.CharacterFormat.Underline != UnderlineType.None; }
            set { selection.CharacterFormat.Underline = value ? UnderlineType.Single : UnderlineType.None; }
        }

        public object Selection
        {
            get { return selection; }
            set
            {
                selection = (ITextSelection)value;
                Refresh();
            }
        }
    }
}
