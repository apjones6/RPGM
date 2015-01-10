using System;
using Windows.UI.Text;

namespace RPGM.Notes.ViewModels
{
    public class TextFormatViewModel : ViewModel
    {
        private readonly ITextDocument document;
        private bool open;

        public TextFormatViewModel(ITextDocument document)
        {
            if (document == null) throw new ArgumentNullException("document");
            this.document = document;
        }

        private ITextCharacterFormat CharacterFormat
        {
            get { return document.Selection.CharacterFormat; }
        }

        public bool IsBold
        {
            get { return CharacterFormat.Bold == FormatEffect.On; }
            set { CharacterFormat.Bold = value ? FormatEffect.On : FormatEffect.Off; }
        }

        public bool IsItalic
        {
            get { return CharacterFormat.Italic == FormatEffect.On; }
            set { CharacterFormat.Italic = value ? FormatEffect.On : FormatEffect.Off; }
        }

        public bool IsNotOpen
        {
            get { return !open; }
        }

        public bool IsOpen
        {
            get { return open; }
            set
            {
                open = value;
                Refresh();
            }
        }

        public bool IsUnderline
        {
            get { return CharacterFormat.Underline != UnderlineType.None; }
            set { CharacterFormat.Underline = value ? UnderlineType.Single : UnderlineType.None; }
        }
    }
}
