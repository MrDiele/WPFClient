using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using WpfClient.Model;
using WpfClient.Utility;
using WpfClient.WorkWithServer;

namespace WpfClient.ViewModel
{
    class MainWindowViewModel : ViewModelBase
    {

        #region Fields

        private Person _SelectedPerson;
        private ObservableCollection<Person> _Persons;
        private int _PersonCount;
        private ICommand _AddPersonWindow;
        private ICommand _EditPersonWindow;
        private ICommand _getAppointmentCommand;
        private ICommand _getAuthorCommand;
        private ICommand _deleteCommand;
        
        #endregion

        #region Constructor

        public MainWindowViewModel()
        {
            this.GetData();
        }

        public MainWindowViewModel(Person person)
        {
            SelectedPerson = person;
        }

        #endregion

        #region Set/Get fields

        /// <summary>
        /// Список людей.
        /// </summary>
        public ObservableCollection<Person> PersonsList
        {
            get { return _Persons; }

            set
            {
                _Persons = value;
                base.RaisePropertyChangedEvent("PersonsList");
            }
        }

        /// <summary>
        /// Выбранный человек из списка.
        /// </summary>
        public Person SelectedPerson
        {
            get { return _SelectedPerson; }

            set
            {
                _SelectedPerson = value;
                base.RaisePropertyChangedEvent("SelectedPerson");
            }
        }

        /// <summary>
        /// Колличество людей в списке.
        /// </summary>
        public int PersonCount
        {
            get { return _PersonCount; }

            set
            {
                _PersonCount = value;
                base.RaisePropertyChangedEvent("PersonCount");
            }
        }

        #endregion

        #region Command

        /// <summary>
        /// Команда на добавление нового пользователя.
        /// </summary>
        public ICommand AddPersonWindow
        {
            get
            {
                if (_AddPersonWindow == null)
                {
                    _AddPersonWindow = new AddPersonWindowCommand(this);
                }
                return _AddPersonWindow;
            }
        }

        /// <summary>
        /// Команда на редактирование пользователя.
        /// </summary>
        public ICommand EditPersonWindow
        {
            get
            {
                if (_EditPersonWindow == null)
                {
                    _EditPersonWindow = new EditPersonWindowCommand(this);
                }
                return _EditPersonWindow;
            }
        }

        /// <summary>
        /// Команда на удаление.
        /// </summary>
        public ICommand DeleteCommand
        {
            get
            {
                if (_deleteCommand == null)
                {
                    _deleteCommand = new DeleteCommand(this);
                }
                return _deleteCommand;
            }
        }

        /// <summary>
        /// Команда на получение информации о проекте.
        /// </summary>
        public ICommand GetAppointmentCommand
        {
            get
            {
                if (_getAppointmentCommand == null)
                {
                    _getAppointmentCommand = new GetAppointmentCommand(this);
                }
                return _getAppointmentCommand;
            }
        }

        /// <summary>
        /// Команда на получение информации об авторе.
        /// </summary>
        public ICommand GetAuthorCommand
        {
            get
            {
                if (_getAuthorCommand == null)
                {
                    _getAuthorCommand = new GetAuthorCommand(this);
                }
                return _getAuthorCommand;
            }
        }

        #endregion

        #region Method logic

        /// <summary>
        /// Удаление пользователя из списка.
        /// </summary>
        public void DeletePerson()
        {
            if (SelectedPerson != null)
            {
                var dialogResult = MessageBox.Show("Вы действительно хотите удалить пользователя " + _SelectedPerson.Name + "?", "Сообщение", MessageBoxButton.YesNo);
                if (dialogResult == MessageBoxResult.Yes)
                {
                    InteractionServer response = new InteractionServer();
                    response.DeletePerson(SelectedPerson.Idperson);
                    this.GetData();
                }
            }
            else
            {
                MessageBox.Show("Выберите пользователя для удаления", "Сообщение", MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// Событие обновления при изменении списка людей .
        /// </summary>
        void OnGroceryListChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this._PersonCount = this.PersonsList.Count;
        }

        /// <summary>
        /// Метод для заполнения списка пользователей.
        /// </summary>
        private void GetData()
        {
            _Persons = new ObservableCollection<Person>();

            _Persons.CollectionChanged += OnGroceryListChanged;

            InteractionServer response = new InteractionServer();
            List<Person> lp = response.GetPersonList();
            for (int i = 0; i < lp.Count; i++)
            {
                PersonsList.Add(new Person { Idperson = lp[i].Idperson, Name = lp[i].Name, Dateofbirth = Convert.ToDateTime(lp[i].Dateofbirth), City = lp[i].City });
            }

            base.RaisePropertyChangedEvent("PersonsList");
        }

        #endregion
    }

    #region Class for work with command

    /// <summary>
    /// Абстрактный класс для обработки принятых команд из MainWindow.
    /// </summary>
    abstract class MainWindowMyCommand : ICommand                  //Возможно надо вынести в Utility
    {
        protected MainWindowViewModel _mainWindowVeiwModel;

        public MainWindowMyCommand(MainWindowViewModel mainWindowVeiwModel)
        {
            _mainWindowVeiwModel = mainWindowVeiwModel;
        }

        public event EventHandler CanExecuteChanged;

        public abstract bool CanExecute(object parameter);

        public abstract void Execute(object parameter);
    }

    /// <summary>
    /// Класс для обработки команды добавления нового пользователя.
    /// </summary>
    class AddPersonWindowCommand : MainWindowMyCommand                  //Возможно надо вынести в Utility
    {
        public AddPersonWindowCommand(MainWindowViewModel mainWindowVeiwModel) : base(mainWindowVeiwModel)
        {
        }
        public override bool CanExecute(object parameter)
        {
            return true;
        }
        public override async void Execute(object parameter)
        {
            var displayRootRegistry = (Application.Current as App).displayRootRegistry;

            var personWindowViewModel = new PersonWindowViewModel();
            await displayRootRegistry.ShowModalPresentation(personWindowViewModel);
        }
    }

    /// <summary>
    /// Класс для обработки команды редактирования существующего пользователя.
    /// </summary>
    class EditPersonWindowCommand : MainWindowMyCommand                 //Возможно надо вынести в Utility
    {
        public EditPersonWindowCommand(MainWindowViewModel mainWindowVeiwModel) : base(mainWindowVeiwModel)
        {
        }
        public override bool CanExecute(object parameter)
        {
            return true;
        }
        public override async void Execute(object parameter)
        {
            var displayRootRegistry = (Application.Current as App).displayRootRegistry;

            if (_mainWindowVeiwModel.SelectedPerson != null)
            {
                var personWindowViewModel = new PersonWindowViewModel(_mainWindowVeiwModel.SelectedPerson);
                await displayRootRegistry.ShowModalPresentation(personWindowViewModel);
            }
            else
            {
                MessageBox.Show("Не выбран ни один пользователь из списка", "Сообщение", MessageBoxButton.OK);
            }
        }
    }

    /// <summary>
    /// Класс для обработки команды удаления существующего пользователя.
    /// </summary>
    class DeleteCommand : MainWindowMyCommand               //Возможно надо вынести в Utility
    {
        public DeleteCommand(MainWindowViewModel mainWindowVeiwModel) : base(mainWindowVeiwModel)
        {
        }
        public override bool CanExecute(object parameter)
        {
            return true;
        }
        public override void Execute(object parameter)
        {
            var displayRootRegistry = (Application.Current as App).displayRootRegistry;
            MainWindowViewModel main = new MainWindowViewModel(_mainWindowVeiwModel.SelectedPerson);
            main.DeletePerson();
        }
    }

    /// <summary>
    /// Класс для обработки команды получения информации о проекте.
    /// </summary>
    class GetAppointmentCommand : MainWindowMyCommand               //Возможно надо вынести в Utility
    {
        public GetAppointmentCommand(MainWindowViewModel mainWindowVeiwModel) : base(mainWindowVeiwModel)
        {
        }
        public override bool CanExecute(object parameter)
        {
            return true;
        }
        public override void Execute(object parameter)
        {
            var displayRootRegistry = (Application.Current as App).displayRootRegistry;

            MessageBox.Show("Этот тестовый проект написан с целью технологии WPF и паттерна для работы MVVM для работы с ним ", "Информация", MessageBoxButton.OK);
        }
    }

    /// <summary>
    /// Класс для обработки команды получения информации о разработчике.
    /// </summary>
    class GetAuthorCommand : MainWindowMyCommand               //Возможно надо вынести в Utility
    {
        public GetAuthorCommand(MainWindowViewModel mainWindowVeiwModel) : base(mainWindowVeiwModel)
        {
        }
        public override bool CanExecute(object parameter)
        {
            return true;
        }
        public override void Execute(object parameter)
        {
            var displayRootRegistry = (Application.Current as App).displayRootRegistry;

            MessageBox.Show("А.В. Печенюк", "Информация", MessageBoxButton.OK);
        }
    }

    #endregion
}