﻿// ****************************************************************************
// <copyright file="NewsCompleteEqualityComparer.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// Equality comparer for News. It checks title and content.
// </summary>
// ****************************************************************************

namespace GoingOn.Common.Tests
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using GoingOn.Frontend.Entities;

    [ExcludeFromCodeCoverage]
    public class NewsCompleteEqualityComparer : IEqualityComparer<News>
    {
        public bool Equals(News news1, News news2)
        {
            return
                string.Equals(news1.Title, news2.Title) &&
                string.Equals(news1.Content, news2.Content);
        }

        public int GetHashCode(News news)
        {
            return news.Title.GetHashCode() ^
                news.Content.GetHashCode();
        }
    }
}
