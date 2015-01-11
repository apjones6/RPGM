using System;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using RPGM.Notes.Models;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;

namespace RPGM.Notes.ViewModels
{
    public class NoteViewModel : ViewModel
    {
        private static readonly Color COLOR_BLACK = Color.FromArgb(0, 0, 0, 0);

        private ITextDocument document;
        private bool editMode;
        private Guid? id;
        private Note note;
        private Note original;
        private TextFormatViewModel textFormat;

        public NoteViewModel(INavigationService navigation, IDatabase database)
            : base(navigation, database)
        {
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
            get { return note != null && !string.IsNullOrWhiteSpace(note.Title); }
        }

        public Guid? Id
        {
            get { return id; }
            set
            {
                id = value;
                NotifyOfPropertyChange(() => Id);
            }
        }

        public bool IsEditMode
        {
            get { return editMode; }
            set
            {
                editMode = value;
                NotifyOfPropertyChange(() => CanDiscard);
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
        }

        public string Title
        {
            get { return note != null ? note.Title : null; }
            set
            {
                if (note != null)
                {
                    note.Title = value;
                    NotifyOfPropertyChange(() => CanSave);
                    NotifyOfPropertyChange(() => Title);
                }
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
            // Restore note to original as it likely has changes
            note = new Note(original);
            NotifyOfPropertyChange(() => Title);
            document.SetText(TextSetOptions.FormatRtf, note.RtfContent);
            await SetLinks();
            IsEditMode = false;
        }

        public void Edit()
        {
            document.SetText(TextSetOptions.FormatRtf, note.RtfContent);
            IsEditMode = true;
        }

        public void Navigate(Uri uri)
        {
            var parts = uri.AbsoluteUri.ToLower().Replace("richtea.rpgm://", string.Empty).Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            var parameter = parts.Length > 1 ? parts[1] : null;

            if (parts[0] == "notes")
            {
                Navigation
                    .UriFor<NoteViewModel>()
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
                original = await Database.GetAsync(Id.Value);
            }
            else
            {
                original = new Note();

                // Directly to edit mode for new
                IsEditMode = true;
            }

            // Copy so we can revert changes without database hit
            note = new Note(original);

            if (document != null && !string.IsNullOrEmpty(note.RtfContent))
            {
                document.SetText(TextSetOptions.FormatRtf, note.RtfContent);
                if (!IsEditMode)
                {
                    await SetLinks();
                }
            }

            NotifyOfPropertyChange(() => CanSave);
            NotifyOfPropertyChange(() => Title);
        }

        public async Task Save()
        {
            if (document != null)
            {
                string rtfContent;
                document.GetText(TextGetOptions.FormatRtf, out rtfContent);
                note.RtfContent = rtfContent;
            }

            await Database.SaveAsync(note);
            Id = note.Id;

            await SetLinks();

            IsEditMode = false;

            // Update the stored original for reverting changes
            // NOTE: This is messy
            original = new Note(note);

            NotifyOfPropertyChange(() => CanDelete);
            NotifyOfPropertyChange(() => IsNew);
        }

        public async Task SetDocument(RichEditBox richEditBox)
        {
            // NOTE: We'd like to take the ITextDocument directly, and deal with the control
            //       at the message binder, but it attempts to cast the document on another
            //       thread, which results in an InvalidCastException.

            if (this.document != null) throw new InvalidOperationException("Document has already been initialized.");
            if (richEditBox == null) throw new ArgumentNullException("richEditBox");

            this.document = richEditBox.Document;
            this.textFormat = new TextFormatViewModel(document);
            NotifyOfPropertyChange(() => TextFormat);

            if (note != null && !string.IsNullOrEmpty(note.RtfContent))
            {
                document.SetText(TextSetOptions.FormatRtf, note.RtfContent);
                await SetLinks();
            }
        }

        private async Task SetLinks()
        {
            // TODO: Use an alias table
            var notes = await Database.ListAsync();
            var links = notes.Except(new[] { note }).ToDictionary(x => x.Title, x => string.Format("richtea.rpgm://notes/{0}", x.Id));
            document.AutoHyperlinks(links, AccentColor);
        }
    }
}
