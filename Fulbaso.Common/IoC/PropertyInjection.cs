﻿using System;
using Fulbaso.Common.Resources;

namespace Fulbaso.Common
{
    /// <summary>
    /// Reflection helpers to help deal with attributes.
    /// </summary>
    public static class PropertyInjectionHelpers
    {
        /// <summary>
        /// Helper method to remove verbosity from properties intended to be used in dependency injection scenarios.
        /// </summary>
        public static T GetInjectedProperty<T>(this T property, string propertyName) where T : class
        {
            if (property == null)
            {
                throw new ArgumentNullException(propertyName);
            }
            else
            {
                return property;
            }
        }

        /// <summary>
        /// Helper method to remove verbosity from properties intended to be used in dependency injection scenarios.
        /// </summary>
        public static T InjectProperty<T>(this T property, T value, string propertyName) where T : class
        {
            if (value == null)
            {
                throw new ArgumentNullException(propertyName);
            }
            else if (property == null)
            {
                return value;
            }
            else
            {
                throw new InvalidOperationException(string.Format(Error.DuplicatePropertyInjection, propertyName));
            }
        }

        /// <summary>
        /// Helper method to remove verbosity from properties intended to be used in dependency injection scenarios.
        /// </summary>
        public static T? GetInjectedProperty<T>(this T? property, string propertyName) where T : struct
        {
            if (property == null)
            {
                throw new ArgumentNullException(propertyName);
            }
            else
            {
                return property;
            }
        }

        /// <summary>
        /// Helper method to remove verbosity from properties intended to be used in dependency injection scenarios.
        /// </summary>
        public static T? InjectProperty<T>(this T? property, T? value, string propertyName) where T : struct
        {
            if (value == null)
            {
                throw new ArgumentNullException(propertyName);
            }
            else if (property == null)
            {
                return value;
            }
            else
            {
                throw new InvalidOperationException(string.Format(Error.DuplicatePropertyInjection, propertyName));
            }
        }
    }
}
