using Plexity.UI.ViewModels.About;
using System.Windows;
using System.Windows.Navigation;

namespace Plexity.UI.Elements.About.Pages
{
    /// <summary>
    /// Interaction logic for SupportersPage.xaml
    /// </summary>
    public partial class SupportersPage
    {
        private readonly SupportersViewModel _viewModel = new();

        public SupportersPage()
        {
            DataContext = _viewModel;
            InitializeComponent();
        }
		private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			try
			{
				Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
				e.Handled = true;
			}
			catch (Exception)
			{
				// по желанию: показать MessageBox/лог
			}
		}
		private void UiPage_SizeChanged(object sender, SizeChangedEventArgs e)
            => _viewModel.WindowResizeEvent?.Invoke(sender, e);
    }
}
