using System.Collections.Generic;
using System.Web.Mvc;

namespace Fulbaso.Common
{
	/// <summary>
	/// Returns a JSON result that contains exception messages.
	/// </summary>
	public sealed class ExceptionJsonResult : JsonResult
	{
		/// <summary>
		/// Returns a JSON result that contains exception messages.
		/// </summary>
		public ExceptionJsonResult(IEnumerable<string> exceptions)
		{
			Data = new
			{
				Exceptions = exceptions
			};
		}
	}
}