using System;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using RPGM.Notes.Models;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace RPGM.Notes.ViewModels
{
    public class NoteViewModel : ViewModel
    {
        private static readonly Color COLOR_BLACK = Color.FromArgb(0, 0, 0, 0);

        private ITextDocument document;
        private bool editMode;
        private Note note;
        private TextFormatViewModel textFormat;
        private string title;

        public NoteViewModel(INavigationService navigation, IDatabase database)
            : base(navigation, database)
        {
        }

        public Guid? BackId
        {
            get;
            set;
        }

        public bool CanDelete
        {
            get { return !IsNew; }
        }

        public bool CanDiscard
        {
            get { return IsEditMode && !IsNew; }
        }

        public bool CanSave
        {
            get { return !string.IsNullOrWhiteSpace(Title); }
        }

        public Guid? Id
        {
            get;
            set;
        }

        public bool IsEditMode
        {
            get { return editMode; }
            set
            {
                editMode = value;
                NotifyOfPropertyChange(() => CanDiscard);
                NotifyOfPropertyChange(() => CanSave);
                NotifyOfPropertyChange(() => IsEditMode);
                NotifyOfPropertyChange(() => IsNotEditMode);
            }
        }

        public bool IsNew
        {
            get { return note != null && note.Id == Guid.Empty; }
        }

        public bool IsNotEditMode
        {
            get { return !editMode; }
        }

        public TextFormatViewModel TextFormat
        {
            get { return textFormat; }
            set
            {
                textFormat = value;
                NotifyOfPropertyChange(() => TextFormat);
            }
        }

        public string Title
        {
            get { return title ?? (note != null ? note.Title : null); }
            set
            {
                title = value;
                NotifyOfPropertyChange(() => CanSave);
                NotifyOfPropertyChange(() => Title);
            }
        }

        public override async void CanClose(Action<bool> callback)
        {
            // NOTE: If we can't save, we're discarding, but we should probably show a message
            if (IsEditMode && !IsNew && CanSave)
            {
                callback(false);
                await Save();
            }
            else
            {
                callback(true);
            }
        }

        public async void Delete()
        {
            await Database.DeleteAsync(note.Id);

            // NOTE: Don't just close edit
            IsEditMode = false;

            // TODO: Navigate forward to notes list, and possibly clean back stack
            Navigation.GoBack();
        }

        public async void Discard()
        {
            await SetText();
            IsEditMode = false;
            Title = null;
        }

        public void Edit()
        {
            if (!string.IsNullOrEmpty(note.RtfContent))
            {
                document.SetText(TextSetOptions.FormatRtf, note.RtfContent);
            }

            IsEditMode = true;
        }

        private void HandleBackId()
        {
            // Back ID is provided when the prevous page was a new note, so we need to update the back stack entry
            // so that we return to the note, rather than the 'new note' view
            if (BackId != null && Navigation.BackStack.Count > 0)
            {
                var oldEntry = Navigation.BackStack[Navigation.BackStack.Count - 1];
                var newEntry = new PageStackEntry(oldEntry.SourcePageType, Navigation.UriFor<NoteViewModel>().WithParam(x => x.Id, BackId.Value).BuildUri().ToString(), oldEntry.NavigationTransitionInfo);
                Navigation.BackStack.Remove(oldEntry);
                Navigation.BackStack.Add(newEntry);
                BackId = null;
            }
        }

        public void Navigate(Uri uri)
        {
            var parts = uri.AbsoluteUri.ToLower().Replace("richtea.rpgm://", string.Empty).Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            var parameter = parts.Length > 1 ? parts[1] : null;

            if (parts[0] == "notes")
            {
                Navigation
                    .UriFor<NoteViewModel>()
                    .WithParam(x => x.BackId, Id == null ? (Guid?)note.Id : null)
                    .WithParam(x => x.Id, Guid.Parse(parameter))
                    .Navigate();
            }
            else
            {
                // TODO: Throw?
            }
        }

        protected override void OnActivate()
        {
            StatusBar.GetForCurrentView().ForegroundColor = COLOR_BLACK;
        }

        protected override void OnDeactivate(bool close)
        {
            StatusBar.GetForCurrentView().ForegroundColor = null;
        }

        protected override async void OnInitialize()
        {
            if (Id != null)
            {
                // TODO: Online advice is that this method can be async void, but Caliburn incorrectly
                //       thinks we've initialized once we unblock the UI thread
                note = await Database.GetAsync(Id.Value);
            }
            else
            {
                note = new Note();
            }

            // Navigate over the note twice, delete it, and back to an earlier access
            if (note == null)
            {
                Navigation.GoBack();
                return;
            }

            NotifyOfPropertyChange(() => CanSave);
            NotifyOfPropertyChange(() => Title);

            if (IsNew)
            {
                await SetText(true, false);
                IsEditMode = true;
            }
            else
            {
                await SetText();
            }

            HandleBackId();
        }

        public async Task Save()
        {
            // NOTE: Use property as if not changed the field may be null
            note.Title = Title;

            if (document != null)
            {
                string rtfContent;
                document.GetText(TextGetOptions.FormatRtf, out rtfContent);
                note.RtfContent = rtfContent;
            }

            await Database.SaveAsync(note);
            await SetText(false);

            IsEditMode = false;

            NotifyOfPropertyChange(() => CanDelete);
            NotifyOfPropertyChange(() => IsNew);
        }

        public async Task SetDocument(RichEditBox richEditBox)
        {
            // NOTE: We'd like to take the ITextDocument directly, and deal with the control
            //       at the message binder, but it attempts to cast the document on another
            //       thread, which results in an InvalidCastException.

            if (document != null) throw new InvalidOperationException("Document has already been initialized.");
            if (richEditBox == null) throw new ArgumentNullException("richEditBox");

            document = richEditBox.Document;
            TextFormat = new TextFormatViewModel(document);

            await SetText();
        }

        private async Task SetText(bool setText = true, bool setLinks = true)
        {
            if (document == null || note == null || string.IsNullOrEmpty(note.RtfContent))
            {
                return;
            }

            if (setText)
            {
                document.SetText(TextSetOptions.FormatRtf, note.RtfContent);
            }

            if (setLinks)
            {
                // TODO: Use an alias table
                var notes = await Database.ListAsync();
                var links = notes.Except(new[] { note }).ToDictionary(x => x.Title, x => string.Format("richtea.rpgm://notes/{0}", x.Id));
                document.AutoHyperlinks(links, AccentColor);
            }
        }
    }
}
