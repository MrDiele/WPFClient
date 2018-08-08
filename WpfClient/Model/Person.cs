using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WpfClient.Utility;

namespace WpfClient.Model
{
    class Person : ViewModelBase
    {
        private int _idperson;
        private string _name;
        private DateTime _dateofbirth;
        private string _city;


        public int Idperson
        {
            get { return _idperson; }
            set
            {
                _idperson = value;
                base.RaisePropertyChangedEvent("Idperson");
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                base.RaisePropertyChangedEvent("Name");
            }
        }

        public DateTime Dateofbirth
        {
            get { return _dateofbirth; }
            set
            {
                _dateofbirth = value;
                base.RaisePropertyChangedEvent("Dateofbirth");
            }
        }

        public string City
        {
            get
            {
                return _city;
            }
            set
            {
                _city = value;
                base.RaisePropertyChangedEvent("City");
            }
        }
    }  
}