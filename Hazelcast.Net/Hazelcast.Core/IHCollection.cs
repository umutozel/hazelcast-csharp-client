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

namespace Hazelcast.Core
{
    /// <summary>Concurrent, distributed, partitioned, listenable collection.</summary>
    /// <remarks>Concurrent, distributed, partitioned, listenable collection.</remarks>
    public interface IHCollection<T> : ICollection<T>,  IDistributedObject
    {

        /// <summary>Adds an item listener for this collection.</summary>
        /// <remarks>
        /// Adds an item listener for this collection. Listener will get notified
        /// for all collection add/remove events.
        /// </remarks>
        /// <param name="listener">item listener</param>
        /// <param name="includeValue">
        /// <tt>true</tt> updated item should be passed
        /// to the item listener, <tt>false</tt> otherwise.
        /// </param>
        /// <returns>returns registration id.</returns>
        string AddItemListener(IItemListener<T> listener, bool includeValue);

        /// <summary>Removes the specified item listener.</summary>
        /// <remarks>
        /// Removes the specified item listener.
        /// Returns silently if the specified listener is not added before.
        /// </remarks>
        /// <param name="registrationId">Id of listener registration.</param>
        /// <returns>true if registration is removed, false otherwise</returns>
        bool RemoveItemListener(string registrationId);

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns><c>true</c> if added, <c>false</c> otherwise.</returns>
        new bool Add(T item);

        /// <summary>
        /// Gets the number of elements contained in the collection.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the collection.
        /// </returns>
        int Size();

        /// <summary>
        /// Determines whether this instance is empty.
        /// </summary>
        /// <returns><c>true</c> if this instance is empty; otherwise, <c>false</c>.</returns>
        bool IsEmpty();

        /// <summary>
        /// Returns an array containing all of the elements in this collection.
        /// </summary>
        /// <returns>an array containing all of the elements in this collection.</returns>
        T[] ToArray();

        /// <summary>
        /// Returns an array containing all of the elements in this collection 
        /// the runtime type of the returned array is that of the specified array
        /// </summary>
        /// <typeparam name="T">return array type</typeparam>
        /// <param name="a">the array into which the elements of this collection are to be 
        /// stored, if it is big enough; otherwise, a new array of the same 
        /// runtime type is allocated for this purpose</param>
        /// <returns>an array containing all of the elements in this collection</returns>
        T[] ToArray<T>(T[] a);

        /// <summary>
        /// Determines whether this collection contains all of the elements in the specified collection.
        /// </summary>
        /// <typeparam name="T">type of elements</typeparam>
        /// <param name="c">The collection</param>
        /// <returns><c>true</c> if this collection contains all of the elements in the specified collection; otherwise, <c>false</c>.</returns>
        bool ContainsAll<T>(ICollection<T> c);

        /// <summary>
        /// Removes all of the elements in the specified collection from this collection.
        /// </summary>
        /// <typeparam name="T">type of elements</typeparam>
        /// <param name="c">element collection to be removed</param>
        /// <returns><c>true</c> if all removed, <c>false</c> otherwise.</returns>
        bool RemoveAll<T>(ICollection<T> c);

        /// <summary>
        /// Retains only the elements in this collection that are contained in the specified collection (optional operation).
        /// </summary>
        /// <remarks>
        /// Retains only the elements in this collection that are contained in the specified collection (optional operation).
        /// In other words, removes from this collection all of its elements that are not contained in the specified collection
        /// </remarks>
        /// <typeparam name="T">type of elements</typeparam>
        /// <param name="c">The c.</param>
        /// <returns><c>true</c> if this collection changed, <c>false</c> otherwise.</returns>
        bool RetainAll<T>(ICollection<T> c);

        /// <summary>
        /// Adds all.
        /// </summary>
        /// <typeparam name="T">type of elements</typeparam>
        /// <param name="c">element collection/param>
        /// <returns><c>true</c> if this collection changed, <c>false</c> otherwise.</returns>
        bool AddAll<T>(ICollection<T> c);
    }

}