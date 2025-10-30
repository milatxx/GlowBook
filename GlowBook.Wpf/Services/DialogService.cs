

using System.Windows;

namespace GlowBook.Wpf.Services
{
    public static class DialogService
    {
        public static void Info(string msg) => MessageBox.Show(msg, "GlowBook", MessageBoxButton.OK, MessageBoxImage.Information);
        public static void Warn(string msg) => MessageBox.Show(msg, "GlowBook", MessageBoxButton.OK, MessageBoxImage.Warning);
        public static void Error(string msg) => MessageBox.Show(msg, "GlowBook", MessageBoxButton.OK, MessageBoxImage.Error);
        public static bool Confirm(string msg) => MessageBox.Show(msg, "Bevestigen", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
    }
}
