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
using System.IO;
using System.Linq;
using System.Text;

namespace Microsoft.Synchronization.ClientServices
{
    /// <summary>
    ///     Utility class to parse the input arguments
    /// </summary>
    internal class ArgsParser
    {
        private static string _helpString;
        private CodeGenTarget _codeGenMode;
        private string _generateFilePrefix = string.Empty;
        private CodeLanguage _language;
        private string _namespace = string.Empty;
        private OperationMode _operationMode;
        private string _scopeName;
        private string _targetDatabaseName;
        private DirectoryInfo _workingDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());

        public ArgsParser()
        {
            ModeSpecified = false;
            CSDLUrl = null;
            UseVerbose = false;
            HelpRequested = false;
        }

        public bool HelpRequested { get; private set; }

        public string GeneratedFilePrefix
        {
            get { return _generateFilePrefix; }
        }

        public OperationMode OperationMode
        {
            get { return _operationMode; }
        }

        public string ConfigFile { get; private set; }

        public bool UseCSDLUrl
        {
            get { return CSDLUrl != null; }
        }

        public bool UseVerbose { get; private set; }
        public string CSDLUrl { get; private set; }

        public string ScopeName
        {
            get { return _scopeName; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException(value);
                }
                _scopeName = value;
            }
        }

        public string TargetDatabaseName
        {
            get { return _targetDatabaseName; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException(value);
                }
                _targetDatabaseName = value;
            }
        }

        public CodeLanguage Language
        {
            get { return _language; }
        }

        public CodeGenTarget CodeGenMode
        {
            get { return _codeGenMode; }
        }

        public DirectoryInfo WorkingDirectory
        {
            get { return _workingDirectory; }
        }

        public bool ModeSpecified { get; private set; }

        public string Namespace
        {
            get { return _namespace; }
        }

        public static String GetHelpString()
        {
            if (String.IsNullOrEmpty(_helpString))
            {
                var builder = new StringBuilder(Constants.AssemblyName);
                builder.AppendLine(
                    " Params: [/?] [/directory:out-directory] /mode:operation-mode [/target:codegen-target] (/scopeconfig:configfile | /url:serviceCSDLurl) [/scopename:scope-name] [/database:target-database] [/language:codegen-language] [/outprefix:filenameprefix] [/verbose]");
                builder.AppendLine("Param Details:");
                builder.AppendLine("\t/?           : Print this help message");
                builder.AppendLine("\t/directory   : Working directory. Default is current directory");
                builder.AppendLine("\t/scopeconfig : XML file containing the SyncConfiguration entries.");
                builder.AppendLine("\t/url         : A URL for the Sync Scope Schema CSDL document.");
                builder.AppendLine(
                    "\t/scopename   : Name of <SyncScope> element. Required if more than one <SyncScope> entry is present.");
                builder.AppendLine(
                    "\t/database    : Name of <TargetDatabase> element. Required if more than one <TargetDatabase> entry is present.");
                builder.AppendLine("\t/language    : Code generation language. Options are CS and VB. Default is CS.");
                builder.AppendLine(
                    "\t/mode        : Operation mode of tool. Options are Provision, Deprovision and CodeGen. Required parameter.");
                builder.AppendLine(
                    "\t/target      : Target for which code is being generated. Options are server, isclient and client.");
                builder.AppendLine(
                    "\t\tserver - This will generate the .SVC and an Entities.cs file that represents the Sync Service and the types objects for that service.");
                builder.AppendLine(
                    "\t\tisclient - This will generate an Silverlight client context, that uses the isolated storage as the storage layer, and its corresponding entities.");
                builder.AppendLine(
                    "\t\tclient - This will generate just the entities that can be used with any custom silverlight storage layer.");
                builder.AppendLine(
                    "\t/outprefix   : Prefix name for all generated files. Default is the value of /scopename entry.");
                builder.AppendLine(
                    "\t/namespace   : Namespace for generated types. Default is the value of /scopename entry.");
                builder.AppendLine("\t/verbose     : Emit verbose information.");

                _helpString = builder.ToString();
            }
            return _helpString;
        }

        public static ArgsParser ParseArgs(string[] args)
        {
            var parser = new ArgsParser();

            foreach (var param in args)
            {
                var tokens = param.Split(new[] {':'}, 2, StringSplitOptions.RemoveEmptyEntries);

                if (tokens.Count() != 2 &&
                    !tokens[0].Equals("/?", StringComparison.InvariantCultureIgnoreCase) &&
                    !tokens[0].Equals("/verbose", StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new ArgumentException("Invalid parameter passed", param);
                }

                switch (tokens[0].ToLowerInvariant())
                {
                    case "/mode":
                        parser.ModeSpecified = true;
                        if (!EnumUtils.TryEnumParse(tokens[1], out parser._operationMode))
                        {
                            throw new ArgumentOutOfRangeException(param,
                                string.Format("Invalid {0} option specified.", tokens[0]));
                        }
                        break;
                    case "/target":
                        if (!EnumUtils.TryEnumParse(tokens[1], out parser._codeGenMode))
                        {
                            throw new ArgumentOutOfRangeException(param,
                                string.Format("Invalid {0} option specified.", tokens[0]));
                        }
                        break;
                    case "/language":
                        if (!EnumUtils.TryEnumParse(tokens[1], out parser._language))
                        {
                            throw new ArgumentOutOfRangeException(param,
                                string.Format("Invalid {0} option specified.", tokens[0]));
                        }
                        break;
                    case "/scopeconfig":
                        if (parser.CSDLUrl != null)
                        {
                            throw new InvalidOperationException("Cannot specify both /scopeconfig and /url option.");
                        }
                        parser.ConfigFile = tokens[1];
                        break;
                    case "/url":
                        if (parser.ConfigFile != null)
                        {
                            throw new InvalidOperationException("Cannot specify both /scopeconfig and /url option.");
                        }
                        parser.CSDLUrl = tokens[1];
                        if (!new Uri(parser.CSDLUrl).IsAbsoluteUri)
                        {
                            throw new ArgumentException(string.Format("Sync scope schema Uri cannot be relative."),
                                param);
                        }
                        break;
                    case "/scopename":
                        parser._scopeName = tokens[1];
                        break;
                    case "/database":
                        parser._targetDatabaseName = tokens[1];
                        break;
                    case "/outprefix":
                        parser._generateFilePrefix = tokens[1];
                        break;
                    case "/directory":
                        parser._workingDirectory = new DirectoryInfo(tokens[1]);
                        break;
                    case "/namespace":
                        parser._namespace = tokens[1];
                        break;
                    case "/verbose":
                        parser.UseVerbose = true;
                        break;
                    case "/?":
                        parser.HelpRequested = true;
                        Console.WriteLine(GetHelpString());
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(param);
                }
            }

            return parser;
        }
    }
}