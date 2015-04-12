using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Windows.Foundation;

namespace Windows.UI.Text
{
    public static class ITextDocumentExtensions
    {
        private static readonly Regex TRIM_WRAPPING_QUOTES = new Regex("^\"?(?<uri>.*?)\"?$", RegexOptions.IgnoreCase);

        public static Uri GetLinkFromPoint(this ITextDocument document, Point point, PointOptions options)
        {
            var range = document.GetRangeFromPoint(point, options);
            range.StartOf(TextRangeUnit.Link, true);

            if (!string.IsNullOrEmpty(range.Link))
            {
                var match = TRIM_WRAPPING_QUOTES.Match(range.Link);
                if (match.Success)
                {
                    return new Uri(match.Groups["uri"].Value);
                }
            }

            return null;
        }

        public static void SetLinks(this ITextDocument document, IDictionary<string, Uri> links, Color? color = null)
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
                        range.Link = '\"' + link.Value.AbsoluteUri + '\"';
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
