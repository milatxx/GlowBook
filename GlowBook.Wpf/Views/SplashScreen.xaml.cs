
using System.Threading.Tasks;
using System.Windows;

namespace GlowBook.Wpf.Views
{
    /// <summary>
    /// Interaction logic for SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen : Window
    {
        public SplashScreen()
        {
            InitializeComponent();
            Loaded += async (_, __) =>
            {
                await Task.Delay(1500);
                new LoginWindow().Show();
                Close();
            };
        }
    }
}
