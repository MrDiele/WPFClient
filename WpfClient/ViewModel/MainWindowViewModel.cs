using Comands;
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

        private string filterName;
        private string filterTown;
        private DateTime filterDate;

        private Person selectedPerson;
        private ObservableCollection<Person> personsList;
        private int personCount;

        private ICommand applyFiltersCommand;
        private ICommand resetFiltersCommand;
        private ICommand openWindowAddPersonCommand;
        private ICommand openWindowEditPersonCommand;
        private ICommand getAppointmentCommand;
        private ICommand getInformationAboutAuthorCommand;
        private ICommand deletePersonCommand;
        private ICommand exitFromApp;

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
        /// Выбранная Ф.И.О. для фильтра.
        /// </summary>
        public string FilterName
        {
            get { return filterName; }
            set
            {
                filterName = value;
                base.RaisePropertyChangedEvent("FilterName");
            }
        }

        /// <summary>
        /// Выбранный город для фильтра.
        /// </summary>
        public string FilterTown
        {
            get { return filterTown; }
            set
            {
                filterTown = value;
                base.RaisePropertyChangedEvent("FilterTown");
            }
        }

        /// <summary>
        /// Выбранная дата рождения для фильтра.
        /// </summary>
        public DateTime FilterDate
        {
            get { return filterDate; }
            set
            {
                filterDate = value;
                base.RaisePropertyChangedEvent("FilterDate");
            }
        }

        /// <summary>
        /// Список людей.
        /// </summary>
        public ObservableCollection<Person> PersonsList
        {
            get { return personsList; }

            set
            {
                personsList = value;
                base.RaisePropertyChangedEvent("PersonsList");
            }
        }

        /// <summary>
        /// Выбранный человек из списка.
        /// </summary>
        public Person SelectedPerson
        {
            get { return selectedPerson; }

            set
            {
                selectedPerson = value;
                base.RaisePropertyChangedEvent("SelectedPerson");
            }
        }

        /// <summary>
        /// Колличество людей в списке.
        /// </summary>
        public int PersonCount
        {
            get { return personCount; }

            set
            {
                personCount = value;
                base.RaisePropertyChangedEvent("PersonCount");
            }
        }

        #endregion

        #region Command

        /// <summary>
        /// Команда применения фильтра.
        /// </summary>
        public ICommand ApllyFiltersCommand
        {
            get
            {
                return applyFiltersCommand ?? (applyFiltersCommand = new RelayCommand((o) => { GetFilterData(); }));
            }
        }

        /// <summary>
        /// Команда сброса фильтра.
        /// </summary>
        public ICommand ResetFiltersCommand                                                                          // проперти как переменная
        {                                                                                                                                      // с маленькой буквы поправить
            get
            {
                return resetFiltersCommand ?? (resetFiltersCommand = new RelayCommand((o) => { GetData(); }));
            }
        }

        /// <summary>
        /// Команда на добавление нового пользователя.
        /// </summary>
        public ICommand OpenWindowAddPersonCommand
        {
            get
            {
                return openWindowAddPersonCommand ?? (openWindowAddPersonCommand = new RelayCommand(async (o) =>
                {
                    var displayRootRegistry = (Application.Current as App).displayRootRegistry;

                    var personWindowViewModel = new PersonWindowViewModel();

                    await displayRootRegistry.ShowModalPresentation(personWindowViewModel);
                    GetData();
                }));
            }
        }

        /// <summary>
        /// Команда на редактирование пользователя.
        /// </summary>
        public ICommand OpenWindowEditPersonCommand
        {
            get
            {
                return openWindowEditPersonCommand ?? (openWindowEditPersonCommand = new RelayCommand(async (o) =>
                {
                    var displayRootRegistry = (Application.Current as App).displayRootRegistry;

                    if (SelectedPerson != null)
                    {
                        var personWindowViewModel = new PersonWindowViewModel(SelectedPerson);
                        await displayRootRegistry.ShowModalPresentation(personWindowViewModel);
                    }
                    else
                    {
                        MessageBox.Show("Не выбран ни один пользователь из списка", "Сообщение", MessageBoxButton.OK);
                    }
                }));
            }
        }

        /// <summary>
        /// Команда на удаление.
        /// </summary>
        public ICommand DeletePersonCommand
        {
            get
            {
                return deletePersonCommand ?? (deletePersonCommand = new RelayCommand((o) =>
                {
                    MainWindowViewModel main = new MainWindowViewModel(SelectedPerson);
                    main.DeletePerson();
                }));
            }
        }

        /// <summary>
        /// Команда на получение информации о проекте.
        /// </summary>
        public ICommand GetAppointmentCommand
        {
            get
            {
                return getAppointmentCommand ?? (getAppointmentCommand = new RelayCommand((o) => { MessageBox.Show("Этот тестовый проект написан с целью технологии WPF и паттерна для работы MVVM для работы с ним ", "Информация", MessageBoxButton.OK); }));
            }
        }

        /// <summary>
        /// Команда на получение информации об авторе.
        /// </summary>
        public ICommand GetInformationAboutAuthorCommand
        {
            get
            {
                return getInformationAboutAuthorCommand ?? (getInformationAboutAuthorCommand = new RelayCommand((o) => { MessageBox.Show("А.В. Печенюк", "Информация", MessageBoxButton.OK); }));
            }
        }

        /// <summary>
        /// Команда закрытия программы.
        /// </summary>
        public ICommand ExitFromApp
        {
            get
            {
                return exitFromApp ?? (exitFromApp = new RelayCommand((o) => { Application.Current.MainWindow.Close(); }));
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
                var dialogResult = MessageBox.Show("Вы действительно хотите удалить пользователя " + selectedPerson.Name + "?", "Сообщение", MessageBoxButton.YesNo);
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
        private void OnGroceryListChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.personCount = this.PersonsList.Count;
        }
         
        /// <summary>
        /// Метод для заполнения списка пользователей.
        /// </summary>
        public void GetData()
        {
            personsList = new ObservableCollection<Person>();

            personsList.CollectionChanged += OnGroceryListChanged;

            InteractionServer response = new InteractionServer();
            List<Person> lp = response.GetPersonList();
            for (int i = 0; i < lp.Count; i++)
            {
                PersonsList.Add(new Person { Idperson = lp[i].Idperson, Name = lp[i].Name, Dateofbirth = Convert.ToDateTime(lp[i].Dateofbirth), City = lp[i].City });
            }

            base.RaisePropertyChangedEvent("PersonsList");
        }

        /// <summary>
        /// Метод заполнения списка пользователей с учётом фильтра.
        /// </summary>
        public void GetFilterData()
        {
            if (FilterName != null || FilterDate.Date != Convert.ToDateTime("01.01.0001 0:00:00") || FilterTown != null)    //поставить вместо дата == нул стандартную дату
            {
                personsList.CollectionChanged += OnGroceryListChanged;

                InteractionServer response = new InteractionServer();
                List<Person> fp = response.FilterPerson(FilterName, FilterDate, FilterTown);
                PersonsList.Clear();
                for (int i = 0; i < fp.Count; i++)
                {
                    PersonsList.Add(new Person { Idperson = fp[i].Idperson, Name = fp[i].Name, Dateofbirth = Convert.ToDateTime(fp[i].Dateofbirth), City = fp[i].City });
                }
                base.RaisePropertyChangedEvent("PersonsList");
            }
            else
            {
                MessageBox.Show("Выберите поля для фильтрации", "Сообщение", MessageBoxButton.OK);
            }
        }
        #endregion
    }
}