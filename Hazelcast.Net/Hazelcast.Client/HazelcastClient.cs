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
using System.Collections.Concurrent;
using System.Collections.Generic;
using Hazelcast.Client.Connection;
using Hazelcast.Client.Protocol.Codec;
using Hazelcast.Client.Proxy;
using Hazelcast.Client.Spi;
using Hazelcast.Config;
using Hazelcast.Core;
using Hazelcast.IO.Serialization;
using Hazelcast.Net.Ext;
using Hazelcast.Partition.Strategy;
using Hazelcast.Transaction;
using Hazelcast.Util;

namespace Hazelcast.Client
{
    /// <summary>
    ///     Hazelcast Client enables you to do all Hazelcast operations without
    ///     being a member of the cluster.
    /// </summary>
    /// <remarks>
    ///     Hazelcast Client enables you to do all Hazelcast operations without
    ///     being a member of the cluster. It connects to one of the
    ///     cluster members and delegates all cluster wide operations to it.
    ///     When the connected cluster member dies, client will
    ///     automatically switch to another live member.
    /// </remarks>
    public sealed class HazelcastClient : IHazelcastInstance
    {
        public const string PropPartitioningStrategyClass = "hazelcast.partitioning.strategy.class";
        private static readonly AtomicInteger ClientId = new AtomicInteger();

        private static readonly ConcurrentDictionary<int, HazelcastClientProxy> Clients =
            new ConcurrentDictionary<int, HazelcastClientProxy>();

        private readonly ClientClusterService clusterService;
        private readonly ClientConfig config;
        private readonly IClientConnectionManager connectionManager;
        private readonly IClientExecutionService executionService;
        private readonly int id = ClientId.GetAndIncrement();
        private readonly string instanceName;
        private readonly IClientInvocationService invocationService;
        private readonly ILoadBalancer loadBalancer;

        //private readonly ThreadGroup threadGroup;

        private readonly LifecycleService lifecycleService;
        private readonly ClientListenerService listenerService;
        private readonly ClientPartitionService partitionService;
        private readonly ProxyManager proxyManager;
        private readonly ISerializationService serializationService;
        private readonly ConcurrentDictionary<string, object> userContext;

        private HazelcastClient(ClientConfig config)
        {
            this.config = config;
            var groupConfig = config.GetGroupConfig();
            instanceName = "hz.client_" + id + (groupConfig != null ? "_" + groupConfig.GetName() : string.Empty);

            //threadGroup = new ThreadGroup(instanceName);
            lifecycleService = new LifecycleService(this);
            try
            {
                string partitioningStrategyClassName = null;
                //TODO make partition strategy parametric                
                //Runtime.GetProperty(PropPartitioningStrategyClass);
                IPartitioningStrategy partitioningStrategy;
                if (partitioningStrategyClassName != null && partitioningStrategyClassName.Length > 0)
                {
                    partitioningStrategy = null;
                }
                else
                {
                    //new Instance for partitioningStrategyClassName;
                    partitioningStrategy = new DefaultPartitioningStrategy();
                }
                serializationService =
                    new SerializationServiceBuilder().SetManagedContext(new HazelcastClientManagedContext(this,
                        config.GetManagedContext()))
                        .SetConfig(config.GetSerializationConfig())
                        .SetPartitioningStrategy(partitioningStrategy)
                        .SetVersion(SerializationService.SerializerVersion)
                        .Build();
            }
            catch (Exception e)
            {
                throw ExceptionUtil.Rethrow(e);
            }
            proxyManager = new ProxyManager(this);

            //TODO EXECUTION SERVICE
            executionService = new ClientExecutionService(instanceName, config.GetExecutorPoolSize());
            clusterService = new ClientClusterService(this);
            loadBalancer = config.GetLoadBalancer() ?? new RoundRobinLB();
            connectionManager = new ClientConnectionManager(this, loadBalancer);
            invocationService = GetInvocationService(config);
            listenerService = new ClientListenerService(this);
            userContext = new ConcurrentDictionary<string, object>();
            loadBalancer.Init(GetCluster(), config);
            proxyManager.Init(config);
            partitionService = new ClientPartitionService(this);
        }

        public string GetName()
        {
            return instanceName;
        }

        public IQueue<E> GetQueue<E>(string name)
        {
            return GetDistributedObject<IQueue<E>>(ServiceNames.Queue, name);
        }

        public IRingbuffer<E> GetRingbuffer<E>(string name)
        {
            return GetDistributedObject<IRingbuffer<E>>(ServiceNames.Ringbuffer, name);
        }

        public ITopic<E> GetTopic<E>(string name)
        {
            return GetDistributedObject<ITopic<E>>(ServiceNames.Topic, name);
        }

        public IHSet<E> GetSet<E>(string name)
        {
            return GetDistributedObject<IHSet<E>>(ServiceNames.Set, name);
        }

        public IHList<E> GetList<E>(string name)
        {
            return GetDistributedObject<IHList<E>>(ServiceNames.List, name);
        }

        public IMap<K, V> GetMap<K, V>(string name)
        {
            return GetDistributedObject<IMap<K, V>>(ServiceNames.Map, name);
        }

        public IMultiMap<K, V> GetMultiMap<K, V>(string name)
        {
            return GetDistributedObject<IMultiMap<K, V>>(ServiceNames.MultiMap, name);
        }

        public ILock GetLock(string key)
        {
            return GetDistributedObject<ILock>(ServiceNames.Lock, key);
        }

        public ICluster GetCluster()
        {
            return new ClientClusterProxy(clusterService);
        }

        public IEndpoint GetLocalEndpoint()
        {
            return clusterService.GetLocalClient();
        }

        public ITransactionContext NewTransactionContext()
        {
            return NewTransactionContext(TransactionOptions.GetDefault());
        }

        public ITransactionContext NewTransactionContext(TransactionOptions options)
        {
            return new TransactionContextProxy(this, options);
        }

        public IIdGenerator GetIdGenerator(string name)
        {
            return GetDistributedObject<IIdGenerator>(ServiceNames.IdGenerator, name);
        }

        public IAtomicLong GetAtomicLong(string name)
        {
            return GetDistributedObject<IAtomicLong>(ServiceNames.AtomicLong, name);
        }

        public ICountDownLatch GetCountDownLatch(string name)
        {
            return GetDistributedObject<ICountDownLatch>(ServiceNames.CountDownLatch, name);
        }

        public ISemaphore GetSemaphore(string name)
        {
            return GetDistributedObject<ISemaphore>(ServiceNames.Semaphore, name);
        }

        public ICollection<IDistributedObject> GetDistributedObjects()
        {
            try
            {
                var request = ClientGetDistributedObjectsCodec.EncodeRequest();
                var task = invocationService.InvokeOnRandomTarget(request);
                var response = ThreadUtil.GetResult(task);
                var result = ClientGetDistributedObjectsCodec.DecodeResponse(response).infoCollection;
                foreach (var data in result)
                {
                    var o = serializationService.ToObject<DistributedObjectInfo>(data);
                    GetDistributedObject<IDistributedObject>(o.GetServiceName(), o.GetName());
                }
                return proxyManager.GetDistributedObjects();
            }
            catch (Exception e)
            {
                throw ExceptionUtil.Rethrow(e);
            }
        }

        public string AddDistributedObjectListener(IDistributedObjectListener distributedObjectListener)
        {
            return proxyManager.AddDistributedObjectListener(distributedObjectListener);
        }

        public bool RemoveDistributedObjectListener(string registrationId)
        {
            return proxyManager.RemoveDistributedObjectListener(registrationId);
        }

        public IClientService GetClientService()
        {
            throw new NotSupportedException();
        }

        public ILifecycleService GetLifecycleService()
        {
            return lifecycleService;
        }

        public T GetDistributedObject<T>(string serviceName, string name) where T : IDistributedObject
        {
            var clientProxy = proxyManager.GetOrCreateProxy<T>(serviceName, name);
            return (T) (clientProxy as IDistributedObject);
        }

        public ConcurrentDictionary<string, object> GetUserContext()
        {
            return userContext;
        }

        public void Shutdown()
        {
            GetLifecycleService().Shutdown();
        }

        /// <summary>
        ///     Gets all Hazelcast clients.
        /// </summary>
        /// <returns>ICollection&lt;IHazelcastInstance&gt;</returns>
        public static ICollection<IHazelcastInstance> GetAllHazelcastClients()
        {
            return (ICollection<IHazelcastInstance>) Clients.Values;
        }

        //    @Override
        public IClientPartitionService GetPartitionService()
        {
            throw new NotSupportedException("not supported yet");
            //return new PartitionServiceProxy(partitionService);
        }

        /// <summary>
        ///     Creates a new hazelcast client using default configuration.
        /// </summary>
        /// <remarks>
        ///     Creates a new hazelcast client using default configuration.
        /// </remarks>
        /// <returns>IHazelcastInstance.</returns>
        /// <example>
        ///     <code>
        ///     var hazelcastInstance = Hazelcast.NewHazelcastClient();
        ///     var myMap = hazelcastInstance.GetMap("myMap");
        /// </code>
        /// </example>
        public static IHazelcastInstance NewHazelcastClient()
        {
            return NewHazelcastClient(XmlClientConfigBuilder.Build());
        }

        /// <summary>
        ///     Creates a new hazelcast client using the given configuration xml file
        /// </summary>
        /// <param name="configFile">The configuration file with full or relative path.</param>
        /// <returns>IHazelcastInstance.</returns>
        /// <example>
        ///     <code>
        ///     //Full path
        ///     var hazelcastInstance = Hazelcast.NewHazelcastClient(@"C:\Users\user\Hazelcast.Net\hazelcast-client.xml");
        ///     var myMap = hazelcastInstance.GetMap("myMap");
        ///     
        ///     //relative path
        ///     var hazelcastInstance = Hazelcast.NewHazelcastClient(@"..\Hazelcast.Net\Resources\hazelcast-client.xml");
        ///     var myMap = hazelcastInstance.GetMap("myMap");
        /// </code>
        /// </example>
        public static IHazelcastInstance NewHazelcastClient(string configFile)
        {
            return NewHazelcastClient(XmlClientConfigBuilder.Build(configFile));
        }

        /// <summary>
        ///     Creates a new hazelcast client using the given configuration object created programmaticly.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <returns>IHazelcastInstance.</returns>
        /// <code>
        ///     var clientConfig = new ClientConfig();
        ///     //configure clientConfig ...
        ///     var hazelcastInstance = Hazelcast.NewHazelcastClient(clientConfig);
        ///     var myMap = hazelcastInstance.GetMap("myMap");
        /// </code>
        public static IHazelcastInstance NewHazelcastClient(ClientConfig config)
        {
            if (config == null)
            {
                config = XmlClientConfigBuilder.Build();
            }
            var client = new HazelcastClient(config);
            client.Start();
            var proxy = new HazelcastClientProxy(client);
            Clients.TryAdd(client.id, proxy);
            return proxy;
        }

        /// <summary>
        ///     Shutdowns all Hazelcast Clients .
        /// </summary>
        public static void ShutdownAll()
        {
            foreach (var proxy in Clients.Values)
            {
                try
                {
                    proxy.GetClient().GetLifecycleService().Shutdown();
                }
                catch (Exception)
                {
                }
            }
            Clients.Clear();
        }

        internal void DoShutdown()
        {
            HazelcastClientProxy _out;
            Clients.TryRemove(id, out _out);
            executionService.Shutdown();
            partitionService.Stop();
            connectionManager.Shutdown();
            proxyManager.Destroy();
            invocationService.Shutdown();
        }

        internal IClientClusterService GetClientClusterService()
        {
            return clusterService;
        }

        internal ClientConfig GetClientConfig()
        {
            return config;
        }

        internal IClientExecutionService GetClientExecutionService()
        {
            return executionService;
        }

        internal IClientPartitionService GetClientPartitionService()
        {
            return partitionService;
        }

        internal IClientConnectionManager GetConnectionManager()
        {
            return connectionManager;
        }

        internal IClientInvocationService GetInvocationService()
        {
            return invocationService;
        }

        internal IClientListenerService GetListenerService()
        {
            return listenerService;
        }

        internal ISerializationService GetSerializationService()
        {
            return serializationService;
        }

        private IClientInvocationService GetInvocationService(ClientConfig config)
        {
            return config.GetNetworkConfig().IsSmartRouting()
                ? (IClientInvocationService)new ClientSmartInvocationService(this)
                : new ClientNonSmartInvocationService(this);
        }
        private void Start()
        {
            lifecycleService.SetStarted();
            try
            {
                connectionManager.Start();
                clusterService.Start();
                partitionService.Start();
            }
            catch (InvalidOperationException e)
            {
                //there was an authentication failure (todo: perhaps use an AuthenticationException
                // ??)
                lifecycleService.Shutdown();
                throw;
            }
        }

        public ILoadBalancer GetLoadBalancer()
        {
            return loadBalancer;
        }
    }
}