﻿// [[[[INFO>
// Copyright 2015 Epicycle (http://epicycle.org, https://github.com/open-epicycle)
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// For more information check https://github.com/open-epicycle/Epicycle.Commons-cs
// ]]]]

using Epicycle.Commons.FileSystem;

// Authors: untrots

namespace Epicycle.Commons.FileSystemBasedObjects
{
    /// <summary>
    /// This class is a base for objects that are based on a specific directory on a file system. In addition those objects
    /// have a YAML configuration file that is found in the root of their directory. The YAML file contains the configuration
    /// of the object. The implementation takes cares of the serialization/deserialization of the configuration object.
    /// </summary>
    /// <typeparam name="ConfigurationType">The type of the configuration object.</typeparam>
    public abstract class DirectoryBasedObjectWithConfiguration<ConfigurationType> : DirectoryBasedObject where ConfigurationType : new()
    {
        /// <summary>
        /// The name of the configuration file.
        /// </summary>
        private readonly string _configurationFileName;

        /// <summary>
        /// The confiugration of the object.
        /// </summary>
        private ConfigurationType _configuration;

        /// <summary>
        /// If false, a non-forced save will not actually touch the configuration file.
        /// </summary>
        private bool _configurationDirty;

        /// <summary>
        /// Checks whether the provided path points to a valid DirectoryBasedObjectWithConfiguration. A valid path should point to
        /// a directory and to contains a configuration file with the provided name.
        /// </summary>
        /// <param name="fileSystem">The file system to use. Must not be null.</param>
        /// <param name="path">The path to check.</param>
        /// <param name="configurationFileName">The file name of the configuration file.</param>
        /// <returns>True if the path points to a valid DirectoryBasedObjectWithConfiguration</returns>
        protected static bool IsPathToSuchObject(IFileSystem fileSystem, FileSystemPath path, string configurationFileName)
        {
            ArgAssert.NotNull(fileSystem, "fileSystem");
            ArgAssert.NotNull(path, "path");
            ArgAssert.NotNull(configurationFileName, "configurationFileName");

            if (!fileSystem.IsDirectory(path))
            {
                return false;
            }

            var configurationPath = path.Join(configurationFileName);

            return fileSystem.IsFile(configurationPath);
        }

        /// <summary>
        /// C-tor. If autoInit is true and the path does not exist then a new empty directory will be (recursevly) created.
        /// If the directory was created or already exists but is missing the configuration file, then a configuration file
        /// will be created with the default configuration. The default configuration is generated by the ConfigurationType's
        /// default constructor.
        /// 
        /// If autoInit is false then the path must point to a directory that contains the configuration file. The configuration
        /// will be read and stored in the object.
        /// </summary>
        /// <param name="fileSystem">The file system to use. Must not be null.</param>
        /// <param name="path">The path to the directory that the object is based on. Must not be null and point at nothing (if auto-init) or a directory.</param>
        /// <param name="configurationFileName">The file name of the configuration file. The configuration file is always at the root of the directory.</param>
        /// <param name="autoInit">If true, the project will initialize if not already initialized.</param>
        /// <exception cref="FileSystemPathDoesNotExistException">Thrown if the path points to a non-existing directory or if it lacks the configuration file and autoInit is false.</exception>
        /// <exception cref="DirectoryExpectedException">In case the path points to a non-directory file system entity.</exception>
        /// <exception cref="FileExpectedException">In case the configuration path points to a non-directory file system entity.</exception>
        public DirectoryBasedObjectWithConfiguration(IFileSystem fileSystem, FileSystemPath path, string configurationFileName, bool autoInit)
            : base(fileSystem, path, autoInit)
        {
            ArgAssert.NotNull(configurationFileName, "configurationFileName");

            _configurationFileName = configurationFileName;

            if (autoInit && !FileSystem.Exists(ConfigurationPath))
            {
                _configuration = new ConfigurationType();
                WriteConfiguration();
            }

            ReadConfiguration();

            _configurationDirty = false;
        }

        /// <summary>
        /// The path to the configuration file.
        /// </summary>
        public FileSystemPath ConfigurationPath
        {
            get { return Path.Join(_configurationFileName); }
        }

        /// <summary>
        /// Marked the current configuration as dirty.
        /// </summary>
        protected void ConfigurationDirty()
        {
            _configurationDirty = true;
        }

        /// <summary>
        /// Serialized the configuration to the file-system if dirty. If force is true then it is assumed to be always dirty.
        /// The dirty flag is always reseted.
        /// </summary>
        /// <param name="force">If true then the dirty flag is ignored and the configuration is always written to the file system.</param>
        protected void Save(bool force=false)
        {
            if (force || _configurationDirty) {
                WriteConfiguration();
            }
            _configurationDirty = false;
        }

        /// <summary>
        /// The current configuration.
        /// </summary>
        protected ConfigurationType Configuration
        {
            get { return _configuration; }
        }

        /// <summary>
        /// Deserializes configuration from the configuration file.
        /// </summary>
        private void ReadConfiguration()
        {
            var configurationPath = ConfigurationPath;

            FileSystem.AssertFile(configurationPath);
            _configuration = FileSystem.ReadYaml<ConfigurationType>(configurationPath);
        }

        /// <summary>
        /// Serializes the configuration to the configuration file
        /// </summary>
        private void WriteConfiguration()
        {
            var configurationPath = ConfigurationPath;

            FileSystem.AssertFileOrNotExsits(configurationPath);
            FileSystem.WriteYaml(configurationPath, _configuration);
        }
    }
}
