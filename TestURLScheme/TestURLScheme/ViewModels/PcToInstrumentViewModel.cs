using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace TestURLScheme.ViewModels
{
    public class PcToInstrumentViewModel : BaseModelAndViewModel
    {

        /// <summary>
        /// localhost address
        /// </summary>
        private const string _ipAddress = "127.0.0.1";
        /// <summary>
        /// Tcp port of MRU4u app
        /// </summary>
        private const int _port = 21113;


        private string _xmlPcToInstrumentData = _pcToInstrumentData;
        /// <summary>
        /// Xml data string
        /// </summary>
        public string XmlPcToInstrumentData
        {
            get => _xmlPcToInstrumentData;
            set => SetProperty(ref _xmlPcToInstrumentData, value);
        }
        
        /// <summary>
        /// Test PcToInstrument data
        /// </summary>
        private const string _pcToInstrumentData =
            $"<?xml version=\"1.0\" encoding=\"iso-8859-1\"?>" +
            "<PcToInstrument xmlns=\"urn:ZIV-schema\" TransmissionId=\"2012_05_11_11_59\">" +
            "<Version>3.00</Version>" +

            "<Control>" +
                "<DeleteCustomers>true</DeleteCustomers>" +
                "<DeleteMeasurements>true</DeleteMeasurements>" +
                "<DeleteTesters>true</DeleteTesters>" +
            "</Control>" +

            "<Customer CustomerId=\"111-111-111\">" +

                "<Address>" +
                    "<Name> Thomas Müller</Name>" +
                    "<Street>Hauptstraße 12</Street>" +
                    "<Postcode>01234</Postcode>" +
                    "<City>Kappel</City>" +
                "</Address>" +

                "<Fireplace FireplaceNumber=\"123\">" +
                    "<Address>" +
                        "<Street>Hauptstraße 12</Street>" +
                        "<Postcode>01234</Postcode>" +
                        "<City>Kappel</City>" +
                        "<Telephone>01234567</Telephone>" +
                    "</Address>" +
                    "<InstallPlace>Keller</InstallPlace>" +
                    "<Fuel FuelId=\"FUEL_LIGHT_OIL\"></Fuel>" +
                "</Fireplace>" +

            "</Customer>" +

            "<Customer CustomerId=\"222-222-222\">" +

                "<Address>" +
                    "<Name>Thomas Meier</Name>" +
                    "<Street>Hauptstraße 12</Street>" +
                    "<Postcode>01234</Postcode>" +
                    "<City>Kappel</City>" +
                "</Address>" +

                "<Fireplace FireplaceNumber=\"123\">" +
                    "<Address>" +
                        "<Street>Hauptstraße 12</Street>" +
                        "<Postcode>012345</Postcode>" +
                        "<City>Kappel</City>" +
                        "<Telephone>01234567</Telephone>" +
                    "</Address>" +
                    "<InstallPlace>Keller</InstallPlace>" +
                    "<Fuel FuelId=\"FUEL_LIGHT_OIL\"></Fuel>" +
                "</Fireplace>" +

            "</Customer>" +
        "</PcToInstrument>";

        /// <summary>
        /// Mandatory id for tarnsfer
        /// PcToInstrument = '-S'
        /// </summary>
        private readonly string _id = "-S";


        #region Commands

        /// <summary>
        /// Send xml request via url (mru4urequest)
        /// </summary>
        public ICommand StartTestURLCommand => new Command(
            execute: () =>
            {
                string urlString = $"mru4urequest://mru/{WebUtility.UrlEncode($"{_id}{XmlPcToInstrumentData}")}";
                MessagingCenter.Send<string>(urlString, "mru4urequest");
            });

        /// <summary>
        /// Send xml request via file stream
        /// </summary>
        public ICommand StartTestFileExchangeCommand => new Command(
            execute: () =>
            {
                MessagingCenter.Send<string>(XmlPcToInstrumentData, "filerequest");
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
                    // Set sample data
                    XmlPcToInstrumentData = _xmlPcToInstrumentData;
                    // Connect to localhost
                    using Socket socket = new(SocketType.Stream, ProtocolType.Tcp);
                    socket.Connect(IPAddress.Parse(_ipAddress), _port);
                    // Set data
                    var data = WebUtility.UrlEncode($"{_id}{XmlPcToInstrumentData}");
                    socket.Send(System.Text.Encoding.ASCII.GetBytes(data));
                    // Get data
                    byte[] buffer = new byte[2048];
                    var count = socket.Receive(buffer);
                    var rawData = Encoding.UTF8.GetString(buffer, 0, count);
                    // Display data
                    XmlPcToInstrumentData = WebUtility.UrlDecode(rawData);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message, "SendTcpRequestCommand");
                }
            });

        #endregion Commands
    }
}
