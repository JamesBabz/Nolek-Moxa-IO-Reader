using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Nolek_Moxa_IO_Reader.Annotations;

namespace Nolek_Moxa_IO_Reader.Model
{
    public class DigitalInput : INotifyPropertyChanged
    {
        private int _diIndex;
        public int diIndex
        {
            get => _diIndex;
            set
            {
                _diIndex = value;
                NotifyPropertyChanged("diIndex");
            }
        }
        private int _diStatus;
        public int diStatus
        {
            get => _diStatus;
            set
            {
                _diStatus = value;
                NotifyPropertyChanged("diStatus");
                SetEllipseColorBrush();
            }
        }
        private int _diMode;
        public int diMode
        {
            get => _diMode;
            set
            {
                _diMode = value;
                NotifyPropertyChanged("diMode");
            }
        }
        private void SetEllipseColorBrush()
        {
            if (diStatus == 1)
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
