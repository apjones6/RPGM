using System;
using System.Linq;
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
        private readonly RelayCommand discard;
        private readonly ICommand edit;
        private readonly RelayCommand save;
        private readonly TextFormatViewModel textFormat;

        private ITextDocument document;
        private bool editMode;
        private Note note;
        private Note original;

        public NoteViewModel(INavigationService navigation, IDatabase database)
            : base(navigation, database)
        {
            Messenger.Register<BackMessage>(this, OnBackMessage);

            delete = new RelayCommand(OnDelete, () => !IsNew);
            discard = new RelayCommand(OnDiscard, () => IsEditMode);
            edit = new RelayCommand(() => IsEditMode = true);
            save = new RelayCommand(OnSave, CanSave);
            textFormat = new TextFormatViewModel();

            if (IsInDesignMode)
            {
                note = new Note { RtfContent = @"{\rtf1\ansi This is RTF content...}", Title = "Story ideas" };
            }
        }

        public ICommand DeleteCommand
        {
            get { return delete; }
        }

        public ICommand DiscardCommand
        {
            get { return discard; }
        }

        public object Document
        {
            set
            {
                if (document == null)
                {
                    InitializeHyperlinks(document = (ITextDocument)value);
                    RaisePropertyChanged("Document");
                }
            }
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
                    discard.RaiseCanExecuteChanged();
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

            // Clear document to reinitialize
            document = null;

            save.RaiseCanExecuteChanged();
            RaisePropertyChanged("RtfContent");
            RaisePropertyChanged("Title");
        }

        private void InitializeHyperlinks(ITextDocument document)
        {
            // TODO: Use an alias table
            var notes = Task.Run(() => Database.ListAsync()).Result.Except(new[] { note }).ToArray();

            ITextRange range;
            var len = 0;

            foreach (var n in notes)
            {
                var link = string.Format("\"richtea.rpgm://notes/{0}\"", n.Id);
                var start = 0;

                do
                {
                    // TODO: Investigate why this always seems to find more instances of the search
                    //       text than exists in the document
                    len = (range = document.GetRange(start, TextConstants.MaxUnitCount)).FindText(n.Title, TextConstants.MaxUnitCount, FindOptions.None);
                    if (len > 0)
                    {
                        range.Link = link;
                        start = range.StartPosition + len;
                    }
                }
                while (len > 0);
            }
        }

        private void OnBackMessage(BackMessage message)
        {
            if (IsEditMode)
            {
                // TODO: Investigate if this method can be async safely
                Task.Run(() => Database.SaveAsync(note)).Wait();
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

        private void OnDiscard()
        {
            // Restore note to original as it likely has changes
            note = new Note(original);
            RaisePropertyChanged("RtfContent");
            RaisePropertyChanged("Title");
            IsEditMode = false;
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
