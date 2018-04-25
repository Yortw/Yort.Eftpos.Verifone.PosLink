# Yort.Eftpos.Verifone.PosLink

An **unofficial** .Net implementation of the Verifone PosLink protocol version 2.2 for EFTPOS pinpads

# UNDER CONSTRUCTION - NOTHING TO SEE HERE

This project is still untested/in development. Check back later.

## Status

[![Build status](https://ci.appveyor.com/api/projects/status/igna2bbereqn8qff?svg=true)](https://ci.appveyor.com/project/Yortw/yort-eftpos-verifone-poslink) [![NuGet Badge](https://buildstats.info/nuget/Yort.Eftpos.Verifone.PosLink)](https://www.nuget.org/packages/Yort.Eftpos.Verifone.PosLink/) [![GitHub license](https://img.shields.io/github/license/mashape/apistatus.svg)](https://github.com/Yortw/Yort.Eftpos.Verifone.PosLink/blob/master/LICENSE) 

## Supported Platforms

Currently;

* .Net Standard 2.0
    * Includes Xamarin.iOS, Xamarin.Android, .Net Core 2.0 and .Net 4.71
* .Net 4.5+
* UWP 10+ (Windows 10 Universal Programs, v10240 and up)

## Supported Features

* Standard financial transactions
    * Purchase with or without cash out
    * Manual purchase (for PCIDSS compliance, cannot provide card details programmatically, must be entered on pinpad)
    * Cash out (only)
    * Manual cash out 
    * Refund
    * Manual refund (for PCIDSS compliance, cannot provide card details programmatically, must be entered on pinpad)
* Common admin functions    
    * Logon
    * Settlement Enquiry
    * Settlement Cutover
    * Reprint Last Receipt
    * Print (with optional reset) totals
* Poll request (device status)
* Cancel transaction
* Query card
* Ask responses (don't think these are used anymore but they have been implemented)

## Unsupported Features

* All tip related transactions (advised these are obsolete)
* Cheque Authorisation (advised these are obsolete)
* All tab related transactions (unknown if these are obsolete or not)
* PRN messages (host printing)
* Host comms

Adding missing messages, where still supported by current protocol and hardware, should be fairly easy.

# Getting Started

Install the Nuget package like this;

```powershell
    PM> Install-Package Yort.Eftpos.Verifone.PosLink
```

There is a secondary project/Nuget package for Windows Forms based projects which will provide default user interface and prompts during a transaction (you can build your own, but this saves you having too if you're ok with the UI style provided). 

```powershell
    PM> Install-Package Yort.Eftpos.Verifone.PosLink.WinForms
```

# Sample Code

The general pattern for implementing EFTPOS intergations is;

1. Persist your current request details to non-volatile storage. The request must be recoverable in the event of an unexpected failure (power cut/blue screen etc).
2. Begin the request
3. Process the result into your transaction model/complete the transaction or persist the result to storage.
4. Clear the persisted request details.

On start up, you must;

1. Check for a persisted request and if found;
    * Resend the request to the device using the PinpadClient.RetryRequest method. The request details must be the same as the last request sent.
    * Continue from step 3 above.
2. If not, send a poll request to check the device state.

You must also handle exceptions from the library when making requests;

* A TransactionFailureException represents a critical failure to obtain a result from the pinpad, as per the POS Link specification. You should prompt the user to either retry, or manually check the pinpad and choose an accepted/declined response manually.
* A PinPadConnectionException indicates the request was never sent to the pinpad because a connection could not be established. The request can be retried or treated as failed. The user should be advised to check the pinpad connection, network address etc.
* ArgumentException and derived exception types indicate invalid data in the request object (and the request was not sent).
 
```c#

// Sample code for making a purchase request
private async void EftposPurchaseButton_Click(object sender, EventArgs e)
{
    var request = new Yort.Eftpos.Verifone.PosLink.PurchaseRequest()
    {
        Amount = 10.00M,
    };

    SaveCurrentRequest(request);

    var pinpad = new Yort.Eftpos.Verifone.PosLink.PinpadClient("10.10.10.118");
    //This event handler attached here should update the status on screen. 
    pinpad.DisplayMessage += Pinpad_DisplayMessage;
    //The event handler attached here should prompt the user for a response to the query passed in the event arguments.
    pinpad.QueryOperator += Pinpad_QueryOperator; 

    try
    {
        var response = await pinpad.ProcessRequest<Yort.Eftpos.Verifone.PosLink.PurchaseRequest, Yort.Eftpos.Verifone.PosLink.PurchaseResponse>(request);

        bool paymentAccepted = response.ResponseCode == Yort.Eftpos.Verifone.PosLink.ResponseCodes.Accepted;
        ProcessResponse(paymentAccepted, response);
    }
    catch (Yort.Eftpos.Verifone.PosLink.TransactionFailureException)
    {
        //Prompt user to retry or manually provide result
    }
    catch (Yort.Eftpos.Verifone.PosLink.PosLinkConnectionException)
    {
        //Failed, treat as declined/failed, or prompt user to retry.
    }
    catch (ArgumentException)
    {
        //Something is wrong with the request, inform user.
    }
    finally
    {
        ClearCurrentRequest(request);
    }
}

// Code similar to this should run at startup
public async Task RecoverEftposResult()
{
    var request = RecoverCurrentRequest();
    if (request == null) return;

    var pinpad = new Yort.Eftpos.Verifone.PosLink.PinpadClient("10.10.10.118");
    //This event handler attached here should update the status on screen.
    pinpad.DisplayMessage += Pinpad_DisplayMessage;
    //The event handler attached here should prompt the user for a response to the query passed in the event arguments.
    pinpad.QueryOperator += Pinpad_QueryOperator;

    try
    {
        var response = await pinpad.RetryRequest<Yort.Eftpos.Verifone.PosLink.PurchaseRequest, Yort.Eftpos.Verifone.PosLink.PurchaseResponse>(request);

        bool paymentAccepted = response.ResponseCode == Yort.Eftpos.Verifone.PosLink.ResponseCodes.Accepted;
        ProcessResponse(paymentAccepted, response);
    }
    catch (Yort.Eftpos.Verifone.PosLink.TransactionFailureException)
    {
        //Prompt user to retry or manually provide result
    }
    catch (Yort.Eftpos.Verifone.PosLink.PosLinkConnectionException)
    {
        //Failed, treat as declined/failed, or prompt user to retry.
    }
    catch (ArgumentException)
    {
        //Something is wrong with the request, inform user.
    }
    finally
    {
        ClearCurrentRequest(request);
    }
}

// For Windows Forms you can import the *Yort.Eftpos.Verifone.PosLink.WinForms* Nuget package, 
// and use it to aid with the UI prompts/progress display. Create a 'DialogAdapter' instance
// and pass the pinpad client to it. It will attach to the pinpad client events and automatically 
// show dialogs as required during an EFTPOS request.
// A new DialogAdapter should be created for each request. Dispose the adatper after the response
// is returned to hide the dialog.

Yort.Eftpos.Verifone.PosLink.PurchaseResponse response = null;
using (var dialogAdapter = new Yort.Eftpos.Verifone.PosLink.DialogAdapter(pinpad, null, false))
{
    response = await pinpad.ProcessRequest<Yort.Eftpos.Verifone.PosLink.PurchaseRequest, Yort.Eftpos.Verifone.PosLink.PurchaseResponse>(request);

    bool paymentAccepted = response.ResponseCode == Yort.Eftpos.Verifone.PosLink.ResponseCodes.Accepted;
}
ProcessResponse(paymentAccepted, response);

```