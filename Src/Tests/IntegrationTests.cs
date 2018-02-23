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
		public void TestInitialize()
		{
			var now = DateTime.Now;
			var tempStr = now.ToString("yyMMddHHmmss").PadLeft(12);
			tempStr = tempStr.Substring(tempStr.Length - 12, 12);

			DefaultMerchantReferenceGenerator.Instance.Seed(Convert.ToInt64(tempStr));
		}

		[TestCategory("Integration")]
		[TestMethod]
		public async Task Integration_LogonSucceeds()
		{
			var client = new PinpadClient(PinPadIP, PinPadPort);

			var request = new LogonRequestMessage();
			var result = await client.ProcessRequest<LogonRequestMessage, LogonResponseMessage>(request).ConfigureAwait(false);

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

			var request = new PurchaseRequestMessage()
			{
				PurchaseAmount = 10.00M,
			};
			var result = await client.ProcessRequest<PurchaseRequestMessage, PurchaseResponseMessage>(request).ConfigureAwait(false);

			Assert.IsNotNull(result);
			System.Diagnostics.Trace.WriteLine("Status: " + result.Response);
			System.Diagnostics.Trace.WriteLine("Account: " + result.Account);
			System.Diagnostics.Trace.WriteLine("Bank Ref: " + result.BankReference);
			System.Diagnostics.Trace.WriteLine("Truncated Pan: " + result.TruncatedPan);
			System.Diagnostics.Trace.WriteLine("Merchant Receipt:\r\n " + result.MerchantReceipt);
			System.Diagnostics.Trace.WriteLine("\r\nCustomer Receipt:\r\n " + result.CustomerReceipt);

			Assert.AreEqual(ResponseCodes.Accepted, result.Response);
		}

		private void Client_DisplayMessage(object sender, DisplayMessageEventArgs e)
		{
			System.Diagnostics.Trace.WriteLine($"Display Message from {e.Message.Source.ToString()}: {e.Message.MessageText}");
		}
	}
}
