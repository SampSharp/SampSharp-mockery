using System.Reflection;

namespace SampSharp.Mockery
{
    internal class Callback
    {
        public Callback(string name, object target, MethodInfo methodInfo)
        {
            Name = name;
            Target = target;
            MethodInfo = methodInfo;
        }

        public string Name { get; }
        public object Target { get; }
        public MethodInfo MethodInfo { get; }

        public object Invoke(params object[] parameters)
        {
            // TODO: Type and argument count checking.
            return MethodInfo.Invoke(Target, parameters);
        }
    }
}