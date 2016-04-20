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

namespace Microsoft.Synchronization.ClientServices
{
    /// <summary>
    ///     Represents the various operation modes supported by SyncSvcUtil.exe
    ///     Maps to the /mode param
    /// </summary>
    internal enum OperationMode
    {
        Provision,
        Deprovision,
        Codegen
    }

    /// <summary>
    ///     Represents the various code generation language options supported by SyncSvcUtil.exe
    ///     Maps to /language param
    /// </summary>
    internal enum CodeLanguage
    {
        CS,
        VB,
        SVC
    }

    /// <summary>
    ///     Represents the mode (server/client/isclient) for which the entities supported by SyncSvcUtil.exe.
    ///     Maps to the /target param.
    /// </summary>
    internal enum CodeGenTarget
    {
        Server,
        ISClient,
        Client
    }

    internal static class EnumUtils
    {
        public static bool TryEnumParse<T>(string enumString, out T mode)
        {
            mode = default(T);
            try
            {
                mode = (T) Enum.Parse(typeof (T), enumString, true);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}