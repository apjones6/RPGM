﻿using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using Windows.UI.Text;

namespace RPGM.Notes.ViewModels
{
    public class TextFormatViewModel : ViewModel
    {
        private readonly ICommand closed;
        private readonly ICommand opened;
        private readonly ICommand selectionChanged;

        private bool open;
        private ITextSelection selection;

        public TextFormatViewModel()
        {
            this.closed = new RelayCommand(() => IsOpen = false);
            this.opened = new RelayCommand(() => IsOpen = true);
            this.selectionChanged = new RelayCommand<ITextSelection>(OnSelectionChanged);
        }

        public ICommand ClosedCommand
        {
            get { return closed; }
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
            get { return selection.CharacterFormat.Bold == FormatEffect.On; }
            set { selection.CharacterFormat.Bold = value ? FormatEffect.On : FormatEffect.Off; }
        }

        public bool IsItalic
        {
            get { return selection.CharacterFormat.Italic == FormatEffect.On; }
            set { selection.CharacterFormat.Italic = value ? FormatEffect.On : FormatEffect.Off; }
        }

        public bool IsUnderline
        {
            get { return selection.CharacterFormat.Underline != UnderlineType.None; }
            set { selection.CharacterFormat.Underline = value ? UnderlineType.Single : UnderlineType.None; }
        }

        private void OnSelectionChanged(ITextSelection selection)
        {
            this.selection = selection;
            RaisePropertyChanged("IsBold");
            RaisePropertyChanged("IsItalic");
            RaisePropertyChanged("IsUnderline");
        }
    }
}
