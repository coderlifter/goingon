﻿// ****************************************************************************
// <copyright file="NewsImageBlobRepository.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// Class to manage the repository of images
// </summary>
// ****************************************************************************

namespace GoingOn.Repository
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Threading.Tasks;
    using GoingOn.Model;
    using GoingOn.XStoreProxy.BlobStore;

    public class NewsNewsImageRepository : INewsImageRepository
    {
        private readonly IBlobStore blobStore;

        private readonly IImageManager imageManager;

        public NewsNewsImageRepository(IBlobStore blobStore, IImageManager imageManager)
        {
            this.blobStore = blobStore;
            this.imageManager = imageManager;
        }

        public async Task<Image> GetNewsImage(string city, DateTime date, Guid id)
        {
            using (var memoryStream = new MemoryStream())
            {
                await this.blobStore.GetBlob(string.Format("{0};{1};{2}", city, date.ToString("yy-MM-dd"), id), memoryStream);

                return this.imageManager.CreateFromStream(memoryStream);
            }
        }

        public async Task<Image> GetNewsThumbnailImage(string city, DateTime date, Guid id)
        {
            using (var memoryStream = new MemoryStream())
            {
                await this.blobStore.GetBlob(string.Format("thumbnail;{0};{1};{2}", city, date.ToString("yy-MM-dd"), id), memoryStream);

                return this.imageManager.CreateFromStream(memoryStream);
            }
        }

        public async Task CreateNewsImage(string city, DateTime date, Guid id, Image image)
        {
            string blobName = string.Format("{0};{1};{2}", city, date.ToString("yy-MM-dd"), id);

            using (var memoryStream = new MemoryStream())
            {
                this.imageManager.SaveToSteam(image, memoryStream);

                await this.blobStore.CreateBlob(blobName, memoryStream);
            }

            if (await this.blobStore.ContainsBlob(blobName))
            {
                using (var memoryStream = new MemoryStream())
                {
                    this.imageManager.SaveThumbnailToSteam(image, memoryStream, 40, 40);

                    await this.blobStore.CreateBlob(string.Format("thumbnail;{0};", blobName), memoryStream);
                }
            }
            else
            {
                throw new AzureRepositoryException("The image was not stored in the database.");
            }
        }

        public async Task DeleteNewsImage(string city, DateTime date, Guid id)
        {
            string blobName = string.Format("{0};{1};{2}", city, date.ToString("yy-MM-dd"), id);

            await this.blobStore.DeleteBlob(blobName);
        }

        public async Task<bool> ContainsImage(string city, DateTime date, Guid id)
        {
            string blobName = string.Format("{0};{1};{2}", city, date.ToString("yy-MM-dd"), id);

            return await this.blobStore.ContainsBlob(blobName);
        }

        public async Task<bool> ContainsImageThumbnail(string city, DateTime date, Guid id)
        {
            string blobName = string.Format("thumbnail;{0};{1};{2}", city, date.ToString("yy-MM-dd"), id);

            return await this.blobStore.ContainsBlob(blobName);
        }
    }
}
