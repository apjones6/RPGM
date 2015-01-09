using System;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using RPGM.Notes.Models;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.ViewManagement;

namespace RPGM.Notes.ViewModels
{
    public class NoteViewModel : ViewModel
    {
        private static readonly Color COLOR_BLACK = Color.FromArgb(0, 0, 0, 0);

        private readonly TextFormatViewModel textFormat = new TextFormatViewModel();

        private ITextDocument document;
        private bool editMode;
        private Note note;
        private Note original;

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
            get { return IsEditMode; }
        }

        public bool CanSave
        {
            get { return note != null && !string.IsNullOrWhiteSpace(note.Title); }
        }

        public object Document
        {
            get { return document; }
            set
            {
                if (document == null)
                {
                    document = (ITextDocument)value;
                    NotifyOfPropertyChange(() => Document);

                    if (note != null && !string.IsNullOrEmpty(note.RtfContent))
                    {
                        document.SetText(TextSetOptions.FormatRtf, note.RtfContent);
                        ApplyHyperlinks();
                    }
                }
            }
        }

        public bool IsEditMode
        {
            get { return editMode; }
            set
            {
                if (editMode != value)
                {
                    editMode = value;
                    NotifyOfPropertyChange(() => CanDiscard);
                    NotifyOfPropertyChange(() => IsEditMode);
                    NotifyOfPropertyChange(() => IsNotEditMode);

                    if (document != null)
                    {
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
        }

        public bool IsNew
        {
            get { return note != null && note.Id == Guid.Empty; }
        }

        public bool IsNotEditMode
        {
            get { return !editMode; }
        }

        public Guid? Parameter { get; set; }

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

        private async Task ApplyHyperlinks()
        {
            // TODO: Use an alias table
            var notes = (await Database.ListAsync()).Except(new[] { note }).ToArray();

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
                    using (document.SuppressSelection())
                    {
                        range.CharacterFormat.ForegroundColor = AccentColor;
                        range.Link = link;
                        skip = range.EndPosition;
                    }
                }
            }

            document.ApplyDisplayUpdates();
        }

        public override async void CanClose(Action<bool> callback)
        {
            // NOTE: If we can't save, we're discarding, but we should probably show a message
            if (IsEditMode && !IsNew && CanSave)
            {
                IsEditMode = false;
                callback(false);
                await Database.SaveAsync(note);
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

        public void Discard()
        {
            // Restore note to original as it likely has changes
            note = new Note(original);
            document.SetText(TextSetOptions.FormatRtf, note.RtfContent);
            NotifyOfPropertyChange(() => Title);
            IsEditMode = false;
        }

        public void Edit()
        {
            IsEditMode = true;
        }

        public void Navigate(Uri uri)
        {
            var parts = uri.AbsoluteUri.ToLower().Replace("richtea.rpgm://", string.Empty).Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            var parameter = parts.Length > 1 ? parts[1] : null;

            if (parts[0] == "notes")
            {
                Navigation.NavigateToViewModel<NoteViewModel>(Guid.Parse(parameter));
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
            // NOTE: Caliburn navigation by URI parameters does not support Guid
            if (Parameter != null)
            {
                // TODO: Online advice is that this method can be async void, but Caliburn incorrectly
                //       thinks we've initialized once we unblock the UI thread
                original = await Database.GetAsync(Parameter.Value);
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
                await ApplyHyperlinks();
            }

            NotifyOfPropertyChange(() => CanSave);
            NotifyOfPropertyChange(() => Title);
        }

        public async void Save()
        {
            if (document != null)
            {
                string rtfContent;
                document.GetText(TextGetOptions.FormatRtf, out rtfContent);
                note.RtfContent = rtfContent;
            }

            await Database.SaveAsync(note);
            IsEditMode = false;

            // Update the stored original for reverting changes
            // NOTE: This is messy
            original = new Note(note);

            NotifyOfPropertyChange(() => CanDelete);
            NotifyOfPropertyChange(() => IsNew);
        }
    }
}
