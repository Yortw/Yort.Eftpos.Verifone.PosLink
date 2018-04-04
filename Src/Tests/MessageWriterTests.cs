using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Yort.Eftpos.Verifone.PosLink.Tests
{
	[TestClass]
	public class MessageWriterTests
	{
		[TestMethod]
		public async Task MessageWriter_WritesWithCrc()
		{
			var expected = "\u0002D25310001,PUR,1,000010.00,000000.00,Test Text,YY\u0003\u000f";

			var writer = new MessageWriter();

			var message = new PurchaseRequest()
			{
				AllowCredit = true,
				CashAmount = 0.00M,
				Merchant = 1,
				MerchantReference = "D25310001",
				Id = "Test Text",
				PurchaseAmount = 10.00M,
				ReturnReceipt = true
			};

			string formattedMessage;
			using (var stream = new System.IO.MemoryStream())
			{
				await writer.WriteMessageAsync<PurchaseRequest>(message, stream).ConfigureAwait(false);

				stream.Seek(0, System.IO.SeekOrigin.Begin);

				using (var reader = new System.IO.StreamReader(stream, System.Text.Encoding.ASCII))
				{
					formattedMessage = reader.ReadToEnd();
				}
			}

			Assert.AreEqual(expected, formattedMessage);
		}

		[TestMethod]
		public async Task MessageWriter_WritesEncodedString()
		{
			var expected = "\u0002D25310001,PUR,1,000010.00,000000.00,Hi\u001c Yort,YY\u0003\t";

			var writer = new MessageWriter();

			var message = new PurchaseRequest()
			{
				AllowCredit = true,
				CashAmount = 0.00M,
				Merchant = 1,
				MerchantReference = "D25310001",
				Id = "Hi, Yort",
				PurchaseAmount = 10.00M,
				ReturnReceipt = true
			};

			string formattedMessage;
			using (var stream = new System.IO.MemoryStream())
			{
				await writer.WriteMessageAsync<PurchaseRequest>(message, stream).ConfigureAwait(false);

				stream.Seek(0, System.IO.SeekOrigin.Begin);

				using (var reader = new System.IO.StreamReader(stream, System.Text.Encoding.ASCII))
				{
					formattedMessage = reader.ReadToEnd();
				}
			}

			Assert.AreEqual(expected, formattedMessage);
		}

		[TestMethod]
		public async Task MessageWriter_WritesStx()
		{
			var writer = new MessageWriter();

			var message = new PurchaseRequest()
			{
				AllowCredit = true,
				CashAmount = 0.00M,
				Merchant = 1,
				MerchantReference = "D25310001",
				Id = "Hi, Yort",
				PurchaseAmount = 10.00M,
				ReturnReceipt = true
			};

			string formattedMessage;
			using (var stream = new System.IO.MemoryStream())
			{
				await writer.WriteMessageAsync<PurchaseRequest>(message, stream).ConfigureAwait(false);

				stream.Seek(0, System.IO.SeekOrigin.Begin);

				using (var reader = new System.IO.StreamReader(stream, System.Text.Encoding.ASCII))
				{
					formattedMessage = reader.ReadToEnd();
				}
			}

			Assert.AreEqual(((char)0x02).ToString(), formattedMessage.Substring(0, 1));
		}

		[TestMethod]
		public async Task MessageWriter_WritesEtx()
		{
			var writer = new MessageWriter();

			var message = new PurchaseRequest()
			{
				AllowCredit = true,
				CashAmount = 0.00M,
				Merchant = 1,
				MerchantReference = "D25310001",
				Id = "Hi, Yort",
				PurchaseAmount = 10.00M,
				ReturnReceipt = true
			};

			string formattedMessage;
			using (var stream = new System.IO.MemoryStream())
			{
				await writer.WriteMessageAsync<PurchaseRequest>(message, stream).ConfigureAwait(false);

				stream.Seek(0, System.IO.SeekOrigin.Begin);

				using (var reader = new System.IO.StreamReader(stream, System.Text.Encoding.ASCII))
				{
					formattedMessage = reader.ReadToEnd();
				}
			}

			Assert.AreEqual(((char)0x03).ToString(), formattedMessage.Substring(formattedMessage.Length - 2, 1));
		}


	}
}
