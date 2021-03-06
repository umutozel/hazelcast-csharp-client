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
    internal sealed class ClientAddMembershipListenerCodec
    {

        public static readonly ClientMessageType RequestType = ClientMessageType.ClientAddMembershipListener;
        public const int ResponseType = 104;
        public const bool Retryable = false;

        //************************ REQUEST *************************//

        public class RequestParameters
        {
            public static readonly ClientMessageType TYPE = RequestType;
            public bool localOnly;

            public static int CalculateDataSize(bool localOnly)
            {
                int dataSize = ClientMessage.HeaderSize;
                dataSize += Bits.BooleanSizeInBytes;
                return dataSize;
            }
        }

        public static ClientMessage EncodeRequest(bool localOnly)
        {
            int requiredDataSize = RequestParameters.CalculateDataSize(localOnly);
            ClientMessage clientMessage = ClientMessage.CreateForEncode(requiredDataSize);
            clientMessage.SetMessageType((int)RequestType);
            clientMessage.SetRetryable(Retryable);
            clientMessage.Set(localOnly);
            clientMessage.UpdateFrameLength();
            return clientMessage;
        }

        //************************ RESPONSE *************************//


        public class ResponseParameters
        {
            public string response;
        }

        public static ResponseParameters DecodeResponse(IClientMessage clientMessage)
        {
            ResponseParameters parameters = new ResponseParameters();
            string response = null;
            response = clientMessage.GetStringUtf8();
            parameters.response = response;
            return parameters;
        }


        //************************ EVENTS *************************//
        public abstract class AbstractEventHandler
        {
            public static void Handle(IClientMessage clientMessage, HandleMember handleMember, HandleMemberSet handleMemberSet, HandleMemberAttributeChange handleMemberAttributeChange)
            {
                int messageType = clientMessage.GetMessageType();
                if (messageType == EventMessageConst.EventMember)
                {
                    Core.IMember member = null;
                    member = MemberCodec.Decode(clientMessage);
                    int eventType;
                    eventType = clientMessage.GetInt();
                    handleMember(member, eventType);
                    return;
                }
                if (messageType == EventMessageConst.EventMemberSet)
                {
                    ISet<Core.IMember> members = null;
                    int members_size = clientMessage.GetInt();
                    members = new HashSet<Core.IMember>();
                    for (int members_index = 0; members_index < members_size; members_index++)
                    {
                        Core.IMember members_item;
                        members_item = MemberCodec.Decode(clientMessage);
                        members.Add(members_item);
                    }
                    handleMemberSet(members);
                    return;
                }
                if (messageType == EventMessageConst.EventMemberAttributeChange)
                {
                    string uuid = null;
                    uuid = clientMessage.GetStringUtf8();
                    string key = null;
                    key = clientMessage.GetStringUtf8();
                    int operationType;
                    operationType = clientMessage.GetInt();
                    string value = null;
                    bool value_isNull = clientMessage.GetBoolean();
                    if (!value_isNull)
                    {
                        value = clientMessage.GetStringUtf8();
                    }
                    handleMemberAttributeChange(uuid, key, operationType, value);
                    return;
                }
                Hazelcast.Logging.Logger.GetLogger(typeof(AbstractEventHandler)).Warning("Unknown message type received on event handler :" + clientMessage.GetMessageType());
            }

            public delegate void HandleMember(Core.IMember member, int eventType);
            public delegate void HandleMemberSet(ISet<Core.IMember> members);
            public delegate void HandleMemberAttributeChange(string uuid, string key, int operationType, string value);
        }

    }
}
