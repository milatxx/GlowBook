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
        public StaffPage()
        {
            InitializeComponent();
            DataContext = new StaffViewModel();
        }
    }
}
        