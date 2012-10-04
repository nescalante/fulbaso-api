using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using log4net;
using Fulbaso.Common.Resources;

namespace Fulbaso.Common
{
    public class HttpApplicationErrorHandler
    {
        private readonly ILog log = LogManager.GetLogger(typeof(HttpApplicationErrorHandler));
        private readonly HttpApplication application;
        private readonly ExceptionHelper helper;

        public HttpApplicationErrorHandler(HttpApplication application, ExceptionHelper helper)
        {
            if (application == null)
            {
                throw new ArgumentNullException("application");
            }
            if (helper == null)
            {
                throw new ArgumentNullException("helper");
            }
            this.application = application;
            this.helper = helper;
        }

        public void HandleApplicationError()
        {
            HttpContextWrapper context = new HttpContextWrapper(HttpContext.Current);
            using (ErrorController controller = ErrorController.Instance(context))
            {
                Exception exception = application.Server.GetLastError();
                if (exception != null) // prevent bizarre scenario when handling requests to *.cshtml physical files.
                {
                    application.Response.Clear();
                    application.Response.Status = Constants.HttpServerError;
                    application.Response.TrySkipIisCustomErrors = true;

                    LogApplicationException(application.Response, exception);
                    try
                    {
                        WriteViewResponse(exception, controller);
                    }
                    catch (Exception exceptionWritingView) // now we're in trouble. lets be as graceful as possible.
                    {
                        WriteGracefulResponse(exceptionWritingView, controller);
                    }
                    finally
                    {
                        application.Server.ClearError();
                    }
                }
            }
        }

        private void LogApplicationException(HttpResponse response, Exception exception)
        {
            if (exception.IsHttpNotFound())
            {
                log.Debug(Error.WebResourceNotFound, exception);
                response.Status = Constants.HttpNotFound;
                return;
            }
            log.Error(Error.UnhandledException, exception);
        }

        private bool WriteJsonResponse(string message)
        {
            if (application.Request.IsAjaxRequest())
            {
                application.Response.Status = Constants.HttpSuccess;
                application.Response.ContentType = Constants.JsonContentType;
                application.Response.Write(message);
                return true;
            }
            return false;
        }

        private void WriteViewResponse(Exception exception, StringRenderingController controller)
        {
            if (!WriteJsonResponse(User.UnhandledExceptionJson))
            {
                ErrorViewModel model = helper.GetErrorViewModel(controller.RouteData, exception);
                string result = controller.ViewString(Constants.ErrorViewName, model);
                application.Response.ContentType = Constants.HtmlContentType;
                application.Response.Write(result);
            }
        }

        private void WriteGracefulResponse(Exception exception, ErrorController controller, bool clear = false)
        {
            try
            {
                // write an HTML response from an embedded resource in the assembly.
                WriteHtmlResponse(exception, controller);
            }
            catch (Exception exceptionWritingHtml) // we seem to be having a very rough day, lets just call it a day.
            {
                // write a plain text response.
                WritePlainTextResponse(exceptionWritingHtml);
            }
            finally
            {
                if (clear)
                {
                    application.Server.ClearError();
                }
            }
        }

        private void WriteHtmlResponse(Exception exceptionWritingView, Controller controller)
        {
            application.Response.Clear();

            if (!WriteJsonResponse(User.UnhandledExceptionJson))
            {
                ErrorViewModel model = helper.GetErrorViewModel(controller.RouteData, exceptionWritingView);
                string html = GetHtmlResponse(model);

                application.Response.ContentType = Constants.HtmlContentType;
                application.Response.Write(html);
            }
            log.Fatal(Error.FatalException, exceptionWritingView);
        }

        private void WritePlainTextResponse(Exception exceptionWritingHtml)
        {
            application.Response.Clear();

            if (!WriteJsonResponse(User.FatalExceptionJson))
            {
                application.Response.ContentType = Constants.PlainTextContentType;
                application.Response.Write(User.FatalException);
            }
            log.Fatal(Error.FatalException, exceptionWritingHtml);
        }

        private string GetHtmlResponse(ErrorViewModel model)
        {
            string html = GetEmbeddedHtmlTemplate(Constants.UnrecoverableViewName);
            string htmlForException = model.DisplayException ? GetHtmlException(model.Exception) : string.Empty;
            html = html.Replace(Unrecoverable.ModelTitle, HttpUtility.HtmlEncode(User.FatalException));
            html = html.Replace(Unrecoverable.ModelRefresh, HttpUtility.HtmlEncode(User.Refresh));
            html = html.Replace(Unrecoverable.ModelMessage, HttpUtility.HtmlEncode(model.Message));
            html = html.Replace(Unrecoverable.ModelException, htmlForException);
            return html;
        }

        public string GetHtmlException(Exception exception)
        {
            if (exception == null)
            {
                return string.Empty;
            }
            StringBuilder stackTrace = new StringBuilder();
            string sqlHtml = string.Empty;
            string stackTraceHtml = string.Empty;

            if (exception.Data.Contains("SQL"))
            {
                string sql = exception.Data["SQL"].ToString();
                sqlHtml = string.Format(Unrecoverable.Sql, HttpUtility.HtmlEncode(sql));
            }
            if (exception.StackTrace != null)
            {
                string[] lines = exception.StackTrace.SplitOnNewLines();
                foreach (string line in lines)
                {
                    stackTrace.AppendFormat(Unrecoverable.StackTraceLine, HttpUtility.HtmlEncode(line.Trim()));
                }
                stackTraceHtml = string.Format(Unrecoverable.StackTrace, stackTrace);
            }

            string innerException = GetHtmlException(exception.InnerException);
            if (!string.IsNullOrWhiteSpace(innerException))
            {
                innerException = string.Format(Unrecoverable.InnerException, innerException);
            }
            string html = GetEmbeddedHtmlTemplate(Constants.UnrecoverableExceptionViewName);
            html = html.Replace(Unrecoverable.ModelMessage, HttpUtility.HtmlEncode(exception.Message));
            html = html.Replace(Unrecoverable.ModelSql, sqlHtml);
            html = html.Replace(Unrecoverable.ModelStackTrace, stackTraceHtml);
            html = html.Replace(Unrecoverable.ModelInnerException, innerException);
            return html;
        }

        private string GetEmbeddedHtmlTemplate(string viewName)
        {
            Type type = typeof(HttpApplicationErrorHandler);
            Assembly assembly = type.Assembly;
            Stream stream = assembly.GetManifestResourceStream(type, viewName);
            string html = stream.GetString(); // we don't use RazorEngine here, to avoid any complications in case what's faulty is RazorEngine itself.
            return html;
        }
    }
}
