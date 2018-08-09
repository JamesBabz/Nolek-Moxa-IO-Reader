using Nolek_Moxa_IO_Reader.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Console = System.Console;

namespace Nolek_Moxa_IO_Reader
{
    /// <summary>
    /// Interaction logic for IOReaderHome.xaml
    /// </summary>
    public partial class IOReaderHome : UserControl
    {
        private VmIOReaderHome _model;
        public IOReaderHome()
        {
            DataContext = _model = new VmIOReaderHome();
            InitializeComponent();
            icDiState.ItemsSource = _model.DIs;
            //icDiStateNew.ItemsSource = _model.DIs;
            icRelayState.ItemsSource = _model.Relays;
        }
        private void TestMethod(object sender, RoutedEventArgs e)
        {
            _model.UpdateRelay(1);
           // _model.startDIListenerThread();
        }

        private void RelayButton_OnClick(object sender, RoutedEventArgs e)
        {
            string content = (sender as Button).Content.ToString();
            int index = Int32.Parse(content);
            _model.UpdateRelay(index);
        }
    }
    
}
