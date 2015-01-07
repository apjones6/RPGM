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
        private readonly ICommand navigate;
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
            navigate = new RelayCommand<Uri>(OnNavigate);
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
                    Set<ITextDocument>(ref document, (ITextDocument)value, "Document");

                    // This does nothing if note is null
                    TryInitializeDocument();
                }
            }
        }

        public ICommand EditCommand
        {
            get { return edit; }
        }

        public ICommand NavigateCommand
        {
            get { return navigate; }
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
                    Set<bool>(ref editMode, value, "IsEditMode");
                    discard.RaiseCanExecuteChanged();

                    if (value)
                    {
                        // Remove links so we don't save them
                        document.SetText(TextSetOptions.FormatRtf, note.RtfContent);
                    }
                    else
                    {
                        ApplyHyperlinks();
                    }
                }
            }
        }

        public bool IsNew
        {
            get { return note != null && note.Id == Guid.Empty; }
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

        public void ApplyHyperlinks()
        {
            // TODO: Use an alias table
            var notes = Database.ListAsync().Result.Except(new[] { note }).ToArray();

            // Avoid performance implications of many small updates
            document.BatchDisplayUpdates();

            ITextRange range;
            foreach (var n in notes)
            {
                var link = string.Format("\"richtea.rpgm://notes/{0}\"", n.Id);
                var skip = 0;

                while ((range = document.GetRange(skip, TextConstants.MaxUnitCount)).FindText(n.Title, TextConstants.MaxUnitCount, FindOptions.Word) != 0)
                {
                    // NOTE: Set the document selection as workaround to prevent intermittent AccessViolationException,
                    //       probably caused by a timing issue in the lower level code
                    document.Selection.StartPosition = document.Selection.EndPosition = range.StartPosition;
                    range.Link = link;
                    skip = range.EndPosition;
                    document.Selection.StartPosition = document.Selection.EndPosition = range.EndPosition;
                }
            }

            document.ApplyDisplayUpdates();
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

            // This does nothing if document is null
            TryInitializeDocument();

            save.RaiseCanExecuteChanged();
            RaisePropertyChanged("Title");
        }

        private void OnBackMessage(BackMessage message)
        {
            if (IsEditMode)
            {
                string rtfContent;
                document.GetText(TextGetOptions.FormatRtf, out rtfContent);
                note.RtfContent = rtfContent;

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
            document.SetText(TextSetOptions.FormatRtf, note.RtfContent);
            RaisePropertyChanged("Title");
            IsEditMode = false;
        }

        private void OnNavigate(Uri uri)
        {
            var parts = uri.AbsoluteUri.ToLower().Replace("richtea.rpgm://", string.Empty).Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            var parameter = parts.Length > 1 ? parts[1] : null;

            if (parts[0] == "notes")
            {
                Navigation.NavigateTo("Note", Guid.Parse(parameter));
            }
            else
            {
                // TODO: Throw?
            }
        }

        private async void OnSave()
        {
            if (document != null)
            {
                string rtfContent;
                document.GetText(TextGetOptions.FormatRtf, out rtfContent);
                note.RtfContent = rtfContent;
            }

            await Database.SaveAsync(note);

            // Update the stored original for reverting changes
            // NOTE: This is messy
            original = new Note(note);

            if (!IsEditMode)
            {
                // TODO: Navigate to note contents (sometimes?)
                Navigation.GoBack();
            }
            
            IsEditMode = false;
        }

        private void TryInitializeDocument()
        {
            // When initialize complete
            if (document != null && note != null && !string.IsNullOrEmpty(note.RtfContent))
            {
                document.SetText(TextSetOptions.FormatRtf, note.RtfContent);
                ApplyHyperlinks();
            }
        }
    }
}
