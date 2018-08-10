using WpfClient.Utility;

namespace WpfClient.Model
{
    class City : ViewModelBase
    {
        private int _idcity;
        private string _town;

        public int Idcity
        {
            get { return _idcity; }
            set
            {
                _idcity = value;
                base.RaisePropertyChangedEvent("Idcity");
            }
        }

        public string Town
        {
            get { return _town; }
            set
            {
                _town = value;
                base.RaisePropertyChangedEvent("Town");
            }
        }
    }
}