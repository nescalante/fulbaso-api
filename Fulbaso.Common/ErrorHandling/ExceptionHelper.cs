using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web;
using System.Web.Routing;


namespace Fulbaso.Common
{
    public class ExceptionHelper
    {
        private readonly HttpContextBase context;

        public ExceptionHelper(HttpContextBase context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            this.context = context;
        }

        /// <summary>
        /// Gets all inner exceptions for the current exception, not including itself.
        /// </summary>
        internal Stack<Exception> GetExceptionStack(Exception exception)
        {
            Stack<Exception> stack = new Stack<Exception>();
            Exception inner = exception.InnerException;
            while (inner != null)
            {
                stack.Push(inner);
                inner = inner.InnerException;
            }
            return stack;
        }

        /// <summary>
        /// Examines the exception inside out (innermost-first) and returns the most reasonable explanation about what is going on to the user,
        /// without actually disclosing any sensitive information about the error that was raised.
        /// </summary>
        public string GetMessage(Exception exception, bool ajax)
        {
            Stack<Exception> stack = GetExceptionStack(exception);
            while (exception != null)
            {
                string specificMessage = GetSpecificExceptionMessage(exception, ajax);
                if (!string.IsNullOrEmpty(specificMessage))
                {
                    return specificMessage;
                }
                if (stack.Count == 0)
                {
                    break;
                }
                exception = stack.Pop();
            }

            string genericMessage = "Unhandled Exception"; // generic default exception response
            if (ajax)
            {
                genericMessage = "Unhandled Ajax Exception";
            }

            return genericMessage;
        }

        internal string GetSpecificExceptionMessage(Exception exception, bool ajax)
        {
            if (exception == null)
            {
                return null;
            }
            string errorMessage = null;

            bool argument = exception is ArgumentException;

            if (exception is ApplicationException)
            {
                errorMessage = exception.Message;
            }
            else if (ajax && argument)
            {
                errorMessage = "Bad Ajax Request";
            }
            else if (!ajax && argument)
            {
                errorMessage = "Bad Request";
            }
            else if (exception is SqlException)
            {
                errorMessage = "Database Error";
            }
            else if (exception.IsHttpNotFound())
            {
                errorMessage = "Web Resource Not Found";
            }

            return errorMessage;
        }

        public ErrorViewModel GetErrorViewModel(RouteData data, Exception exception, bool ajax = false)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            string controllerName = data.GetControllerString(Resources.Error.EmptyController);
            string actionName = data.GetActionString(Resources.Error.EmptyAction);
            string errorMessage = GetMessage(exception, ajax);
            ErrorViewModel model = new ErrorViewModel(context, exception, controllerName, actionName, errorMessage);
            return model;
        }
    }
}
