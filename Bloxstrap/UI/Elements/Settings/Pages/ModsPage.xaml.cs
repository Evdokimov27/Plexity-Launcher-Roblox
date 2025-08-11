using Plexity.UI.ViewModels.Settings;

namespace Plexity.UI.Elements.Settings.Pages
{
    /// <summary>
    /// Interaction logic for ModsPage.xaml
    /// </summary>
    public partial class ModsPage
    {
        public ModsPage()
        {
            DataContext = new ModsViewModel();
            InitializeComponent();
        }

        private void OptionControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void CustomIntegrationSelection(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
    }
}
