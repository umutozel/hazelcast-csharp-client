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

using System;
using System.Text;
using Hazelcast.IO;
using Hazelcast.IO.Serialization;

namespace Hazelcast.Client.Spi
{
    [Serializable]
    internal class ObjectNamespace //: IDataSerializable
    {
        private string objectName;
        private string service;

        public ObjectNamespace()
        {
        }

        public ObjectNamespace(string serviceName, string objectName)
        {
            service = serviceName;
            this.objectName = objectName;
        }

        ///// <exception cref="System.IO.IOException"></exception>
        //public virtual void WriteData(IObjectDataOutput output)
        //{
        //    output.WriteUTF(service);
        //    output.WriteObject(objectName);
        //}

        //// writing as object for backward-compatibility
        ///// <exception cref="System.IO.IOException"></exception>
        //public virtual void ReadData(IObjectDataInput input)
        //{
        //    service = input.ReadUTF();
        //    objectName = input.ReadObject<string>();
        //}

        public virtual string GetServiceName()
        {
            return service;
        }

        public virtual string GetObjectName()
        {
            return objectName;
        }

        public override bool Equals(object o)
        {
            if (this == o)
            {
                return true;
            }
            if (o == null || GetType() != o.GetType())
            {
                return false;
            }
            var that = (ObjectNamespace) o;
            if (objectName != null ? !objectName.Equals(that.objectName) : that.objectName != null)
            {
                return false;
            }
            if (service != null ? !service.Equals(that.service) : that.service != null)
            {
                return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            int result = service != null ? service.GetHashCode() : 0;
            result = 31*result + (objectName != null ? objectName.GetHashCode() : 0);
            return result;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("ObjectNamespace");
            sb.Append("{service='").Append(service).Append('\'');
            sb.Append(", objectName=").Append(objectName);
            sb.Append('}');
            return sb.ToString();
        }
    }
}