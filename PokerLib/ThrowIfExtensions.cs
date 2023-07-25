using System;
using System.Diagnostics.CodeAnalysis;

// These are copied from a github repo I own (https://github.com/AbandonedRickshaw/ThrowIf) for simplicity.  Normally they'd come in as a NuGet package
namespace Armstrong.Poker
{
    public static class ThrowIfExtensions
    {
        /// <summary>
        /// Ensures that the specified string argument is not null or empty.
        /// </summary>
        /// <param name="argument">The argument to test</param>
        /// <param name="argumentName">The name of the argument to test</param>
        public static string ThrowIfNullOrEmpty(this string argument, string argumentName = "unspecified")
        {
            return argument.ThrowIfNullOrEmpty(new ArgumentException("String arguement cannot be null and must contain at least one character.", argumentName));
        }

        /// <summary>
        /// Ensures that the specified string argument is not null or empty.
        /// </summary>
        /// <param name="argument">The argument to test</param>
        /// <param name="exception">The exception to throw</param>
        public static string ThrowIfNullOrEmpty(this string argument, Exception exception)
        {
            if (string.IsNullOrEmpty(argument))
            {
                throw exception;
            }

            return argument;
        }

        /// <summary>
        /// Ensures that the specified string argument is not null, empty, or whitespace.
        /// </summary>
        /// <param name="argument">The argument to test</param>
        /// <param name="argumentName">The name of the argument to test</param>
        public static string ThrowIfNullOrWhitespace(this string argument, string argumentName = "unspecified")
        {
            return argument.ThrowIfNullOrWhitespace(new ArgumentException("String arguement cannot be null and must contain at least one non-whitespace character.", argumentName));
        }

        /// <summary>
        /// Ensures that the specified string argument is not null, empty, or whitespace.
        /// </summary>
        /// <param name="argument">The argument to test</param>
        /// <param name="exception">The exception to throw</param>
        public static string ThrowIfNullOrWhitespace(this string argument, Exception exception)
        {
            if (string.IsNullOrWhiteSpace(argument))
            {
                throw exception;
            }

            return argument;
        }

        /// <summary>
        /// Ensures that the specified argument is not null (or default for value types)
        /// </summary>
        /// <typeparam name="T">The type of argument.  T must be an Enum.</typeparam>
        /// <param name="argument">The argument to test</param>
        /// <param name="argumentName">The name of the argument to test</param>
        public static T ThrowIfNullOrDefault<T>(this T argument, string argumentName = "unspecified")
        {
            return argument.ThrowIfNullOrDefault(new ArgumentException($"Argument cannot be {(argument == null ? "null" : $"'{default(T)}'")}.", argumentName));
        }

        /// <summary>
        /// Ensures that the specified argument is not null (or default for value types)
        /// </summary>
        /// <typeparam name="T">The type of argument.  T must be an Enum.</typeparam>
        /// <param name="argument">The argument to test</param>
        /// <param name="exception">The exception to throw</param>
        public static T ThrowIfNullOrDefault<T>(this T argument, Exception exception)
        {
            if (argument == null)
                throw exception;

            if (typeof(T).IsValueType)
            {
                if (argument.Equals(default(T)))
                    throw exception;
            }

            return argument;
        }

        /// <summary>
        /// Ensures that the specified argument is defined in an enumeration
        /// </summary>
        /// <typeparam name="T">The enumeration type to test</typeparam>
        /// <param name="argument">The argument to test</param>
        /// <param name="argumentName">The name of the argument to test</param>
        public static T ThrowIfOutOfRange<T>(this T argument, string argumentName = "unspecified")
            where T : struct, Enum
        {
            return argument.ThrowIfOutOfRange(new ArgumentOutOfRangeException(argumentName));
        }

        /// <summary>
        /// Ensures that the specified argument is defined in an enumeration
        /// </summary>
        /// <typeparam name="T">The enumeration type to test</typeparam>
        /// <param name="argument">The argument to test</param>
        /// <param name="exception">The exception to throw</param>
        public static T ThrowIfOutOfRange<T>(this T argument, Exception exception)
            where T : struct, Enum
        {
            if (!Enum.IsDefined(typeof(T),argument))
            {
                throw exception;
            }

            return argument;
        }

        /// <summary>
        /// Ensures that a value is within a specified range
        /// </summary>
        /// <typeparam name="T">The type of argument.  T must implement <see cref="IComparable{T}"/></typeparam>
        /// <param name="argument">The argument to test</param>
        /// <param name="minValue">The minimum value T can be</param>
        /// <param name="maxValue">The maximum value T can be</param>
        /// <param name="argumentName">The name of the argument to test</param>
        public static T ThrowIfOutOfRange<T>(this T argument, T minValue, T maxValue, string argumentName = "unspecified")
            where T : IComparable<T>
        {
            return argument.ThrowIfOutOfRange(minValue, maxValue, new ArgumentOutOfRangeException(argumentName));
        }

        /// <summary>
        /// Ensures that a value is within a specified range
        /// </summary>
        /// <typeparam name="T">The type of argument.  T must implement <see cref="IComparable{T}"/></typeparam>
        /// <param name="argument">The argument to test</param>
        /// <param name="minValue">The minimum value T can be</param>
        /// <param name="maxValue">The maximum value T can be</param>
        /// <param name="exception">The exception to throw</param>
        public static T ThrowIfOutOfRange<T>(this T argument, T minValue, T maxValue, Exception exception)
            where T : IComparable<T>
        {
            if (argument.CompareTo(minValue) < 0 || argument.CompareTo(maxValue) > 0)
            {
                throw exception;
            }

            return argument;
        }

        /// <summary>
        /// Throws an exception if a condition evaluates to TRUE
        /// </summary>
        /// <typeparam name="T">The type of argument.</typeparam>
        /// <param name="argument">The argument to test</param>
        /// <param name="condition">The test</param>
        /// <param name="argumentName">The name of the argument to test</param>
        public static T ThrowIfTrue<T>(this T argument, Func<T, bool> condition, string argumentName = "unspecified")=>
            argument.ThrowIfTrue(condition, new ArgumentException("The argument is invalid.", argumentName));

		/// <summary>
		/// Throws an exception if a condition evaluates to TRUE
		/// </summary>
		/// <typeparam name="T">The type of argument.</typeparam>
		/// <param name="argument">The argument to test</param>
		/// <param name="condition">The test</param>
		/// <param name="exception"></param>
		/// <param name="exception">The exception to throw</param>
		public static T ThrowIfTrue<T>(this T argument, Func<T, bool> condition, Exception exception)=>
            condition(argument) ? throw exception : argument;

		/// <summary>
		/// Throws an exception if a condition evaluates to FALSE
		/// </summary>
		/// <typeparam name="T">The type of argument.</typeparam>
		/// <param name="argument">The argument to test</param>
		/// <param name="condition">The test</param>
		/// <param name="argumentName">The name of the argument to test</param>
		public static T ThrowIfFalse<T>(this T argument, Func<T, bool> condition, string argumentName = "unspecified") =>
			argument.ThrowIfFalse(condition, new ArgumentException("The argument is invalid.", argumentName));

		/// <summary>
		/// Throws an exception if a condition evaluates to FALSE
		/// </summary>
		/// <typeparam name="T">The type of argument.</typeparam>
		/// <param name="argument">The argument to test</param>
		/// <param name="condition">The test</param>
		/// <param name="exception"></param>
		/// <param name="exception">The exception to throw</param>
		public static T ThrowIfFalse<T>(this T argument, Func<T, bool> condition, Exception exception) =>
			condition(argument) ? argument : throw exception;
	}
}
