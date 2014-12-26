﻿// ****************************************************************************
// <copyright file="INewsStorage.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

using System;
using System.Threading.Tasks;

using Model.EntitiesBll;

public interface INewsStorage
{
    Task AddNews(NewsBll newsBll);
    Task<NewsBll> GetNews(Guid id);
    Task<bool> ContainsNews(Guid id);
    Task<bool> ContainsNews(NewsBll newsBll);
    Task UpdateNews(Guid id, NewsBll newsBll);
    Task DeleteNews(Guid id);
    Task DeleteAllNews();
}