
# Yort.Eftpos.Verifone.PosLink

An **unofficial** .Net implementation of the Verifone PosLink protocol for EFTPOS pinpads

# UNDER CONSTRUCTION - NOTHING TO SEE HERE

This project is still untested/in development. Check back later.

# Status

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
* Cheque Authorisation
* Poll request (device status)
* Cancel transaction
* Query card
* Ask responses (don't think these are used anymore but they have been implemented)

**Unsupported Features**
* All tip related transactions (advised these are obsolete)
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


