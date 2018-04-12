using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.Eftpos.Verifone.PosLink
{
	/// <summary>
	/// An enum whose values determine how a failure to obtain a transaction response is handled.
	/// </summary>
	public enum TransactionFailureHandlingStrategy
	{
		/// <summary>
		/// The <see cref="PinpadClient"/> methods will throw a <see cref="TransactionFailureException"/> when a response cannot be determined by communication with the pinpad.
		/// </summary>
		/// <remarks>
		/// <para>According to the POS Link specification the typical handling here is to ask the user what the response was anyway, but using this mechansim gives the calling code control over what happens next.</para>
		/// </remarks>
		ThrowException = 0,
		/// <summary>
		/// The <see cref="PinpadClient"/> will raise a <see cref="PinpadClient.QueryOperator"/> event asking the user to check the terminal and provide an accepted/declined response.
		/// </summary>
		/// <remarks>
		/// <para>Once the user has chosen as response, the <see cref="PinpadClient"/> method will return a result object allowing normal processing, though some values on the response may be empty/null/default values (such as the receipt data) as the user only provides an accepted/declined status and not full transaction data.</para>
		/// <para>This eliminates the need for the POS to separately implement UI and logic for querying the operator and makes the handling of this case transparent to the caller, but eliminates any opportunity for the calling code to attempt retries, provide additional logging etc.</para>
		/// </remarks>
		QueryOperatorForResult
	}
}