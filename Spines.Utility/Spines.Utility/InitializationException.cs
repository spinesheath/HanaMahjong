using System;
using System.Runtime.Serialization;

namespace Spines.Utility
{
    /// <summary>
    /// Represents an Error that occurs during initialization.
    /// </summary>
    [Serializable]
    public class InitializationException : Exception
    {
        public InitializationException()
        {
        }

        public InitializationException(string message)
            : base(message)
        {
        }

        public InitializationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected InitializationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}