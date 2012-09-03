using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;
using System.Data.Linq;
using Fulbaso.Importer;
using Fulbaso.EntityFramework.Logic;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            new HoySeJuegaImporter(new InternalConsole(), new PlaceService(new CourtBookService(new ClientService())), new CourtService(), new TerritoryService(), new RegionService(), new LocationService()).Import();
        }
    }
}
