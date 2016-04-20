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

using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using Microsoft.Synchronization.Data;

namespace Microsoft.Synchronization.ClientServices.CodeDom
{
    /// <summary>
    ///     Generates the file containing entities that inherit from a base class that inherits from IOfflineEntity.
    ///     The custom base class implements the ServiceMetadata property of IOfflineEntity so we dont pollute all entities
    ///     with the ServiceMetadata property
    /// </summary>
    internal class GenericClientEntityGenerator : EntityGenerator
    {
        public override void GenerateEntities(string filePrefix, string nameSpace, DbSyncScopeDescription desc,
            Dictionary<string, Dictionary<string, string>> colsMappingInfo,
            DirectoryInfo dirInfo, CodeLanguage option, string serviceUri)
        {
            var entitiesCC = new CodeCompileUnit();

            var entityScopeNs = new CodeNamespace(nameSpace);

            // Generate the base class for all the entities which will implement IOfflineEntity
            var baseEntity = CodeDomUtility.CreateIOfflineEntityCustomBaseClass(
                string.IsNullOrEmpty(filePrefix) ? desc.ScopeName : filePrefix,
                false /*isServer*/);

            // Set the base type
            // VB uses different keywords for class and interface inheritence. For it to emit the
            // right keyword it must inherit from object first before the actual interface.
            baseEntity.BaseTypes.Add(new CodeTypeReference(typeof (object)));
            baseEntity.BaseTypes.Add(new CodeTypeReference(Constants.ClientIOfflineEntity));

            entityScopeNs.Types.Add(baseEntity);

            // Generate the entities
            foreach (var table in desc.Tables)
            {
                Dictionary<string, string> curTableMapping = null;
                colsMappingInfo.TryGetValue(table.UnquotedGlobalName, out curTableMapping);

                // Generate the actual entity
                var entityDecl = CodeDomUtility.GetEntityForTableDescription(table, true, curTableMapping);

                // Set the base type
                entityDecl.BaseTypes.Add(baseEntity.Name);

                //Add it to the overall scope
                entityScopeNs.Types.Add(entityDecl);
            }

            entitiesCC.Namespaces.Add(entityScopeNs);

            // Generate the files
            CodeDomUtility.SaveCompileUnitToFile(entitiesCC, option,
                CodeDomUtility.GenerateFileName(desc.ScopeName, dirInfo, filePrefix, "Entities", option));
        }
    }
}