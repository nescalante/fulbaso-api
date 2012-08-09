using System;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Web.Mvc;
using System.Web.UI;

namespace Fulbaso.UI
{
    /// <summary>
    /// ResourceHtmlHelpers - Extends Html class with internationalization support methods.
    /// </summary>
    /// <remarks>
    /// <ul>
    /// <li>Resource files are stored in the same path as the view, view model, or controller, in a Resources sub-directory</li>
    /// <li>Resource file has the same name as the view, with a .resx extension</li>
    /// <li>Resource file must have Access Modifier set to: Public</li>
    /// </ul>
    /// </remarks>
    /// <example>
    /// &lt;%: Html.Resource("PageTitle") %&gt; - Local resource in the /Resources directory
    /// &lt;%: Html.FormatResource("MessagesFormat", Model.Messages.Count) %&gt; - Use local resource as a format string to build a formatted string
    /// &lt;%: Html.SharedResource("Buttons", "OK") %&gt; - Shared resource in the Views/Shared/Resources directory
    /// &lt;%: Html.FormatSharedResource("SiteMaster", "TodaysDateFormat", DateTime.Now.ToLocalTime()) %&gt; - Formated shared resource
    /// </example>
    public static class ResourceHtmlHelpers
    {
        static string NamespaceRoot { get { return "Fulbaso.UI"; } }

        /// <summary>
        /// Returns the value of the request string resource from the local resource file. 
        /// </summary>
        /// <param name="helper">This class extends HtmlHelper.</param>
        /// <param name="key">The name of the string resource to return.</param>
        public static string Resource(this HtmlHelper helper, string key)
        {
            string resx = null;

            // Convention: Resources are stored in the same path as the view in a Resources directory:
            //                 + Views
            //                   + Account
            //                     + Resources
            //                        o Register.resx (Class name: [NamespaceRoot].Views.[Controller].Resources.[View])
            //                     o Register.aspx (the view)
            //
            // Convert current request path into resource type name
            if (helper.ViewDataContainer is TemplateControl || helper.ViewDataContainer is WebViewPage)
            {
                string viewPath;
                if (helper.ViewDataContainer is TemplateControl)
                {
                    viewPath = ((TemplateControl)(helper.ViewDataContainer)).AppRelativeVirtualPath;
                }
                else
                {
                    viewPath = ((WebViewPage)(helper.ViewDataContainer)).VirtualPath;
                }

                if (!String.IsNullOrEmpty(viewPath))
                {
                    // Expecting: "~/Views/User/LogOn.aspx"
                    StringBuilder sb = new StringBuilder();
                    sb.Append(NamespaceRoot);

                    string[] parts = viewPath.Split("/\\".ToCharArray());
                    foreach (string part in parts)
                    {
                        if (part == "~") continue;
                        if (part.Contains('.'))
                        {
                            sb.Append(".Resources.");
                            sb.Append(part.Substring(0, part.IndexOf('.')));
                        }
                        else
                        {
                            sb.Append('.');
                            sb.Append(part);
                        }
                    }
                    sb.Append(", ");
                    sb.Append(helper.ViewContext.Controller.GetType().Assembly.FullName);

                    string resourceTypeName = sb.ToString();

                    resx = GetStringFromType(key, resourceTypeName);
                }
            }

            // Return key if resource not found
            if (resx == null) resx = key;

            return resx;
        }

        /// <summary>
        /// Returns a formatted string using a string resource as the format string.
        /// </summary>
        /// <param name="helper">This class extends HtmlHelper.</param>
        /// <param name="key">The name of the string resource to use as the format string.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <returns></returns>
        public static string FormatResource(this HtmlHelper helper, string key, params object[] args)
        {
            string formatString = Resource(helper, key);
            return String.Format(formatString, args);
        }

        /// <summary>
        /// Returns the value of the specified string from the /Views/Shared/Resources directory
        /// </summary>
        /// <param name="helper">HtmlHelper class this class extends</param>
        /// <param name="file">Name (without extension) of the Shared resource file to read from.</param>
        /// <param name="key">Name of the string resource to get.</param>
        public static string SharedResource(this HtmlHelper helper, string key, string file)
        {
            // Convention: Shared resources are stored in the /Views/Shared/Resources directory:
            //                 + Views
            //                   + Shared
            //                     + Resources
            //                        o Buttons.resx  (view is "Buttons")
            //
            string resourceTypeName = String.Format("{0}.Views.Shared.Resources.{1}, {2}", NamespaceRoot, file,
                helper.ViewContext.Controller.GetType().Assembly.FullName);

            string resx = GetStringFromType(key, resourceTypeName);

            // Return key if resource not found
            if (resx == null) resx = key;

            return resx;
        }

        /// <summary>
        /// Returns a formatted string using a shared string resource from the /Views/Shared/Resources directory as the format string.
        /// </summary>
        /// <param name="helper">HtmlHelper class this class extends</param>
        /// <param name="file">Name (without extension) of the Shared resource file to read from.</param>
        /// <param name="key">Name of the string resource to use as the format string.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static string FormatSharedResource(this HtmlHelper helper, string key, string file, params object[] args)
        {
            string formatString = SharedResource(helper, key, file);
            return String.Format(formatString, args);
        }

        // Loads a resource from a resource file using reflection of the resource's type.
        internal static string GetStringFromType(string key, string resourceTypeName)
        {
            string resx = null;
            try
            {
                // Use reflection to get the ResourceManager property for requested resource file
                Type resourceType = Type.GetType(resourceTypeName, false, true);
                if (resourceType != null)
                {
                    ResourceManager resourceMgr = null;
                    PropertyInfo rm = resourceType.GetProperty("ResourceManager", BindingFlags.Public | BindingFlags.Static);
                    if (rm != null) resourceMgr = rm.GetValue(null, null) as ResourceManager;

                    // Get the resource value for the requested key 
                    if (resourceMgr != null)
                    {
                        resx = resourceMgr.GetString(key);
                    }
                }
            }
            catch
            {

            }
            return resx;
        }
    }
}
