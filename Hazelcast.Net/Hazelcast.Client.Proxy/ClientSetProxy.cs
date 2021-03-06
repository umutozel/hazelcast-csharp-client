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
using System.Collections.Generic;
using Hazelcast.Client.Protocol.Codec;
using Hazelcast.Core;
using Hazelcast.Util;

namespace Hazelcast.Client.Proxy
{
    internal class ClientSetProxy<E> : AbstractClientCollectionProxy<E>, IHSet<E>
    {
        public ClientSetProxy(string serviceName, string name) : base(serviceName, name)
        {
        }

        public override string AddItemListener(IItemListener<E> listener, bool includeValue)
        {
            var request = SetAddListenerCodec.EncodeRequest(GetName(), includeValue, false);

            DistributedEventHandler handler = message => SetAddListenerCodec.AbstractEventHandler.Handle(message,
                ((item, uuid, type) => { HandleItemListener(item, uuid, (ItemEventType)type, listener, includeValue); }));

            return Listen(request,
                m => SetAddListenerCodec.DecodeResponse(m).response, GetPartitionKey(), handler);
        }

        public override bool RemoveItemListener(string registrationId)
        {
            return StopListening(s => SetRemoveListenerCodec.EncodeRequest(GetName(), s),
                message => SetRemoveListenerCodec.DecodeResponse(message).response,
                registrationId);
        }

        public override bool Add(E item)
        {
            ThrowExceptionIfNull(item);
            var value = ToData(item);
            var request = SetAddCodec.EncodeRequest(GetName(), value);
            return Invoke(request, m => SetAddCodec.DecodeResponse(m).response);
        }

        public override void Clear()
        {
            var request = SetClearCodec.EncodeRequest(GetName());
            Invoke(request);
        }

        public override bool Contains(E item)
        {
            var request = SetContainsCodec.EncodeRequest(GetName(), ToData(item));
            return Invoke(request, m => SetContainsCodec.DecodeResponse(m).response);
        }

        public override bool Remove(E item)
        {
            var request = SetRemoveCodec.EncodeRequest(GetName(), ToData(item));
            return Invoke(request, m => SetRemoveCodec.DecodeResponse(m).response);
        }

        public override int Size()
        {
            var request = SetSizeCodec.EncodeRequest(GetName());
            return Invoke(request, m => SetSizeCodec.DecodeResponse(m).response); ;
        }

        public override bool IsEmpty()
        {
            var request = SetIsEmptyCodec.EncodeRequest(GetName());
            return Invoke(request, m => SetIsEmptyCodec.DecodeResponse(m).response); ;
        }

        public override bool ContainsAll<T>(ICollection<T> c)
        {
            var values = ToDataSet(c);
            var request = SetContainsAllCodec.EncodeRequest(GetName(), values);
            return Invoke(request, m => SetContainsAllCodec.DecodeResponse(m).response);
        }

        public override bool RemoveAll<T>(ICollection<T> c)
        {
            var values = ToDataSet(c);
            var request = SetCompareAndRemoveAllCodec.EncodeRequest(GetName(), values);
            return Invoke(request, m => SetCompareAndRemoveAllCodec.DecodeResponse(m).response);
        }

        public override bool RetainAll<T>(ICollection<T> c)
        {
            var values = ToDataSet(c);
            var request = SetCompareAndRetainAllCodec.EncodeRequest(GetName(), values);
            return Invoke(request, m => SetCompareAndRetainAllCodec.DecodeResponse(m).response);
        }

        public override bool AddAll<T>(ICollection<T> c)
        {
            var values = ToDataList(c);
            var request = SetAddAllCodec.EncodeRequest(GetName(), values);
            return Invoke(request, m => SetAddAllCodec.DecodeResponse(m).response);
        }

        protected override ICollection<E> GetAll()
        {
            var request = SetGetAllCodec.EncodeRequest(GetName());
            var result = Invoke(request, m => SetGetAllCodec.DecodeResponse(m).list);
            return ToList<E>(result);
        }
    }
}