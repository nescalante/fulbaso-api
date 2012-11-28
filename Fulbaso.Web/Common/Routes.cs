using System.Web.Mvc;
using System.Web.Routing;

namespace Fulbaso.Web
{
    internal static class Routes
    {
        internal static void Map(this RouteCollection routes)
        {
            // user routes
            routes.MapRoute("LogInRoute", "login", new { controller = "Home", action = "LogIn", });
            routes.MapRoute("LogOutRoute", "cerrarsesion", new { controller = "Home", action = "LogOut", });
            routes.MapRoute("Error404Route", "404", new { controller = "Home", action = "Error404", });

            // search routes
            routes.MapRoute("AdvancedSearchRoute", "buscador", new { controller = "Search", action = "Advanced" });
            routes.MapRoute("AdvancedSearchResultRoute", "b", new { controller = "Search", action = "Schedule" });

            // court routes
            routes.MapRoute("DeleteCourtRoute", "p/{place}/canchas/eliminar", new { controller = "Court", action = "Delete", });
            routes.MapRoute("CourtAddRoute", "p/{place}/canchas/agregar", new { controller = "Court", action = "Add", });
            routes.MapRoute("CourtsListRoute", "p/{place}/canchas", new { controller = "Court", action = "Index", });
            routes.MapRoute("CourtEditRoute", "p/{place}/cancha/{court}", new { controller = "Court", action = "Edit", });

            // place routes
            routes.MapRoute("DeleteFavouriteRoute", "eliminarfavorito", new { controller = "Place", action = "DeleteFavourite", });
            routes.MapRoute("AddFavouriteRoute", "agregarfavorito", new { controller = "Place", action = "AddFavourite", });
            routes.MapRoute("AddPlace", "agregarcomplejo", new { controller = "Place", action = "Add", });
            routes.MapRoute("PlaceEditRoute", "p/{place}/editar", new { controller = "Place", action = "Edit", });
            routes.MapRoute("PlaceCalendarRoute", "p/{place}/agenda", new { controller = "Place", action = "Schedule" });
            routes.MapRoute("PlaceGetNearest", "p/{place}/cercanas", new { controller = "Place", action = "GetNearest", });
            routes.MapRoute("PlaceGetNearestLayout", "p/{place}/distancias", new { controller = "Place", action = "GetNearestLayout", });
            routes.MapRoute("PlaceGetNearestList", "listarcercanas", new { controller = "Place", action = "GetNearestList", });
            routes.MapRoute("PlaceInfoViewRoute", "infoview", new { controller = "Place", action = "InfoView", });
            routes.MapRoute("PlaceGetNearestFromLocationRoute", "cercanas", new { controller = "Place", action = "GetNearestFromLocation", });
            routes.MapRoute("PlaceSearchRoute", "buscar", new { controller = "Home", action = "Find", });

            // schedule routes
            routes.MapRoute("ClientsListRoute", "p/{place}/clientes", new { controller = "Client", action = "Index", });
            routes.MapRoute("BookEditRoute", "agendar", new { controller = "Schedule", action = "EditBook", });
            routes.MapRoute("BookDeleteRoute", "eliminaragenda", new { controller = "Schedule", action = "DeleteBook", });
            routes.MapRoute("CalendarRoute", "p/{place}/calendario", new { controller = "Schedule", action = "Index", });

            // images routes
            routes.MapRoute("ImagesFromURLRoute", "p/{place}/imagenes/agregardesdeurl", new { controller = "Image", action = "FromURL", });
            routes.MapRoute("ImagesFromFileRoute", "p/{place}/imagenes/agregardesdearchivo", new { controller = "Image", action = "FromFile", });
            routes.MapRoute("ImagesRoute", "p/{place}/imagenes", new { controller = "Image", action = "Index", });

            // configuration routes
            routes.MapRoute("ConfigurationRoute", "p/{place}/config/{court}", new { controller = "Configuration", action = "Index", });
            routes.MapRoute("ConfigurationEditRout", "p/{place}/config/{court}/editar/{config}", new { controller = "Configuration", action = "Edit", });
            routes.MapRoute("ConfigurationAddRoute", "p/{place}/config/{court}/agregar", new { controller = "Configuration", action = "Add", });

            // misc
            routes.MapRoute("PlaceRoute", "p/{place}", new { controller = "Place", action = "ItemView", });
            routes.MapRoute("PlaceListRoute", "canchas", new { controller = "Home", action = "List", });
            routes.MapRoute("SetPositionRoute", "posicionar", new { controller = "Home", action = "SetPosition", });
            routes.MapRoute("MapRoute", "mapa", new { controller = "Place", action = "Map" });
            routes.MapRoute("AdminRoute", "admin", new { controller = "Home", action = "Admin" });
            routes.MapRoute("Default", "{controller}/{action}", new { controller = "Home", action = "Index" });
        }
    }
}