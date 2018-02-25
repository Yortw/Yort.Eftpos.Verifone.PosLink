using Ladon;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace Yort.Eftpos.Verifone.PosLink
{
	/// <summary>
	/// Event arguments for the <see cref="PinpadClient.QueryOperator"/> event.
	/// </summary>
	public class QueryOperatorEventArgs : EventArgs
	{

		private readonly string _Prompt;
		private readonly string _ReceiptText;

		private IReadOnlyList<string> _AllowedResponses;

		private System.Threading.Tasks.TaskCompletionSource<string> _ResponseCompletionSource;

		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="prompt">The text of the prompt to be displayed to the operator.</param>
		/// <param name="receiptText">The text of a receipt to print when the prompt is displayed, if any, such as a signature receipt.</param>
		/// <param name="allowedResponses">A list of strings containing the allowed responses from the operator.</param>
		public QueryOperatorEventArgs(string prompt, string receiptText, IReadOnlyList<string> allowedResponses)
		{
			_Prompt = prompt;
			_ReceiptText = receiptText;
			_AllowedResponses = allowedResponses;

			_ResponseCompletionSource = new System.Threading.Tasks.TaskCompletionSource<string>();
		}

		/// <summary>
		/// The text of the prompt to be displayed to the operator.
		/// </summary>
		public string Prompt { get { return _Prompt; } }

		/// <summary>
		/// The text of a receipt to print when the prompt is displayed, if any, such as a signature receipt.
		/// </summary>
		public string ReceiptText { get { return _ReceiptText; } }

		/// <summary>
		/// A list of strings containing the allowed responses from the operator.
		/// </summary>
		public IReadOnlyList<string> AllowedResponses { get => _AllowedResponses; set => _AllowedResponses = value; }


		/// <summary>
		/// Notifies the system of the user selected response.
		/// </summary>
		/// <param name="response"></param>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="response"/> is null.</exception>
		/// <exception cref="System.ArgumentException">Thrown if <paramref name="response"/> is not in <see cref="AllowedResponses"/>.</exception>
		/// <remarks>
		/// <para>Async/await or other threading code can be used in the event handler without interrupting the transaction flow. The answer does not need to be provided before the event handler method ends, so long as the event arguments instance is captured in the event handler and <see cref="SetResponse(string)"/> called on it when the user has selected an answer.</para>
		/// </remarks>
		public void SetResponse(string response)
		{
			response.GuardNullOrWhiteSpace(response);
			if (!this.AllowedResponses.Contains(response)) throw new ArgumentException(String.Format(ErrorMessages.InvalidQueryResponse, response, String.Join(", ", AllowedResponses)));

			_ResponseCompletionSource.TrySetResult(response);
		}

		internal void Cancel()
		{
			_ResponseCompletionSource.TrySetCanceled();
		}

		internal Task<string> ResponseTask { get { return _ResponseCompletionSource.Task; } }
	}
} 