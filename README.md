
Get started

This app is to demonstrate how to natively interact with the MRU4u app from MRU GmbH (www.mru.eu) with the help of the xml based ZIV interface (www.schornsteinfeger.de). It is a Xamarin Forms app, so most of the UI and business logic is in the "Core" project. The platform specific functions are in the iOS, Droid, etc. projects. The projects communicate through messages, that can be subscribed and then send through the projects.

Android

On Android there will be used the standard "interacting with other apps" (https://developer.android.com/training/basics/intents/result.html) with StartActivityForResult. We have assigned a xml SingleStart demo string in the app:

`string xmlSendData = "42<?xml version=\"1.0\" encoding=\"utf-8\"?><SingleStart xmlns=\"urn:ZIV-schema\" TransmissionId=\"2016-10-13\"> <Version>1.00</Version> <FuelId>FUEL_NAT_GAS</FuelId> <RequiredMeasurements>REQ_MEAS_BIMSCHV</RequiredMeasurements> </SingleStart>";`

When pressing the URL button this string will be send to the Droid project:

`string urlString = "mru4urequest://mru/" + System.Net.WebUtility.UrlEncode(xmlSendData);`

`MessagingCenter.Send<string>(urlString, "mru4urequest");`

There the mru4urequest message is subscribed and catches the string creates a new Intent and call the StartActivityForResult function.


`MessagingCenter.Subscribe<string>(this, "mru4urequest", urlString =>`

`{`

    `Intent intent = new Intent(Intent.ActionView, Uri.Parse(urlString));`
  
    `StartActivityForResult(intent, MRU_REQUEST);`
  
`});`


After that the MRU4u app shows up with Abgasmessung (REQ_MEAS_BIMSCHV) and Heizöl EL (FUEL_NAT_GAS). You should make your measurement and save it. If you have more than one measurement, the measurement is titled as TODO. Click on the measurement on the top left to select the other one(s). Measure. Save. Press "Confirm Save" to set the SetResult function, that writes back the data to the calling activity.
In the TestURLScheme App the SetResult of the MRU4u will invoke the OnActivityResult.

`protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)`

`{`

    `base.OnActivityResult(requestCode, resultCode, data);`

    `if (requestCode == MRU_REQUEST)`

    `{`

        `if (data?.Data != null)`
    
        `{`
    
            `MessagingCenter.Send<string>(data.Data.EncodedSchemeSpecificPart, "mru4uresponse");`
        
        `}`       
    
`}`

`}`

The requestcode will be checked. The data (the xml SingleResult) will be passed to the Core projects, where it will be displayed on the screen.
