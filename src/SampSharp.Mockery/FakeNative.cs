using System;
using SampSharp.GameMode.API;

namespace SampSharp.Mockery
{
    public abstract class FakeNative : INative
    {
        protected FakeNative(string name, Type[] parameterTypes)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (parameterTypes == null) throw new ArgumentNullException(nameof(parameterTypes));

            Name = name;
            ParameterTypes = parameterTypes;
        }

        public int Invoke(params object[] arguments)
        {
            var result = FakeInvoke(arguments);

            if (result is int)
                return (int) result;

            if (result is bool)
                return (bool) result ? 1 : 0;
            throw new MockeryException(
                $"Native {Name} was expected to return an System.Int32 but returned a {result?.GetType()} instance.");
        }

        public float InvokeFloat(params object[] arguments)
        {
            var result = FakeInvoke(arguments);

            if (result is float)
                return (float) result;

            throw new MockeryException(
                $"Native {Name} was expected to return an System.Single but returned a {result?.GetType()} instance.");
        }

        public bool InvokeBool(params object[] arguments)
        {
            var result = FakeInvoke(arguments);

            if (result is bool)
                return (bool) result;

            if (result is int)
            {
                var iResult = (int)result;
                switch (iResult)
                {
                    case 1:
                        return true;
                    case 0:
                        return false;
                }
            }

            throw new MockeryException(
                $"Native {Name} was expected to return an System.Boolean but returned a {result?.GetType()} instance.");
        }

        public int Handle { get; set; }
        public string Name { get; }
        public Type[] ParameterTypes { get; }
        public abstract object FakeInvoke(object[] arguments);
        public abstract bool AreValidParameters(int[] sizes, params Type[] parameters);
    }
}