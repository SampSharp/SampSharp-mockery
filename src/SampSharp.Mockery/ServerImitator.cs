// SampSharp.Mockery
// Copyright 2016 Tim Potze
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using SampSharp.GameMode;
using SampSharp.GameMode.API;
using SampSharp.GameMode.Pools;

namespace SampSharp.Mockery
{
    public sealed class ServerImitator
    {
        private readonly Dictionary<string, Callback> _callbacks = new Dictionary<string, Callback>();
        private List<IExtension> _extensions;
        private readonly Type _gameModeType;
        private ServerImitator(Type gameModeType)
        {
            if (gameModeType == null) throw new ArgumentNullException(nameof(gameModeType));
            _gameModeType = gameModeType;

            Reset();
        }

        private Dictionary<Type, FakeNativeObjectDefinition> _definitions = new Dictionary<Type, FakeNativeObjectDefinition>();
        public BaseMode GameMode { get; private set; }

        public FakeNativeLoader NativeLoader { get; private set; }

        public static ServerImitator Boot<TGameMode>() where TGameMode : BaseMode
        {
            return new ServerImitator(typeof(TGameMode));
        }

        private Type GetBaseTypeOfGenericType(Type type, Type genericType)
        {
            if (genericType == null) throw new ArgumentNullException(nameof(genericType));
            if (type.GetGenericTypeDefinition() == genericType)
                return type;

            if ((type.BaseType == null) || (type.BaseType == typeof(object)))
                return null;

            return GetBaseTypeOfGenericType(type.BaseType, genericType);
        }

        private void ResetNativeLoader()
        {
            Native.NativeLoader = NativeLoader = new FakeNativeLoader();
        }

        public void Define<TDefinition>() where TDefinition : FakeNativeObjectDefinition
        {
            var def = (FakeNativeObjectDefinition) Activator.CreateInstance(typeof(TDefinition), this);
            def.Define();

            _definitions.Add(typeof(TDefinition), def);
        }

        public TDefinition GetDefinition<TDefinition>() where TDefinition : FakeNativeObjectDefinition
        {
            // TODO: Type check
            return _definitions[typeof(TDefinition)] as TDefinition;
        }

        private void ResetDefinitions()
        {
            Define<FakePlayerDefinition>();
        }

        private void ResetPools()
        {
            // Reset pools
            var pooledTypes = new[]
            {
                typeof(Pool<>),
                typeof(IdentifiedPool<>),
                typeof(IdentifiedOwnedPool<,>)
            };

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                if (!assembly.IsDynamic)
                    foreach (var type in assembly.GetTypes())
                        if (!type.IsAbstract && type.IsClass && !type.IsGenericType &&
                            pooledTypes.Any(t => t.IsAssignableFrom(type)))
                        {
                            Debug.WriteLine("Resettign pool " + type);

                            var genType =
                                pooledTypes.Select(t => GetBaseTypeOfGenericType(type, t))
                                    .FirstOrDefault(t => t != null);
                            if (genType == null)
                                throw new Exception("Missing pool base type");

                            var property = genType.GetProperty("All", BindingFlags.Public | BindingFlags.Static);

                            if (property == null)
                                throw new Exception("Missing All property");

                            var propertyValue = property.GetValue(null) as IEnumerable;

                            if (propertyValue == null)
                                throw new Exception("Prool All property value is null");

                            var values = propertyValue.Cast<object>().Select(value => value as IDisposable).ToList();

                            foreach (var value in values)
                                value.Dispose();
                        }
        }

        private void ResetGameMode()
        {
            GameMode = Activator.CreateInstance(_gameModeType) as BaseMode;

            var extensions = typeof(BaseMode).GetField("_extensions", BindingFlags.Instance | BindingFlags.NonPublic);

            if (extensions == null)
                throw new Exception("Missing _extensions field");

            _extensions = (List<IExtension>)extensions.GetValue(GameMode);

            DiscoverCallbacks();
        }
        public void Reset()
        {
            ResetNativeLoader();
            ResetGameMode();
            ResetDefinitions();
            ResetPools();
        }

        public TExtension GetExtension<TExtension>() where TExtension : IExtension
        {
            return _extensions.OfType<TExtension>().FirstOrDefault();
        }

        private void DiscoverCallbacks(object target)
        {
            foreach (
                var method in
                target.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                    .Where(m => IsValidCallbackName(m.Name) &&
                                m.GetParameters().All(p => ParameterTypes.IsValidType(p.ParameterType))))
                _callbacks[method.Name] = new Callback(method.Name, target, method);
        }

        private void DiscoverCallbacks()
        {
            _callbacks.Clear();

            DiscoverCallbacks(GameMode);

            foreach (var extension in _extensions)
                DiscoverCallbacks(extension);
        }

        private static bool IsValidCallbackName(string name)
        {
            return (name != null) && name.StartsWith("On") && (name.Length > 2);
        }

        public object Signal(string callbackName, params object[] parameters)
        {
            if (callbackName == null) throw new ArgumentNullException(nameof(callbackName));
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));

            Callback callback;
            _callbacks.TryGetValue(callbackName, out callback);

            if (callback == null)
                throw new ArgumentException("Invalid callback name", nameof(callbackName));

            return callback.Invoke(parameters);
        }

        public int ConnectPlayer(string name, string ip)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (ip == null) throw new ArgumentNullException(nameof(ip));

            var id = 0;
            var def = GetDefinition<FakePlayerDefinition>();
            while (def[id] != null) id++;

            var obj = def.Create(id);

            obj.SetValue("name", name);
            obj.SetValue("ip", ip);

            return id;
        }
    }
}