using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Yort.Eftpos.Verifone.PosLink
{
	/// <summary>
	/// Interface for the <see cref="PinpadClient"/> class, allowing mocking/stubbing etc.
	/// </summary>
	/// <seealso cref="PinpadClient"/>
	public interface IPinpadClient
	{

		/// <summary>
		/// Raised when there is an information prompt or status change that should be displayed to the user.
		/// </summary>
		/// <remarks>
		/// <para>This event may be raised from background threads, any code updating UI may need to invoke to the UI thread.</para>
		/// </remarks>
		event EventHandler<DisplayMessageEventArgs> DisplayMessage;
		/// <summary>
		/// Raised when there is a question that must be answered by the operator.
		/// </summary>
		/// <remarks>
		/// <para>This event may be raised from background threads, any code updating UI may need to invoke to the UI thread.</para>
		/// </remarks>
		event EventHandler<QueryOperatorEventArgs> QueryOperator;

		/// <summary>
		/// Sends a request to the pin pad and returns the response. If the device is already busy or processing a request, throws a <see cref="DeviceBusyException"/>.
		/// </summary>
		/// <typeparam name="TRequestMessage">The type of request to send.</typeparam>
		/// <typeparam name="TResponseMessage">The type of response expected.</typeparam>
		/// <param name="requestMessage">The request message to be sent.</param>
		/// <returns>A instance of {TResponseMessage} containing the pin pad response.</returns>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="requestMessage"/> message is null, or if any required property of the request is null.</exception>
		/// <exception cref="System.ArgumentException">Thrown if any property of <paramref name="requestMessage"/> is invalid.</exception>
		/// <exception cref="TransactionFailureException">Thrown if a critical error occurred determining a transaction status and the user must be prompted to provide the result instead.</exception>
		/// <exception cref="DeviceBusyException">Thrown if the device is already processing a request or does not respond to the request.</exception>
		Task<TResponseMessage> ProcessRequest<TRequestMessage, TResponseMessage>(TRequestMessage requestMessage)
			where TRequestMessage : PosLinkRequestBase
			where TResponseMessage : PosLinkResponseBase;

		/// <summary>
		/// Sends a request to the pin pad and returns the response. Intended to be used for retrying requests that have already been sent, does not check if the pinpad is already busy.
		/// </summary>
		/// <typeparam name="TRequestMessage">The type of request to send.</typeparam>
		/// <typeparam name="TResponseMessage">The type of response expected.</typeparam>
		/// <param name="requestMessage">The request message to be sent.</param>
		/// <returns>A instance of {TResponseMessage} containing the pin pad response.</returns>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="requestMessage"/> message is null, or if any required property of the request is null.</exception>
		/// <exception cref="System.ArgumentException">Thrown if any property of <paramref name="requestMessage"/> is invalid.</exception>
		/// <exception cref="TransactionFailureException">Thrown if a critical error occurred determining a transaction status and the user must be prompted to provide the result instead.</exception>
		/// <exception cref="DeviceBusyException">Thrown if the device does not respond to the request.</exception>
		Task<TResponseMessage> RetryRequest<TRequestMessage, TResponseMessage>(TRequestMessage requestMessage)
			where TRequestMessage : PosLinkRequestBase
			where TResponseMessage : PosLinkResponseBase;
	}
}