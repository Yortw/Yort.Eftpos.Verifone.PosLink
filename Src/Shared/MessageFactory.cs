using Ladon;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Yort.Eftpos.Verifone.PosLink
{
	/// <summary>
	/// Creates new response messages from decoded protocol fields.
	/// </summary>
	public class ResponseMessageFactory
	{
		private static readonly Type[] ResponseMessageConstructorArgTypes = new Type[] { typeof(IList<string>) };

		private static readonly System.Collections.Concurrent.ConcurrentDictionary<Type, ConstructorInfo> _ResponseMessageConstructorCache = new System.Collections.Concurrent.ConcurrentDictionary<Type, ConstructorInfo>();
		private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, Type> _MessageNameToTypeMap = new System.Collections.Concurrent.ConcurrentDictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

		/// <summary>
		/// Default constructor.
		/// </summary>
		public ResponseMessageFactory()
		{
			LoadResponseMessageTypes();
		}

		/// <summary>
		/// Creates a new message of the type specified by {TResponseMessage} and loads it using the fields provided.
		/// </summary>
		/// <typeparam name="TResponseMessage">The .Net type of the response message to create.</typeparam>
		/// <param name="fieldValues">The list of pre-decoded field values to load the response message with.</param>
		/// <returns>A {TResponseMessage} loaded with the values provided by <paramref name="fieldValues"/>.</returns>
		public static TResponseMessage CreateMessage<TResponseMessage>(IList<string> fieldValues) where TResponseMessage : PosLinkResponseBase
		{
			return (TResponseMessage)CreateMessage(typeof(TResponseMessage), fieldValues);
		}

		/// <summary>
		/// Creates a new message, using the protocol field at index 1 to determine the message type.
		/// </summary>
		/// <param name="fieldValues">The list of pre-decoded field values to load the response message with.</param>
		/// <returns>An object derived from <see cref="PosLinkResponseBase"/> that is a response message loaded with the values provided by <paramref name="fieldValues"/>.</returns>
		public PosLinkResponseBase CreateMessage(IList<string> fieldValues)
		{
			fieldValues.GuardNull(nameof(fieldValues));

			var messageName = fieldValues[1];

			if (_MessageNameToTypeMap.TryGetValue(messageName, out var messageObjectType))
				return CreateMessage(messageObjectType, fieldValues);

			return null;
		}

		private static void LoadResponseMessageTypes()
		{
			lock (_MessageNameToTypeMap)
			{
				if (_MessageNameToTypeMap.Count == 0)
				{
					_MessageNameToTypeMap.TryAdd(ProtocolConstants.MessageType_Ask, typeof(AskRequest));
					_MessageNameToTypeMap.TryAdd(ProtocolConstants.MessageType_CashOnly, typeof(CashOutResponse));
					_MessageNameToTypeMap.TryAdd(ProtocolConstants.MessageType_Error, typeof(ErrorResponse));
					_MessageNameToTypeMap.TryAdd(ProtocolConstants.MessageType_Display, typeof(DisplayMessageResponse));
					_MessageNameToTypeMap.TryAdd(ProtocolConstants.MessageType_Logon, typeof(LogonResponse));
					_MessageNameToTypeMap.TryAdd(ProtocolConstants.MessageType_ManualPanPurchase, typeof(ManualPanPurchaseResponse));
					_MessageNameToTypeMap.TryAdd(ProtocolConstants.MessageType_ManualPanRefund, typeof(ManualPanRefundResponse));
					_MessageNameToTypeMap.TryAdd(ProtocolConstants.MessageType_Poll, typeof(PollResponse));
					_MessageNameToTypeMap.TryAdd(ProtocolConstants.MessageType_Purchase, typeof(PurchaseResponse));
					_MessageNameToTypeMap.TryAdd(ProtocolConstants.MessageType_QueryCard, typeof(QueryCardResponse));
					_MessageNameToTypeMap.TryAdd(ProtocolConstants.MessageType_Refund, typeof(RefundResponse));
					_MessageNameToTypeMap.TryAdd(ProtocolConstants.MessageType_ReprintLastReceipt, typeof(ReprintLastReceiptResponse));
					_MessageNameToTypeMap.TryAdd(ProtocolConstants.MessageType_Sig, typeof(SignatureRequest));
					_MessageNameToTypeMap.TryAdd(ProtocolConstants.MessageType_SettlementCutover, typeof(SettlementCutoverResponse));
					_MessageNameToTypeMap.TryAdd(ProtocolConstants.MessageType_SettlementEnquiry, typeof(SettlementEnquiryResponse));
					_MessageNameToTypeMap.TryAdd(ProtocolConstants.MessageType_TerminalTotals, typeof(TerminalTotalsResponse));
				}
			}
		}

		private static PosLinkResponseBase CreateMessage(Type type, IList<string> fieldValues)
		{
			var constructor = GetConstructorForResponseMessage(type);

			if (constructor == null) throw new InvalidOperationException(String.Format(System.Globalization.CultureInfo.CurrentCulture, ErrorMessages.ResponseMessageTypeDoesNotContainRequiredConstructor, type.FullName));
			return (PosLinkResponseBase)constructor.Invoke(new object[] { fieldValues });
		}

		private static ConstructorInfo GetConstructorForResponseMessage(Type type)
		{
			if (!_ResponseMessageConstructorCache.TryGetValue(type, out var retVal))
			{
				retVal = type.GetConstructor(ResponseMessageConstructorArgTypes);
				_ResponseMessageConstructorCache.TryAdd(type, retVal);
			}

			return retVal;
		}

		/// <summary>
		/// Creates a new transaction response message based on the provided request and accepted flag.
		/// </summary>
		/// <param name="request">The request to create a manual response for.</param>
		/// <param name="accepted">A boolean indicating if the response should say the transaction was accepted and funds transferred (true), or declined.</param>
		/// <returns></returns>
		public TransactionResponseBase CreateManualTransactionResponse(PosLinkTransactionRequestBase request, bool accepted)
		{
			request.GuardNull(nameof(request));

			var fieldList = new List<string>(13)
			{
				request.MerchantReference,
				request.RequestType,
				request.Merchant.ToString(System.Globalization.CultureInfo.InvariantCulture),
				request.GetManualResponseTransactionAmount().ToString(System.Globalization.CultureInfo.InvariantCulture)
			};

			if (request.RequestType == ProtocolConstants.MessageType_Purchase)
				fieldList.Add(((PurchaseRequest)request).CashAmount.ToString(System.Globalization.CultureInfo.InvariantCulture));
			fieldList.Add(accepted ? ResponseCodes.Accepted : ResponseCodes.Declined);
			fieldList.Add(StatusMessages.ManualResponse);
			fieldList.Add(String.Empty);
			fieldList.Add(String.Empty);
			fieldList.Add(String.Empty);
			fieldList.Add(String.Empty);
			fieldList.Add(String.Empty);
			fieldList.Add(String.Empty);

			return (TransactionResponseBase)CreateMessage(fieldList);
		}
	}
}