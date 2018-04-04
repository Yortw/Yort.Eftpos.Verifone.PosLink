using Ladon;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.Eftpos.Verifone.PosLink
{
	/// <summary>
	/// Represents a request to commit any pending tip transactions for settlement. To ensure tip transactions are included in merchant settlement for the current trading day, a Tip Batch Upload will be required. This can be performed manually from the terminal or automated from the POS. 
	/// </summary>
	/// <seealso cref="PosLinkTransactionRequestBase"/>
	/// <seealso cref="TransactionResponseBase"/>
	/// <seealso cref="TipBatchUploadResponse"/>
	public class TipBatchUploadRequest : PosLinkTransactionRequestBase
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public TipBatchUploadRequest() : base()
		{
		}

		/// <summary>
		/// Returns "TBU".
		/// </summary>
		public override string RequestType { get { return ProtocolConstants.MessageType_TipBatchUpload;	}	}

		/// <summary>
		/// Not used, terminal will ignore value for this request.
		/// </summary>
		/// <remarks>
		/// <para>Limited to 10 characters.</para>
		/// </remarks>
		[PosLinkMessageField(Format = PosLinkMessageFieldFormat.Text, MaxLength = 10, Required = false, Sequence = 5)]
		public string Id { get; set; }

		/// <summary>
		/// Validates the properties of this message are valid.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Performs the following validations in addition to those provided by base classes;
		/// <list type="bullet">
		/// <item>If <see cref="Id"/>.Length is not more than 10 characters.</item>
		/// </list>
		/// </para>
		/// </remarks>
		public override void Validate()
		{
			(Id?.Length ?? 0).GuardRange(nameof(Id), nameof(Id.Length), 0, 10);

			base.Validate();
		}

	}
}