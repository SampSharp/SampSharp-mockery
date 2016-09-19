using System;
using System.Collections.Generic;
using SampSharp.GameMode.API;

namespace SampSharp.Mockery
{
    public sealed class FakeNativeLoader : INativeLoader
    {
        private readonly List<FakeNative> _nativesById = new List<FakeNative>();
        private readonly Dictionary<string, FakeNative> _nativesByName = new Dictionary<string, FakeNative>();

        public INative Load(string name, int[] sizes, Type[] parameterTypes)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (parameterTypes == null) throw new ArgumentNullException(nameof(parameterTypes));

            if (!Exists(name))
                return null;

            var native = _nativesByName[name];

            if (!native.AreValidParameters(sizes, parameterTypes))
                throw new MockeryException($"Invalid parameter types were presented while loading native {name}.");

            return native;
        }

        public INative Get(int handle)
        {
            return (handle < 0) || (handle >= _nativesById.Count) ? null : _nativesById[handle];
        }

        public bool Exists(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            return _nativesByName.ContainsKey(name);
        }

        public void Register(FakeNative fakeNative)
        {
            if (fakeNative == null) throw new ArgumentNullException(nameof(fakeNative));

            _nativesByName[fakeNative.Name] = fakeNative;
            _nativesById.Add(fakeNative);
            fakeNative.Handle = _nativesById.Count - 1;
        }
    }
}