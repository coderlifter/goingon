﻿// ****************************************************************************
// <copyright file="PersistenceException.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

using System;

namespace Storage
{
    [Serializable]
    public class StorageException : Exception
    {
        public StorageException(string message)
            : base(message)
        {
        }
    }
}