﻿using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Mvvm.Interfaces;
using RPGM.Notes.Models;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace RPGM.Notes.ViewModels
{
    [Export]
    public class NoteViewModel : ViewModel, IBackNavigationAware
    {
        private readonly IDatabase database;
        private readonly DelegateCommandBase delete;
        private readonly DelegateCommandBase discard;
        private readonly ICommand edit;
        private readonly INavigationService navigation;
        private readonly DelegateCommandBase save;

        private ITextDocument document;
        private bool isEditMode;
        private Note note;
        private TextFormatViewModel textFormat;
        private string title;

        [ImportingConstructor]
        public NoteViewModel(IDatabase database, INavigationService navigation)
        {
            if (database == null) throw new ArgumentNullException("database");
            if (navigation == null) throw new ArgumentNullException("navigation");

            this.database = database;
            this.delete = DelegateCommand.FromAsyncHandler(Delete, () => !IsNew);
            this.discard = DelegateCommand.FromAsyncHandler(Discard, () => IsEditMode && !IsNew);
            this.edit = new DelegateCommand(Edit);
            this.navigation = navigation;
            this.save = DelegateCommand.FromAsyncHandler(Save, () => !string.IsNullOrWhiteSpace(Title));
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
        public Guid Id { get; set; }

        public bool IsEditMode
        {
            get { return isEditMode; }
            set
            {
                SetProperty(ref isEditMode, value);
                discard.RaiseCanExecuteChanged();
                save.RaiseCanExecuteChanged();
                OnPropertyChanged(() => IsNotEditMode);
            }
        }

        public bool IsNew
        {
            get { return note != null && note.Id == Guid.Empty; }
        }

        public bool IsNotEditMode
        {
            get { return !isEditMode; }
        }

        public bool IsNotNew
        {
            get { return note == null || note.Id != Guid.Empty; }
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

        public async Task Delete()
        {
            await database.DeleteAsync(note.Id);

            // Stop TryGoBack from preventing navigation
            IsEditMode = false;

            navigation.GoBack();
        }

        public async Task Discard()
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

            if (Id != Guid.Empty)
            {
                note = await database.GetAsync(Id);

                if (note == null && navigationMode == NavigationMode.Back)
                {
                    // NOTE: This does not work! Cannot navigate again until current navigation finishes
                    //       Likely we need to notify app to clean entries from back stack
                    navigation.GoBack();
                    return;
                }
            }
            else
            {
                note = new Note();
            }

            // TODO: I think SetText never has document now, so just set edit mode
            if (IsNew)
            {
                await SetText(true, false);
                IsEditMode = true;
            }
            else
            {
                await SetText();
            }
        }

        public async Task Save()
        {
            // Use property as if not changed the field may be null
            note.Title = Title;

            if (document != null)
            {
                string rtfContent;
                document.GetText(TextGetOptions.FormatRtf, out rtfContent);

                // The UI control appends newlines, so we remove them
                // NOTE: We may add a single newline in edit mode, to simplify adding new content
                var regex = new Regex(@"(\r|\n|\\par|\\pard|\\ltrpar|\\tx\d+|\\fs\d+)*}(\r|\n|\0)*$", RegexOptions.IgnoreCase);
                rtfContent = regex.Replace(rtfContent, "}\r\n\0");

                note.RtfContent = rtfContent;
            }

            await database.SaveAsync(note);
            await SetText(false);

            IsEditMode = false;
            Id = note.Id;

            delete.RaiseCanExecuteChanged();
            OnPropertyChanged(() => IsNew);
        }

        public async Task SetDocument(ITextDocument document)
        {
            if (this.document != null) throw new InvalidOperationException("Document has already been initialized.");
            if (document == null) throw new ArgumentNullException("document");

            TextFormat = new TextFormatViewModel(document);
            this.document = document;

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

            // TODO: Use edit mode instead of parameter?
            if (setLinks)
            {
                // TODO: Use an alias table
                var notes = await database.ListAsync();
                var links = notes.Except(new[] { note }).ToDictionary(x => x.Title, x => string.Format("richtea.rpgm://notes/{0}", x.Id));
                var color = (Color)Application.Current.Resources["SystemColorHighlightColor"];
                document.AutoHyperlinks(links, color);
            }
        }

        public bool TryGoBack()
        {
            // TODO: Trigger a confirmation instead of simply discarding unsaved new notes
            if (IsEditMode && !IsNew && !string.IsNullOrWhiteSpace(Title))
            {
                // NOTE: If we try to synchronously wait for the save to finish we
                //       block the UI thread return. This means nothing validates it
                //       completes at all
                Save();
                return false;
            }

            return true;
        }
    }
}
