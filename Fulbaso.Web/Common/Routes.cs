using System.Web.Mvc;
using System.Web.Routing;

namespace Fulbaso.UI
{
    internal static class Routes
    {
        internal static void Map(this RouteCollection routes)
        {
            // user routes
            routes.MapRoute("LogOutRoute", "cerrarsesion", new { controller = "Home", action = "LogOut", });
            routes.MapRoute("Error404Route", "404", new { controller = "Home", action = "Error404", });

            // search routes
            routes.MapRoute("AdvancedSearchRoute", "buscador", new { controller = "Search", action = "Advanced" });
            routes.MapRoute("AdvancedSearchResultRoute", "b", new { controller = "Search", action = "Schedule" });

            // court routes
            routes.MapRoute("DeleteCourtRoute", "admin/{place}/canchas/eliminar", new { controller = "Court", action = "Delete", });
            routes.MapRoute("CourtAddRoute", "admin/{place}/canchas/agregar", new { controller = "Court", action = "Add", });
            routes.MapRoute("CourtsListRoute", "admin/{place}/canchas", new { controller = "Court", action = "Index", });
            routes.MapRoute("CourtEditRoute", "admin/{place}/cancha/{court}", new { controller = "Court", action = "Edit", });

            // place routes
            routes.MapRoute("DeleteFavouriteRoute", "eliminarfavorito", new { controller = "Place", action = "DeleteFavourite", });
            routes.MapRoute("AddFavouriteRoute", "agregarfavorito", new { controller = "Place", action = "AddFavourite", });
            routes.MapRoute("PlaceEditRoute", "admin/{place}/editar", new { controller = "Place", action = "Edit", });
            routes.MapRoute("PlaceCalendarRoute", "p/{place}/agenda", new { controller = "Place", action = "Schedule" });
            routes.MapRoute("PlaceGetNearest", "p/{place}/cercanas", new { controller = "Place", action = "GetNearest", });
            routes.MapRoute("PlaceGetNearestLayout", "p/{place}/distancias", new { controller = "Place", action = "GetNearestLayout", });
            routes.MapRoute("PlaceInfoViewRoute", "infoview", new { controller = "Place", action = "InfoView", });
            routes.MapRoute("PlaceGetNearestFromLocationRoute", "cercanas", new { controller = "Place", action = "GetNearestFromLocation", });
            routes.MapRoute("PlaceRoute", "p/{place}", new { controller = "Place", action = "ItemView", });
            routes.MapRoute("PlaceSearchRoute", "buscar", new { controller = "Home", action = "Find", });

            // schedule routes
            routes.MapRoute("ClientsListRoute", "admin/{place}/clientes", new { controller = "Client", action = "Index", });
            routes.MapRoute("BookEditRoute", "agendar", new { controller = "Schedule", action = "EditBook", });
            routes.MapRoute("BookDeleteRoute", "eliminaragenda", new { controller = "Schedule", action = "DeleteBook", });
            routes.MapRoute("CalendarRoute", "admin/{place}/calendario", new { controller = "Schedule", action = "Index", });

            // configuration routes
            routes.MapRoute("ConfigurationRoute", "admin/{place}/config/{court}", new { controller = "Configuration", action = "Index", });
            routes.MapRoute("ConfigurationEditRout", "admin/{place}/config/{court}/editar/{config}", new { controller = "Configuration", action = "Edit", });
            routes.MapRoute("ConfigurationAddRoute", "admin/{place}/config/{court}/agregar", new { controller = "Configuration", action = "Add", });

            // misc
            routes.MapRoute("MapRoute", "mapa", new { controller = "Places", action = "Map" });
            routes.MapRoute("AdminRoute", "admin/{place}", new { controller = "Home", action = "Admin" });
            routes.MapRoute("Default", "{controller}/{action}", new { controller = "Home", action = "Index" });
        }
    }
}