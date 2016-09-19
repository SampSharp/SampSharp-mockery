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

namespace SampSharp.Mockery
{
    public struct Parameter
    {
        public Parameter(ParameterType parameterType, bool isByRef)
        {
            ParameterType = parameterType;
            IsByRef = isByRef;
        }

        public ParameterType ParameterType { get; }
        public Type Type => ParameterTypes.GetType(ParameterType);
        public bool IsByRef { get; }

        public static implicit operator Parameter(ParameterType type)
        {
            return new Parameter(type, false);
        }
    }

    public static class ParameterTypeHelper
    {
        public static Parameter ByRef(this ParameterType parameterType)
        {
            return new Parameter(parameterType, true);
        }
    }
}