﻿// ****************************************************************************
// <copyright file="NewsMemoryTests.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace Storage.Tests
{
    using System;
    
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Common.Tests;
    using MemoryStorage;
    using Model.EntitiesBll;

    [TestClass]
    public class NewsMemoryStorageTests
    {
        private static readonly Guid newsGuid = Guid.NewGuid();
        private static readonly NewsBll News = new NewsBll
        {
            Id = newsGuid,
            Title = "title",
            Content = "content",
            Author = "author",
            Date = new DateTime(2014, 12, 24, 13, 0, 0),
            Rating = 1
        };

        private INewsStorage storage;

        [TestInitialize]
        public void Initialize()
        {
            storage = NewsMemoryStorage.GetInstance();
        }

        [TestCleanup]
        public void Cleanup()
        {
            storage.DeleteAllNews();
        }

        [TestMethod]
        public void TestAddNews()
        {
            storage.AddNews(News);

            Assert.IsTrue(storage.ContainsNews(News).Result);
        }

        [TestMethod]
        public void TestGetNews()
        {
            storage.AddNews(News);

            NewsBll actualNews = storage.GetNews(newsGuid).Result;

            Assert.IsTrue(new NewsBllEqualityComparer().Equals(News, actualNews));
        }

        [TestMethod]
        public void TestGetNewsEmptyStorage()
        {
            Assert.IsNull(storage.GetNews(newsGuid).Result);
        }

        [TestMethod]
        public void TestContainsNews()
        {
            storage.AddNews(News);

            Assert.IsTrue(storage.ContainsNews(News).Result);
        }

        [TestMethod]
        public void TestUpdateNews()
        {
            Guid guid1 = Guid.NewGuid();
            Guid guid2 = Guid.NewGuid();
            Guid guid3 = Guid.NewGuid();

            NewsBll oldNews1 = new NewsBll
            {
                Id = guid1,
                Title = "title 1",
                Content = "content",
                Author = "author",
                Date = new DateTime(2014, 12, 24, 13, 0, 0),
                Rating = 1
            };
            NewsBll oldNews2 = new NewsBll
            {
                Id = guid2,
                Title = "title 2",
                Content = "content",
                Author = "author",
                Date = new DateTime(2014, 12, 24, 13, 0, 0),
                Rating = 1
            };
            NewsBll oldNews3 = new NewsBll
            {
                Id = guid3,
                Title = "title 3",
                Content = "content",
                Author = "author",
                Date = new DateTime(2014, 12, 24, 13, 0, 0),
                Rating = 1
            };

            storage.AddNews(oldNews1);
            storage.AddNews(oldNews2);
            storage.AddNews(oldNews3);

            NewsBll updatedTitleNews = new NewsBll
            {
                Id = guid1,
                Title = "title 1",
                Content = "content",
                Author = "author",
                Date = new DateTime(2014, 12, 24, 13, 0, 0),
                Rating = 1
            }; 
            NewsBll updatedContentNews = new NewsBll
            {
                Id = guid2,
                Title = "title 2",
                Content = "updated content",
                Author = "author",
                Date = new DateTime(2014, 12, 24, 13, 0, 0),
                Rating = 1
            };
            NewsBll updatedDateNews = new NewsBll
            {
                Id = guid3,
                Title = "title 3",
                Content = "content",
                Author = "author",
                Date = new DateTime(2014, 12, 25, 13, 0, 0),
                Rating = 1
            }; ;

            storage.UpdateNews(updatedTitleNews);
            storage.UpdateNews(updatedContentNews);
            storage.UpdateNews(updatedDateNews);

            Assert.IsTrue(storage.ContainsNews(updatedTitleNews).Result);
            Assert.IsTrue(storage.ContainsNews(updatedContentNews).Result);
            Assert.IsTrue(storage.ContainsNews(updatedDateNews).Result);
        }

        [TestMethod]
        public void TestContainsNewsReturnsFalse()
        {
            NewsBll newsDifferentTitle = new NewsBll
            {
                Id = newsGuid,
                Title = "different title",
                Content = "content",
                Author = "author",
                Date = new DateTime(2014, 12, 24, 13, 0, 0),
                Rating = 1
            }; 
            NewsBll newsDifferentYear = new NewsBll
            {
                Id = newsGuid,
                Title = "title",
                Content = "content",
                Author = "author",
                Date = new DateTime(2015, 12, 24, 13, 0, 0),
                Rating = 1
            };
            NewsBll newsDifferentMonth = new NewsBll
            {
                Id = newsGuid,
                Title = "title",
                Content = "content",
                Author = "author",
                Date = new DateTime(2014, 11, 24, 13, 0, 0),
                Rating = 1
            };
            NewsBll newsDifferentHour = new NewsBll
            {
                Id = newsGuid,
                Title = "title",
                Content = "content",
                Author = "author",
                Date = new DateTime(2014, 12, 24, 14, 0, 0),
                Rating = 1
            };
            
            storage.AddNews(News);

            Assert.IsFalse(storage.ContainsNews(newsDifferentTitle).Result);
            Assert.IsFalse(storage.ContainsNews(newsDifferentYear).Result);
            Assert.IsFalse(storage.ContainsNews(newsDifferentMonth).Result);
            Assert.IsFalse(storage.ContainsNews(newsDifferentHour).Result);
        }

        [TestMethod]
        public void TestDeleteNews()
        {
            storage.AddNews(News);

            storage.DeleteNews(newsGuid);

            Assert.IsFalse(storage.ContainsNews(News).Result);
        }

        [TestMethod]
        public void TestDeleteNewsDoesNotAffectOtherNews()
        {
            Guid guid1 = Guid.NewGuid();
            Guid guid2 = Guid.NewGuid();
            NewsBll news1 = new NewsBll
            {
                Id = guid1,
                Title = "title",
                Content = "content",
                Author = "author",
                Date = new DateTime(2014, 12, 24, 13, 0, 0),
                Rating = 1
            };
            NewsBll news2 = new NewsBll
            {
                Id = guid2,
                Title = "other title",
                Content = "content",
                Author = "author",
                Date = new DateTime(2014, 12, 24, 13, 0, 0),
                Rating = 1
            };

            storage.AddNews(news1);
            storage.AddNews(news2);

            storage.DeleteNews(guid1);

            Assert.IsFalse(storage.ContainsNews(news1).Result);
            Assert.IsTrue(storage.ContainsNews(news2).Result);
        }

        [TestMethod]
        public void TestDeleteAll()
        {
            storage.AddNews(News);

            storage.DeleteAllNews();

            Assert.IsFalse(storage.ContainsNews(News).Result);
        }

        #region Helper methods

        #endregion
    }
}