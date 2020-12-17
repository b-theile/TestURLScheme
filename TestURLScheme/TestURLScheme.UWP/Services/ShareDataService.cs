using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;

namespace TestURLScheme.UWP.Services
{
    //public event TypedEventHandler<DataTransferManager, DataRequestedEventArgs> DataRequested;

    public class ShareDataService
    {        
        //To see this code in action, add a call to ShareSourceLoad to your constructor or other
        //initializing function.
        private void ShareSourceLoad()
        {
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.DataRequested);
        }

        private void DataRequested(DataTransferManager sender, DataRequestedEventArgs e)
        {
            DataRequest request = e.Request;
            request.Data.Properties.Title = "Share Text Example";
            request.Data.Properties.Description = "An example of how to share text.";
            request.Data.SetText("Hello World!");
        }
    }
}
