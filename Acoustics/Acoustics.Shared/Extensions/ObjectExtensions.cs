﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectExtensions.cs" company="QutEcoacoustics">
// All code in this file and all associated files are the copyright and property of the QUT Ecoacoustics Research Group (formerly MQUTeR, and formerly QUT Bioacoustics Research Group).
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable CheckNamespace
namespace System
// ReSharper restore CheckNamespace
{
    using Collections.Generic;
    using Diagnostics.Contracts;
    using JetBrains.Annotations;
    using Linq.Expressions;

    using Newtonsoft.Json;

    /// <summary>
    /// The object extensions.
    /// </summary>
    public static class ObjectExtensions
    {
        #region Public Methods

        /// <summary>
        /// The name of.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="propertyExpressions">
        /// The property expression.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The name of.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// </exception>
        public static IEnumerable<string> NameOf<T>(
            this T target,
            params Expression<Func<T, object>>[] propertyExpressions)
        {
            foreach (var expression in propertyExpressions)
            {
                yield return NameOf<T>(target, expression);
            }
        }

        public static string NameOf<T>(Expression<Func<T, object>> propertyExpression)
        {
            return NameOf(default(T), propertyExpression);
        }

        /// <summary>
        /// The name of.
        /// </summary>
        /// <param name="target">
        ///   The target.
        /// </param>
        /// <param name="propertyExpression">
        ///   The property expression.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The name of.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// </exception>

        public static string NameOf<T>(this T target, Expression<Func<T, object>> propertyExpression)
        {
            MemberExpression body = null;

            if (propertyExpression.Body is UnaryExpression)
            {
                var unary = propertyExpression.Body as UnaryExpression;

                if (unary.Operand is MemberExpression)
                {
                    body = unary.Operand as MemberExpression;
                }
            }
            else if (propertyExpression.Body is MemberExpression)
            {
                body = propertyExpression.Body as MemberExpression;
            }

            if (body == null)
            {
                throw new ArgumentException("'propertyExpression' should be a member expression");
            }

            // Extract the right part (after "=>")
            var vmExpression = body.Expression as ConstantExpression;

            // Extract the name of the property to raise a change on
            return body.Member.Name;
        }

        public static T JsonClone<T>(this T source)
        {
            // No settings actually set elsewhere in solution
            //var settings = JsonConvert.DefaultSettings();

            var serialized = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<T>(serialized);
        }

        [ContractAnnotation("obj:null => false; obj:notnull => true")]
        public static bool NotNull(this object obj) => obj != null;

        public static T[] AsArray<T>(this T item)
        {
            return new[] { item };
        }

        public static List<T> AsList<T>(this T item)
        {
            return new List<T> { item };
        }

        #endregion
    }
}