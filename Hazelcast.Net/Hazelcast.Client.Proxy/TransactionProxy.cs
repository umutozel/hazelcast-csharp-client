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
using System.Threading;
using Hazelcast.Client.Protocol;
using Hazelcast.Client.Protocol.Codec;
using Hazelcast.Core;
using Hazelcast.IO;
using Hazelcast.Transaction;
using Hazelcast.Util;

namespace Hazelcast.Client.Proxy
{
    internal sealed class TransactionProxy
    {
        [ThreadStatic]
        private static bool? _threadFlag ;

        private readonly TransactionOptions options;
        private readonly long threadId = ThreadUtil.GetThreadId();

        private readonly HazelcastClient client;

        private IMember txOwner;

        private long startTime;
        private TransactionState state = TransactionState.NoTxn;
        private string txnId;

        internal TransactionProxy(HazelcastClient client, TransactionOptions options, IMember txOwner)
        {
            this.options = options;
            this.client =  client;
            this.txOwner = txOwner;
        }

        public string GetTxnId()
        {
            return txnId;
        }

        public TransactionState GetState()
        {
            return state;
        }

        public long GetTimeoutMillis()
        {
            return options.GetTimeoutMillis();
        }

        internal void Begin()
        {
            try
            {
                if (state == TransactionState.Active)
                {
                    throw new InvalidOperationException("Transaction is already active");
                }
                CheckThread();
                if (_threadFlag != null)
                {
                    throw new InvalidOperationException("Nested transactions are not allowed!");
                }
                _threadFlag = true;
                startTime = Clock.CurrentTimeMillis();
                var request = TransactionCreateCodec.EncodeRequest(GetTimeoutMillis(), options.GetDurability(),
                    (int)options.GetTransactionType(), threadId);
                var response = Invoke(request);
                txnId = TransactionCreateCodec.DecodeResponse(response).response;
                state = TransactionState.Active;
            }
            catch (Exception e)
            {
                _threadFlag = null;
                throw ExceptionUtil.Rethrow(e);
            }
        }

        internal void Commit(bool prepareAndCommit)
        {
            try
            {
                if (state != TransactionState.Active)
                {
                    throw new TransactionNotActiveException("Transaction is not active");
                }
                CheckThread();
                CheckTimeout();
                var request = TransactionCommitCodec.EncodeRequest(GetTxnId(), threadId, prepareAndCommit);
                Invoke(request);
                state = TransactionState.Committed;
            }
            catch (Exception e)
            {
                state = TransactionState.RollingBack;
                throw ExceptionUtil.Rethrow(e);
            }
            finally
            {
                _threadFlag = null;
            }
        }

        internal void Rollback()
        {
            try
            {
                if (state == TransactionState.NoTxn || state == TransactionState.RolledBack)
                {
                    throw new InvalidOperationException("Transaction is not active");
                }
                if (state == TransactionState.RollingBack)
                {
                    state = TransactionState.RolledBack;
                    return;
                }
                CheckThread();
                try
                {
                    var request = TransactionRollbackCodec.EncodeRequest(txnId, threadId);
                    Invoke(request);
                }
                catch (Exception)
                {
                }
                state = TransactionState.RolledBack;
            }
            finally
            {
                _threadFlag = null;
            }
        }

        private IClientMessage Invoke(IClientMessage request)
        {
            var rpc = client.GetInvocationService();
            try
            {
                var task = rpc.InvokeOnMember(request, txOwner);
                return ThreadUtil.GetResult(task);
            }
            catch (Exception e)
            {
                throw ExceptionUtil.Rethrow(e);
            }
        }

        private void CheckThread()
        {
            if (threadId != Thread.CurrentThread.ManagedThreadId)
            {
                throw new InvalidOperationException("Transaction cannot span multiple threads!");
            }
        }

        private void CheckTimeout()
        {
            if (startTime + options.GetTimeoutMillis() < Clock.CurrentTimeMillis())
            {
                throw new TransactionException("Transaction is timed-out!");
            }
        }


    }
}