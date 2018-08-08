using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Input;
using WpfClient.Model;
using WpfClient.Utility;
using WpfClient.WorkWithServer;

namespace WpfClient.ViewModel
{
    class PersonWindowViewModel : ViewModelBase
    {
        #region Fields

        private Person _editPerson;
        private ObservableCollection<City> _Cities;
        private City _selectedCity;
        private bool _edit;
        private InteractionServer response;
        private ICommand _okCommand;
        private ICommand _closeCommand;

        #endregion

        #region Constructor

        public PersonWindowViewModel()
        {
            EditPerson = new Person();
            this.GetData();
            FEdit = false;
        }

        public PersonWindowViewModel(Person person)
        {
            _selectedCity = new City();
            EditPerson = person;
            this.GetData();
            _selectedCity.Town = EditPerson.City;
            FEdit = true;
        }

        #endregion

        #region Get/Set fields

        /// <summary>
        /// Признак того изменяем ли мы или добавляем нового.
        /// </summary>
        public bool FEdit
        {
            get { return _edit; }

            set
            {
                _edit = value;
            }
        }

        /// <summary>
        /// Список городов.
        /// </summary>
        public ObservableCollection<City> CitiesList
        {
            get { return _Cities; }

            set
            {
                _Cities = value;
                base.RaisePropertyChangedEvent("CitiesList");
            }
        }

        /// <summary>
        /// Выбранный город из списка.
        /// </summary>
        public City SelectedCity
        {
            get { return _selectedCity; }

            set
            {
                _selectedCity = value;
                base.RaisePropertyChangedEvent("SelectedCity");
            }
        }

        /// <summary>
        /// Редактируемый пользователь.
        /// </summary>
        public Person EditPerson
        {
            get { return _editPerson; }

            set
            {
                _editPerson = value;
                base.RaisePropertyChangedEvent("EditPerson");
            }
        }

        #endregion

        #region Command 

        /// <summary>
        /// Команда на добавление/изменение пользователя
        /// </summary>
        public ICommand OkCommand
        {
            get
            {
                if (_okCommand == null)
                {
                    _okCommand = new AddPersonCommand(this);
                }
                return _okCommand;
            }
        }

        /// <summary>
        /// Команда на закрытие окна.
        /// </summary>
        public ICommand CloseCommand
        {
            get
            {
                if (_closeCommand == null)
                {
                    _closeCommand = new ClosePersonCommand(this);
                }
                return _closeCommand;
            }
        }

        #endregion

        #region Method logic

        /// <summary>
        /// Получения списока городов.
        /// </summary>
        private void GetData()
        {
            _Cities = new ObservableCollection<City>();

            response = new InteractionServer();
            List<City> lp = response.GetCitiesList();
            for (int i = 0; i < lp.Count; i++)
            {
                CitiesList.Add(new City { Idcity = lp[i].Idcity, Town = lp[i].Town });
            }

            base.RaisePropertyChangedEvent("CitiesList");
        }

        /// <summary>
        /// Добавление/изменение пользователя.
        /// </summary>
        public void Add(Person person)
        {
            response = new InteractionServer();
            response.AddNewPerson(person);
        }

        /// <summary>
        /// Добавление/изменение пользователя.
        /// </summary>
        public void Edit(Person person)
        {
            response = new InteractionServer();
            response.EditPerson(person);
        }

        #endregion
    }

    #region Class for work with command

    /// <summary>
    /// Абстрактный класс для обработки принятых команд из PersonWindow.
    /// </summary>
    abstract class PersonWindowMyCommand : ICommand                  //Возможно надо вынести в Utility
    {
        protected PersonWindowViewModel _personWindowViewModel;

        public PersonWindowMyCommand(PersonWindowViewModel personWindowViewModel)
        {
            _personWindowViewModel = personWindowViewModel;
        }

        public event EventHandler CanExecuteChanged;

        public abstract bool CanExecute(object parameter);

        public abstract void Execute(object parameter);
    }

    /// <summary>
    /// Класс для обработки команды добавления нового пользователя.
    /// </summary>
    class AddPersonCommand : PersonWindowMyCommand                  //Возможно надо вынести в Utility
    {
        public AddPersonCommand(PersonWindowViewModel personWindowViewModel) : base(personWindowViewModel)
        {
        }
        public override bool CanExecute(object parameter)
        {
            return true;
        }
        public override async void Execute(object parameter)
        {
            var displayRootRegistry = (Application.Current as App).displayRootRegistry;

            Person person = new Person();
            person.Idperson = _personWindowViewModel.EditPerson.Idperson;
            person.Name = _personWindowViewModel.EditPerson.Name;
            person.Dateofbirth = _personWindowViewModel.EditPerson.Dateofbirth;
            person.City = _personWindowViewModel.SelectedCity.Town;

            if (_personWindowViewModel.FEdit == false)
            {
                _personWindowViewModel.Add(person);
                PersonWindow personWindow = Application.Current.Windows.OfType<PersonWindow>().FirstOrDefault();
                if (personWindow != null)
                    personWindow.Close();
            }
            else
            {
                _personWindowViewModel.Edit(person);
                PersonWindow personWindow = Application.Current.Windows.OfType<PersonWindow>().FirstOrDefault();
                if (personWindow != null)
                    personWindow.Close();
            }
        }
    }

    /// <summary>
    /// Класс для обработки команды закрытия окна PersonWindow.
    /// </summary>
    class ClosePersonCommand : PersonWindowMyCommand                  //Возможно надо вынести в Utility
    {
        public ClosePersonCommand(PersonWindowViewModel personWindowViewModel) : base(personWindowViewModel)
        {
        }
        public override bool CanExecute(object parameter)
        {
            return true;
        }
        public override async void Execute(object parameter)
        {
            var displayRootRegistry = (Application.Current as App).displayRootRegistry;
            PersonWindow personWindow = Application.Current.Windows.OfType<PersonWindow>().FirstOrDefault();
            if (personWindow != null)
                personWindow.Close();
        }
    }

    #endregion
}