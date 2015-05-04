﻿// ****************************************************************************
// <copyright file="ApiBusinessLogicValidationChecksTests.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace GoingOn.Frontend.Tests.Validation
{
    using System;
    using System.Threading.Tasks;
    using GoingOn.Frontend.Entities;
    using GoingOn.Frontend.Validation;
    using GoingOn.Storage;
    using GoingOn.Storage.TableStorage.Entities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;
    using Model.EntitiesBll;
    
    [TestClass]
    public class ApiBusinessLogicValidationChecksTests
    {
        private Mock<IUserStorage> userStorageMock;
        private Mock<INewsStorage> newsStorageMock;
        private Mock<IHotNewsStorage> hotNewsStorageMock;
        private ApiBusinessLogicValidationChecks businessValidation;

        private static readonly User user = new User { Nickname = "nickname", Password = "password", City = "Malaga" };

        private static readonly News news = new News { Title = "title", Content = "content" };

        [TestInitialize]
        public void Initialize()
        {
            this.userStorageMock = new Mock<IUserStorage>();
            this.newsStorageMock = new Mock<INewsStorage>();
            this.hotNewsStorageMock = new Mock<IHotNewsStorage>();
            this.businessValidation = new ApiBusinessLogicValidationChecks();
        }

        [TestMethod]
        public void TestIsValidCreateUserSucceedsIfStorageDoesNotContainsTheUser()
        {
            this.userStorageMock.Setup(storage => storage.ContainsUser(It.IsAny<UserBll>())).Returns(Task.FromResult(false));

            Assert.IsTrue(this.businessValidation.IsValidCreateUser(this.userStorageMock.Object, user).Result);
        }

        [TestMethod]
        public void TestIsValidCreateUserFailsIfStorageContainsTheUser()
        {
            this.userStorageMock.Setup(storage => storage.ContainsUser(It.IsAny<UserBll>())).Returns(Task.FromResult(true));

            Assert.IsFalse(this.businessValidation.IsValidCreateUser(this.userStorageMock.Object, user).Result);
        }

        [TestMethod]
        public void TestIsValidCreateNewsSucceedsIfStorageDoesNotContainsTheNews()
        {
            this.newsStorageMock.Setup(storage => storage.ContainsNewsCheckContent(It.IsAny<NewsEntity>())).Returns(Task.FromResult(false));

            Assert.IsTrue(this.businessValidation.IsValidCreateNews(this.newsStorageMock.Object, news, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()).Result);
        }

        [TestMethod]
        public void TestIsValidCreateNewsFailsIfStorageContainsTheNews()
        {
            this.newsStorageMock.Setup(storage => storage.ContainsNewsCheckContent(It.IsAny<NewsEntity>())).Returns(Task.FromResult(true));

            Assert.IsFalse(this.businessValidation.IsValidCreateNews(this.newsStorageMock.Object, news, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()).Result);
        }

        [TestMethod]
        public void TestIsValidGetHotNews_SucceedsIfStorageHasNews()
        {
            this.hotNewsStorageMock.Setup(storage => storage.ContainsHotNews(It.IsAny<string>(), It.IsAny<DateTime>())).Returns(Task.FromResult(true));

            Assert.IsTrue(this.businessValidation.IsValidGetHotNews(this.hotNewsStorageMock.Object, It.IsAny<string>(), It.IsAny<DateTime>()).Result);
        }

        [TestMethod]
        public void TestIsValidGetHotNews_FailsIfStorageDoesNotHaveNews()
        {
            this.hotNewsStorageMock.Setup(storage => storage.ContainsHotNews(It.IsAny<string>(), It.IsAny<DateTime>())).Returns(Task.FromResult(false));

            Assert.IsFalse(this.businessValidation.IsValidGetHotNews(this.hotNewsStorageMock.Object, It.IsAny<string>(), It.IsAny<DateTime>()).Result);
        }
    }
}
