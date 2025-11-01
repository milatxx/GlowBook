using GlowBook.Model.Entities;
using GlowBook.Wpf.Services;
using GlowBook.Wpf.ViewModels;
using System.Windows;
using System.Windows.Controls;


namespace GlowBook.Wpf.Views.Pages
{
    /// <summary>
    /// Interaction logic for StaffPage.xaml
    /// </summary>
    public partial class StaffPage : Page
    {
        public StaffPage() { InitializeComponent(); }
        private StaffViewModel VM => (StaffViewModel)DataContext;

        private void New_Click(object s, RoutedEventArgs e)
        {
            var item = new Staff { Name = "Nieuwe medewerker" };
            VM.Items.Add(item);
            Grid.SelectedItem = item;
        }

        private void Save_Click(object s, RoutedEventArgs e)
        {
            if (Grid.SelectedItem is Staff item)
            {
                try { VM.Save(item); DialogService.Info("Opgeslagen."); VM.Load(); }
                catch (System.Exception ex) { DialogService.Error(ex.Message); }
            }
        }

        private void Delete_Click(object s, RoutedEventArgs e)
        {
            if (Grid.SelectedItem is Staff item && DialogService.Confirm("Verwijderen?"))
            {
                try { VM.SoftDelete(item); VM.Items.Remove(item); }
                catch (System.Exception ex) { DialogService.Error(ex.Message); }
            }
        }
    }
}