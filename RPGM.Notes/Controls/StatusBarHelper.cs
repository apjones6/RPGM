using Windows.UI;
using Windows.UI.ViewManagement;

namespace RPGM.Notes.Controls
{
    public static class StatusBarHelper
    {
        public static readonly Color COLOR_BLACK = new Color { R = 0, G = 0, B = 0 };

        public static Color? ForegroundColor
        {
            set { StatusBar.GetForCurrentView().ForegroundColor = value; }
        }
    }
}
