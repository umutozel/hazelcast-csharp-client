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
using Hazelcast.Client.Spi;
using Hazelcast.Core;

namespace Hazelcast.Client.Proxy
{
    internal class ClientClusterProxy : ICluster
    {
        private readonly IClientClusterService clusterService;

        public ClientClusterProxy(IClientClusterService clusterService)
        {
            this.clusterService = clusterService;
        }

        public virtual string AddMembershipListener(IMembershipListener listener)
        {
            return clusterService.AddMembershipListener(listener);
        }

        public virtual bool RemoveMembershipListener(string registrationId)
        {
            return clusterService.RemoveMembershipListener(registrationId);
        }

        public virtual ISet<IMember> GetMembers()
        {
            ICollection<IMember> members = clusterService.GetMemberList();

            return members != null ? new HashSet<IMember>(members) : new HashSet<IMember>();
        }

        public virtual IMember GetLocalMember()
        {
            throw new NotSupportedException("IClient has no local member!");
        }

        public virtual long GetClusterTime()
        {
            return clusterService.GetClusterTime();
        }
    }
}