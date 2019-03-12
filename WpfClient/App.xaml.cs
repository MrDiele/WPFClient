using System.Windows;
using WpfClient.ViewModel;

namespace WpfClient
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        MainWindowViewModel mainWindowViewModel;
        public DisplayRootRegistry displayRootRegistry = new DisplayRootRegistry();

        public App()
        {
            displayRootRegistry.RegisterWindowType<MainWindowViewModel, MainWindow>();
            displayRootRegistry.RegisterWindowType<PersonWindowViewModel, PersonWindow>();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            mainWindowViewModel = new MainWindowViewModel();

            await displayRootRegistry.ShowModalPresentation(mainWindowViewModel);

            Shutdown();
        }
    }
}