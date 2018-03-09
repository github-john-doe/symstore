﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.IO;
using System.Linq;
using TestHelpers;
using Xunit;

namespace Microsoft.FileFormats.ELF.Tests
{
    public class Tests
    {
        [Fact]
        public void CheckIndexingInfo()
        {
            using (Stream libcoreclr = TestUtilities.OpenCompressedFile("TestBinaries/libcoreclr.so.gz"))
            {
                StreamAddressSpace dataSource = new StreamAddressSpace(libcoreclr);
                ELFFile elf = new ELFFile(dataSource);
                Assert.True(elf.IsValid());
                Assert.True(elf.Header.Type == ELFHeaderType.Shared);
                string buildId = TestUtilities.ToHexString(elf.BuildID);

                //this is the build id for libcoreclr.so from package:
                // https://dotnet.myget.org/feed/dotnet-core/package/nuget/runtime.ubuntu.14.04-x64.Microsoft.NETCore.Runtime.CoreCLR/2.0.0-preview3-25428-01
                Assert.Equal("ef8f58a0b402d11c68f78342ef4fcc7d23798d4c", buildId);
            }

            // 32 bit arm ELF binary
            using (Stream apphost = TestUtilities.OpenCompressedFile("TestBinaries/apphost.gz"))
            {
                StreamAddressSpace dataSource = new StreamAddressSpace(apphost);
                ELFFile elf = new ELFFile(dataSource);
                Assert.True(elf.IsValid());
                Assert.True(elf.Header.Type == ELFHeaderType.Executable);
                string buildId = TestUtilities.ToHexString(elf.BuildID);

                //this is the build id for apphost from package:
                // https://dotnet.myget.org/F/dotnet-core/symbols/runtime.linux-arm.Microsoft.NETCore.DotNetAppHost/2.1.0-preview2-25512-03
                Assert.Equal("316d55471a8d5ebd6f2cb0631f0020518ab13dc0", buildId);
            }
        }

        [Fact]
        public void CheckDbgIndexingInfo()
        {
            using (Stream stream = TestUtilities.OpenCompressedFile("TestBinaries/libcoreclrtraceptprovider.so.dbg.gz"))
            {
                StreamAddressSpace dataSource = new StreamAddressSpace(stream);
                ELFFile elf = new ELFFile(dataSource);
                Assert.True(elf.IsValid());
                Assert.True(elf.Header.Type == ELFHeaderType.Shared);
                string buildId = TestUtilities.ToHexString(elf.BuildID);
                Assert.Equal("ce4ce0558d878a05754dff246ccea2a70a1db3a8", buildId);
            }
        }

        [Fact]
        public void ParseCore()
        {
            using (Stream core = TestUtilities.OpenCompressedFile("TestBinaries/core.gz"))
            {
                StreamAddressSpace dataSource = new StreamAddressSpace(core);
                ELFCoreFile coreReader = new ELFCoreFile(dataSource);
                Assert.True(coreReader.IsValid());
                ELFLoadedImage loadedImage = coreReader.LoadedImages.Where(i => i.Path.EndsWith("librt-2.17.so")).First();
                Assert.True(loadedImage.Image.IsValid());
                Assert.True(loadedImage.Image.Header.Type == ELFHeaderType.Shared);
                string buildId = TestUtilities.ToHexString(loadedImage.Image.BuildID);
                Assert.Equal("1d2ad4eaa62bad560685a4b8dccc8d9aa95e22ce", buildId);
            }
        }

        [Fact]
        public void ParseTriageDump()
        {
            using (Stream core = TestUtilities.OpenCompressedFile("TestBinaries/triagedump.gz"))
            {
                StreamAddressSpace dataSource = new StreamAddressSpace(core);
                ELFCoreFile coreReader = new ELFCoreFile(dataSource);
                Assert.True(coreReader.IsValid());
                ELFLoadedImage loadedImage = coreReader.LoadedImages.Where(i => i.Path.EndsWith("libcoreclr.so")).First();
                Assert.True(loadedImage.Image.IsValid());
                Assert.True(loadedImage.Image.Header.Type == ELFHeaderType.Shared);
                string buildId = TestUtilities.ToHexString(loadedImage.Image.BuildID);
                Assert.Equal("8f39a52a756311ab365090bfe9edef7ee8c44503", buildId);
            }
        }
    }
}
