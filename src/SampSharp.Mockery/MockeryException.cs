using System;
using System.Runtime.Serialization;

namespace SampSharp.Mockery
{
    [Serializable]
    public class MockeryException : Exception
    {
        public MockeryException()
        {
        }

        public MockeryException(string message) : base(message)
        {
        }

        public MockeryException(string message, Exception inner) : base(message, inner)
        {
        }

        protected MockeryException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}