using Comands;
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

        private Person editPerson;
        private ObservableCollection<City> citiesList;
        private City selectedCity;
        private bool edit;
        private InteractionServer response;
        private ICommand okCommand;
        private ICommand closeCommand;

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
            selectedCity = new City();
            EditPerson = person;
            this.GetData();
            selectedCity.Town = EditPerson.City;
            FEdit = true;
        }

        #endregion

        #region Get/Set fields

        /// <summary>
        /// Признак того изменяем ли мы или добавляем нового.
        /// </summary>
        public bool FEdit
        {
            get { return edit; }

            set
            {
                edit = value;
            }
        }

        /// <summary>
        /// Список городов.
        /// </summary>
        public ObservableCollection<City> CitiesList
        {
            get { return citiesList; }

            set
            {
                citiesList = value;
                base.RaisePropertyChangedEvent("CitiesList");
            }
        }

        /// <summary>
        /// Выбранный город из списка.
        /// </summary>
        public City SelectedCity
        {
            get { return selectedCity; }

            set
            {
                selectedCity = value;
                base.RaisePropertyChangedEvent("SelectedCity");
            }
        }

        /// <summary>
        /// Редактируемый пользователь.
        /// </summary>
        public Person EditPerson
        {
            get { return editPerson; }

            set
            {
                editPerson = value;
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
                return okCommand ?? (okCommand = new RelayCommand((o) => {
                    var displayRootRegistry = (Application.Current as App).displayRootRegistry;

                    Person person = new Person();
                    person.Idperson = EditPerson.Idperson;
                    person.Name = EditPerson.Name;
                    person.Dateofbirth = EditPerson.Dateofbirth;
                    person.City = SelectedCity.Town;

                    if (FEdit == false)
                    {
                        Add(person);
                        PersonWindow personWindow = Application.Current.Windows.OfType<PersonWindow>().FirstOrDefault();
                        if (personWindow != null)
                            personWindow.Close();
                    }
                    else
                    {
                        Edit(person);
                        PersonWindow personWindow = Application.Current.Windows.OfType<PersonWindow>().FirstOrDefault();
                        if (personWindow != null)
                            personWindow.Close();
                    }
                }));
            }
        }

        /// <summary>
        /// Команда на закрытие окна.
        /// </summary>
        public ICommand CloseCommand
        {
            get
            {
                return closeCommand ?? (closeCommand = new RelayCommand((o) => {
                    var displayRootRegistry = (Application.Current as App).displayRootRegistry;
                    PersonWindow personWindow = Application.Current.Windows.OfType<PersonWindow>().FirstOrDefault();
                    if (personWindow != null)
                        personWindow.Close();
                }));
            }
        }

        #endregion

        #region Method logic

        /// <summary>
        /// Получения списока городов.
        /// </summary>
        private void GetData()
        {
            citiesList = new ObservableCollection<City>();

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
}