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

using Hazelcast.Client.Protocol;
using Hazelcast.Client.Protocol.Util;
using Hazelcast.IO;
using Hazelcast.IO.Serialization;
using System.Collections.Generic;

namespace Hazelcast.Client.Protocol.Codec
{
    internal sealed class TransactionalMapSetCodec
    {

        public static readonly TransactionalMapMessageType RequestType = TransactionalMapMessageType.TransactionalMapSet;
        public const int ResponseType = 100;
        public const bool Retryable = false;

        //************************ REQUEST *************************//

        public class RequestParameters
        {
            public static readonly TransactionalMapMessageType TYPE = RequestType;
            public string name;
            public string txnId;
            public long threadId;
            public IData key;
            public IData value;

            public static int CalculateDataSize(string name, string txnId, long threadId, IData key, IData value)
            {
                int dataSize = ClientMessage.HeaderSize;
                dataSize += ParameterUtil.CalculateDataSize(name);
                dataSize += ParameterUtil.CalculateDataSize(txnId);
                dataSize += Bits.LongSizeInBytes;
                dataSize += ParameterUtil.CalculateDataSize(key);
                dataSize += ParameterUtil.CalculateDataSize(value);
                return dataSize;
            }
        }

        public static ClientMessage EncodeRequest(string name, string txnId, long threadId, IData key, IData value)
        {
            int requiredDataSize = RequestParameters.CalculateDataSize(name, txnId, threadId, key, value);
            ClientMessage clientMessage = ClientMessage.CreateForEncode(requiredDataSize);
            clientMessage.SetMessageType((int)RequestType);
            clientMessage.SetRetryable(Retryable);
            clientMessage.Set(name);
            clientMessage.Set(txnId);
            clientMessage.Set(threadId);
            clientMessage.Set(key);
            clientMessage.Set(value);
            clientMessage.UpdateFrameLength();
            return clientMessage;
        }

        

    }
}
