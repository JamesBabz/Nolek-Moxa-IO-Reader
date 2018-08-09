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
        private byte DibytCount;
        private byte DibytStartChannel = 0;
        private byte RelaybytCount;
        private byte RelaybytStartChannel = 0;
        Int32 dwShiftValue;
        int i;
        Int32[] hConnection = new Int32[1];

        public ObservableCollection<DigitalInput> DIs { get; set; }
        public ObservableCollection<Relay> Relays { get; set; }
        private SynchronizationContext uiContext;
        public string IpAddress = "192.168.127.254";

        private Stopwatch _timer = Stopwatch.StartNew();

        private string _response;
        public string Response
        {
            get { return _response; }
            set
            {
                _response = value; NotifyPropertyChanged("Response");
                updateUI();
            }
        }


        public VmIOReaderHome()
        {

            uiContext = SynchronizationContext.Current; // For ui update while threading
            DIs = new ObservableCollection<DigitalInput>();
            Relays = new ObservableCollection<Relay>();
            SetupConnection();
            InitialGUISetup();
            startDIListenerThread();

        }

        /// <summary>
        /// Sets up the different gui elements on startup
        /// </summary>
        private void InitialGUISetup()
        {
            DISetup();
            RelaySetup();
        }

        /// <summary>
        /// Starts the connection to the IO Server
        /// </summary>
        private void SetupConnection()
        {
            ret = MXIO_CS.MXEIO_Init();
            ret = MXIO_CS.MXEIO_E1K_Connect(System.Text.Encoding.UTF8.GetBytes(IpAddress), Port, Timeout, hConnection,
                System.Text.Encoding.UTF8.GetBytes(Password));
        }

        /// <summary>
        /// Sets up all the Digital input related gui elements
        /// </summary>
        private void DISetup()
        {
            ret = MXIO_CS.E1K_DI_Reads(hConnection[0], DibytStartChannel, DibytCount, new UInt32[1]);
            DibytCount = 0;
            while (ret != 4004)
            {
                DibytCount++;
                ret = MXIO_CS.E1K_DI_Reads(hConnection[0], DibytStartChannel, DibytCount, new UInt32[1]);
            }
            DibytCount--;
            Response = "method=create,type=di,amount=" + DibytCount;
        }

        /// <summary>
        /// Sets up all the Relay related gui elements
        /// </summary>
        private void RelaySetup()
        {
            RelaybytCount = 0;
            ret = MXIO_CS.E1K_DO_Reads(hConnection[0], RelaybytStartChannel, RelaybytCount, new UInt32[1]);
            while (ret != 4004)
            {
                RelaybytCount++;
                ret = MXIO_CS.E1K_DO_Reads(hConnection[0], RelaybytStartChannel, RelaybytCount, new UInt32[1]);
            }
            RelaybytCount--;
            Response = "method=create,type=relay,amount=" + RelaybytCount;
        }

        /// <summary>
        /// Updates the UI depending on the response recieved from anywhere in the class
        /// </summary>
        private void updateUI()
        {
            string[] data = Response.Split(',');
            string[] method = data[0].Split('=');
            string[] type;
            string[] amount;
            string[] objectID;
            string[] propertyName;
            string[] propertyValue;

            switch (method[1]) // What method should be executed
            {
                case "create":
                    type = data[1].Split('=');
                    amount = data[2].Split('=');
                    CreateObjects(type[1], int.Parse(amount[1])); // Create an object of the specified type with the specified parameters
                    break;
                case "update":
                    type = data[1].Split('=');
                    objectID = data[2].Split('=');
                    propertyName = data[3].Split('=');
                    propertyValue = data[4].Split('=');
                    UpdateObject(type[1], int.Parse(objectID[1]), propertyName[1], propertyValue[1]); // Updates a property of an object of the specified type with the specified parameters
                    break;

            }


        }

        /// <summary>
        /// Updates a single object with the information given
        /// </summary>
        /// <param name="type">What type of object is it (eg. di or relay)</param>
        /// <param name="id">The id of the object</param>
        /// <param name="propName">The name of the property to update</param>
        /// <param name="propVal">The value to give to the property</param>
        private void UpdateObject(string type, int id, string propName, string propVal)
        {
            if (type.Equals("di"))
            {
                DigitalInput currDi = DIs[id];
                switch (propName)
                {
                    case "diStatus":
                        currDi.diStatus = Convert.ToBoolean(propVal);
                        break;
                    case "diMode":
                        currDi.diMode = Convert.ToInt32(propVal);
                        break;

                }
            }
            else if (type.Equals("relay"))
            {
                Relay currRelay = Relays[id];
                switch (propName)
                {
                    case "relayStatus":
                        currRelay.relayStatus = Convert.ToBoolean(propVal);
                        break;
                    case "relayMode":
                        currRelay.relayMode = Convert.ToInt32(propVal);
                        break;
                    case "relayCurrentCount":
                        currRelay.relayCurrentCount = Convert.ToInt64(propVal);
                        break;
                    case "relayTotalCount":
                        currRelay.relayTotalCount = Convert.ToInt64(propVal);
                        break;
                    case "relayCurrentCountReset":
                        currRelay.relayCurrentCountReset = Convert.ToInt32(propVal);
                        break;
                }
            }
        }

        /// <summary>
        /// Creates multiple object of a specified type
        /// </summary>
        /// <param name="type">The type of object to create (eg. di or relay)</param>
        /// <param name="amount">How many to create</param>
        private void CreateObjects(string type, int amount)
        {
            for (int j = 0; j < amount; j++)
            {
                if (type.Equals("di"))
                {
                    DIs.Insert(j, new DigitalInput()
                    {
                        diIndex = j,
                        diMode = 0,
                        diStatus = false
                    });
                }
                else if (type.Equals(("relay")))
                {
                    Relays.Insert(j, new Relay()
                    {
                        relayIndex = j,
                        relayMode = 0,
                        relayStatus = GetRelayStatus(j),
                        relayCurrentCount = 0,
                        relayCurrentCountReset = 0,
                        relayTotalCount = 0
                    });
                }
            }
        }

        /// <summary>
        /// Gets the current status of the relay. Used at startup to prevent accidental shutdowns
        /// </summary>
        /// <param name="index">the index of the relay</param>
        /// <returns>The current state of the relay</returns>
        private bool GetRelayStatus(int index)
        {
            RelaybytCount = 1;
            RelaybytStartChannel = (byte)(index);
            UInt32[] dwGetDOValue = new UInt32[1];
            ret = MXIO_CS.E1K_DO_Reads(hConnection[0], RelaybytStartChannel, RelaybytCount, dwGetDOValue);
            if (ret == MXIO_CS.MXIO_OK)
            {
                return dwGetDOValue[0] == 1;
            }
            return false;
        }

        /// <summary>
        /// Called on a click
        /// </summary>
        /// <param name="index">The index of the button pressed</param>
        public void UpdateRelay(int index)
        {
            SetRelay(index);
        }

        /// <summary>
        /// Sets the status of a single relay
        /// </summary>
        /// <param name="index">index of the relay</param>
        private void SetRelay(int index)
        {
            RelaybytCount = 1;
            RelaybytStartChannel = (byte)(index);
            UInt32 dwSetDOValue = (uint)(Relays[index].relayStatus ? 0 : 1);
            ret = MXIO_CS.E1K_DO_Writes(hConnection[0], RelaybytStartChannel, RelaybytCount, dwSetDOValue);
            if (ret != MXIO_CS.MXIO_OK)
            {
                Console.WriteLine("ERROR: " + ret);
            }
            bool isOn = dwSetDOValue != 0;
            Response = "method=update,type=relay,id=" + index + ",field=relayStatus,value=" + isOn;
        }

        /// <summary>
        /// Creates the listener thread for the DIs
        /// </summary>
        public void startDIListenerThread()
        {
            Thread t = new Thread(DiListenerThread) { IsBackground = true };
            t.Start();
        }

        /// <summary>
        /// A listener for when anything changes about the Digital inputs
        /// </summary>
        private void DiListenerThread()
        {
            bool isFirstRun = true;
            DibytStartChannel = 0;
            UInt32[] dwGetDIValue = new UInt32[1];
            UInt32[] dwOldValue = new UInt32[1];
            dwOldValue[0] = 0;
            while (true)
            {
                ret = MXIO_CS.E1K_DI_Reads(hConnection[0], DibytStartChannel, DibytCount, dwGetDIValue);
                if (ret == MXIO_CS.MXIO_OK)
                {
                    if (dwOldValue[0] != dwGetDIValue[0] || isFirstRun)
                    {
                        dwOldValue[0] = dwGetDIValue[0];
                        for (i = 0, dwShiftValue = 0; i < DibytCount; i++, dwShiftValue++)
                        {
                            bool isOn = (dwGetDIValue[0] & (1 << dwShiftValue)) != 0;
                            uiContext.Send(x => Response = "method=update,type=di,id=" + i + ",field=diStatus,value=" + isOn, null);
                        }
                    }
                }
                Thread.Sleep(100);
                isFirstRun = false;
            }
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