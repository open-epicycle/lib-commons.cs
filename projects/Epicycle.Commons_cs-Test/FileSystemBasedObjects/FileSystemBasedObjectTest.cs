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
using NUnit.Framework;

namespace Epicycle.Commons.FileSystemBasedObjects
{
    [TestFixture]
    public class FileSystemBasedObjectTest
    {
        [Test]
        public void FileSystem_returns_the_filesystem_from_constructor()
        {
            var mockFileSystem = IFileSystemTestUtils.CreateMock();

            var testObject = new TestFileSystemBasedObject(mockFileSystem.Object);

            Assert.AreSame(mockFileSystem.Object, testObject.FileSystem);
        }

        private class TestFileSystemBasedObject : FileSystemBasedObject
        {
            public TestFileSystemBasedObject(IFileSystem fileSystem) : base(fileSystem) { }
        }
    }
}
