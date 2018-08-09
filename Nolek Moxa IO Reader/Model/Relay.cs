using System.ComponentModel;
using System.Windows.Media;

namespace Nolek_Moxa_IO_Reader.Model
{
    public class Relay : INotifyPropertyChanged
    {
        //"relayCurrentCountReset": 0

        private int _relayIndex;
        public int relayIndex
        {
            get => _relayIndex;
            set
            {
                _relayIndex = value;
                NotifyPropertyChanged("relayIndex");
            }
        }
        private int _relayMode;
        public int relayMode
        {
            get => _relayMode;
            set
            {
                _relayMode = value;
                NotifyPropertyChanged("relayMode");
            }
        }
        private bool _relayStatus;
        public bool relayStatus
        {
            get => _relayStatus;
            set
            {
                _relayStatus = value;
                NotifyPropertyChanged("relayStatus");
                SetButtonColor();
            }
        }
        private long _relayTotalCount;
        public long relayTotalCount
        {
            get => _relayTotalCount;
            set
            {
                _relayTotalCount = value;
                NotifyPropertyChanged("relayTotalCount");
            }
        }
        private long _relayCurrentCount;
        public long relayCurrentCount
        {
            get => _relayCurrentCount;
            set
            {
                _relayCurrentCount = value;
                NotifyPropertyChanged("diMode");
            }
        }
        private int _relayCurrentCountReset;
        public int relayCurrentCountReset
        {
            get => _relayCurrentCountReset;
            set
            {
                _relayCurrentCountReset = value;
                NotifyPropertyChanged("relayCurrentCountReset");
            }
        }

        private void SetButtonColor()
        {
            if (relayStatus)
            {
                EllipseColor = new SolidColorBrush(Colors.ForestGreen);
            }
            else
            {
                EllipseColor = new SolidColorBrush(Colors.Red);
            }
        }

        private SolidColorBrush _ellipseColor;
        public SolidColorBrush EllipseColor
        {
            get { return _ellipseColor; }
            set { _ellipseColor = value; NotifyPropertyChanged("EllipseColor"); }
        }

        #region INotify
        /// <summary>
        /// PropertyChanged
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Notify property changed
        /// </summary>
        /// <param name="prop">property name</param>
        public void NotifyPropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        #endregion
    }
}