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
	}
}
