using GlowBook.Wpf.Services;
using GlowBook.Wpf.ViewModels;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.IO;

namespace GlowBook.Wpf.Views.Pages
{
    /// <summary>
    /// Interaction logic for ReportsPage.xaml
    /// </summary>
    public partial class ReportsPage : Page
    {
        public ReportsPage() { InitializeComponent(); }
        private ReportsViewModel VM => (ReportsViewModel)DataContext;

        private void Csv_Click(object s, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog { Filter = "CSV (*.csv)|*.csv", FileName = "rapport.csv" };
            if (dlg.ShowDialog() == true)
            {
                try { VM.ExportCsv(dlg.FileName); DialogService.Info("CSV opgeslagen."); }
                catch (System.Exception ex) { DialogService.Error(ex.Message); }
            }
        }
        private void Pdf_Click(object s, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog { Filter = "PDF (*.pdf)|*.pdf", FileName = "rapport.pdf" };
            if (dlg.ShowDialog() == true)
            {
                try { VM.ExportPdf(dlg.FileName); DialogService.Info("PDF opgeslagen."); }
                catch (System.Exception ex) { DialogService.Error(ex.Message); }
            }
        }
    }
}
