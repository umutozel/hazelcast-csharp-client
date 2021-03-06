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

using System.Collections.Generic;
using Hazelcast.Config;
using Hazelcast.Net.Ext;

namespace Hazelcast.Config
{
	/// <author>ali 06/02/14</author>
	public class ClientNetworkConfig
	{
		/// <summary>List of the initial set of addresses.</summary>
		/// <remarks>
		/// List of the initial set of addresses.
		/// Client will use this list to find a running Member, connect to it.
		/// </remarks>
        private readonly List<string> addressList = new List<string>(10);

		/// <summary>If true, client will route the key based operations to owner of the key at the best effort.</summary>
		/// <remarks>
		/// If true, client will route the key based operations to owner of the key at the best effort.
		/// Note that it  doesn't guarantee that the operation will always be executed on the owner. The cached table is updated every 10 seconds.
		/// </remarks>
		private bool smartRouting = true;

		/// <summary>If true, client will redo the operations that were executing on the server and client lost the connection.</summary>
		/// <remarks>
		/// If true, client will redo the operations that were executing on the server and client lost the connection.
		/// This can be because of network, or simply because the member died. However it is not clear whether the
		/// application is performed or not. For idempotent operations this is harmless, but for non idempotent ones
		/// retrying can cause to undesirable effects. Note that the redo can perform on any member.
		/// <p/>
		/// </remarks>
		private bool redoOperation = false;

        /// <summary>Timeout value in millis for nodes to accept client connection requests</summary>
		/// <remarks>
        /// Timeout value in millis for nodes to accept client connection requests
		/// </remarks>
		private int connectionTimeout = -1;

		/// <summary>
		/// While client is trying to connect initially to one of the members in the
		/// <see cref="addressList">addressList</see>
		/// ,
		/// all might be not available. Instead of giving up, throwing Exception and stopping client, it will
		/// attempt to retry as much as
		/// <see cref="connectionAttemptLimit">connectionAttemptLimit</see>
		/// times.
		/// </summary>
		private int connectionAttemptLimit = 20;

		/// <summary>Period for the next attempt to find a member to connect.</summary>
		/// <remarks>
		/// Period for the next attempt to find a member to connect. (see
		/// <see cref="connectionAttemptLimit">connectionAttemptLimit</see>
		/// ).
		/// </remarks>
		private int connectionAttemptPeriod = 5000;

		/// <summary>Will be called with the Socket, each time client creates a connection to any Member.</summary>
		/// <remarks>Will be called with the Socket, each time client creates a connection to any Member.</remarks>
		private SocketInterceptorConfig socketInterceptorConfig = null;

		/// <summary>Options for creating socket</summary>
		private SocketOptions socketOptions = new SocketOptions();

		/// <summary>Enabling ssl for client</summary>
        //private SSLConfig sslConfig = null;

		public virtual bool IsSmartRouting()
		{
			return smartRouting;
		}

		public virtual ClientNetworkConfig SetSmartRouting(bool smartRouting)
		{
			this.smartRouting = smartRouting;
			return this;
		}

		public virtual SocketInterceptorConfig GetSocketInterceptorConfig()
		{
			return socketInterceptorConfig;
		}

		public virtual ClientNetworkConfig SetSocketInterceptorConfig(SocketInterceptorConfig socketInterceptorConfig)
		{
			this.socketInterceptorConfig = socketInterceptorConfig;
			return this;
		}

		public virtual int GetConnectionAttemptPeriod()
		{
			return connectionAttemptPeriod;
		}

		public virtual ClientNetworkConfig SetConnectionAttemptPeriod(int connectionAttemptPeriod)
		{
			this.connectionAttemptPeriod = connectionAttemptPeriod;
			return this;
		}

		public virtual int GetConnectionAttemptLimit()
		{
			return connectionAttemptLimit;
		}

		public virtual ClientNetworkConfig SetConnectionAttemptLimit(int connectionAttemptLimit)
		{
			this.connectionAttemptLimit = connectionAttemptLimit;
			return this;
		}

		public virtual int GetConnectionTimeout()
		{
			return connectionTimeout;
		}

		public virtual ClientNetworkConfig SetConnectionTimeout(int connectionTimeout)
		{
			this.connectionTimeout = connectionTimeout;
			return this;
		}

		public virtual ClientNetworkConfig AddAddress(params string[] addresses)
		{
            addressList.AddRange(addresses);
			return this;
		}

		// required for spring module
		public virtual ClientNetworkConfig SetAddresses(IList<string> addresses)
		{
            addressList.Clear();
            addressList.AddRange(addresses);
			return this;
		}

		public virtual IList<string> GetAddresses()
		{
			if (addressList.Count == 0)
			{
				AddAddress("localhost");
			}
			return addressList;
		}

		public virtual bool IsRedoOperation()
		{
			return redoOperation;
		}

		public virtual ClientNetworkConfig SetRedoOperation(bool redoOperation)
		{
			this.redoOperation = redoOperation;
			return this;
		}

		public virtual SocketOptions GetSocketOptions()
		{
			return socketOptions;
		}

		public virtual ClientNetworkConfig SetSocketOptions(SocketOptions socketOptions)
		{
			this.socketOptions = socketOptions;
			return this;
		}

        ///// <summary>
        ///// Returns the current
        ///// <see cref="SSLConfig">SSLConfig</see>
        ///// . It is possible that null is returned if no SSLConfig has been
        ///// set.
        ///// </summary>
        ///// <returns>the SSLConfig.</returns>
        ///// <seealso cref="SetSSLConfig(SSLConfig)">SetSSLConfig(SSLConfig)</seealso>
        //public virtual SSLConfig GetSSLConfig()
        //{
        //    return sslConfig;
        //}

        ///// <summary>
        ///// Sets the
        ///// <see cref="SSLConfig">SSLConfig</see>
        ///// . null value indicates that no SSLConfig should be used.
        ///// </summary>
        ///// <param name="sslConfig">the SSLConfig.</param>
        ///// <returns>the updated ClientNetworkConfig.</returns>
        ///// <seealso cref="GetSSLConfig()">GetSSLConfig()</seealso>
        //public virtual ClientNetworkConfig SetSSLConfig(SSLConfig sslConfig)
        //{
        //    this.sslConfig = sslConfig;
        //    return this;
        //}
	}
}
