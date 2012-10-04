using System.Web.Mvc;

namespace Fulbaso.Common
{
	public class JsonRedirectResult : JsonResult
	{
		public JsonRedirectResult(RedirectResult redirectResult)
		{
			Data = new
			{
				redirect = true,
				href = redirectResult.Url
			};
		}
	}
}