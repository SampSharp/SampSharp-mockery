using System;

namespace SampSharp.Mockery
{
    public struct FakeNativeValue
    {
        public FakeNativeValue(string key, Parameter type)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            Key = key;
            Type = type;
        }

        public string Key { get; }
        public Parameter Type { get; }
    }
}