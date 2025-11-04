using GlowBook.Model.Entities;
using GlowBook.Wpf.Services;
using GlowBook.Wpf.ViewModels;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;


namespace GlowBook.Wpf.Views.Pages
{
    /// <summary>
    /// Interaction logic for CustomersPage.xaml
    /// </summary>
    public partial class CustomersPage : Page
    {
        public CustomersPage() 
            {
            InitializeComponent();
            if (DesignerProperties.GetIsInDesignMode(this)) return;

            DataContext = new CustomersViewModel();
        }

        private CustomersViewModel VM => (CustomersViewModel)DataContext;


    }
}
