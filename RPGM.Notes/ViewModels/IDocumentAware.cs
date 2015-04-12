using Windows.UI.Text;

namespace RPGM.Notes.ViewModels
{
    public interface IDocumentAware
    {
        void SetDocument(ITextDocument document);
    }
}
