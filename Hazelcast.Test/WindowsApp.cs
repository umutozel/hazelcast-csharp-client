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

﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using Hazelcast.Client;

namespace WindowsFormsApplication1
{
    public class Form1
    {


        static void Main131231(string[] args)
        {
            /***************************************************************
             * 3.2
             ***************************************************************/
            Hazelcast.Config.ClientConfig cc = new Hazelcast.Config.ClientConfig();

            Hazelcast.Config.ClientNetworkConfig nc = new Hazelcast.Config.ClientNetworkConfig();
            nc.AddAddress("127.0.0.1");
            //nc.AddAddress("192.168.10.16:5703");
            cc.SetNetworkConfig(nc);

            //Hazelcast.Config.GroupConfig gc = new Hazelcast.Config.GroupConfig();
            //gc.SetName("test");
            //gc.SetPassword("test1234");
            //cc.SetGroupConfig(gc);

            Hazelcast.Core.IHazelcastInstance hc = HazelcastClient.NewHazelcastClient(cc);

            test myTestInstance = new test();
            myTestInstance.myNumber = 999;

            Hazelcast.Core.IMap<int, test> a;
            a = hc.GetMap<int, test>((string)"test");

            a.Put(999, myTestInstance);

            test myTestInstance2 = a.Get(999);

            int abc = myTestInstance2.myNumber;
        }


    }
    public class test : Hazelcast.IO.Serialization.IDataSerializable
    {
        public int myNumber = 0;

        public string GetJavaClassName()
        {
            return this.GetType().FullName;
        }

        public void ReadData(Hazelcast.IO.IObjectDataInput input)
        {
            myNumber = input.ReadInt();
        }

        public void WriteData(Hazelcast.IO.IObjectDataOutput output)
        {
            output.WriteInt(myNumber);
        }
    }
}
