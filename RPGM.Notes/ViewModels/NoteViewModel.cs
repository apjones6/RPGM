using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Mvvm.Interfaces;
using Microsoft.Practices.Prism.PubSubEvents;
using RPGM.Notes.Models;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace RPGM.Notes.ViewModels
{
    [Export]
    public class NoteViewModel : ViewModel, IBackNavigationAware, IDocumentAware
    {
        private static readonly Regex RTF_TRAILING_NEWLINES = new Regex(@"(\r|\n|\\par|\\pard|\\ltrpar|\\tx\d+|\\fs\d+)*}(\r|\n|\0)*$", RegexOptions.IgnoreCase);

        private readonly IDatabase database;
        private readonly DelegateCommandBase delete;
        private readonly DelegateCommandBase discard;
        private readonly ICommand edit;
        private readonly IEventAggregator eventAggregator;
        private readonly INavigationService navigation;
        private readonly DelegateCommandBase save;

        private ITextDocument document;
        private Guid id;
        private bool isEditMode;
        private bool isFormatEnabled;
        private bool isPage;
        private Note note;
        private TextFormatViewModel textFormat;
        private string title;

        [ImportingConstructor]
        public NoteViewModel(IDatabase database, IEventAggregator eventAggregator, INavigationService navigation)
        {
            if (database == null) throw new ArgumentNullException("database");
            if (eventAggregator == null) throw new ArgumentNullException("eventAggregator");
            if (navigation == null) throw new ArgumentNullException("navigation");

            this.database = database;
            this.delete = DelegateCommand.FromAsyncHandler(Delete, () => !IsNew);
            this.discard = new DelegateCommand(Discard, () => IsEditMode && !IsNew);
            this.edit = new DelegateCommand(Edit);
            this.eventAggregator = eventAggregator;
            this.navigation = navigation;
            this.save = DelegateCommand.FromAsyncHandler(Save, () => !string.IsNullOrWhiteSpace(Title));
        }

        public NoteViewModel(Note note, IDatabase database, IEventAggregator eventAggregator, INavigationService navigation)
            : this(database, eventAggregator, navigation)
        {
            if (note == null) throw new ArgumentNullException("note");

            this.id = note.Id;
            this.note = note;
        }

        public DateTimeOffset DateModified
        {
            get { return note != null ? note.DateModified : default(DateTimeOffset); }
        }

        public ICommand DeleteCommand
        {
            get { return delete; }
        }

        public ICommand DiscardCommand
        {
            get { return discard; }
        }

        public ICommand EditCommand
        {
            get { return edit; }
        }

        [RestorableState]
        public Guid Id
        {
            get { return id; }
            set
            {
                SetProperty(ref id, value);
                delete.RaiseCanExecuteChanged();
                OnPropertyChanged(() => IsNew);
            }
        }

        public bool IsEditMode
        {
            get { return isEditMode; }
            set
            {
                SetProperty(ref isEditMode, value);
                discard.RaiseCanExecuteChanged();
                save.RaiseCanExecuteChanged();
            }
        }

        public bool IsFormatEnabled
        {
            get { return isFormatEnabled; }
            set { SetProperty(ref isFormatEnabled, value); }
        }

        public bool IsNew
        {
            get { return note != null && note.Id == Guid.Empty; }
        }

        public ICommand SaveCommand
        {
            get { return save; }
        }

        public TextFormatViewModel TextFormat
        {
            get { return textFormat; }
            set { SetProperty(ref textFormat, value); }
        }

        public string Title
        {
            get { return title ?? (note != null ? note.Title : null); }
            set
            {
                SetProperty(ref title, value);
                save.RaiseCanExecuteChanged();
            }
        }

        private async Task Delete()
        {
            await database.DeleteAsync(note.Id);

            // Stop TryGoBack from preventing navigation
            IsEditMode = false;

            // Navigate away if we're on the note's page
            if (isPage)
            {
                navigation.GoBack();
            }
        }

        private void Discard()
        {
            IsEditMode = false;
            Title = null;

            OnTextChanged();
        }

        private void Edit()
        {
            if (!string.IsNullOrEmpty(note.RtfContent))
            {
                document.SetText(TextSetOptions.FormatRtf, note.RtfContent);
            }

            IsEditMode = true;
        }

        public override void OnNavigatedFrom(Dictionary<string, object> viewModelState, bool suspending)
        {
            base.OnNavigatedFrom(viewModelState, suspending);

            eventAggregator.GetEvent<SetTextEvent>().Unsubscribe(OnSetText);
            eventAggregator.GetEvent<SaveEvent>().Unsubscribe(OnSave);
        }

        public async override void OnNavigatedTo(object navigationParameter, NavigationMode navigationMode, Dictionary<string, object> viewModelState)
        {
            base.OnNavigatedTo(navigationParameter, navigationMode, viewModelState);

            // Keep Id property correct
            if (Id == Guid.Empty && navigationParameter != null)
            {
                Guid id;
                if (Guid.TryParse(navigationParameter.ToString(), out id))
                {
                    Id = id;
                }
            }

            // Load the note, or prepare a new note
            if (Id != Guid.Empty)
            {
                note = await database.GetAsync(Id);
            }
            else
            {
                note = new Note();
                IsEditMode = true;
            }

            // Must be on the dedicated note page
            isPage = true;

            eventAggregator.GetEvent<SetTextEvent>().Subscribe(OnSetText, ThreadOption.UIThread);
            eventAggregator.GetEvent<SaveEvent>().Subscribe(OnSave, ThreadOption.UIThread);

            OnPropertyChanged(() => Title);
        }

        private async void OnSave(bool ignored)
        {
            await Save();
        }

        private async void OnSetText(bool setText)
        {
            if (document == null || note == null || string.IsNullOrEmpty(note.RtfContent))
            {
                return;
            }

            if (setText)
            {
                document.SetText(TextSetOptions.FormatRtf, note.RtfContent);
            }

            if (!isEditMode)
            {
                // TODO: Use an alias table
                // TODO: Add disambiguation page instead of picking first note
                // NOTE: Don't link to other notes with the same name
                var notes = (await database.ListAsync())
                    .GroupBy(x => x.Title)
                    .Where(x => !x.Any(y => y.Title.Equals(note.Title, StringComparison.CurrentCultureIgnoreCase)))
                    .Select(x => x.First())
                    .ToArray();
                var links = notes.ToDictionary(x => x.Title, x => new Uri(string.Format("richtea.rpgm://notes/{0}", x.Id)));
                var color = (Color)Application.Current.Resources["SystemColorHighlightColor"];
                document.SetLinks(links, color);
            }
        }

        private void OnTextChanged(bool setText = true)
        {
            eventAggregator.GetEvent<SetTextEvent>().Publish(setText);
        }

        private async Task Save()
        {
            // Use property as if not changed the field may be null
            note.Title = Title;

            if (document != null)
            {
                string rtfContent;
                document.GetText(TextGetOptions.FormatRtf, out rtfContent);

                // The UI control appends newlines, so we remove them
                // NOTE: We may add a single newline in edit mode, to simplify adding new content
                rtfContent = RTF_TRAILING_NEWLINES.Replace(rtfContent, "}\r\n\0");

                note.RtfContent = rtfContent;
            }

            await database.SaveAsync(note);
            OnTextChanged(false);

            IsEditMode = false;
            Id = note.Id;
        }

        public void SetDocument(ITextDocument document)
        {
            if (this.document != null) throw new InvalidOperationException("Document has already been initialized.");
            if (document == null) throw new ArgumentNullException("document");

            TextFormat = new TextFormatViewModel(document);
            this.document = document;

            OnTextChanged();
        }

        public void SetNote(Note note)
        {
            if (note == null) throw new ArgumentNullException("note");

            this.note = note;
            this.Id = note.Id;

            // NOTE: If title is null then we need to trigger the change event explicitly for the UI
            //       to update the value
            if (this.title == null)
            {
                OnPropertyChanged(() => Title);
            }
            else
            {
                this.Title = null;
            }

            OnTextChanged();
        }

        public bool TryGoBack()
        {
            // TODO: Trigger a confirmation instead of simply discarding unsaved new notes
            if (IsEditMode && !IsNew && !string.IsNullOrWhiteSpace(Title))
            {
                eventAggregator.GetEvent<SaveEvent>().Publish(false);
                return false;
            }

            return true;
        }

        private class SetTextEvent : PubSubEvent<bool>
        {
        }

        private class SaveEvent : PubSubEvent<bool>
        {
        }
    }
}
