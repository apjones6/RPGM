using System;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using Windows.UI.Text;

namespace RPGM.Notes.ViewModels
{
    public class TextFormatViewModel : ViewModel
    {
        private readonly ITextDocument document;
        private readonly ICommand selectionChanged;

        public TextFormatViewModel(ITextDocument document)
        {
            if (document == null) throw new ArgumentNullException("document");

            this.document = document;
            this.selectionChanged = new RelayCommand(OnSelectionChanged);
        }

        public ITextDocument Document
        {
            get { return document; }
        }

        public ICommand SelectionChangedCommand
        {
            get { return selectionChanged; }
        }

        public bool IsBold
        {
            get { return document.Selection.CharacterFormat.Bold == FormatEffect.On; }
            set { document.Selection.CharacterFormat.Bold = value ? FormatEffect.On : FormatEffect.Off; }
        }

        public bool IsItalic
        {
            get { return document.Selection.CharacterFormat.Italic == FormatEffect.On; }
            set { document.Selection.CharacterFormat.Italic = value ? FormatEffect.On : FormatEffect.Off; }
        }

        public bool IsUnderline
        {
            get { return document.Selection.CharacterFormat.Underline != UnderlineType.None; }
            set { document.Selection.CharacterFormat.Underline = value ? UnderlineType.Single : UnderlineType.None; }
        }

        private void OnSelectionChanged()
        {
            RaisePropertyChanged("IsBold");
            RaisePropertyChanged("IsItalic");
            RaisePropertyChanged("IsUnderline");
        }
    }
}
