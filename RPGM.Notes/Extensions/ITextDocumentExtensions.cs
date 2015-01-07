using System;
using Windows.UI.Text;

namespace Windows.UI.Text
{
    public static class ITextDocumentExtensions
    {
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
