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
using Hazelcast.Client.Protocol.Codec;
using Hazelcast.Core;

namespace Hazelcast.Client.Proxy
{
    internal class ClientTxnQueueProxy<E> : ClientTxnProxy, ITransactionalQueue<E>
    {
        public ClientTxnQueueProxy(string name, TransactionContextProxy proxy) : base(name, proxy)
        {
        }

        public virtual bool Offer(E e)
        {
            try
            {
                return Offer(e, 0, TimeUnit.MILLISECONDS);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <exception cref="System.Exception"></exception>
        public virtual bool Offer(E e, long timeout, TimeUnit unit)
        {
            var data = ToData(e);
            var request = TransactionalQueueOfferCodec.EncodeRequest(GetName(), GetTransactionId(), GetThreadId(), data, 
                unit.ToMillis(timeout));
            return Invoke(request, m => TransactionalQueueOfferCodec.DecodeResponse(m).response);
        }

        public virtual E Poll()
        {
            try
            {
                return Poll(0, TimeUnit.MILLISECONDS);
            }
            catch (Exception)
            {
                return default(E);
            }
        }

        /// <exception cref="System.Exception"></exception>
        public virtual E Poll(long timeout, TimeUnit unit)
        {
            var request = TransactionalQueuePollCodec.EncodeRequest(GetName(), GetTransactionId(), GetThreadId(),
                unit.ToMillis(timeout));
            var result = Invoke(request, m => TransactionalQueuePollCodec.DecodeResponse(m).response);
            return ToObject<E>(result);
        }

        public virtual E Peek()
        {
            try
            {
                return Peek(0, TimeUnit.MILLISECONDS);
            }
            catch (Exception)
            {
                return default(E);
            }
        }

        /// <exception cref="System.Exception"></exception>
        public virtual E Peek(long timeout, TimeUnit unit)
        {
            var request = TransactionalQueuePeekCodec.EncodeRequest(GetName(), GetTransactionId(), GetThreadId(),
                unit.ToMillis(timeout));
            var result = Invoke(request, m => TransactionalQueuePeekCodec.DecodeResponse(m).response);
            return ToObject<E>(result);
        }

        public E Take()
        {
            var request = TransactionalQueueTakeCodec.EncodeRequest(GetName(), GetTransactionId(), GetThreadId());
            var result = Invoke(request, m => TransactionalQueueTakeCodec.DecodeResponse(m).response);
            return ToObject<E>(result);
        }

        public virtual int Size()
        {
            var request = TransactionalQueueSizeCodec.EncodeRequest(GetName(), GetTransactionId(), GetThreadId());
            return Invoke(request, m => TransactionalQueueSizeCodec.DecodeResponse(m).response);
        }

        public override string GetServiceName()
        {
            return ServiceNames.Queue;
        }

        internal override void OnDestroy()
        {
        }
    }
}