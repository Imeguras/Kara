using System;

namespace InstanceManager.Utility
{

    /// <summary>Provides utility functions for working with <see cref="Action"/>.</summary>
    public static class ActionUtility
    {

        /// <summary>Runs the <see cref="Func{TResult}"/> in a try catch block.</summary>
        /// <param name="action">The action to run.</param>
        /// <param name="hideError">If <see langword="true"/>, then the error won't be rethrown.</param>
        public static T Try<T>(this Func<T> func, bool hideError = false)
        {
            var result = Try(func, out var e);
            if (e != null && !hideError)
                throw e;
            return result;
        }

        /// <summary>Runs the <see cref="Func{TResult}"/> in a try catch block.</summary>
        /// <param name="func">The action to run.</param>
        /// <param name="exception">The exception that occured.</param>
        public static T Try<T>(this Func<T> func, out Exception exception)
        {
            T obj = default;
            TryInternal(action: () => obj = func.Invoke(), out exception);
            return obj;
        }

        /// <summary>Runs the <see cref="Action"/> in a try catch block.</summary>
        /// <param name="action">The action to run.</param>
        /// <param name="hideError">If <see langword="true"/>, then the error won't be rethrown.</param>
        public static void Try(this Action action, bool hideError = false)
        {
            TryInternal(action, out var e);
            if (e != null && !hideError)
                throw e;
        }

        /// <summary>Runs the <see cref="Action"/> in a try catch block.</summary>
        /// <param name="action">The action to run.</param>
        /// <param name="exception">The exception that occured.</param>
        public static void Try(this Action action, out Exception exception) =>
            TryInternal(action, out exception);

        static void TryInternal(this Action action, out Exception exception)
        {

            exception = null;
            try
            {
                action?.Invoke();
            }
            catch (Exception ex)
            {
                exception = ex;
            }

        }

    }

}
