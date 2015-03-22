﻿// ****************************************************************************
// <copyright file="UriBuilderTests.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace GoingOn.Common.Tests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class GOUriBuilderTests
    {
        [TestMethod]
        public void BuildUserUriTest()
        {
            Assert.AreEqual("api/user/alberto", GOUriBuilder.BuildUserUri("alberto"));
        }

        [TestMethod]
        public void BuildAbsoluteUserUriTest()
        {
            Assert.AreEqual("scheme://host/api/user/alberto", GOUriBuilder.BuildAbsoluteUserUri("scheme", "host", "alberto"));
        }

        [TestMethod]
        public void BuildDiaryEntryUri()
        {
            Assert.AreEqual("api/city/Malaga/date/2015-12-05", GOUriBuilder.BuildDiaryEntryUri("Malaga", "2015-12-05"));
        }

        [TestMethod]
        public void BuildNewsUri()
        {
            var guid = Guid.NewGuid().ToString();

            Assert.AreEqual("api/city/Malaga/date/2015-12-05/news/" + guid, GOUriBuilder.BuildNewsUri("Malaga", "2015-12-05", guid));
        }

        [TestMethod]
        public void BuildAbsoluteNewsUri()
        {
            var guid = Guid.NewGuid().ToString();

            Assert.AreEqual("scheme://host/api/city/Malaga/date/2015-12-05/news/" + guid, GOUriBuilder.BuildAbsoluteNewsUri("scheme", "host", "Malaga", "2015-12-05", guid));
        }
    }
}