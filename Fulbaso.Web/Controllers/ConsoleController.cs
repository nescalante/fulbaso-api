using System.Web.Mvc;
using Fulbaso.Common;
using Fulbaso.Common.Security;
using Fulbaso.Web.Console;
using Fulbaso.Importer;

namespace Fulbaso.Web.Controllers
{
    [AdminAuthorize]
    public class ConsoleController : BaseController
    {
        private IImporter _importer;

        public ConsoleController(IImporter importer)
        {
            _importer = importer;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public void Action(string args)
        {
            var arg = args.ToLower().Trim();

            switch (arg)
            {
                case "import":
                    _importer.Import();
                    break;
                default:
                    ConsoleUtil.Call("error", "La función no existe.");
                    break;
            }
        }
    }
}
