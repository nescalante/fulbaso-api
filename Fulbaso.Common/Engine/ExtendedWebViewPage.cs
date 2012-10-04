using System;
using System.Web.Mvc;
using System.Web.WebPages;
using Castle.Windsor;

namespace Fulbaso.Common
{
	/// <summary>
	/// Our implementation of view page base.
	/// </summary>
	public abstract class ExtendedWebViewPage : ExtendedWebViewPage<dynamic>
	{
	}

	/// <summary>
	/// Our implementation of view page base.
	/// </summary>
	public abstract class ExtendedWebViewPage<TModel> : WebViewPage<TModel>
	{
		/// <summary>
		/// Gets or sets the title for this view.
		/// </summary>
		public string Title
		{
			get { return ViewBag.Title ?? Resource.Shared("Application", "Title"); }
			set { ViewBag.Title = value; }
		}

		private MvcResourceHelper resource;
		public MvcResourceHelper Resource
		{
			get { return resource.GetInjectedProperty("resource"); }
			private set { resource = resource.InjectProperty(value, "resource"); }
		}

		private JavaScriptHelper javascript;
		public JavaScriptHelper JavaScript
		{
			get { return javascript.GetInjectedProperty("javascript"); }
			private set { javascript = javascript.InjectProperty(value, "javascript"); }
		}

		public override void InitHelpers()
		{
			base.InitHelpers();

            IWindsorContainer container = ContainerUtil.GetApplicationContainer();
			Resource = container.Resolve<MvcResourceHelper>(new { htmlHelper = Html });
			JavaScript = container.Resolve<JavaScriptHelper>();
		}

		#region Section Methods

		private readonly object empty = new object();

		public HelperResult RenderSection(string sectionName, Func<object, HelperResult> defaultContent)
		{
			if (IsSectionDefined(sectionName))
			{
				return RenderSection(sectionName);
			}
			else
			{
				return defaultContent(empty);
			}
		}

		public HelperResult RedefineSection(string sectionName)
		{
			return RedefineSection(sectionName, null);
		}

		public HelperResult RedefineSection(string sectionName, Func<object, HelperResult> defaultContent)
		{
			if (IsSectionDefined(sectionName))
			{
				DefineSection(sectionName, () => Write(RenderSection(sectionName)));
			}
			else if (defaultContent != null)
			{
				DefineSection(sectionName, () => Write(defaultContent(empty)));
			}
			return new HelperResult(_ => { });
		}

		#endregion
	}
}