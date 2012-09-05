using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using System.Collections.Generic;
using Fasterflect;

namespace Fulbaso.Web
{
    public static class ImageExtensions
    {
        public static MvcHtmlString Image(this HtmlHelper helper, string imageRelativeUrl, string alt, object htmlAttributes)
        {
            return Image(helper, imageRelativeUrl, alt, new RouteValueDictionary(htmlAttributes));
        }

        public static MvcHtmlString Image(this HtmlHelper helper, string imageRelativeUrl, string alt, IDictionary<string, object> htmlAttributes)
        {
            if (String.IsNullOrEmpty(imageRelativeUrl))
            {
                throw new ArgumentException("imageRelativeUrl");
            }

            string imageUrl = UrlHelper.GenerateContentUrl(imageRelativeUrl, helper.ViewContext.HttpContext);
            return MvcHtmlString.Create(Image(imageUrl, alt, htmlAttributes).ToString(TagRenderMode.SelfClosing));
        }

        public static TagBuilder Image(string imageUrl, string alt, IDictionary<string, object> htmlAttributes)
        {
            if (String.IsNullOrEmpty(imageUrl))
            {
                throw new ArgumentException("imageUrl");
            }

            TagBuilder imageTag = new TagBuilder("img");

            if (!String.IsNullOrEmpty(imageUrl))
            {
                imageTag.MergeAttribute("src", imageUrl);
            }

            if (!String.IsNullOrEmpty(alt))
            {
                imageTag.MergeAttribute("alt", alt);
            }

            imageTag.MergeAttributes(htmlAttributes, true);

            if (imageTag.Attributes.ContainsKey("alt") && !imageTag.Attributes.ContainsKey("title"))
            {
                imageTag.MergeAttribute("title", (imageTag.Attributes["alt"] ?? "").ToString());
            }
            return imageTag;
        }
    }


}