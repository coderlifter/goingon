﻿// ****************************************************************************
// <copyright file="ApiInputValidationChecksTests.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// Validation of user input
// </summary>
// ****************************************************************************

namespace GoingOn.Frontend.Tests.Validation
{
    using System;
    using System.Net.Http.Headers;
    using GoingOn.Common.Tests;
    using GoingOn.Frontend.Common;
    using GoingOn.Frontend.Entities;
    using GoingOn.Frontend.Validation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    [TestClass]
    public class ApiInputValidationChecksTests
    {
        private ApiInputValidationChecks inputValidation;
        private Mock<IApiInputValidationChecks> mockInputValidation;

        [TestInitialize]
        public void Initialize()
        {
            this.mockInputValidation = new Mock<IApiInputValidationChecks>();
            this.inputValidation = new ApiInputValidationChecks(mockInputValidation.Object);
        }

        [TestMethod]
        public void TestIsValidUserSucceedsWithWellFormedUser()
        {
            User user = new User();

            this.mockInputValidation.Setup(iv => iv.IsValidNickName(It.IsAny<string>())).Returns(true);
            this.mockInputValidation.Setup(iv => iv.IsValidPassword(It.IsAny<string>())).Returns(true);
            this.mockInputValidation.Setup(iv => iv.IsValidCity(It.IsAny<string>())).Returns(true);
            this.mockInputValidation.Setup(iv => iv.IsValidName(It.IsAny<string>())).Returns(true);
            this.mockInputValidation.Setup(iv => iv.IsValidEmail(It.IsAny<string>())).Returns(true);
            this.mockInputValidation.Setup(iv => iv.IsValidBirthDate(It.IsAny<DateTime>())).Returns(true);

            Assert.IsTrue(inputValidation.IsValidUser(user));
        }

        [TestMethod]
        public void TestIsValidUserFailsWithNullUser()
        {
            Assert.IsFalse(inputValidation.IsValidUser(null));
        }

        [TestMethod]
        public void TestIsValidNickname()
        {
            Assert.IsTrue(inputValidation.IsValidNickName("nickname"));
            Assert.IsFalse(inputValidation.IsValidNickName(null));
            Assert.IsFalse(inputValidation.IsValidNickName(string.Empty));
            Assert.IsFalse(inputValidation.IsValidNickName(" \n\t"));   
        }

        [TestMethod]
        public void TestIsValidUserFailsWithIfWhenIsValidNicknameFails()
        {
            mockInputValidation.Setup(iv => iv.IsValidNickName(It.IsAny<string>())).Returns(false);
            mockInputValidation.Setup(iv => iv.IsValidPassword(It.IsAny<string>())).Returns(true);
            mockInputValidation.Setup(iv => iv.IsValidCity(It.IsAny<string>())).Returns(true);
            mockInputValidation.Setup(iv => iv.IsValidName(It.IsAny<string>())).Returns(true);
            mockInputValidation.Setup(iv => iv.IsValidEmail(It.IsAny<string>())).Returns(true);
            mockInputValidation.Setup(iv => iv.IsValidBirthDate(It.IsAny<DateTime>())).Returns(true);

            Assert.IsFalse(this.inputValidation.IsValidUser(It.IsAny<User>()));
        }

        [TestMethod]
        public void TestIsValidPassword()
        {
            Assert.IsTrue(inputValidation.IsValidPassword("password"));
            Assert.IsFalse(inputValidation.IsValidPassword(null));
            Assert.IsFalse(inputValidation.IsValidPassword(string.Empty));
            Assert.IsFalse(inputValidation.IsValidPassword(" \n\t"));   
        }

        [TestMethod]
        public void TestIsValidUserFailsWithIfWhenIsValidPasswordFails()
        {
            mockInputValidation.Setup(iv => iv.IsValidNickName(It.IsAny<string>())).Returns(true);
            mockInputValidation.Setup(iv => iv.IsValidPassword(It.IsAny<string>())).Returns(false);
            mockInputValidation.Setup(iv => iv.IsValidCity(It.IsAny<string>())).Returns(true);
            mockInputValidation.Setup(iv => iv.IsValidName(It.IsAny<string>())).Returns(true);
            mockInputValidation.Setup(iv => iv.IsValidEmail(It.IsAny<string>())).Returns(true);
            mockInputValidation.Setup(iv => iv.IsValidBirthDate(It.IsAny<DateTime>())).Returns(true);

            Assert.IsFalse(this.inputValidation.IsValidUser(It.IsAny<User>()));
        }

        [TestMethod]
        public void TestIsValidCity()
        {
            Assert.IsTrue(inputValidation.IsValidCity("Malaga"));
            Assert.IsTrue(inputValidation.IsValidCity("Granada"));
            Assert.IsTrue(inputValidation.IsValidCity("Sevilla"));
            Assert.IsTrue(inputValidation.IsValidCity("Cadiz"));
            Assert.IsTrue(inputValidation.IsValidCity("Almeria"));
            Assert.IsTrue(inputValidation.IsValidCity("Cordoba"));
            Assert.IsTrue(inputValidation.IsValidCity("Huelva"));
            Assert.IsFalse(inputValidation.IsValidCity(null));
            Assert.IsFalse(inputValidation.IsValidCity(string.Empty));
            Assert.IsFalse(inputValidation.IsValidCity(" \n\t"));
        }

        [TestMethod]
        public void TestIsValidUserFailsWithIfWhenIsValidCityFails()
        {
            mockInputValidation.Setup(iv => iv.IsValidNickName(It.IsAny<string>())).Returns(true);
            mockInputValidation.Setup(iv => iv.IsValidPassword(It.IsAny<string>())).Returns(true);
            mockInputValidation.Setup(iv => iv.IsValidCity(It.IsAny<string>())).Returns(false);
            mockInputValidation.Setup(iv => iv.IsValidName(It.IsAny<string>())).Returns(true);
            mockInputValidation.Setup(iv => iv.IsValidEmail(It.IsAny<string>())).Returns(true);
            mockInputValidation.Setup(iv => iv.IsValidBirthDate(It.IsAny<DateTime>())).Returns(true);

            Assert.IsFalse(this.inputValidation.IsValidUser(It.IsAny<User>()));
        }

        [TestMethod]
        public void TestIsValidName()
        {
            Assert.IsTrue(inputValidation.IsValidName(null));
            Assert.IsTrue(inputValidation.IsValidName("Alberto"));
            Assert.IsFalse(inputValidation.IsValidName(string.Empty));
            Assert.IsFalse(inputValidation.IsValidName(" \n\t"));
        }

        [TestMethod]
        public void TestIsValidUserFailsWithIfWhenIsValidNameFails()
        {
            mockInputValidation.Setup(iv => iv.IsValidNickName(It.IsAny<string>())).Returns(true);
            mockInputValidation.Setup(iv => iv.IsValidPassword(It.IsAny<string>())).Returns(true);
            mockInputValidation.Setup(iv => iv.IsValidCity(It.IsAny<string>())).Returns(true);
            mockInputValidation.Setup(iv => iv.IsValidName(It.IsAny<string>())).Returns(false);
            mockInputValidation.Setup(iv => iv.IsValidEmail(It.IsAny<string>())).Returns(true);
            mockInputValidation.Setup(iv => iv.IsValidBirthDate(It.IsAny<DateTime>())).Returns(true);

            Assert.IsFalse(this.inputValidation.IsValidUser(It.IsAny<User>()));
        }

        [TestMethod]
        public void TestIsValidEmail()
        {
            Assert.IsTrue(inputValidation.IsValidEmail("alberto@gmail.com"));
            Assert.IsFalse(inputValidation.IsValidEmail("something else"));
        }

        [TestMethod]
        public void TestIsValidUserFailsWithIfWhenIsValidEmailFails()
        {
            mockInputValidation.Setup(iv => iv.IsValidNickName(It.IsAny<string>())).Returns(true);
            mockInputValidation.Setup(iv => iv.IsValidPassword(It.IsAny<string>())).Returns(true);
            mockInputValidation.Setup(iv => iv.IsValidCity(It.IsAny<string>())).Returns(true);
            mockInputValidation.Setup(iv => iv.IsValidName(It.IsAny<string>())).Returns(true);
            mockInputValidation.Setup(iv => iv.IsValidEmail(It.IsAny<string>())).Returns(false);
            mockInputValidation.Setup(iv => iv.IsValidBirthDate(It.IsAny<DateTime>())).Returns(true);

            Assert.IsFalse(this.inputValidation.IsValidUser(It.IsAny<User>()));
        }

        [TestMethod]
        public void TestIsValidUserSucceedsWithAnyBirthDate()
        {
            Assert.IsTrue(inputValidation.IsValidBirthDate(It.IsAny<DateTime>()));
        }

        [TestMethod]
        public void TestIsValidUserFailsWithIfWhenIsValidBirthDateFails()
        {
            mockInputValidation.Setup(iv => iv.IsValidNickName(It.IsAny<string>())).Returns(true);
            mockInputValidation.Setup(iv => iv.IsValidPassword(It.IsAny<string>())).Returns(true);
            mockInputValidation.Setup(iv => iv.IsValidCity(It.IsAny<string>())).Returns(true);
            mockInputValidation.Setup(iv => iv.IsValidName(It.IsAny<string>())).Returns(true);
            mockInputValidation.Setup(iv => iv.IsValidEmail(It.IsAny<string>())).Returns(true);
            mockInputValidation.Setup(iv => iv.IsValidBirthDate(It.IsAny<DateTime>())).Returns(false);

            Assert.IsFalse(this.inputValidation.IsValidUser(It.IsAny<User>()));
        }

        [TestMethod]
        public void TestIsValidNewsFailsWithNullNews()
        {
            Assert.IsFalse(this.inputValidation.IsValidNews(null));
        }

        [TestMethod]
        public void TestIsValidNewsFailsWithWrongTitle()
        {
            var nullTitleNews = new News { Title = null, Content = "content" };
            var emptyTitleNews = new News { Title = string.Empty, Content = "content" };
            var whiteSpaceTitleNews = new News { Title = " \n\t", Content = "content" };

            Assert.IsFalse(this.inputValidation.IsValidNews(nullTitleNews));
            Assert.IsFalse(this.inputValidation.IsValidNews(emptyTitleNews));
            Assert.IsFalse(this.inputValidation.IsValidNews(whiteSpaceTitleNews));
        }

        [TestMethod]
        public void TestIsValidNewsFailsWithWrongContent()
        {
            var nullContent = new News { Title = "title", Content = null };
            var emptyContentNews = new News { Title = "title", Content = string.Empty };
            var whiteSpaceContentNews = new News { Title = "title", Content = " \n\t" };

            Assert.IsFalse(inputValidation.IsValidNews(nullContent));
            Assert.IsFalse(inputValidation.IsValidNews(emptyContentNews));
            Assert.IsFalse(inputValidation.IsValidNews(whiteSpaceContentNews));
        }

        [TestMethod]
        public void TestIsValidNewsFailsWithWrongId()
        {
            Assert.IsTrue(this.inputValidation.IsValidNewsId(Guid.NewGuid().ToString()));
            Assert.IsFalse(this.inputValidation.IsValidNewsId("not id"));
        }

        [TestMethod]
        public void TestIsValidNewsDate()
        {
            Assert.IsTrue(this.inputValidation.IsValidNewsDate("2015-05-21"));
            Assert.IsFalse(this.inputValidation.IsValidNewsDate("2015/05/21"));
            Assert.IsFalse(this.inputValidation.IsValidNewsDate("not a date"));
            Assert.IsFalse(this.inputValidation.IsValidNewsDate(string.Empty));
        }

        [TestMethod]
        public void TestValidateDiaryEntryParametersThrowsException_IfCityIsInvalid()
        {
            this.mockInputValidation.Setup(validation => validation.IsValidCity(It.IsAny<string>())).Returns(false);
            this.mockInputValidation.Setup(validation => validation.IsValidNewsDate(It.IsAny<string>())).Returns(true);

            AssertExtensions.Throws<InputValidationException>(() => this.inputValidation.ValidateDiaryEntryParameters(It.IsAny<string>(), It.IsAny<string>()));
        }

        [TestMethod]
        public void TestValidateDiaryEntryParametersThrowsException_IfDateIsInvalid()
        {
            this.mockInputValidation.Setup(validation => validation.IsValidCity(It.IsAny<string>())).Returns(true);
            this.mockInputValidation.Setup(validation => validation.IsValidNewsDate(It.IsAny<string>())).Returns(false);

            AssertExtensions.Throws<InputValidationException>(() => this.inputValidation.ValidateDiaryEntryParameters(It.IsAny<string>(), It.IsAny<string>()));
        }

        [TestMethod]
        public void TestValidateNewsParametersThrowsException_IfDateIsInvalid()
        {
            this.mockInputValidation.Setup(validation => validation.IsValidCity(It.IsAny<string>())).Returns(true);
            this.mockInputValidation.Setup(validation => validation.IsValidNewsDate(It.IsAny<string>())).Returns(true);
            this.mockInputValidation.Setup(validation => validation.IsValidNewsId(It.IsAny<string>())).Returns(false);

            AssertExtensions.Throws<InputValidationException>(() => this.inputValidation.ValidateNewsParameters(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
        }

        [TestMethod]
        public void TestValidateImageThrowsException_IfImageHasWrongFormat()
        {
            AssertExtensions.Throws<InputValidationException>(() => this.inputValidation.ValidateImage(new byte[0], new MediaTypeHeaderValue("image/png")));
        }
    }
}
