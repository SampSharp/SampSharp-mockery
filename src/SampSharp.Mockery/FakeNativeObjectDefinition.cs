using System;
using System.Collections.Generic;
using System.Linq;

namespace SampSharp.Mockery
{
    public abstract class FakeNativeObjectDefinition
    {
        private readonly ServerImitator _server;
        private Dictionary<int, FakeNativeObject> _objects = new Dictionary<int, FakeNativeObject>();

        public abstract void Define();

        protected FakeNativeObjectDefinition(ServerImitator server)
        {
            if (server == null) throw new ArgumentNullException(nameof(server));
            _server = server;
        }

        public FakeNativeObject this[int handle]
        {
            get
            {
                FakeNativeObject obj;
                _objects.TryGetValue(handle, out obj);
                return obj;
            }
        }

        public FakeNativeObject Create(int id)
        {
            // TODO checking or make this better with a behaviour enum and then do create logic here.
            return _objects[id] = new FakeNativeObject();
        }

        public void Destroy(int id)
        {
            _objects.Remove(id);
        }
        
        protected void ProvidesNative(string name, int handleIndex, FakeNativeReturnBehavior returnBehavior, FakeNativeValue[] arguments)
        {
            _server.NativeLoader.Register(new FakeAccessorNative(name, handleIndex, this, returnBehavior, arguments));
        }

        protected void ProvidesGetterNative(string name, ParameterType returnType, string varName)
        {
            throw new NotImplementedException();
        }

        protected void ProvidesCreatorNative(string name, FakeNativeValue[] arguments)
        {
            throw new NotImplementedException();
        }

        private class FakeAccessorNative : FakeNative
        {
            private readonly FakeNativeValue[] _arguments;
            private readonly int _handleIndex;
            private readonly FakeNativeObjectDefinition _definition;
            private readonly FakeNativeReturnBehavior _returnBehavior;

            public FakeAccessorNative(string name, int handleIndex, FakeNativeObjectDefinition definition,
                FakeNativeReturnBehavior returnBehavior, FakeNativeValue[] arguments)
                : base(name, arguments.Select(a => a.Type.IsByRef ? a.Type.Type.MakeByRefType() : a.Type.Type).ToArray()
                )
            {
                _handleIndex = handleIndex;
                _definition = definition;
                _returnBehavior = returnBehavior;
                _arguments = arguments;
            }

            public override object FakeInvoke(object[] arguments)
            {
                if (_arguments.Length != arguments.Length)
                {
                    throw new MockeryException($"Invalid argument count provided to native {Name}.");
                }

                if (_handleIndex >= arguments.Length || (_handleIndex >= 0 && !(arguments[_handleIndex] is int)))
                {
                    throw new MockeryException("Argument at index {_handleIndex} is expected to be a handle id.");
                }

                var handle = _handleIndex < 0 ? 0 : (int) arguments[_handleIndex];

                var target = _definition[handle];

                if (target != null)
                {
                    for (var i = 0; i < _arguments.Length; i++)
                    {
                        var arg = _arguments[i];

                        // TODO: type checking
                        if (arg.Type.IsByRef)
                        {
                            // out
                            arguments[i] = target.GetValue(arg.Key, _arguments[i].Type.ParameterType);
                        }
                        else
                        {
                            // in
                            target.SetValue(arg.Key, arguments[i]);
                        }
                    }
                }

                switch (_returnBehavior)
                {
                    case FakeNativeReturnBehavior.False:
                        return true;
                    case FakeNativeReturnBehavior.True:
                        return true;
                    case FakeNativeReturnBehavior.HandleExists:
                        return target != null;
                    default:
                        throw new MockeryException("Unimplemented return behavior");
                }
            }

            public override bool AreValidParameters(int[] sizes, params Type[] parameters)
            {
                // TODO: Implement
                return true;
            }
        }
    }
}