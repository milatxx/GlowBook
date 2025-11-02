using GlowBook.Wpf.ViewModels;
using System.Windows;
using System.Windows.Controls;
using GlowBook.Wpf.Views.Windows;


namespace GlowBook.Wpf.Views.Pages
{
    /// <summary>
    /// Interaction logic for AppointmentsPage.xaml
    /// </summary>
    public partial class AppointmentsPage : Page
    {
        public AppointmentsPage()
        {
            InitializeComponent();
            Loaded += (_, __) => VM.Load();
        }

        private AppointmentsViewModel VM => (AppointmentsViewModel)DataContext;

        private void Reload_Click(object sender, RoutedEventArgs e) => VM.Load();

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (VM.Selected == null)
            {
                MessageBox.Show("Selecteer eerst een afspraak.", "GlowBook");
                return;
            }

            var dlg = new AppointmentEditWindow(VM.Selected.Id);
            if (dlg.ShowDialog() == true)
                VM.Load(); // refresh na bewaren
        }
    }
}


