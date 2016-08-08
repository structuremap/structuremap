using System;
using System.Diagnostics;

namespace StructureMap
{
    [DebuggerStepThrough]
    internal static class ArgumentChecker
    {
        /// <summary>
        /// Utility method to throw <see cref="ArgumentNullException"/> if the argument is <see langword="null"/>.
        /// </summary>
        /// <param name="argumentName">The argument name.</param>
        /// <param name="value">The argument value.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="value"/> is <see langword="null"/>.</exception>
        public static void ThrowIfNull(string argumentName, object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(argumentName, $"[{argumentName}] cannot be null.");
            }
        }

        /// <summary>
        /// Utility method to throw <see cref="ArgumentNullException"/> if the argument is <see langword="null"/> or
        /// <see cref="ArgumentException"/> if the argument is an empty string.
        /// </summary>
        /// <param name="argumentName">The argument name.</param>
        /// <param name="value">The argument value.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="value"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="value"/> is an empty string.</exception>
        public static void ThrowIfNullOrEmptyString(string argumentName, string value)
        {
            ThrowIfNull(argumentName, value);

            if (value.Length == 0)
            {
                throw new ArgumentException($"[{argumentName}] cannot be an empty string.", argumentName);
            }
        }
    }
}