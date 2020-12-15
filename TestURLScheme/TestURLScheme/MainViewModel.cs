
using System.Windows.Input;
using Xamarin.Forms;

namespace TestURLScheme
{
    public class MainViewModel : BaseModelAndViewModel
    {
        private const string XmlSendData = "42<?xml version=\"1.0\" encoding=\"utf-8\"?><SingleStart xmlns=\"urn:ZIV-schema\" TransmissionId=\"2016-10-13\"> <Version>1.00</Version> <FuelId>FUEL_NAT_GAS</FuelId> <RequiredMeasurements>REQ_MEAS_BIMSCHV</RequiredMeasurements> </SingleStart>";

        public MainViewModel()
        {
            XMLData = XmlSendData;
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

        public ICommand StartTestURLCommand => new Command(
            execute: () =>
            {
                string urlString = "mru4urequest://mru/" + System.Net.WebUtility.UrlEncode(XmlSendData);
                //Device.OpenUri(new Uri(urlString));
                MessagingCenter.Send<string>(urlString, "mru4urequest");
            });


        public ICommand StartTestFileExchangeCommand => new Command(
            execute: () =>
            {
                var index = XmlSendData.IndexOf("<?xml");
                string urlString = XmlSendData.Substring(index, XmlSendData.Length - index);
                MessagingCenter.Send<string>(urlString, "filerequest");
            });

        #endregion Commands
    }
}
