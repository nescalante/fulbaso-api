﻿using System;
using System.Net;
using System.Web;

namespace Fulbaso.Common
{
    public static class ExceptionExtensions
    {
        public static bool IsHttpNotFound(this Exception exception)
        {
            if (exception is HttpException)
            {
                return ((HttpException)exception).GetHttpCode() == (int)HttpStatusCode.NotFound;
            }
            return false;
        }
    }
}
