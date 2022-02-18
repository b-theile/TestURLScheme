using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace TestURLScheme.ViewModels
{
    public class SingleStartViewModel : BaseModelAndViewModel
    {

        public SingleStartViewModel()
        {
            ReqMeasurements = new ObservableCollection<string>(Enum.GetNames(typeof(ReqMeasurement)).ToList());

            SelectedFuelId = Enum.GetName(typeof(FuelId), FuelId.FUEL_NAT_GAS);
            //_selectedReqMeasurement = Enum.GetName(typeof(ReqMeasurement), ReqMeasurement.REQ_MEAS_BIMSCHV);
            SelectedReqMeasurements = new ObservableCollection<object>(new[] { ReqMeasurements[0] });


            XMLData = XmlSingleStartData;
            MessagingCenter.Subscribe<string>(this, "mru4uresponse", (response) => XMLData = WebUtility.UrlDecode(response));
            MessagingCenter.Subscribe<string>(this, "fileresponse", response => XMLData = response);
        }

        /// <summary>
        /// localhost address
        /// </summary>
        private const string _ipAddress = "127.0.0.1";
        /// <summary>
        /// Tcp port of MRU4u app
        /// </summary>
        private const int _port = 21113;
        /// <summary>
        /// Xml data string
        /// </summary>
        private string XmlSingleStartData =>
        $"{_id}" +
            $"<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
            $"<SingleStart xmlns=\"urn:ZIV-schema\" TransmissionId=\"2016-10-13\">" +
                $"<Version>3.00</Version>" +
                $"<FuelId>{_selectedFuelId}</FuelId>" +
                $"<RequiredMeasurements>{_reqMeasurements}</RequiredMeasurements>" +
            $"</SingleStart>";

        string _reqMeasurements = Enum.GetName(typeof(ReqMeasurement), ReqMeasurement.REQ_MEAS_BIMSCHV);

        /// <summary>
        /// xml id
        /// </summary>
        private readonly string _id = "-O";

        /// <summary>
        /// Available fuel IDs
        /// </summary>
        private enum FuelId
        {
            FUEL_LIGHT_OIL,
            FUEL_NAT_GAS,
            FUEL_CITY_GAS,
            FUEL_COKE_GAS,
            FUEL_PROPANE,
            FUEL_BUTANE,
            FUEL_HEAVY_OIL,
            FUEL_BROWN_COAL,
            FUEL_HARD_COAL,
            FUEL_PELLETS,
            FUEL_RAPE_OIL,
            FUEL_LEAN_COAL,
            FUEL_WOOD,
            FUEL_BRIQUETTES,
            FUEL_WOOD_CHIPS,
            FUEL_WOOD_SHAVINGS,
            FUEL_GRAIN,
            FUEL_BIO_GAS,
            FUEL_OTHER,
            /// <summary>
            /// MRU FUEL
            /// </summary>
            FUEL_DIESEL,
            /// <summary>
            /// MRU FUEL
            /// </summary>
            FUEL_COAL,
            /// <summary>
            /// MRU FUEL
            /// </summary>
            FUEL_KEROSINE,
            /// <summary>
            /// MRU FUEL
            /// </summary>
            FUEL_PETROL,
            /// <summary>
            /// MRU FUEL
            /// </summary>
            FUEL_LPG,
        };

        /// <summary>
        /// Supported measurements
        /// </summary>
        private enum ReqMeasurement
        {
            REQ_MEAS_BIMSCHV,
            REQ_MEAS_EXHAUST,
            REQ_MEAS_RING,
            REQ_MEAS_44BIMSCHV
        };

        #region Properties


        public List<string> FuelIds => Enum.GetNames(typeof(FuelId)).ToList();

        private string _selectedFuelId;
        /// <summary>
        /// Selected Fuel
        /// </summary>
        public string SelectedFuelId
        {
            get => _selectedFuelId;
            set
            {
                if (SetProperty(ref _selectedFuelId, value))
                    XMLData = XmlSingleStartData;
            }
        }

        public ObservableCollection<string> ReqMeasurements { get; set; }

        //private string _selectedReqMeasurement;
        //public string SelectedReqMeasurement
        //{
        //    get => _selectedReqMeasurement;
        //    set
        //    {
        //        if (SetProperty(ref _selectedReqMeasurement, value))
        //        {
        //            _reqMeasurements = _selectedReqMeasurement;
        //            XMLData = XmlSingleStartData;
        //        }
        //    }
        //}

        ObservableCollection<object> _selectedReqMeasurements;
        public ObservableCollection<object> SelectedReqMeasurements
        {
            get => _selectedReqMeasurements;
            set
            {
                if (SetProperty(ref _selectedReqMeasurements, value))
                {
                    if (_selectedReqMeasurements != null)
                    {
                        _reqMeasurements = string.Join(" ", _selectedReqMeasurements.ToArray());

                        XMLData = XmlSingleStartData;
                    }
                }
            }
        }

        




        private string _xmlData;
        public string XMLData
        {
            get => _xmlData;
            set => SetProperty(ref _xmlData, value);
        }

        #endregion Properties

        #region Commands

        /// <summary>
        /// Send xml request via url (mru4urequest)
        /// </summary>
        public ICommand StartTestURLCommand => new Command(
            execute: () =>
            {
                string urlString = $"mru4urequest://mru/{WebUtility.UrlEncode(XmlSingleStartData)}";
                MessagingCenter.Send<string>(urlString, "mru4urequest");
            });

        /// <summary>
        /// Send xml request via file stream
        /// </summary>
        public ICommand StartTestFileExchangeCommand => new Command(
            execute: () =>
            {
                var index = XmlSingleStartData.IndexOf("<?xml");
                string urlString = XmlSingleStartData.Substring(index, XmlSingleStartData.Length - index);
                MessagingCenter.Send<string>(urlString, "filerequest");
            });

        /// <summary>
        /// Send xml request via tcp
        /// </summary>
        public ICommand SendTcpRequestCommand => new Command(
            execute: () =>
            {
                try
                {
                    // Connect to localhost
                    using Socket socket = new(SocketType.Stream, ProtocolType.Tcp);
                    socket.Connect(IPAddress.Parse(_ipAddress), _port);
                    // Set data
                    var data = WebUtility.UrlEncode(XmlSingleStartData);
                    socket.Send(Encoding.ASCII.GetBytes(data));
                    // Get data
                    byte[] buffer = new byte[4096];
                    var count = socket.Receive(buffer);
                    var rawData = Encoding.UTF8.GetString(buffer, 0, count);
                    XMLData = WebUtility.UrlDecode(rawData);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message, "SendTcpRequestCommand");
                }
            });

        #endregion Commands
    }
}
