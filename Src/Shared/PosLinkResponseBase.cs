using Ladon;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.Eftpos.Verifone.PosLink
{
	/// <summary>
	/// A common base class for all messages that are received from the pin pad.
	/// </summary>
	public abstract class PosLinkResponseBase
	{

		private readonly IList<string> _Fields;

		/// <summary>
		/// Full constructor. Constructs a new message instance using the pre-decoded field values in protocol order.
		/// </summary>
		/// <param name="fieldValues">A list of strings containing each field value in the order specified by the protocol and message type.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="fieldValues"/> is null.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="fieldValues"/> does not contain any values, or if the message type in the fields list does not match <see cref="MessageType"/>.</exception>
		protected PosLinkResponseBase(IList<string> fieldValues)
		{
			fieldValues.GuardNull(nameof(fieldValues));
			fieldValues.Count.GuardZero(nameof(fieldValues), nameof(fieldValues.Count));
			if (fieldValues[1] != MessageType) throw new ArgumentException(ErrorMessages.MessageTypeDoesNotMatch, nameof(fieldValues));
			
			_Fields = fieldValues;
		}

		/// <summary>
		/// The merchant reference for this request that generated this response.
		/// </summary>
		public string MerchantReference { get { return _Fields[0]; } }

		/// <summary>
		/// An abstract property that must be overridden on derived message types. Specifies the protocol name for this message.
		/// </summary>
		public abstract string MessageType { get; }

		/// <summary>
		/// Returns the merchant id associated with this response and it's request.
		/// </summary>
		public int Merchant { get { return Convert.ToInt32(_Fields[2]); } }

		/// <summary>
		/// Returns the list of fields provided by the constructor.
		/// </summary>
		protected IList<string> Fields { get { return _Fields; } }

		/// <summary>
		/// Returns the value of the field at the specified index in the <see cref="Fields"/> list, or null if that index does not exist (index is greater than or equal to the count of fields).
		/// </summary>
		/// <returns>A string containing the pre-decoded value of the field.</returns>
		protected string FieldValueOrNull(int fieldIndex)
		{
			if (_Fields.Count > fieldIndex) return _Fields[fieldIndex];

			return null;
		}
	}
}
