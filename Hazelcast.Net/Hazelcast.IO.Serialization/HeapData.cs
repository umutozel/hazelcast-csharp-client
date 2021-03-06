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
using Hazelcast.Net.Ext;
using Hazelcast.Util;

namespace Hazelcast.IO.Serialization
{
    public sealed class HeapData : IData
    {
        internal const int TypeOffset = 0;

        internal const int DataOffset = 4;

        private const int ArrayHeaderSizeInBytes = 16;

        //first 4 byte is type id + last 4 byte is partition hash code
        private const int HeapDataOverHead = DataOffset + Bits.IntSizeInBytes;

        private byte[] data;

        public HeapData()
        {
        }

        public HeapData(byte[] data)
        {
            // type and partition_hash are always written with BIG_ENDIAN byte-order
            // will use a byte to store partition_hash bit
            // array (12: array header, 4: length)
            if (data != null && data.Length > 0 && data.Length < HeapDataOverHead)
            {
                throw new ArgumentException("Data should be either empty or should contain more than " + HeapDataOverHead);
            }
            this.data = data;
        }

        public int DataSize()
        {
            return Math.Max(TotalSize() - HeapDataOverHead, 0);
        }

        public int TotalSize()
        {
            return data != null ? data.Length : 0;
        }

        public int GetPartitionHash()
        {
            if (HasPartitionHash())
            {
                return Bits.ReadIntB(data, data.Length - Bits.IntSizeInBytes);
            }
            return GetHashCode();
        }

        public bool HasPartitionHash()
        {
            return data != null && data.Length > HeapDataOverHead
                    && Bits.ReadInt(data, data.Length - Bits.IntSizeInBytes, true) != 0;
        }

        public byte[] ToByteArray()
        {
            return data;
        }

        public int GetTypeId()
        {
            if (TotalSize() == 0)
            {
                return SerializationConstants.ConstantTypeNull;
            }
            return Bits.ReadIntB(data, TypeOffset);
        }

        public int GetHeapCost()
        {
            // reference (assuming compressed oops)
            int objectRef = Bits.IntSizeInBytes;
            return objectRef + (data != null ? ArrayHeaderSizeInBytes + data.Length : 0);
        }

        public override bool Equals(object o)
        {
            if (this == o)
            {
                return true;
            }
            if (o == null)
            {
                return false;
            }
            if (!(o is IData))
            {
                return false;
            }
            IData data = (IData)o;
            if (GetTypeId() != data.GetTypeId())
            {
                return false;
            }
            int dataSize = DataSize();
            if (dataSize != data.DataSize())
            {
                return false;
            }
            return dataSize == 0 || Equals(this.data, data.ToByteArray());
        }

        // Same as Arrays.equals(byte[] a, byte[] a2) but loop order is reversed.
        private static bool Equals(byte[] data1, byte[] data2)
        {
            if (data1 == data2)
            {
                return true;
            }
            if (data1 == null || data2 == null)
            {
                return false;
            }
            int length = data1.Length;
            if (data2.Length != length)
            {
                return false;
            }
            for (int i = length - 1; i >= DataOffset; i--)
            {
                if (data1[i] != data2[i])
                {
                    return false;
                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            return HashUtil.MurmurHash3_x86_32(data, DataOffset, DataSize());
        }

        public bool IsPortable()
        {
            return SerializationConstants.ConstantTypePortable == GetTypeId();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("DefaultData{");
            sb.Append("type=").Append(GetTypeId());
            sb.Append(", hashCode=").Append(GetHashCode());
            sb.Append(", partitionHash=").Append(GetPartitionHash());
            sb.Append(", totalSize=").Append(TotalSize());
            sb.Append(", dataSize=").Append(DataSize());
            sb.Append(", heapCost=").Append(GetHeapCost());
            sb.Append('}');
            return sb.ToString();
        }
    }
}
