using System;
using System.Collections.Generic;

namespace Windows.UI.Text
{
    public static class ITextDocumentExtensions
    {
        public static void AutoHyperlinks(this ITextDocument document, IDictionary<string, string> links, Color? color = null)
        {
            // Avoid performance implications of many small updates
            document.BatchDisplayUpdates();

            ITextRange range;
            foreach (var link in links)
            {
                var skip = 0;
                while ((range = document.GetRange(skip, TextConstants.MaxUnitCount)).FindText(link.Key, TextConstants.MaxUnitCount, FindOptions.Word) != 0)
                {
                    // NOTE: Set the document selection as workaround to prevent intermittent AccessViolationException,
                    //       probably caused by a timing issue in the lower level code
                    using (document.SuppressSelection())
                    {
                        range.Link = '\"' + link.Value + '\"';
                        if (color != null)
                        {
                            range.CharacterFormat.ForegroundColor = color.Value;
                        }

                        skip = range.EndPosition;
                    }
                }
            }

            document.ApplyDisplayUpdates();
        }

        public static IDisposable SuppressSelection(this ITextDocument document)
        {
            var start = document.Selection.StartPosition;
            var end = document.Selection.EndPosition;

            var disposable = new ActionDisposable(() => document.Selection.SetRange(start, end));
            document.Selection.SetRange(0, 0);

            return disposable;
        }

        private sealed class ActionDisposable : IDisposable
        {
            private readonly Action dispose;

            public ActionDisposable(Action dispose)
            {
                this.dispose = dispose;
            }

            public void Dispose()
            {
                dispose();
            }
        }
    }
}
