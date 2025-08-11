using System.Windows;
using System.Windows.Navigation;

namespace Plexity.UI.ViewModels.About
{
    public class SupportersViewModel : NotifyPropertyChangedViewModel
    {
        public SupporterData? SupporterData { get; private set; }

        public GenericTriState LoadedState { get; set; } = GenericTriState.Unknown;

        public string LoadError { get; set; } = "";

        public int Columns { get; set; } = 3;

        public SizeChangedEventHandler? WindowResizeEvent;

        public SupportersViewModel()
        {
            WindowResizeEvent += OnWindowResize;

            // this will cause momentary freezes only when ran under the debugger
            LoadSupporterData();
        }

        private void OnWindowResize(object sender, SizeChangedEventArgs e)
        {
            if (!e.WidthChanged)
                return;

            int newCols = (int)Math.Floor(e.NewSize.Width / 200);

            if (Columns == newCols)
                return;

            Columns = newCols;
            OnPropertyChanged(nameof(Columns));
        }

		public async void LoadSupporterData()
        {
            const string LOG_IDENT = "AboutViewModel::LoadSupporterData";
			var data = new SupporterData
			{
				Monthly = new SupporterGroup
				{
					Columns = 4,
					Supporters =
		{
			new Supporter {
				Name = "ROBLOGUY",
				Image = "https://yt3.googleusercontent.com/MW7oEOKM23a8MVC2aU06dcZtKBWKRkZIdRcGomohfJLfjJ_DQ0k4RqfSmGwY57E_RJLdIwQU3sY=s160-c-k-c0x00ffffff-no-rj",
				Link = "https://www.youtube.com/@ROBLOGUY-p7o",
				LinkText = "YouTube"
			},
			new Supporter {
				Name = "Tripi Tropa YT",
				Image = "https://yt3.googleusercontent.com/xzjlx1TKw6JTm3uOBXtkAV_q-IwdcONSX0AOLbUFSVHPvNiqTdXVBM-HhyqKMfYbAgbwjHAN9Q=s120-c-k-c0x00ffffff-no-rj",
				Link = "https://www.youtube.com/@TripiTropaYT-w9m",
				LinkText = "YouTube"
			}
		}
				}
			};

			try
            {
                SupporterData = data;
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine(LOG_IDENT, "Could not load supporter data");
                App.Logger.WriteException(LOG_IDENT, ex);

                LoadedState = GenericTriState.Failed;
                LoadError = ex.Message;

                OnPropertyChanged(nameof(LoadError));
            }

            if (SupporterData is not null)
            {
                LoadedState = GenericTriState.Successful;

                OnPropertyChanged(nameof(SupporterData));
            }

            OnPropertyChanged(nameof(LoadedState));
        }
    }
}
