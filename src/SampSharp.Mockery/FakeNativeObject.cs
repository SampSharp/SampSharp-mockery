using System.Collections.Generic;

namespace SampSharp.Mockery
{
    public class FakeNativeObject
    {
        private readonly Dictionary<string, object> _values = new Dictionary<string, object>();

        public void SetValue(string key, object value)
        {
            _values[key] = value;
        }

        public object GetValue(string key, ParameterType type)
        {
            if (!_values.ContainsKey(key))
                switch (type)
                {
                    case ParameterType.Bool:
                        return false;
                    case ParameterType.BoolArray:
                        return new bool[0];
                    case ParameterType.Float:
                        return 0.0f;
                    case ParameterType.FloatArray:
                        return new float[0];
                    case ParameterType.Int:
                        return 0;
                    case ParameterType.IntArray:
                        return new int[0];
                    case ParameterType.String:
                        return "";
                }

            // TODO: may not be of right type;
            return _values[key];
        }
    }
}