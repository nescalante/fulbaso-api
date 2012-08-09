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
            routes.MapRoute("AdminRoute", "admin/{id}", new { controller = "Home", action = "Admin" });

            // search routes
            routes.MapRoute("AdvancedSearchRoute", "buscador", new { controller = "Search", action = "Advanced" });
            routes.MapRoute("AdvancedSearchResultRoute", "b", new { controller = "Search", action = "Schedule" });

            // court routes
            routes.MapRoute("DeleteCourtRoute", "editar/{id}/eliminar", new { controller = "Court", action = "Delete", });
            routes.MapRoute("CourtsListRoute", "editar/{id}/canchas", new { controller = "Court", action = "Index", });
            routes.MapRoute("CourtEditRoute", "editar/{id}/cancha/{court}", new { controller = "Court", action = "Edit", });
            routes.MapRoute("CourtAddRoute", "editar/{id}/agregar", new { controller = "Court", action = "Add", });

            // place routes
            routes.MapRoute("DeleteFavouriteRoute", "eliminarfavorito", new { controller = "Place", action = "DeleteFavourite", });
            routes.MapRoute("AddFavouriteRoute", "agregarfavorito", new { controller = "Place", action = "AddFavourite", });
            routes.MapRoute("PlaceEditRoute", "editar/{id}", new { controller = "Place", action = "Edit", });
            routes.MapRoute("PlaceCalendarRoute", "p/{id}/agenda", new { controller = "Place", action = "Schedule", current = UrlParameter.Optional });
            routes.MapRoute("PlaceRoute", "p/{id}", new { controller = "Place", action = "ItemView", });

            // schedule routes
            routes.MapRoute("ClientsListRoute", "clientes/{id}", new { controller = "Client", action = "Index", });
            routes.MapRoute("BookEditRoute", "agendar", new { controller = "Schedule", action = "EditBook", });
            routes.MapRoute("BookDeleteRoute", "eliminaragenda", new { controller = "Schedule", action = "DeleteBook", });
            routes.MapRoute("CalendarRoute", "calendario/{id}", new { controller = "Schedule", action = "Index", current = UrlParameter.Optional });

            // configuration routes
            routes.MapRoute("ConfigurationRoute", "editar/{id}/config/{court}", new { controller = "Configuration", action = "Index", });
            routes.MapRoute("ConfigurationEditRout", "editar/{id}/config/{court}/editar/{config}", new { controller = "Configuration", action = "Edit", });
            routes.MapRoute("ConfigurationAddRoute", "editar/{id}/config/{court}/agregar", new { controller = "Configuration", action = "Add", });

            routes.MapRoute("PlacesQuery", "q={query}", new { controller = "Home", action = "Search", });
            //, string p, string j, string s, string l, bool? ind, bool? lig)
            routes.MapRoute("Default", "{controller}/{action}", new { controller = "Home", action = "Index" });
        }
    }
}