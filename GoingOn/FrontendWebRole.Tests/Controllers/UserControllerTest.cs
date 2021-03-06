﻿// ****************************************************************************
// <copyright file="UserControllerTest.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// UserController tests class
// </summary>
// ****************************************************************************

namespace GoingOn.FrontendWebRole.Tests.Controllers
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Security.Principal;
    using System.Threading.Tasks;
    using System.Web.Http.Routing;
    
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Newtonsoft.Json;
    using WebApiContrib.Testing;

    using GoingOn.FrontendWebRole.Controllers;
    using GoingOn.Common;
    using GoingOn.Common.Tests;
    using GoingOn.Frontend.Entities;
    using GoingOn.Frontend.Validation;
    using GoingOn.Repository;
    using Model.EntitiesBll;

    [TestClass]
    public class UserControllerTest
    {
        private Mock<IUserRepository> mockUserRepository;
        private Mock<IApiInputValidationChecks> inputValidation;
        private Mock<IApiBusinessLogicValidationChecks> businessValidation;

        private static readonly User DefaultUser = new User { Nickname = "nickname", Password = "password" };

        private const string Scheme = "http";
        private const string Host = "test.com";
        private const int Port = 123;

        [TestInitialize]
        public void Initizalize()
        {
            this.mockUserRepository = new Mock<IUserRepository>();
            this.inputValidation = new Mock<IApiInputValidationChecks>();
            this.businessValidation = new Mock<IApiBusinessLogicValidationChecks>();
        }

        [TestMethod]
        public void TestGetUserReturns200OkWhenTheUserIsInTheDatabase()
        {
            this.inputValidation.Setup(validation => validation.IsValidNickName(It.IsAny<string>())).Returns(true);
            this.businessValidation.Setup(validation => validation.IsAuthorizedUser(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            this.businessValidation.Setup(validation => validation.IsUserCreated(this.mockUserRepository.Object, It.IsAny<string>())).Returns(Task.FromResult(true));
            this.mockUserRepository.Setup(storage => storage.GetUser(It.IsAny<string>())).Returns(Task.FromResult(User.ToUserBll(DefaultUser)));

            var userController = new UserController(this.mockUserRepository.Object, this.inputValidation.Object, this.businessValidation.Object);

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, GOUriBuilder.BuildAbsoluteUserUri(Scheme, Host, Port, DefaultUser.Nickname));
            request.Headers.Referrer = new Uri(GOUriBuilder.BuildAbsoluteUserUri(Scheme, Host, Port, DefaultUser.Nickname));

            userController.ConfigureForTesting(request, "GetUser", new HttpRoute(GOUriBuilder.GetUserTemplate));
            userController.User = new GenericPrincipal(new GenericIdentity(DefaultUser.Nickname), null);

            HttpResponseMessage response = userController.Get(DefaultUser.Nickname).Result;

            var content = response.Content;
            var jsonContent = content.ReadAsStringAsync().Result;
            var actualUser = JsonConvert.DeserializeObject<UserREST>(jsonContent);
            
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(new UserCompleteEqualityComparer().Equals(DefaultUser, UserREST.ToUser(actualUser)));
            Assert.IsTrue(actualUser.Links.Any());
            Assert.AreEqual("self", actualUser.Links.First().Rel);
            Assert.AreEqual(new Uri(GOUriBuilder.BuildAbsoluteUserUri(Scheme, Host, Port, DefaultUser.Nickname)), actualUser.Links.First().Href);
        }

        [TestMethod]
        public void TestGetUserReturns400BadRequestWhenInputValidationFails()
        {
            this.inputValidation.Setup(validation => validation.IsValidNickName(It.IsAny<string>())).Returns(false);

            this.AssertGetFails(url: "http://test.com/api/user/nickname", nickname: "username", resultCode: HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void TestGetUserReturns401_WhenUserTriesToGetAnotherUser()
        {
            this.inputValidation.Setup(validation => validation.IsValidNickName(It.IsAny<string>())).Returns(true);
            this.businessValidation.Setup(validation => validation.IsAuthorizedUser(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            this.AssertGetFails(url: "http://test.com/api/user/nickname", nickname: "username", resultCode: HttpStatusCode.Unauthorized);
        }

        [TestMethod]
        public void TestGetUserReturns404NotFoundWhenBusinessValidationFails()
        {
            this.inputValidation.Setup(validation => validation.IsValidNickName(It.IsAny<string>())).Returns(true);
            this.businessValidation.Setup(validation => validation.IsAuthorizedUser(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            this.businessValidation.Setup(validation => validation.IsUserCreated(this.mockUserRepository.Object, It.IsAny<string>())).Returns(Task.FromResult(false));

            this.AssertGetFails(url: "http://test.com/api/user/nickname", nickname: "username", resultCode: HttpStatusCode.NotFound);
        }

        [TestMethod]
        public void TestPostUserReturns200OkWhenCreatesUser()
        {
            this.inputValidation.Setup(validation => validation.IsValidUser(It.IsAny<User>())).Returns(true);
            this.businessValidation.Setup(validation => validation.IsValidCreateUser(this.mockUserRepository.Object, It.IsAny<User>())).Returns(Task.FromResult(true));

            UserController userController = new UserController(this.mockUserRepository.Object, this.inputValidation.Object, this.businessValidation.Object);

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, GOUriBuilder.BuildCreateAbsoluteUserUri(Scheme, Host, Port));
            request.Headers.Referrer = new Uri(GOUriBuilder.BuildCreateAbsoluteUserUri(Scheme, Host, Port));

            userController.ConfigureForTesting(request, "PostUser", new HttpRoute(GOUriBuilder.GetUserTemplate));
            userController.User = new GenericPrincipal(new GenericIdentity(DefaultUser.Nickname), null);

            HttpResponseMessage response = userController.Post(DefaultUser).Result;

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            Assert.AreEqual(new Uri(GOUriBuilder.BuildAbsoluteUserUri(Scheme, Host, Port, DefaultUser.Nickname)), response.Headers.Location);
            this.mockUserRepository.Verify(storage => storage.AddUser(It.IsAny<UserBll>()), Times.Once());
        }

        [TestMethod]
        public void TestPostUserReturns400BadRequestWhenInputValidationFails()
        {
            this.inputValidation.Setup(validation => validation.IsValidUser(It.IsAny<User>())).Returns(false);
            this.businessValidation.Setup(validation => validation.IsValidCreateUser(this.mockUserRepository.Object, It.IsAny<User>())).Returns(Task.FromResult(true));

            this.AssertPostFails(url: "http://test.com/api/user/", user: DefaultUser, resultCode: HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void TestPostUserReturns400BadRequestWhenBusinessValidationFails()
        {
            this.inputValidation.Setup(validation => validation.IsValidUser(It.IsAny<User>())).Returns(true);
            this.businessValidation.Setup(validation => validation.IsValidCreateUser(this.mockUserRepository.Object, It.IsAny<User>())).Returns(Task.FromResult(false));

            this.AssertPostFails(url: "http://test.com/api/user/", user: DefaultUser, resultCode: HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void TestPatchUserReturns204NoContentWhenUpdatesUser()
        {
            this.inputValidation.Setup(validation => validation.IsValidNickName(It.IsAny<string>())).Returns(true);
            this.inputValidation.Setup(validation => validation.IsValidUser(It.IsAny<User>())).Returns(true);
            this.businessValidation.Setup(validation => validation.IsAuthorizedUser(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            this.businessValidation.Setup(validation => validation.IsUserCreated(this.mockUserRepository.Object, It.IsAny<User>())).Returns(Task.FromResult(true));

            UserController userController = new UserController(this.mockUserRepository.Object, this.inputValidation.Object, this.businessValidation.Object);
            userController.ConfigureForTesting(new HttpMethod("PATCH"), "http://test.com/api/user/nickname", "GetUser", new HttpRoute(GOUriBuilder.GetUserTemplate));
            userController.User = new GenericPrincipal(new GenericIdentity(DefaultUser.Nickname), null);

            HttpResponseMessage response = userController.Patch(DefaultUser.Nickname, DefaultUser).Result;

            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
            this.mockUserRepository.Verify(storage => storage.UpdateUser(It.IsAny<UserBll>()), Times.Once());
        }

        [TestMethod]
        public void TestPatchUserReturns400BadRequestWhenNicknameValidationFails()
        {
            this.inputValidation.Setup(validation => validation.IsValidNickName(It.IsAny<string>())).Returns(false);
            this.inputValidation.Setup(validation => validation.IsValidUser(It.IsAny<User>())).Returns(true);
            this.businessValidation.Setup(validation => validation.IsAuthorizedUser(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            this.businessValidation.Setup(validation => validation.IsUserCreated(this.mockUserRepository.Object, It.IsAny<User>())).Returns(Task.FromResult(true));

            this.AssertPatchFails(nickname: "username", url: "http://test.com/api/user/nickname", user: DefaultUser, resultCode: HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void TestPatchUserReturns400BadRequestWhenUserValidationFails()
        {
            this.inputValidation.Setup(validation => validation.IsValidNickName(It.IsAny<string>())).Returns(true);
            this.inputValidation.Setup(validation => validation.IsValidUser(It.IsAny<User>())).Returns(false);
            this.businessValidation.Setup(validation => validation.IsAuthorizedUser(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            this.businessValidation.Setup(validation => validation.IsUserCreated(this.mockUserRepository.Object, It.IsAny<User>())).Returns(Task.FromResult(true));

            this.AssertPatchFails(nickname: "nickname", url: "http://test.com/api/user/nickname", user: DefaultUser, resultCode: HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void TestPatchUserReturns404NotFoundWhenBusinessValidationFails()
        {
            this.inputValidation.Setup(validation => validation.IsValidNickName(It.IsAny<string>())).Returns(true);
            this.inputValidation.Setup(validation => validation.IsValidUser(It.IsAny<User>())).Returns(true);
            this.businessValidation.Setup(validation => validation.IsAuthorizedUser(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            this.businessValidation.Setup(validation => validation.IsUserCreated(this.mockUserRepository.Object, It.IsAny<User>())).Returns(Task.FromResult(false));

            this.AssertPatchFails(nickname: "nickname", url: "http://test.com/api/user/nickname", user: DefaultUser, resultCode: HttpStatusCode.NotFound);
        }

        [TestMethod]
        public void TestPatchUserReturns401UnauthorizedWhenTheUserTryesToUpdateAnotherUser()
        {
            this.inputValidation.Setup(validation => validation.IsValidNickName(It.IsAny<string>())).Returns(true);
            this.inputValidation.Setup(validation => validation.IsValidUser(It.IsAny<User>())).Returns(true);
            this.businessValidation.Setup(validation => validation.IsAuthorizedUser(It.IsAny<string>(), It.IsAny<string>())).Returns(false);
            this.businessValidation.Setup(validation => validation.IsUserCreated(this.mockUserRepository.Object, It.IsAny<User>())).Returns(Task.FromResult(true));

            this.AssertPatchFails(nickname: "username", url: "http://test.com/api/user/nickname", user: DefaultUser, resultCode: HttpStatusCode.Unauthorized);
        }

        [TestMethod]
        public void TestDeleteUserReturns204NoContentWhenDeletesUser()
        {
            this.inputValidation.Setup(validation => validation.IsValidNickName(It.IsAny<string>())).Returns(true);
            this.businessValidation.Setup(validation => validation.IsAuthorizedUser(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            this.businessValidation.Setup(validation => validation.IsUserCreated(this.mockUserRepository.Object, It.IsAny<string>())).Returns(Task.FromResult(true));

            UserController userController = new UserController(this.mockUserRepository.Object, this.inputValidation.Object, this.businessValidation.Object);
            userController.ConfigureForTesting(HttpMethod.Delete, "http://test.com/api/user/" + DefaultUser.Nickname, "GetUser", new HttpRoute(GOUriBuilder.GetUserTemplate));
            userController.User = new GenericPrincipal(new GenericIdentity(DefaultUser.Nickname), null);

            HttpResponseMessage response = userController.Delete(DefaultUser.Nickname).Result;

            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
            this.mockUserRepository.Verify(storage => storage.DeleteUser(It.IsAny<UserBll>()), Times.Once());
        }

        [TestMethod]
        public void TestDeleteUserReturns400BadRequestWhenInputValidationFails()
        {
            this.inputValidation.Setup(validation => validation.IsValidNickName(It.IsAny<string>())).Returns(false);
            this.businessValidation.Setup(validation => validation.IsAuthorizedUser(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            this.businessValidation.Setup(validation => validation.IsUserCreated(this.mockUserRepository.Object, It.IsAny<string>())).Returns(Task.FromResult(true));

            this.AssertDeleteFails(nickname: DefaultUser.Nickname, url: "http://test.com/api/user/" + DefaultUser.Nickname, resultCode: HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void TestDeleteUserReturns401UnauthorizedWhenTheUserTryesToDeleteAnotherUser()
        {
            this.inputValidation.Setup(validation => validation.IsValidNickName(It.IsAny<string>())).Returns(true);
            this.businessValidation.Setup(validation => validation.IsAuthorizedUser(It.IsAny<string>(), It.IsAny<string>())).Returns(false);
            this.businessValidation.Setup(validation => validation.IsUserCreated(this.mockUserRepository.Object, It.IsAny<string>())).Returns(Task.FromResult(true));

            this.AssertDeleteFails(nickname: DefaultUser.Nickname, url: "http://test.com/api/user/" + DefaultUser.Nickname, resultCode: HttpStatusCode.Unauthorized);
        }

        [TestMethod]
        public void TestDeleteUserReturns404NotFoundWhenTheUserIsNotInTheDatabase()
        {
            this.inputValidation.Setup(validation => validation.IsValidNickName(It.IsAny<string>())).Returns(true);
            this.businessValidation.Setup(validation => validation.IsAuthorizedUser(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            this.businessValidation.Setup(validation => validation.IsUserCreated(this.mockUserRepository.Object, It.IsAny<string>())).Returns(Task.FromResult(false));

            this.AssertDeleteFails(nickname: DefaultUser.Nickname, url: "http://test.com/api/user/" + DefaultUser.Nickname, resultCode: HttpStatusCode.NotFound);
        }

        #region Assert helper methods

        private void AssertGetFails(string url, string nickname, HttpStatusCode resultCode)
        {
            UserController userController = new UserController(this.mockUserRepository.Object, this.inputValidation.Object, this.businessValidation.Object);
            userController.ConfigureForTesting(HttpMethod.Get, url, "GetUser", new HttpRoute(GOUriBuilder.GetUserTemplate));
            userController.User = new GenericPrincipal(new GenericIdentity(DefaultUser.Nickname), null);

            HttpResponseMessage response = userController.Get(nickname).Result;

            Assert.AreEqual(resultCode, response.StatusCode);
            this.mockUserRepository.Verify(storage => storage.GetUser(It.IsAny<string>()), Times.Never());
        }

        private void AssertPostFails(string url, User user, HttpStatusCode resultCode)
        {
            UserController userController = new UserController(this.mockUserRepository.Object, this.inputValidation.Object, this.businessValidation.Object);
            userController.ConfigureForTesting(HttpMethod.Post, url, "PostUser", new HttpRoute(GOUriBuilder.GetUserTemplate));
            userController.User = new GenericPrincipal(new GenericIdentity(DefaultUser.Nickname), null);

            HttpResponseMessage response = userController.Post(user).Result;

            Assert.AreEqual(resultCode, response.StatusCode);
            this.mockUserRepository.Verify(storage => storage.AddUser(It.IsAny<UserBll>()), Times.Never());
        }

        private void AssertPatchFails(string nickname, string url, User user, HttpStatusCode resultCode)
        {
            UserController userController = new UserController(this.mockUserRepository.Object, this.inputValidation.Object, this.businessValidation.Object);
            userController.ConfigureForTesting(new HttpMethod("PATCH"), url, "PatchUser", new HttpRoute(GOUriBuilder.GetUserTemplate));
            userController.User = new GenericPrincipal(new GenericIdentity(user.Nickname), null);

            HttpResponseMessage response = userController.Patch(nickname, user).Result;

            Assert.AreEqual(resultCode, response.StatusCode);
            this.mockUserRepository.Verify(storage => storage.UpdateUser(It.IsAny<UserBll>()), Times.Never());
        }

        private void AssertDeleteFails(string nickname, string url, HttpStatusCode resultCode)
        {
            UserController userController = new UserController(this.mockUserRepository.Object, this.inputValidation.Object, this.businessValidation.Object);
            userController.ConfigureForTesting(HttpMethod.Delete, url, "DeleteUser", new HttpRoute(GOUriBuilder.GetUserTemplate));
            userController.User = new GenericPrincipal(new GenericIdentity(DefaultUser.Nickname), null);

            HttpResponseMessage response = userController.Delete(nickname).Result;

            Assert.AreEqual(resultCode, response.StatusCode);
            this.mockUserRepository.Verify(storage => storage.DeleteUser(It.IsAny<UserBll>()), Times.Never());
        }

        #endregion
    }
}
