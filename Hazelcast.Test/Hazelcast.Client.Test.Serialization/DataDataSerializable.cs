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

﻿using Hazelcast.IO;
using Hazelcast.IO.Serialization;

namespace Hazelcast.Client.Test.Serialization
{
    public class DataDataSerializable : IDataSerializable
    {
        internal IData Data;

        public DataDataSerializable()
        {
        }

        public DataDataSerializable(IData data)
        {
            this.Data = data;
        }

        public void ReadData(IObjectDataInput input)
        {
            Data = input.ReadData();
        }

        public string GetJavaClassName()
        {
            return typeof (DataDataSerializable).FullName;
        }

        public void WriteData(IObjectDataOutput output)
        {
            output.WriteData(Data);
        }

        protected bool Equals(DataDataSerializable other)
        {
            return Equals(Data, other.Data);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DataDataSerializable) obj);
        }

        public override int GetHashCode()
        {
            return (Data != null ? Data.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return string.Format("Data: {0}", Data);
        }
    }
}