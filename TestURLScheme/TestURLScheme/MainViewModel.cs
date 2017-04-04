
using System.Windows.Input;
using Xamarin.Forms;

namespace TestURLScheme
{
    public class MainViewModel : BaseModelAndViewModel
    {
       private static string xmlSendData = "42<?xml version=\"1.0\" encoding=\"utf-8\"?><SingleStart xmlns=\"urn:ZIV-schema\" TransmissionId=\"2016-10-13\"> <Version>1.00</Version> <FuelId>FUEL_NAT_GAS</FuelId> <RequiredMeasurements>REQ_MEAS_BIMSCHV</RequiredMeasurements> </SingleStart>";

        public MainViewModel()
        {
            this.XMLData = MainViewModel.xmlSendData;

            MessagingCenter.Subscribe<string>(this, "mru4uresponse", (response) =>
            {
                this.XMLData = System.Net.WebUtility.UrlDecode(response);
            });

            MessagingCenter.Subscribe<string>(this, "fileresponse", response =>
            {
                this.XMLData = response;
            });
        }

        private string _xmlData;
        public string XMLData
        {
            get { return _xmlData; }
            set { SetProperty(ref _xmlData, value); }
        }

        private Command _startTestURLCommand;
        public ICommand StartTestURLCommand
        {
            get { return _startTestURLCommand = _startTestURLCommand ?? new Command(StartTestURLExecute); }
        }

        private void StartTestURLExecute()
        {
            string urlString = "mru4urequest://mru/" + System.Net.WebUtility.UrlEncode(xmlSendData);
            //Device.OpenUri(new Uri(urlString));
            MessagingCenter.Send<string>(urlString, "mru4urequest");
        }

        private Command _startTestFileExchangeCommand;
        public ICommand StartTestFileExchangeCommand
        {
            get { return _startTestFileExchangeCommand = _startTestFileExchangeCommand ?? new Command(StartTestFileExchangeExecute); }
        }

        private void StartTestFileExchangeExecute()
        {
            var index = xmlSendData.IndexOf("<?xml");
            string urlString = xmlSendData.Substring(index, xmlSendData.Length - index);
            MessagingCenter.Send<string>(urlString, "filerequest");
        }
    }
}
