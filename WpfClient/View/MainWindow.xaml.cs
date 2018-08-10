using System.Windows;
using WpfClient.ViewModel;

namespace WpfClient
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel _mainWindowViewModel;

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Получает модель представления из данных Context и присваивает ее переменной.
        /// </summary>
        private void OnMainGridDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _mainWindowViewModel = (MainWindowViewModel)this.DataContext;
        }

        /// <summary>
        /// Метод вызываемый для закрытия формы.
        /// </summary>
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Close();
        }
    }
}
