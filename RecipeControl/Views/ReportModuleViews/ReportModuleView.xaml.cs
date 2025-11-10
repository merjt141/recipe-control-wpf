using RecipeControl.ViewModels.ReportModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RecipeControl.Views.ReportModuleViews
{
    /// <summary>
    /// Interaction logic for ReportModuleView.xaml
    /// </summary>
    public partial class ReportModuleView : Window
    {
        public ReportModuleView(ReportModuleViewModel reportModuleViewModel)
        {
            InitializeComponent();

            // Load view model to context
            DataContext = reportModuleViewModel;
        }
    }
}
