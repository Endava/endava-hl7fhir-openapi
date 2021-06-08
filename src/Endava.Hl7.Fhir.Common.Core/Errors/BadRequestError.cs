using Endava.Hl7.Fhir.Common.Core.Errors.Base;
using Microsoft.AspNetCore.Http;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Endava.Hl7.Fhir.Common.Core.Errors
{
	[ExcludeFromCodeCoverage]
	public class BadRequestError : ApiError
	{
		public BadRequestError()
			: base(StatusCodes.Status400BadRequest, HttpStatusCode.BadRequest.ToString())
		{
		}

		public BadRequestError(string message)
			: base(StatusCodes.Status400BadRequest, HttpStatusCode.BadRequest.ToString(), message)
		{
		}
	}
}
