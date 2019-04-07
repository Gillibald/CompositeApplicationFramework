#region Usings

using System;
using System.ComponentModel;

#endregion

namespace CompositeApplicationFramework.Interfaces
{
    /// <summary>
    ///     Logger interface
    /// </summary>
    public interface ILoggerFacade
    {
        #region Trace

        /// <summary>
        /// Logs a message on trace level.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments.</param>
        void Trace(IFormatProvider formatProvider, [Localizable(false)] string message, params object[] args);

        /// <summary>
        /// Logs a message on trace level.
        /// </summary>
        /// <param name="message">The message.</param>
        void Trace([Localizable(false)] string message);

        /// <summary>
        /// Logs a message on trace level.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments.</param>
        void Trace([Localizable(false)] string message, params object[] args);

        /// <summary>
        /// Logs a message on trace level.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="args">The arguments.</param>
        void Trace([Localizable(false)] string message, Exception exception, params object[] args);

        #endregion

        #region Debug

        /// <summary>
        /// Logs a message on debug level.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments.</param>
        void Debug(IFormatProvider formatProvider, [Localizable(false)] string message, params object[] args);

        /// <summary>
        /// Logs a message on debug level.
        /// </summary>
        /// <param name="message">The message.</param>
        void Debug([Localizable(false)] string message);

        /// <summary>
        /// Logs a message on debug level.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments.</param>
        void Debug([Localizable(false)] string message, params object[] args);

        /// <summary>
        /// Logs a message on debug level.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="args">The arguments.</param>
        void Debug([Localizable(false)] string message, Exception exception, params object[] args);

        #endregion

        #region Info

        /// <summary>
        /// Logs a message on info level.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments.</param>
        void Info(IFormatProvider formatProvider, [Localizable(false)] string message, params object[] args);

        /// <summary>
        /// Logs a message on info level.
        /// </summary>
        /// <param name="message">The message.</param>
        void Info([Localizable(false)] string message);

        /// <summary>
        /// Logs a message on info level.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments.</param>
        void Info([Localizable(false)] string message, params object[] args);

        /// <summary>
        /// Logs a message on info level.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="args">The arguments.</param>
        void Info([Localizable(false)] string message, Exception exception, params object[] args);

        #endregion

        #region Warn

        /// <summary>
        /// Logs a message on warn level.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments.</param>
        void Warn(IFormatProvider formatProvider, [Localizable(false)] string message, params object[] args);

        /// <summary>
        /// Logs a message on warn level.
        /// </summary>
        /// <param name="message">The message.</param>
        void Warn([Localizable(false)] string message);

        /// <summary>
        /// Logs a message on warn level.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments.</param>
        void Warn([Localizable(false)] string message, params object[] args);

        /// <summary>
        /// Logs a message on warn level.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="args">The arguments.</param>
        void Warn([Localizable(false)] string message, Exception exception, params object[] args);

        #endregion

        #region Error

        /// <summary>
        /// Logs a message on error level.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments.</param>
        void Error(IFormatProvider formatProvider, [Localizable(false)] string message, params object[] args);

        /// <summary>
        /// Logs a message on error level.
        /// </summary>
        /// <param name="message">The message.</param>
        void Error([Localizable(false)] string message);

        /// <summary>
        /// Logs a message on error level.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments.</param>
        void Error([Localizable(false)] string message, params object[] args);

        /// <summary>
        /// Logs a message on error level.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="args">The arguments.</param>
        void Error([Localizable(false)] string message, Exception exception, params object[] args);

        #endregion

        #region Fatal

        /// <summary>
        /// Logs a message on fatal level.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments.</param>
        void Fatal(IFormatProvider formatProvider, [Localizable(false)] string message, params object[] args);

        /// <summary>
        /// Logs a message on fatal level.
        /// </summary>
        /// <param name="message">The message.</param>
        void Fatal([Localizable(false)] string message);

        /// <summary>
        /// Logs a message on fatal level.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments.</param>
        void Fatal([Localizable(false)] string message, params object[] args);

        /// <summary>
        /// Logs a message on fatal level.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="args">The arguments.</param>
        void Fatal([Localizable(false)] string message, Exception exception, params object[] args);

        #endregion
    }
}