using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Tools.RegistrySerialization
{
    [Serializable()]
    public class RegistrySerializationException : Exception
    {
        public RegistrySerializationException()
            : base()
        { }

        public RegistrySerializationException(string message)
            : base(message)
        { }

        public RegistrySerializationException(string message, Exception innerException)
            : base(message, innerException)
        { }

        protected RegistrySerializationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
