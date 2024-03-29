## Get started

This app is to demonstrate how to natively interact with the MRU4u app from MRU GmbH (www.mru.eu) with the help of the xml based ZIV interface (www.schornsteinfeger.de). It is a Xamarin Forms app, so most of the UI and business logic is in the "Core" project. The platform specific functions are in the iOS, Droid, etc. projects. The projects communicate through messages, that can be subscribed and then send through the projects.

### Android

On Android there will be used the standard "interacting with other apps" (https://developer.android.com/training/basics/intents/result.html) with `StartActivityForResult`. We have assigned a xml SingleStart demo string in the app:

    string xmlSendData = "42<?xml version=\"1.0\" encoding=\"utf-8\"?><SingleStart xmlns=\"urn:ZIV-schema\" TransmissionId=\"2016-10-13\"><Version>1.00</Version><FuelId>FUEL_NAT_GAS</FuelId><RequiredMeasurements>REQ_MEAS_BIMSCHV</RequiredMeasurements></SingleStart>";

When pressing the URL button this string will be send to the Droid project:

    string urlString = "mru4urequest://mru/" + System.Net.WebUtility.UrlEncode(xmlSendData);
    MessagingCenter.Send<string>(urlString, "mru4urequest");

In the Droid project the `mru4urequest` message is subscribed and catches the string. A new Intent is created with it and the `StartActivityForResult` function is called.

    MessagingCenter.Subscribe<string>(this, "mru4urequest", urlString =>
    {
        Intent intent = new Intent(Intent.ActionView, Uri.Parse(urlString));
        StartActivityForResult(intent, MRU_REQUEST);
    });

After that the MRU4u app shows up with "Abgasmessung" (`REQ_MEAS_BIMSCHV`) and "Erdgas" (`FUEL_NAT_GAS`). You should make your measurement and save it. If you have more than one measurement, the measurement is titled as TODO. Click on the measurement button on the top left to select the left measurement(s). Measure. Save. Press "Confirm Save" to set the `SetResult` function, that writes back the data to the calling activity.
In the TestURLScheme App the `SetResult` of the MRU4u will invoke the `OnActivityResult`.

    protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
    {
        base.OnActivityResult(requestCode, resultCode, data);

        if (requestCode == MRU_REQUEST)    
        {
            if (data?.Data != null)    
            {    
                MessagingCenter.Send<string>(data.Data.EncodedSchemeSpecificPart, "mru4uresponse");        
            }          
        }   
    }

The `requestCode` will be checked. The data (the xml SingleResult) will be passed to the Core project, where it will be displayed on the screen.

    MessagingCenter.Subscribe<string>(this, "mru4uresponse", (response) =>
    {
        this.XMLData = System.Net.WebUtility.UrlDecode(response);
    });
    
### iOS

When pressing the URL button this string will be send to the iOS project:

    string urlString = "mru4urequest://mru/" + System.Net.WebUtility.UrlEncode(xmlSendData);
    MessagingCenter.Send<string>(urlString, "mru4urequest");

In the iOS project the `mru4urequest` message is subscribed and catches the string. A new NSUrl is created with it and the `OpenUrl` function is called.

    MessagingCenter.Subscribe<string>(this, "mru4urequest", urlString =>
    {
        NSUrl url = new NSUrl(urlString);
        UIApplication.SharedApplication.OpenUrl(url);
    });
    
After that the MRU4u app shows up with "Abgasmessung" (`REQ_MEAS_BIMSCHV`) and "Erdgas" (`FUEL_NAT_GAS`). You should make your measurement and save it. If you have more than one measurement, the measurement is titled as TODO. Click on the measurement button on the top left to select the left measurement(s). Measure. Save. Press "Confirm Save" to call the `OpenUrl` function, that writes back the data to the calling app.
In the TestURLScheme App the `OpenUrl` of the MRU4u will invoke the `OpenUrl`.    
    
    public override bool OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
    {
        string message = System.Net.WebUtility.UrlDecode(url.AbsoluteString);
        MessagingCenter.Send<string>(message, "mru4uresponse");
        return true;
    }
    
The url string (the xml SingleResult) will be passed to the Core project, where it will be displayed on the screen.

    MessagingCenter.Subscribe<string>(this, "mru4uresponse", (response) =>
    {
        this.XMLData = System.Net.WebUtility.UrlDecode(response);
    });
