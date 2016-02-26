using System;

namespace Tortuga.Chain
{
    /// <summary>
    /// This exception is thrown when the requested database object (e.g. table, view, etc.) could not be found.
    /// </summary>
    [Serializable]
    public class MissingObjectException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingObjectException"/> class.
        /// </summary>
        public MissingObjectException()
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingObjectException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public MissingObjectException(string message)
            : base(message)
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingObjectException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public MissingObjectException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingObjectException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        protected MissingObjectException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {

        }

    }
}
