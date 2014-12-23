using System;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using RPGM.Notes.Messages;
using RPGM.Notes.Models;
using Windows.UI.Text;

namespace RPGM.Notes.ViewModels
{
    public class NoteViewModel : ViewModel
    {
        private readonly ICommand delete;
        private readonly ICommand edit;
        private readonly RelayCommand save;

        private bool editMode;
        private Note note;
        private Note original;
        private TextFormatViewModel textFormat;

        public NoteViewModel(INavigationService navigation, IDatabase database)
            : base(navigation, database)
        {
            Messenger.Register<BackMessage>(this, OnBackMessage);

            delete = new RelayCommand(OnDelete, () => !IsNew);
            edit = new RelayCommand(() => IsEditMode = true);
            save = new RelayCommand(OnSave, CanSave);

            if (IsInDesignMode)
            {
                note = new Note { RtfContent = @"{\rtf1\ansi This is RTF content...}", Title = "Story ideas" };
            }
        }

        public ICommand DeleteCommand
        {
            get { return delete; }
        }

        public ICommand EditCommand
        {
            get { return edit; }
        }

        public ICommand SaveCommand
        {
            get { return save; }
        }

        public bool IsEditMode
        {
            get { return editMode; }
            set
            {
                if (editMode != value)
                {
                    editMode = value;
                    RaisePropertyChanged("IsEditMode");
                }
            }
        }

        public bool IsNew
        {
            get { return note != null && note.Id == Guid.Empty; }
        }

        public string RtfContent
        {
            get { return note != null ? note.RtfContent : null; }
            set
            {
                if (note != null)
                {
                    note.RtfContent = value;
                    RaisePropertyChanged("RtfContent");
                }
            }
        }

        public object TextFormat
        {
            get { return textFormat; }
        }

        public string Title
        {
            get { return note != null ? note.Title : null; }
            set
            {
                if (note != null)
                {
                    note.Title = value;
                    RaisePropertyChanged("Title");
                    save.RaiseCanExecuteChanged();
                }
            }
        }

        private bool CanSave()
        {
            return note != null && !string.IsNullOrWhiteSpace(note.Title);
        }

        public override async Task InitializeAsync(object parameter)
        {
            if (parameter is Guid)
            {
                original = await Database.GetAsync((Guid)parameter);
            }
            else
            {
                original = new Note();
            }

            // Copy so we can revert changes without database hit
            note = new Note(original);

            save.RaiseCanExecuteChanged();
            RaisePropertyChanged("RtfContent");
            RaisePropertyChanged("Title");
        }

        public async Task InitializeAsync(object parameter, ITextDocument document)
        {
            await InitializeAsync(parameter);

            this.textFormat = new TextFormatViewModel(document);
            RaisePropertyChanged("TextFormat");
        }

        private void OnBackMessage(BackMessage message)
        {
            if (IsEditMode)
            {
                // Restore note to original as it may have changes
                // TODO: I really think we should save on back, and handle cancel/discard as the edge case
                note = new Note(original);
                RaisePropertyChanged("RtfContent");
                RaisePropertyChanged("Title");
                message.Handled = true;
                IsEditMode = false;
            }
        }

        private async void OnDelete()
        {
            await Database.DeleteAsync(note.Id);

            // TODO: Navigate forward to notes list, and possibly clean back stack
            Navigation.GoBack();
        }

        private async void OnSave()
        {
            await Database.SaveAsync(note);

            if (!IsEditMode)
            {
                // TODO: Navigate to note contents (sometimes?)
                Navigation.GoBack();
            }
            
            IsEditMode = false;
        }
    }
}
