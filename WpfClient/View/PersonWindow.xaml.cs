using System.Windows;
using WpfClient.ViewModel;

namespace WpfClient
{
    /// <summary>
    /// Логика взаимодействия для PersonWindow.xaml
    /// </summary>
    public partial class PersonWindow : Window
    {
        public PersonWindow()
        {
            InitializeComponent();

            DataContext = new PersonWindowViewModel();

            PersonWindowViewModel personWindowViewModel = new PersonWindowViewModel();
            DataContext = personWindowViewModel;
        }
    } 
}