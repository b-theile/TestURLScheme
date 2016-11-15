using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        }

        private string _xmlData;
        public string XMLData
        {
            get { return _xmlData; }
            set { SetProperty(ref _xmlData, value); }
        }

        private Command _startTestURL;
        public ICommand StartTestURL
        {
            get { return _startTestURL = _startTestURL ?? new Command(StartTestURLExecute); }
        }

        private void StartTestURLExecute()
        {
            string urlString = "mru4urequest://mru/" + System.Net.WebUtility.UrlEncode(MainViewModel.xmlSendData);
            //Device.OpenUri(new Uri(urlString));
            MessagingCenter.Send<string>(urlString, "mru4urequest");
        }
    }
}
