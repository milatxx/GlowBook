using GlowBook.Model.Entities;
using GlowBook.Wpf.Services;
using GlowBook.Wpf.ViewModels;
using System.Windows;
using System.Windows.Controls;


namespace GlowBook.Wpf.Views.Pages
{
    /// <summary>
    /// Interaction logic for ServicesPage.xaml
    /// </summary>
    public partial class ServicesPage : Page
    {
        public ServicesPage() { InitializeComponent(); }
        private ServicesViewModel VM => (ServicesViewModel)DataContext;

        private void New_Click(object s, RoutedEventArgs e)
        {
            var item = new Service { Name = "Nieuwe dienst", DurationMin = 30, Price = 0m };
            VM.Items.Add(item);
            Grid.SelectedItem = item;
        }

        private void Save_Click(object s, RoutedEventArgs e)
        {
            if (Grid.SelectedItem is Service item)
            {
                try { VM.Save(item); DialogService.Info("Opgeslagen."); VM.Load(); }
                catch (System.Exception ex) { DialogService.Error(ex.Message); }
            }
        }

        private void Delete_Click(object s, RoutedEventArgs e)
        {
            if (Grid.SelectedItem is Service item && DialogService.Confirm("Verwijderen?"))
            {
                try { VM.SoftDelete(item); VM.Items.Remove(item); }
                catch (System.Exception ex) { DialogService.Error(ex.Message); }
            }
        }
    }
}
