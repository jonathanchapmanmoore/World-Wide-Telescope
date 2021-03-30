using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Renci.Wwt.Core
{
    /// <summary>
    /// This class is responsible for custom exception.
    /// </summary>
    [Serializable]
    public class WwtException : Exception
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the CustomException class.
        /// </summary>
        public WwtException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the CustomException class.
        /// </summary>
        /// <param name="message">
        /// String Message
        /// </param>
        public WwtException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CustomException class. 
        /// </summary>
        /// <param name="message">
        /// Message that describes the Error
        /// </param>
        /// <param name="innerException">
        /// The Exception is the cause of Current Exception
        /// </param>
        public WwtException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        ///  Initializes a new instance of the CustomException class. 
        /// </summary>
        /// <param name="info">
        /// Holds the serialized object data about the Exception being thrown
        /// </param>
        /// <param name="context">
        /// Instance of System.Runtime.Serialization.StreamingContext
        /// </param>
        protected WwtException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }

        #endregion
    }
}
