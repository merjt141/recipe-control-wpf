using ReportMicro.Views;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ReportMicro
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Importante: para que el Binding encuentre RegistroPesoList
            DataContext = this;
        }

        private void BotonLogin_Click(object sender, RoutedEventArgs e)
        {
            var ReportWindow = new ReportView();
            Application.Current.MainWindow = ReportWindow;
            ReportWindow.Show();
            this.Close();
        }
    }
}