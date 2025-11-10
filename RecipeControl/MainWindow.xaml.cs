using Microsoft.Extensions.DependencyInjection;
using RecipeControl.Configuration;
using RecipeControl.Services.Database;
using RecipeControl.Views.RegisterModuleViews;
using RecipeControl.Views.ReportModuleViews;
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

namespace RecipeControl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IDatabaseService _databaseService;
        private readonly AppSettings _appSettings;
        private readonly IServiceProvider _serviceProvider;


        public MainWindow(
            IDatabaseService databaseService,
            AppSettings appSettings,
            IServiceProvider serviceProvider)
        {
            InitializeComponent();

            _databaseService = databaseService;
            _appSettings = appSettings;
            _serviceProvider = serviceProvider;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var RegisterModuleView = _serviceProvider.GetRequiredService<RegisterModuleView>();
            RegisterModuleView.Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var ReportModuleView = _serviceProvider.GetRequiredService<ReportModuleView>();
            ReportModuleView.Show();
        }
    }
}