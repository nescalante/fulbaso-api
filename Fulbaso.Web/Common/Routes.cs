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
            routes.MapRoute("AdminRoute", "admin/{place}", new { controller = "Home", action = "Admin" });

            // search routes
            routes.MapRoute("AdvancedSearchRoute", "buscador", new { controller = "Search", action = "Advanced" });
            routes.MapRoute("AdvancedSearchResultRoute", "b", new { controller = "Search", action = "Schedule" });

            // court routes
            routes.MapRoute("DeleteCourtRoute", "editar/{place}/eliminar", new { controller = "Court", action = "Delete", });
            routes.MapRoute("CourtsListRoute", "editar/{place}/canchas", new { controller = "Court", action = "Index", });
            routes.MapRoute("CourtEditRoute", "editar/{place}/cancha/{court}", new { controller = "Court", action = "Edit", });
            routes.MapRoute("CourtAddRoute", "editar/{place}/agregar", new { controller = "Court", action = "Add", });

            // place routes
            routes.MapRoute("DeleteFavouriteRoute", "eliminarfavorito", new { controller = "Place", action = "DeleteFavourite", });
            routes.MapRoute("AddFavouriteRoute", "agregarfavorito", new { controller = "Place", action = "AddFavourite", });
            routes.MapRoute("PlaceEditRoute", "editar/{place}", new { controller = "Place", action = "Edit", });
            routes.MapRoute("PlaceCalendarRoute", "p/{place}/agenda", new { controller = "Place", action = "Schedule" });
            routes.MapRoute("PlaceRoute", "p/{place}", new { controller = "Place", action = "ItemView", });

            // schedule routes
            routes.MapRoute("ClientsListRoute", "clientes/{place}", new { controller = "Client", action = "Index", });
            routes.MapRoute("BookEditRoute", "agendar", new { controller = "Schedule", action = "EditBook", });
            routes.MapRoute("BookDeleteRoute", "eliminaragenda", new { controller = "Schedule", action = "DeleteBook", });
            routes.MapRoute("CalendarRoute", "calendario/{place}", new { controller = "Schedule", action = "Index", });

            // configuration routes
            routes.MapRoute("ConfigurationRoute", "editar/{place}/config/{court}", new { controller = "Configuration", action = "Index", });
            routes.MapRoute("ConfigurationEditRout", "editar/{place}/config/{court}/editar/{config}", new { controller = "Configuration", action = "Edit", });
            routes.MapRoute("ConfigurationAddRoute", "editar/{place}/config/{court}/agregar", new { controller = "Configuration", action = "Add", });

            routes.MapRoute("Default", "{controller}/{action}", new { controller = "Home", action = "Index" });
        }
    }
}