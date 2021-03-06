/*
* Copyright (c) 2008-2015, Hazelcast, Inc. All Rights Reserved.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using System.Text;
using Hazelcast.IO.Serialization;

namespace Hazelcast.Config
{
    public class GlobalSerializerConfig
    {
        private string className;

        private ISerializer implementation;

        public GlobalSerializerConfig() : base()
        {
        }

        public virtual string GetClassName()
        {
            return className;
        }

        public virtual GlobalSerializerConfig SetClassName(string className)
        {
            this.className = className;
            return this;
        }

        public virtual ISerializer GetImplementation()
        {
            return implementation;
        }

        //public virtual GlobalSerializerConfig SetImplementation(IByteArraySerializer<> implementation)
        //{
        //    this.implementation = implementation;
        //    return this;
        //}

        //public virtual GlobalSerializerConfig SetImplementation(IStreamSerializer<> implementation)
        //{
        //    this.implementation = implementation;
        //    return this;
        //}

        public override string ToString()
        {
            var sb = new StringBuilder("GlobalSerializerConfig{");
            sb.Append("className='").Append(className).Append('\'');
            sb.Append(", implementation=").Append(implementation);
            sb.Append('}');
            return sb.ToString();
        }
    }
}