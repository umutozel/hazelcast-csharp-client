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
    internal sealed class ListAddAllCodec
    {

        public static readonly ListMessageType RequestType = ListMessageType.ListAddAll;
        public const int ResponseType = 101;
        public const bool Retryable = false;

        //************************ REQUEST *************************//

        public class RequestParameters
        {
            public static readonly ListMessageType TYPE = RequestType;
            public string name;
            public IList<IData> valueList;

            public static int CalculateDataSize(string name, IList<IData> valueList)
            {
                int dataSize = ClientMessage.HeaderSize;
                dataSize += ParameterUtil.CalculateDataSize(name);
                dataSize += Bits.IntSizeInBytes;
                foreach (var valueList_item in valueList )
                {
                dataSize += ParameterUtil.CalculateDataSize(valueList_item);
                }
                return dataSize;
            }
        }

        public static ClientMessage EncodeRequest(string name, IList<IData> valueList)
        {
            int requiredDataSize = RequestParameters.CalculateDataSize(name, valueList);
            ClientMessage clientMessage = ClientMessage.CreateForEncode(requiredDataSize);
            clientMessage.SetMessageType((int)RequestType);
            clientMessage.SetRetryable(Retryable);
            clientMessage.Set(name);
            clientMessage.Set(valueList.Count);
            foreach (var valueList_item in valueList) {
            clientMessage.Set(valueList_item);
            }
            clientMessage.UpdateFrameLength();
            return clientMessage;
        }

        //************************ RESPONSE *************************//


        public class ResponseParameters
        {
            public bool response;
        }

        public static ResponseParameters DecodeResponse(IClientMessage clientMessage)
        {
            ResponseParameters parameters = new ResponseParameters();
            bool response ;
            response = clientMessage.GetBoolean();
            parameters.response = response;
            return parameters;
        }

    }
}
