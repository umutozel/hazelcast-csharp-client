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

namespace Hazelcast.Core
{
    /// <summary>Map Entry event.</summary>
    /// <remarks>Map Entry event.</remarks>
    /// <typeparam name="K">type of key</typeparam>
    /// <typeparam name="V">type of value</typeparam>
    /// <seealso cref="IEntryListener{K,V}" />
    /// <seealso cref="IMap{K,V}.AddEntryListener(IEntryListener{K,V}, bool)" />
    [Serializable]
    public class EntryEvent<K, V> : AbstractMapEvent
    {
        private K key;
        private V oldValue;
        private V value;

        public EntryEvent(object source, IMember member, EntryEventType eventType, K key, V value)
            : this(source, member, eventType, key, default(V), value)
        {
        }

        public EntryEvent(object source, IMember member, EntryEventType eventType, K key, V oldValue, V value)
            : base(source, member,eventType)
        {
            this.key = key;
            this.oldValue = oldValue;
            this.value = value;
        }

        public override object GetSource()
        {
            return name;
        }

        /// <summary>Returns the key of the entry event</summary>
        /// <returns>the key</returns>
        public virtual K GetKey()
        {
            return key;
        }

        /// <summary>Returns the old value of the entry event</summary>
        /// <returns>the old value</returns>
        public virtual V GetOldValue()
        {
            return oldValue;
        }

        /// <summary>Returns the value of the entry event</summary>
        /// <returns>the valueS</returns>
        public virtual V GetValue()
        {
            return value;
        }

        public override string ToString()
        {
            return "EntryEvent {" + GetSource() + "} key=" + GetKey() + ", oldValue=" + GetOldValue() + ", value=" +
                   GetValue() + ", event=" + GetEventType() + ", by " + member;
        }
    }

    public abstract class AbstractMapEvent : EventObject
    {

        protected internal readonly string name;
        private readonly EntryEventType entryEventType;
        protected internal readonly IMember member;

        protected AbstractMapEvent(object source, IMember member, EntryEventType eventType)
            : base(source)
        {
            name = (string) source;
            this.member = member;
            entryEventType = eventType;
        }

        public override object GetSource()
        {
            return name;
        }

        /// <summary>Returns the member fired this event.</summary>
        /// <remarks>Returns the member fired this event.</remarks>
        /// <returns>the member fired this event.</returns>
        public virtual IMember GetMember()
        {
            return member;
        }

        /// <summary>Return the event type</summary>
        /// <returns>event type</returns>
        public virtual EntryEventType GetEventType()
        {
            return entryEventType;
        }

        /// <summary>Returns the name of the map for this event.</summary>
        /// <remarks>Returns the name of the map for this event.</remarks>
        /// <returns>name of the map.</returns>
        public virtual string GetName()
        {
            return name;
        }

        public override string ToString()
        {
            return "AbstractMapEvent {" + GetName() + "} eventType=" + entryEventType + ", by " + member;
        }   
    }

    public class EventObject
    {
        [NonSerialized] internal object Source;

        public EventObject(Object source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            Source = source;
        }

        /// <summary>
        /// The object on which the Event initially occurred.
        /// </summary>
        /// <returns>The object on which the Event initially occurred.</returns>
        public virtual Object GetSource()
        {
            return Source;
        }
    }
}