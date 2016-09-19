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
using System.Collections.Generic;
using System.Reflection;

namespace SampSharp.Mockery
{
    internal static class ParameterTypes
    {
        private static readonly Dictionary<ParameterType, Type> Types = new Dictionary<ParameterType, Type>();

        static ParameterTypes()
        {
            foreach (ParameterType type in typeof(ParameterType).GetEnumValues())
            {
                var attr =
                    typeof(ParameterType).GetMember(type.ToString())[0].GetCustomAttribute<ParameterTypeAttribute>();

                if ((attr != null) && (attr.Type != null))
                    Types[type] = attr.Type;
            }
        }

        public static Type GetType(ParameterType type)
        {
            return Types.ContainsKey(type) ? Types[type] : null;
        }

        public static bool IsValidType(Type type)
        {
            return (type != null) && Types.ContainsValue(type);
        }
    }
}