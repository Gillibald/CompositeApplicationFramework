#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using CompositeApplicationFramework.Events;
using CompositeApplicationFramework.Interfaces;
using CompositeApplicationFramework.Utility;

#endregion

namespace CompositeApplicationFramework.Base
{
    /// <summary>
    ///     Exception base class
    /// </summary>
    public class ErrorBase : IErrorBase
    {
        /// <summary>
        ///     Raised when there is an error thrown
        /// </summary>
        public event EventHandler<ErrorEventArgs> ErrorEvent;

        /// <inheritdoc />
        /// <summary>
        ///     Throws an exception of type TException using the provided message
        /// </summary>
        /// <example>
        ///     ThrowException&lt;Exception&gt;("Hello World");
        ///     ThrowException&lt;Exception&gt;("Hello {0}", "World");
        /// </example>
        /// <typeparam name="TException"></typeparam>
        /// <param name="message">Message</param>
        /// <param name="para">parameters to message if {0} values are used</param>
        public virtual void Throw<TException>(string message, params object[] para) where TException : Exception
        {
            // Exception.Message is readonly so we'll have to 
            // dynamically construct and inject
            var exception = FastActivator.CreateInstance<TException>();

            // Message that will be thrown
            var messageToThrow = message;
            if (para != null)
            {
                messageToThrow = string.Format(message, para);
            }

            // Get the constructor with one parameter (string)
            var type = exception.GetType();
            var constructor = type.GetConstructors().FirstOrDefault(c => c.GetParameters().Length == 1);

            // If all is well this will not be null
            if (constructor == null)
            {
                throw new Exception($"ErrorBase.Throw - Could not locate a constructor for {type.FullName}");
            }

            // Set the parameter for the constructor
            var constructorParams = new List<object>(constructor.GetParameters().Length) {messageToThrow};

            // Invoke the exception to throw passing in parameter list
            Exception exceptionToThrow;
            try
            {
                exceptionToThrow = (Exception) constructor.Invoke(constructorParams.ToArray());
            }
            catch (Exception ex)
            {
                // If something went wrong then notify developer
                throw new Exception($"ErrorBase.Throw - Failed to resolve {type.FullName}", ex);
            }
            // Raise event
            OnErrorEvent(exceptionToThrow);

            // If all went well then we'll have an instantiated exception to throw
            throw exceptionToThrow;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Throws an exception of type TException using the
        ///     exception and provided message
        /// </summary>
        /// <example>
        ///     catch(Exception ex) {
        ///     ThrowException&lt;Exception&gt;(ex, "Hello World");
        ///     ThrowException&lt;Exception&gt;(ex, "Hello {0}", "World");
        /// </example>
        /// <param name="ex"></param>
        /// <typeparam name="TException"></typeparam>
        /// <param name="message">Message</param>
        /// <param name="para">parameters to message if {0} values are used</param>
        public virtual void Throw<TException>(Exception ex, string message, params object[] para)
            where TException : Exception
        {
            // Exception.Message is readonly so we'll have to 
            // dynamically construct and inject
            var exception = FastActivator.CreateInstance<TException>();

            var messageToThrow = message;
            if (para != null)
            {
                messageToThrow = string.Format(message, para);
            }

            // Get the constructor with one parameter (string)
            var type = exception.GetType();
            var constructor = type.GetConstructors().FirstOrDefault(c => c.GetParameters().Length == 2);

            // If all is well this will not be null
            if (constructor == null)
            {
                throw new Exception($"ErrorBase.Throw - Could not locate a constructor for {type.FullName}");
            }

            // Set the parameter for the constructor
            var constructorParams = new List<object>(constructor.GetParameters().Length);
            constructorParams.Add(messageToThrow);
            constructorParams.Add(ex);

            // Invoke the exception to throw passing in parameter list
            Exception exceptionToThrow;
            try
            {
                exceptionToThrow = (Exception) constructor.Invoke(constructorParams.ToArray());
            }
            catch (Exception ex1)
            {
                // If something went wrong then notify developer
                throw new Exception($"ErrorBase.Throw - Failed to resolve {type.FullName}", ex1);
            }

            // Raise event
            OnErrorEvent(exceptionToThrow);

            // If all went well then we'll have an instantiated exception to throw
            throw exceptionToThrow;
        }

        private void OnErrorEvent(Exception ex)
        {
            ErrorEvent?.Invoke(this, new ErrorEventArgs(ex));
        }
    }
}