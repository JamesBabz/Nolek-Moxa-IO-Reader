using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.OleDb;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Nolek_Moxa_IO_Reader.Annotations;
using Nolek_Moxa_IO_Reader.Lib;
using Nolek_Moxa_IO_Reader.Model;

namespace Nolek_Moxa_IO_Reader.ViewModel
{
    public class VmIOReaderHome : INotifyPropertyChanged
    {
        private int ret;
        public const UInt16 Port = 502;
        UInt32 Timeout = 1000;
        public string Password = "";
        byte bytCount = 0, bytStartChannel = 0;
        Int32 dwShiftValue;
        UInt32 i;
        Int32[] hConnection = new Int32[1];

        public ObservableCollection<DigitalInput> DIs { get; set; }
        public ObservableCollection<Relay> Relays { get; set; }
        private HttpClient _httpClient;
        private SynchronizationContext uiContext;
        public string HttpAddress = "http://192.168.127.254";
        public string IpAddress = "192.168.127.254";
        private string _apiString;

        private Stopwatch _timer = Stopwatch.StartNew();

        private string _response;
        public string Response
        {
            get { return _response; }
            set
            {
                _response = value; NotifyPropertyChanged("Response");
                SetIOData();
            }
        }

        public VmIOReaderHome()
        {
            uiContext = SynchronizationContext.Current;
            DIs = new ObservableCollection<DigitalInput>();
            Relays = new ObservableCollection<Relay>();
            SetupHTTPClient();
            ret = MXIO_CS.MXEIO_Init();
            ret = MXIO_CS.MXEIO_E1K_Connect(System.Text.Encoding.UTF8.GetBytes(IpAddress), Port, Timeout, hConnection, System.Text.Encoding.UTF8.GetBytes(Password));
            testDelay();
            DIs.Insert(0, new DigitalInput()
            {
                diIndex = 0,
                diMode = 0,
                diStatus = 0
            });
            DIs.Insert(1, new DigitalInput()
            {
                diIndex = 0,
                diMode = 0,
                diStatus = 0
            });DIs.Insert(0, new DigitalInput()
            {
                diIndex = 0,
                diMode = 0,
                diStatus = 0
            });
            DIs.Insert(1, new DigitalInput()
            {
                diIndex = 1,
                diMode = 0,
                diStatus = 0
            });
            Relays.Insert(0, new Relay()
            {
                relayIndex = 0,
                relayStatus = 0,
                relayCurrentCount = 0,
                relayCurrentCountReset = 0,
                relayMode = 0,
                relayTotalCount = 0

            });
            Relays.Insert(1, new Relay()
            {
                relayIndex = 1,
                relayStatus = 0,
                relayCurrentCount = 0,
                relayCurrentCountReset = 0,
                relayMode = 0,
                relayTotalCount = 0

            });
            //StartDIListenerThread();
        }

        /// <summary>
        /// Sets up the HttpClient with the required headers
        /// </summary>
        private void SetupHTTPClient()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Connection.Clear();
            _httpClient.DefaultRequestHeaders.ConnectionClose = false;
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/vdn.dac.v1"));
            _apiString = HttpAddress + "/api/slot/0/io";
            _httpClient.BaseAddress = new Uri(_apiString);
        }

        private void StartDIListenerThread()
        {
            Thread t = new Thread(ApiListener);
            t.IsBackground = true;
            t.Start();
        }

        private void ApiListener()
        {
            string oldDiResp = "";
            string oldRelayResp = "";
            while (true)
            {

                try
                {
                    string newDiResp = _httpClient.GetStringAsync(_apiString + "/di").Result;

                    if (oldDiResp != newDiResp)
                    {
                        oldDiResp = newDiResp;
                        uiContext.Send(x => Response = newDiResp, null);
                        Console.WriteLine("Old register: " + _timer.Elapsed);
                        continue;
                    }
                    string newRelayResp = _httpClient.GetStringAsync(_apiString + "/relay").Result;
                    if (oldRelayResp != newRelayResp)
                    {
                        oldRelayResp = newRelayResp;
                        uiContext.Send(x => Response = newRelayResp, null);
                        continue;
                    }
                    Thread.Sleep(100);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        private void SetIOData()
        {
            Response r = JsonConvert.DeserializeObject<Response>(Response);
            if (r.io.di != null)
            {
                DIs.Clear();
                foreach (DigitalInput di in r.io.di)
                {
                    DIs.Insert(di.diIndex, di);
                }
            }
            if (r.io.relay != null)
            {
                Relays.Clear();
                foreach (Relay relay in r.io.relay)
                {
                    Relays.Insert(relay.relayIndex, relay);
                }
            }

        }

        public void UpdateRelay(int index)
        {
            //OldMethod(index);
            NewMethod(index);
            
        }

        private void NewMethod(int index)
        {
            bytCount = 1;
            bytStartChannel = (byte) (0+index);
            Console.WriteLine("Start new update method");
            TimeSpan startTime = _timer.Elapsed;
            //Set Ch{0}~ch{1} DO Direction DO Mode value = ON
            UInt32 dwSetDOValue = (uint) (Relays[index].relayStatus == 0 ? 1 : 0);
            Console.WriteLine(dwSetDOValue);
            ret = MXIO_CS.E1K_DO_Writes(hConnection[0], bytStartChannel, bytCount, dwSetDOValue);
            Console.WriteLine(ret);
            if (ret == MXIO_CS.MXIO_OK)
            {
            }
            Console.WriteLine("End new update method");
            TimeSpan elapsedTime = _timer.Elapsed - startTime;
            Console.WriteLine("Elapsed time NEW: " + elapsedTime);

        }

        private void OldMethod(int index)
        {
            Console.WriteLine("Start old update method");
            TimeSpan startTime = _timer.Elapsed;
            foreach (Relay relay in Relays)
            {
                if (relay.relayIndex == index)
                {
                    relay.relayStatus = relay.relayStatus == 0 ? relay.relayStatus = 1 : relay.relayStatus = 0;
                }
            }
            Dictionary<string, object> relayList = new Dictionary<string, object>(){
                {"slot", "0"},
                {"io", new Dictionary<string,Object>()
                    {
                        {"relay", Relays}
                    }
                }
            };
            var rel = JsonConvert.SerializeObject(relayList);
            var buffer = System.Text.Encoding.UTF8.GetBytes(rel);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = _httpClient.PutAsync(_apiString + "/relay", byteContent).Result;
            Console.WriteLine("End old update method");
            TimeSpan elapsedTime = _timer.Elapsed - startTime;
            Console.WriteLine("Elapsed time OLD: " + elapsedTime);
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

        public void testDelay()
        {
            Console.WriteLine(ret);
            Thread t = new Thread(testThread) { IsBackground = true };
            t.Start();
            //MXIO_CS.MXEIO_Disconnect(hConnection[0]);
            //Console.WriteLine("Button pressed: " + _timer.Elapsed);
            //UpdateRelay(5);
        }

        private void testThread()
        {
            bytCount = 5;
            bytStartChannel = 0;
            UInt32[] dwGetDIValue = new UInt32[1];
            UInt32[] dwOldValue = new UInt32[1];
            dwOldValue[0] = 0;
            bool[] isOn = new bool[bytCount];
            while (true)
            {
                ret = MXIO_CS.E1K_DI_Reads(hConnection[0], bytStartChannel, bytCount, dwGetDIValue);
                if (ret == MXIO_CS.MXIO_OK)
                {
                    if (dwOldValue[0] != dwGetDIValue[0])
                    {
                        Console.WriteLine(ret);
                        dwOldValue[0] = dwGetDIValue[0];
                        for (i = 0, dwShiftValue = 0; i < bytCount; i++, dwShiftValue++)
                        {
                            isOn[i] = (dwGetDIValue[0] & (1 << dwShiftValue)) != 0;
                            //TODO handle the result (change DIs value to bool?)
                        }
                        Console.WriteLine("New register: " + _timer.Elapsed);
                    }
                }
                Thread.Sleep(100);
            }
        }
    }
}