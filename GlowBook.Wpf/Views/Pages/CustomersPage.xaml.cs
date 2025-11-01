using GlowBook.Model.Entities;
using GlowBook.Wpf.Services;
using GlowBook.Wpf.ViewModels;
using System.Windows;
using System.Windows.Controls;


namespace GlowBook.Wpf.Views.Pages
{
    /// <summary>
    /// Interaction logic for CustomersPage.xaml
    /// </summary>
    public partial class CustomersPage : Page
    {
        public CustomersPage() { InitializeComponent(); }
        private CustomersViewModel VM => (CustomersViewModel)DataContext;

        private void New_Click(object s, RoutedEventArgs e)
        {
            var c = new Customer { Name = "Nieuwe klant" };
            VM.Items.Add(c);
            Grid.SelectedItem = c;
        }

        private void Save_Click(object s, RoutedEventArgs e)
        {
            if (Grid.SelectedItem is Customer c)
            {
                try { VM.Save(c); DialogService.Info("Opgeslagen."); VM.Load(); }
                catch (System.Exception ex) { DialogService.Error($"Opslaan mislukt: {ex.Message}"); }
            }
        }

        private void Delete_Click(object s, RoutedEventArgs e)
        {
            if (Grid.SelectedItem is Customer c && DialogService.Confirm("Verwijderen (soft-delete)?"))
            {
                try { VM.SoftDelete(c); VM.Items.Remove(c); }
                catch (System.Exception ex) { DialogService.Error($"Verwijderen mislukt: {ex.Message}"); }
            }
        }
    }
}
