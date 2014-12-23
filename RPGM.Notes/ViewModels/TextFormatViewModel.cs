using System;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using Windows.UI.Text;

namespace RPGM.Notes.ViewModels
{
    public class TextFormatViewModel : ViewModel
    {
        private readonly ICommand closed;
        private readonly ITextDocument document;
        private readonly ICommand opened;
        private readonly ICommand selectionChanged;

        private bool open;

        public TextFormatViewModel(ITextDocument document)
        {
            if (document == null) throw new ArgumentNullException("document");

            this.closed = new RelayCommand(() => IsOpen = false);
            this.document = document;
            this.opened = new RelayCommand(() => IsOpen = true);
            this.selectionChanged = new RelayCommand(OnSelectionChanged);
        }

        public ICommand ClosedCommand
        {
            get { return closed; }
        }

        public ITextDocument Document
        {
            get { return document; }
        }

        public bool IsOpen
        {
            get { return open; }
            set { Set<bool>(ref open, value, "IsOpen"); }
        }

        public ICommand OpenedCommand
        {
            get { return opened; }
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
