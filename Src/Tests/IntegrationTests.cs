using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yort.Eftpos.Verifone.PosLink.Tests
{
#if !TESTS_INTEGRATION
	[Ignore]	
#endif
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
		public async Task Integration_CanLogon()
		{
			var client = new PinpadClient(PinPadIP, PinPadPort);
			var request = new LogonRequest();
			var result = await client.ProcessRequest<LogonRequest, LogonResponse>(request).ConfigureAwait(false);

			Assert.IsNotNull(result);
			System.Diagnostics.Trace.WriteLine("Status: " + result.Response);
			System.Diagnostics.Trace.WriteLine(result.ReceiptData);

			Assert.AreEqual(ResponseCodes.Accepted, result.Response);
		}

		[ExpectedException(typeof(DeviceBusyException))]
		[TestCategory("Integration")]
		[TestMethod]
		public async Task Integration_ThrowsDeviceBusyIfAlreadyConnected()
		{
			var client = new PinpadClient(PinPadIP, PinPadPort);

			var request = new LogonRequest();
			var t1 = client.ProcessRequest<LogonRequest, LogonResponse>(request);
			var t2 = await client.ProcessRequest<LogonRequest, LogonResponse>(request);

			Assert.Fail("No exception thrown");
		}

		[ExpectedException(typeof(DeviceBusyException))]
		[TestCategory("Integration")]
		[TestMethod]
		public async Task Integration_ThrowsDeviceBusyWhenRequestInProgress()
		{
			var client = new PinpadClient(PinPadIP, PinPadPort);
			var client2 = new PinpadClient(PinPadIP, PinPadPort);

			var request = new LogonRequest();
			var request2 = new LogonRequest();
			var t1 = client.ProcessRequest<LogonRequest, LogonResponse>(request);
			var t2 = await client2.ProcessRequest<LogonRequest, LogonResponse>(request2);

			Assert.Fail("No exception thrown");
		}

		[TestCategory("Integration")]
		[TestMethod]
		public async Task Integration_CanSettlementEnquiry()
		{
			var client = new PinpadClient(PinPadIP, PinPadPort);

			var request = new SettlementEnquiryRequest()
			{
				SettlementDate = DateTime.Today
			};
			var result = await client.ProcessRequest<SettlementEnquiryRequest, SettlementEnquiryResponse>(request).ConfigureAwait(false);

			Assert.IsNotNull(result);
			System.Diagnostics.Trace.WriteLine("Status: " + result.Response);
			System.Diagnostics.Trace.WriteLine(result.ReceiptData);

			Assert.AreEqual(ResponseCodes.Accepted, result.Response);
		}

		[TestCategory("Integration")]
		[TestMethod]
		public async Task Integration_CanSettlementCutover()
		{
			 var client = new PinpadClient(PinPadIP, PinPadPort);

			var request = new SettlementCutoverRequest();
			var result = await client.ProcessRequest<SettlementCutoverRequest, SettlementCutoverResponse>(request).ConfigureAwait(false);

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
			client.QueryOperator += Client_QueryOperator;
			var request = new PurchaseRequest()
			{
				Amount = 10.00M
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

		private void Client_QueryOperator(object sender, QueryOperatorEventArgs e)
		{
			e.SetResponse("YES");
		}

		[TestCategory("Integration")]
		[TestMethod]
		public async Task Integration_CanManPurchase()
		{
			var client = new PinpadClient(PinPadIP, PinPadPort);
			client.DisplayMessage += Client_DisplayMessage;

			var request = new ManualPanPurchaseRequest()
			{
				Amount = 10.00M
			};
			var result = await client.ProcessRequest<ManualPanPurchaseRequest, ManualPanPurchaseResponse>(request).ConfigureAwait(false);

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
		public async Task Integration_CanRefund()
		{
			var client = new PinpadClient(PinPadIP, PinPadPort);
			client.DisplayMessage += Client_DisplayMessage;

			var request = new RefundRequest()
			{
				Amount = 10.00M
			};
			var result = await client.ProcessRequest<RefundRequest, RefundResponse>(request).ConfigureAwait(false);

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
		public async Task Integration_CanManRefund()
		{
			var client = new PinpadClient(PinPadIP, PinPadPort);
			client.DisplayMessage += Client_DisplayMessage;

			var request = new ManualPanRefundRequest()
			{
				Amount = 10.00M
			};
			var result = await client.ProcessRequest<ManualPanRefundRequest, ManualPanRefundResponse>(request).ConfigureAwait(false);

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
		public async Task Integration_CanCashOut()
		{
			var client = new PinpadClient(PinPadIP, PinPadPort);
			client.DisplayMessage += Client_DisplayMessage;

			var request = new CashOutRequest()
			{
				Amount = 10M
			};
			var result = await client.ProcessRequest<CashOutRequest, CashOutResponse>(request).ConfigureAwait(false);

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
				Amount = 10.00M,
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

		[TestCategory("Integration")]
		[TestMethod]
		public async Task Integration_CanQueryCard()
		{
			var client = new PinpadClient(PinPadIP, PinPadPort);
			client.DisplayMessage += Client_DisplayMessage;

			var request = new QueryCardRequest();
			var result = await client.ProcessRequest<QueryCardRequest, QueryCardResponse>(request).ConfigureAwait(false);

			Assert.IsNotNull(result);
			System.Diagnostics.Trace.WriteLine("Status: " + result.Response);
			System.Diagnostics.Trace.WriteLine("Track1: " + result.CardTrack1);
			System.Diagnostics.Trace.WriteLine("Track2: " + result.CardTrack2);

			Assert.AreEqual(ResponseCodes.Accepted, result.Response);
		}

		[TestCategory("Integration")]
		[TestMethod]
		public async Task Integration_CanReprintLastReceipt()
		{
			var client = new PinpadClient(PinPadIP, PinPadPort);
			client.DisplayMessage += Client_DisplayMessage;

			var request = new ReprintLastReceiptRequest();
			var result = await client.ProcessRequest<ReprintLastReceiptRequest, ReprintLastReceiptResponse>(request).ConfigureAwait(false);

			Assert.IsNotNull(result);
			System.Diagnostics.Trace.WriteLine("Status: " + result.Response);
			System.Diagnostics.Trace.WriteLine("Receipt Data: " + result.ReceiptData);

			Assert.AreEqual(ResponseCodes.Accepted, result.Response);
			Assert.IsFalse(String.IsNullOrWhiteSpace(result.ReceiptData));
		}

		[TestCategory("Integration")]
		[TestMethod]
		public async Task Integration_CanPollDevice()
		{
			var client = new PinpadClient(PinPadIP, PinPadPort);
			client.DisplayMessage += Client_DisplayMessage;

			var request = new PollRequest();
			var result = await client.ProcessRequest<PollRequest, PollResponse>(request).ConfigureAwait(false);

			Assert.IsNotNull(result);
			System.Diagnostics.Trace.WriteLine("Status: " + result.Status);
			System.Diagnostics.Trace.WriteLine("Display: " + result.Display);

			Assert.AreEqual(DeviceStatus.Ready, result.Status);
		}

		[TestCategory("Integration")]
		[TestMethod]
		public async Task Integration_CanTip()
		{
			var client = new PinpadClient(PinPadIP, PinPadPort);
			client.DisplayMessage += Client_DisplayMessage;

			//This test does too much, but you have to perform some actions
			//before performing the next (i.e auth before tip, tip before upload)
			//any of which could fail, so I question the value of splitting it 
			//into multiple tests rather than just having good assert messages.

			//Confirm we can start a transaction to add a tip to
			var request = new TipPreauthRequest()
			{
				PurchaseAmount = 20,
				Id = "POS 1"
			};
			var result = await client.ProcessRequest<TipPreauthRequest, TipPreauthResponse>(request).ConfigureAwait(false);

			Assert.IsNotNull(result);
			System.Diagnostics.Trace.WriteLine("Preauth Response: " + result.Response);
			System.Diagnostics.Trace.WriteLine("Preauth Display: " + result.Display);
			Assert.AreEqual(ResponseCodes.Accepted, result.Response, "Failed to create pre-auth transaction for tip. Transaction result was not 'accepted'.");

			//Now confirm we can add a tip
			var tipRequest = new TipAddRequest()
			{
				TipAmount = 2,
				OriginalPurchaseAmount = result.PurchaseAmount,
				Stan = result.Stan				
			};
			var tipResult = await client.ProcessRequest<TipAddRequest, TipAddResponse>(tipRequest).ConfigureAwait(false);

			Assert.IsNotNull(result);
			System.Diagnostics.Trace.WriteLine("Tip Response: " + tipResult.Response);
			System.Diagnostics.Trace.WriteLine("Tip Display: " + tipResult.Display);
			Assert.AreEqual(ResponseCodes.Accepted, tipResult.Response, "Failed to add tip to transaction. Tip Add result was not 'accepted'.");

			//Now confirm we can upload tip batch
			var tipUploadRequest = new TipBatchUploadRequest();
			var tipUploadResult = await client.ProcessRequest<TipBatchUploadRequest, TipBatchUploadResponse>(tipUploadRequest).ConfigureAwait(false);

			Assert.IsNotNull(result);
			System.Diagnostics.Trace.WriteLine("Tip Response: " + tipUploadResult.Response);
			System.Diagnostics.Trace.WriteLine("Tip Display: " + tipUploadResult.Display);
			System.Diagnostics.Trace.WriteLine("Merchant Receipt: " + tipUploadResult.MerchantReceipt);
			System.Diagnostics.Trace.WriteLine("Customer Receipt: " + tipUploadResult.CustomerReceipt);
			Assert.AreEqual(ResponseCodes.Accepted, tipResult.Response, "Failed to upload tip batch. Upload result was not 'accepted'.");
		}

		private void Client_DisplayMessage(object sender, DisplayMessageEventArgs e)
		{
			System.Diagnostics.Trace.WriteLine($"Display Message from {e.Message.Source.ToString()}: {e.Message.MessageText}");
		}
	}
}
