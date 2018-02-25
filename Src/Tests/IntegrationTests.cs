using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yort.Eftpos.Verifone.PosLink.Tests
{
	[TestClass]
	public class IntegrationTests
	{
		private const string PinPadIP = "10.10.10.118";
		private const int PinPadPort = 4444;

		[TestInitialize]
		public void Initialize()
		{
			GlobalSettings.Logger.LogCommunicationPackets = true;
		}

		[TestCategory("Integration")]
		[TestMethod]
		public async Task Integration_LogonSucceeds()
		{
			var client = new PinpadClient(PinPadIP, PinPadPort);

			var request = new LogonRequest();
			var result = await client.ProcessRequest<LogonRequest, LogonResponse>(request).ConfigureAwait(false);

			Assert.IsNotNull(result);
			System.Diagnostics.Trace.WriteLine("Status: " + result.Response);
			System.Diagnostics.Trace.WriteLine(result.ReceiptData);

			Assert.AreEqual(ResponseCodes.Accepted, result.Response);
		}

		[TestCategory("Integration")]
		[TestMethod]
		public async Task Integration_CanPurchase()
		{
			var client = new PinpadClient(PinPadIP, PinPadPort);
			client.DisplayMessage += Client_DisplayMessage;

			var request = new PurchaseRequest()
			{
				PurchaseAmount = 10.00M,
			};
			var result = await client.ProcessRequest<PurchaseRequest, TransactionResponseBase>(request).ConfigureAwait(false);

			Assert.IsNotNull(result);
			System.Diagnostics.Trace.WriteLine("Status: " + result.Response);
			System.Diagnostics.Trace.WriteLine("Account: " + result.Account);
			System.Diagnostics.Trace.WriteLine("Bank Ref: " + result.BankReference);
			System.Diagnostics.Trace.WriteLine("Truncated Pan: " + result.TruncatedPan);
			System.Diagnostics.Trace.WriteLine("Merchant Receipt:\r\n " + result.MerchantReceipt);
			System.Diagnostics.Trace.WriteLine("\r\nCustomer Receipt:\r\n " + result.CustomerReceipt);

			Assert.AreEqual(ResponseCodes.Accepted, result.Response);
		}


		[TestCategory("Integration")]
		[TestMethod]
		public async Task Integration_CanCancelPurchase()
		{
			var client = new PinpadClient(PinPadIP, PinPadPort);
			client.DisplayMessage += Client_DisplayMessage;

			var purchaseRequest = new PurchaseRequest()
			{
				PurchaseAmount = 10.00M,
			};
			var purchaseTask = client.ProcessRequest<PurchaseRequest, TransactionResponseBase>(purchaseRequest);
			await Task.Delay(5000).ConfigureAwait(false);

			var cancelRequest = new EftposCancelRequest()
			{
				MerchantReference = purchaseRequest.MerchantReference
			};

			var result = await client.ProcessRequest<EftposCancelRequest, TransactionResponseBase>(cancelRequest);

			Assert.IsNotNull(result);
			System.Diagnostics.Trace.WriteLine("Status: " + result.Response);
			System.Diagnostics.Trace.WriteLine("Account: " + result.Account);
			System.Diagnostics.Trace.WriteLine("Bank Ref: " + result.BankReference);
			System.Diagnostics.Trace.WriteLine("Truncated Pan: " + result.TruncatedPan);
			System.Diagnostics.Trace.WriteLine("Merchant Receipt:\r\n " + result.MerchantReceipt);
			System.Diagnostics.Trace.WriteLine("\r\nCustomer Receipt:\r\n " + result.CustomerReceipt);

			Assert.AreEqual(ResponseCodes.TransactionCancelled, result.Response);

			var result2 = await purchaseTask.ConfigureAwait(false);

			Assert.IsNotNull(result2);
			System.Diagnostics.Trace.WriteLine("Status: " + result2.Response);
			System.Diagnostics.Trace.WriteLine("Account: " + result2.Account);
			System.Diagnostics.Trace.WriteLine("Bank Ref: " + result2.BankReference);
			System.Diagnostics.Trace.WriteLine("Truncated Pan: " + result2.TruncatedPan);
			System.Diagnostics.Trace.WriteLine("Merchant Receipt:\r\n " + result2.MerchantReceipt);
			System.Diagnostics.Trace.WriteLine("\r\nCustomer Receipt:\r\n " + result2.CustomerReceipt);

			Assert.AreEqual(ResponseCodes.TransactionCancelled, result2.Response);
		}

		private void Client_DisplayMessage(object sender, DisplayMessageEventArgs e)
		{
			System.Diagnostics.Trace.WriteLine($"Display Message from {e.Message.Source.ToString()}: {e.Message.MessageText}");
		}
	}
}
