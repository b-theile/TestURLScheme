using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Windows.Input;
using Xamarin.Forms;

namespace TestURLScheme
{
    public class MainViewModel : BaseModelAndViewModel
    {
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
        private string _xmlSendData =>
        $"{_id}" +
            $"<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
            $"<SingleStart xmlns=\"urn:ZIV-schema\" TransmissionId=\"2016-10-13\">" +
                $"<Version>2.00</Version>" +
                $"<FuelId>{_selectedFuelId}</FuelId>" +
                $"<RequiredMeasurements>{_selectedReqMeasurements}</RequiredMeasurements>" +
            $"</SingleStart>";

        /// <summary>
        /// xml id
        /// </summary>
        private string _id = "42";

        /// <summary>
        /// Available fuel IDs
        /// </summary>
        private string[] _fuelIds = new[]
        {
            "FUEL_LIGHT_OIL",
            "FUEL_NAT_GAS",
            "FUEL_CITY_GAS",
            "FUEL_COKE_GAS",
            "FUEL_PROPANE",
            "FUEL_BUTANE",
            "FUEL_HEAVY_OIL",
            "FUEL_BROWN_COAL",
            "FUEL_HARD_COAL",
            "FUEL_PELLETS",
            "FUEL_RAPE_OIL",
            "FUEL_LEAN_COAL",
            "FUEL_WOOD",
            "FUEL_BRIQUETTES",
            "FUEL_WOOD_CHIPS",
            "FUEL_WOOD_SHAVINGS",
            "FUEL_GRAIN",
            "FUEL_BIO_GAS",
            "FUEL_OTHER",
            /// <summary>
            /// MRU FUEL
            /// </summary>
            "FUEL_DIESEL",
            /// <summary>
            /// MRU FUEL
            /// </summary>
            "FUEL_COAL",
            /// <summary>
            /// MRU FUEL
            /// </summary>
            "FUEL_KEROSINE",
            /// <summary>
            /// MRU FUEL
            /// </summary>
            "FUEL_PETROL",
            /// <summary>
            /// MRU FUEL
            /// </summary>
            "FUEL_LPG",
        };

        /// <summary>
        /// Supported measurements
        /// </summary>
        private string[] _reqMeasurments = new[]
        {
            "REQ_MEAS_EXHAUST",
            "REQ_MEAS_CO",
            "REQ_MEAS_RING"
        };

        public List<string> FuelIds => new List<string>(_fuelIds);

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
                    XMLData = _xmlSendData;
            }
        }

        public List<string> ReqMeasurements => new List<string>(_reqMeasurments);

        private string _selectedReqMeasurements;
        public string SelectedReqMeasurements
        {
            get => _selectedReqMeasurements;
            set
            {
                if (SetProperty(ref _selectedReqMeasurements, value))
                    XMLData = _xmlSendData;
            }
        }

        public MainViewModel()
        {
            SelectedFuelId = _fuelIds[1];
            SelectedReqMeasurements = _reqMeasurments[0];

            XMLData = _xmlSendData;
            MessagingCenter.Subscribe<string>(this, "mru4uresponse", (response) => XMLData = System.Net.WebUtility.UrlDecode(response));
            MessagingCenter.Subscribe<string>(this, "fileresponse", response => XMLData = response);
        }

        #region Properties

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
                string urlString = $"mru4urequest://mru/{WebUtility.UrlEncode(_xmlSendData)}";
                MessagingCenter.Send<string>(urlString, "mru4urequest");
            });

        /// <summary>
        /// Send xml request via file stream
        /// </summary>
        public ICommand StartTestFileExchangeCommand => new Command(
            execute: () =>
            {
                var index = _xmlSendData.IndexOf("<?xml");
                string urlString = _xmlSendData.Substring(index, _xmlSendData.Length - index);
                MessagingCenter.Send<string>(urlString, "filerequest");
            });

        /// <summary>
        /// Send xml request via tcp
        /// </summary>
        /// <remarks>
        /// For UWP use
        /// </remarks>
        public ICommand SendTcpRequestCommand => new Command(
            execute: () =>
            {
                try
                {
                    using (Socket s = new Socket(SocketType.Stream, ProtocolType.Tcp))
                    {
                        s.Connect(IPAddress.Parse(_ipAddress), _port);
                        byte[] message = System.Text.Encoding.ASCII.GetBytes(_xmlSendData);
                        s.Send(message);
                    }
                }
                catch (Exception ex)
                {

                }
            });

    #endregion Commands
    }
}
