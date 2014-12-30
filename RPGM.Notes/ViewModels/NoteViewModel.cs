using System;
using Caliburn.Micro;
using RPGM.Notes.Models;
using Windows.Phone.UI.Input;

namespace RPGM.Notes.ViewModels
{
    public class NoteViewModel : ViewModel
    {
        private readonly TextFormatViewModel textFormat = new TextFormatViewModel();

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

                    if (value)
                    {
                        Navigation.BackPressed += BackPressed;
                    }
                    else
                    {
                        Navigation.BackPressed -= BackPressed;
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

        public string RtfContent
        {
            get { return note != null ? note.RtfContent : null; }
            set
            {
                if (note != null)
                {
                    note.RtfContent = value;
                    NotifyOfPropertyChange(() => RtfContent);
                }
            }
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
                    NotifyOfPropertyChange(() => RtfContent);
                    NotifyOfPropertyChange(() => Title);
                }
            }
        }

        private void BackPressed(object sender, BackPressedEventArgs e)
        {
            // Setting this false removes the handler
            IsEditMode = false;
            Database.SaveAsync(note).Wait();
            e.Handled = true;
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
            NotifyOfPropertyChange(() => RtfContent);
            NotifyOfPropertyChange(() => Title);
            IsEditMode = false;
        }

        public void Edit()
        {
            IsEditMode = true;
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
                // TODO: Review whether to use placeholders and apply default on save
                original = new Note { Title = "New note" };

                // Directly to edit mode for new
                IsEditMode = true;
            }

            // Copy so we can revert changes without database hit
            note = new Note(original);

            NotifyOfPropertyChange(() => CanSave);
            NotifyOfPropertyChange(() => RtfContent);
            NotifyOfPropertyChange(() => Title);
        }

        public async void Save()
        {
            await Database.SaveAsync(note);
            IsEditMode = false;

            NotifyOfPropertyChange(() => CanDelete);
            NotifyOfPropertyChange(() => IsNew);
        }
    }
}
