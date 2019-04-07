#region Usings

using System;
using JetBrains.Annotations;
using NLog;
using NLog.Config;
using Unity;

#endregion

namespace CompositeApplicationFramework.Loggers
{
    using Interfaces;

    /// <summary>
    ///     Default system logger
    /// </summary>
    [UsedImplicitly]
    public class DefaultLogger : ILoggerFacade
    {
        [InjectionConstructor]
        public DefaultLogger()
        {
            Init();

            Logger = LogManager.GetCurrentClassLogger();
        }

        public DefaultLogger(string name)
        {
            Init();

            Logger = LogManager.GetLogger(name);
        }

        private Logger Logger { get; }

        private static void Init()
        {
            if (LogManager.Configuration == null)
            {
                LogManager.Configuration = new LoggingConfiguration();
            }
        }

        #region Trace

        public void Trace(IFormatProvider formatProvider, string message, params object[] args)
        {
            Logger.Trace(formatProvider, message, args);
        }

        public void Trace(string message)
        {
            Logger.Trace(message);
        }

        public void Trace(string message, params object[] args)
        {
            Logger.Trace(message, args);
        }

        public void Trace(string message, Exception exception, params object[] args)
        {
            Logger.Trace(exception, message, args);
        }

        #endregion

        #region Debug

        public void Debug(IFormatProvider formatProvider, string message, params object[] args)
        {
            Logger.Debug(formatProvider, message, args);
        }

        public void Debug(string message)
        {
            Logger.Debug(message);
        }

        public void Debug(string message, params object[] args)
        {
            Logger.Debug(message, args);
        }

        public void Debug(string message, Exception exception, params object[] args)
        {
            Logger.Debug(exception, message, args); 
        }

        #endregion

        #region Info

        public void Info(IFormatProvider formatProvider, string message, params object[] args)
        {
            Logger.Info(formatProvider, message, args);
        }

        public void Info(string message)
        {
            Logger.Info(message);
        }

        public void Info(string message, params object[] args)
        {
            Logger.Info(message, args);
        }

        public void Info(string message, Exception exception, params object[] args)
        {
            Logger.Info(exception, message, args);
        }

        #endregion

        #region Warn

        public void Warn(IFormatProvider formatProvider, string message, params object[] args)
        {
            Logger.Warn(formatProvider, message, args);
        }

        public void Warn(string message)
        {
            Logger.Warn(message);
        }

        public void Warn(string message, params object[] args)
        {
            Logger.Warn(message, args);
        }

        public void Warn(string message, Exception exception, params object[] args)
        {
            Logger.Warn(exception, message, args);
        }

        #endregion

        #region Error

        public void Error(IFormatProvider formatProvider, string message, params object[] args)
        {
            Logger.Error(formatProvider, message, args);
        }

        public void Error(string message)
        {
            Logger.Error(message);
        }

        public void Error(string message, params object[] args)
        {
            Logger.Error(message, args);
        }

        public void Error(string message, Exception exception, params object[] args)
        {
            Logger.Error(exception, message, args);
        }

        #endregion

        #region Fatal

        public void Fatal(IFormatProvider formatProvider, string message, params object[] args)
        {
            Logger.Fatal(formatProvider, message, args);
        }

        public void Fatal(string message)
        {
            Logger.Fatal(message);
        }

        public void Fatal(string message, params object[] args)
        {
            Logger.Fatal(message, args);
        }

        public void Fatal(string message, Exception exception, params object[] args)
        {
            Logger.Fatal(exception, message, args);
        }

        #endregion
    }
}