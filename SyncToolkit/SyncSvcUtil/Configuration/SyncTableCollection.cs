// Copyright 2010 Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License"); 
// You may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

// THIS CODE IS PROVIDED *AS IS* BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, 
// INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR 
// CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, 
// MERCHANTABLITY OR NON-INFRINGEMENT. 

// See the Apache 2 License for the specific language governing 
// permissions and limitations under the License.

using System;
using System.Configuration;

namespace Microsoft.Synchronization.ClientServices.Configuration
{
    /// <summary>
    ///     ConfigElement representing the SyncTable config section.
    /// </summary>
    public class SyncTableCollection : ConfigurationElementCollection
    {
        /// <summary>
        ///     Returns the ConfigurationElementCollectionType type for this collection.
        /// </summary>
        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }

        /// <summary>
        ///     Returns the element name used to represent the collection items
        /// </summary>
        protected override string ElementName
        {
            get { return "SyncTable"; }
        }

        /// <summary>
        ///     Creates a new Collection item element.
        /// </summary>
        /// <returns>SyncTableConfigElement</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new SyncTableConfigElement();
        }

        /// <summary>
        ///     Returns the SyncTableConfigElement.Name property for the specifed element
        /// </summary>
        /// <param name="element">ConfigurationElement</param>
        /// <returns>Name property</returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((SyncTableConfigElement) element).Name;
        }

        /// <summary>
        ///     Allows programmatic addition to the Collection
        /// </summary>
        /// <param name="element">Element to add to collection</param>
        public void Add(SyncTableConfigElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            BaseAdd(element, true);
        }

        /// <summary>
        ///     Allows programmatic addition to the Collection at the specified index
        /// </summary>
        /// <param name="index">Index at which to add the element</param>
        /// <param name="element">Element to add to collection</param>
        public void Add(int index, SyncTableConfigElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            BaseAdd(index, element);
        }

        /// <summary>
        ///     Allows programmatic removal from the Collection
        /// </summary>
        /// <param name="elementKey">Element key to remove from collection</param>
        public void Remove(string elementKey)
        {
            if (elementKey == null)
            {
                throw new ArgumentNullException("element");
            }

            BaseRemove(elementKey);
        }

        /// <summary>
        ///     Allows programmatic removal from the Collection from the specified index
        /// </summary>
        /// <param name="index">Index of element to remove from collection</param>
        public void RemoveAt(int index)
        {
            if (index < 0)
            {
                throw new IndexOutOfRangeException();
            }

            BaseRemoveAt(index);
        }

        /// <summary>
        ///     Allows programmatic retrieval of indexed item from the Collection
        /// </summary>
        /// <param name="index">Index of Element to retrieve</param>
        /// <returns>SyncTableConfigElement</returns>
        public SyncTableConfigElement GetElementAt(int index)
        {
            return (SyncTableConfigElement) BaseGet(index);
        }

        /// <summary>
        ///     Allows programmatic retrieval of item from the Collection based on its key
        /// </summary>
        /// <param name="key">Key of Element to retrieve</param>
        /// <returns>SyncTableConfigElement</returns>
        public SyncTableConfigElement GetElement(string key)
        {
            return (SyncTableConfigElement) BaseGet(key);
        }
    }
}