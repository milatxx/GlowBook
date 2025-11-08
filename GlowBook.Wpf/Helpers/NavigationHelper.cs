using System.Windows.Controls;
using System.Windows;

namespace GlowBook.Wpf.Helpers
{
    public static class NavigationHelper
    {
        public static void Navigate(Page page)
        {
            if (Application.Current.MainWindow is Views.MainWindow mainWindow)
            {
                mainWindow.MainFrame.Content = page;
            }
        }
    }
}
