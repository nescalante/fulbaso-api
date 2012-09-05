namespace Fulbaso.Web.Models
{
    public class ChangePasswordModel
    {
        public string OldPassword { get; set; }

        public string NewPassword { get; set; }

        public string RepeatPassword { get; set; }
    }

    public class BreadcrumbModel
    {
        public bool Enabled { get; set; }

        public string Description { get; set; }

        public string Action { get; set; }

        public string Controller { get; set; }

        public object RouteValues { get; set; }

        public static BreadcrumbModel Add(string desc)
        {
            return new BreadcrumbModel { Description = desc, Enabled = true, };
        }

        public static BreadcrumbModel Add(string desc, string action, string controller, object routeValues)
        {
            return new BreadcrumbModel { Description = desc, Enabled = true, Action = action, Controller = controller, RouteValues = routeValues, };
        }
    }
}
