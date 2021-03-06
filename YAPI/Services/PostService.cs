﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yapi.Contracts.V1.Requests;
using YAPI.Data;
using YAPI.Domain;

namespace YAPI.Services
{
    public class PostService : IPostService
    {
        private readonly DataContext dataContext;

        public PostService(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public async Task<bool> DeletePostAsync(Guid id)
        {
            var post = await GetPostByIdAsync(id);
            if (post == null)
                return false;

            dataContext.Posts.Remove(post);
            var isDeleted = await dataContext.SaveChangesAsync() > 0;
            return isDeleted;
        }

        public async Task<Post> GetPostByIdAsync(Guid id)
        {
            //var post = dataContext.Posts.FromSqlRaw($"SELECT *FROM Posts where Id={id}");
            return await dataContext
                .Posts
                .Include(x => x.Tags)
                .SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Post>> GetPostsAsync(GetAllPostsFilter filter, PaginationFilter paginationFilter)
        {
            //var entity = dataContext.Model.FindEntityType(typeof(Post));

            var queryable = dataContext.Posts.AsNoTracking().AsQueryable();
            if (paginationFilter != null)
            {
                queryable = AddFiltersOnQuery(filter, queryable);
                var skip = (paginationFilter.PageNumber - 1) * paginationFilter.PageSize;
                return await queryable.Include(x => x.Tags).Skip(skip).ToListAsync();

            }
            return await dataContext.Posts.Include(x=> x.AppUser).Include(x => x.Tags).ToListAsync();
        }

        private IQueryable<Post> AddFiltersOnQuery(GetAllPostsFilter filter, IQueryable<Post> queryable)
        {
            if (!string.IsNullOrEmpty(filter?.UserId))
            {
                queryable = queryable.Where(x => x.AppUserId == filter.UserId);
            }
            return queryable;
        }

        public async Task<bool> UpdatePostAsync(UpdatePostRequest postToUpdate, Guid postId)
        {
            var post = await GetPostByIdAsync(postId);
            if (post == null)
                return false;

            post.Name = postToUpdate.Name;
            //update edilmese bile context track ettiği için güncellenidr
            //dataContext.Posts.Update(post);

            //var effectedRows = dataContext.Database.ExecuteSqlRaw("UPDATE Posts " +
            //     "SET Name={0}," +
            //     "WHERE Id={1}", postToUpdate.Name, postId);
            //dataContext.Entry(post).Reload();//to complete execute sqlRaw methods process

            var isNameModified = dataContext.Entry(post).Property(nameof(post.Name)).IsModified;

            var isUpdated = await dataContext.SaveChangesAsync() > 0;
            return isUpdated;
        }

        public async Task<bool> CreatePostAsync(Post post)
        {
            post.Tags?.ForEach(x => x.TagName = x.TagName.ToLower());

            await AddNewTags(post);
            await dataContext.Posts.AddAsync(post);
            var isCraeted = await dataContext.SaveChangesAsync() > 0;
            return isCraeted;
        }

        public async Task<bool> UserOwnsPostAsync(Guid postId, string userId)
        {
            //to prevent exception beacause context will be tracking entities
            var post = await dataContext.Posts.AsNoTracking().SingleOrDefaultAsync(x => x.Id == postId);
            if (post == null || post.AppUserId != userId)
                return false;

            return true;

        }

        public async Task<List<Tag>> GetAllTagsAsync()
        {
            return await dataContext.Tags.ToListAsync();
        }

        private async Task AddNewTags(Post post)
        {
            foreach (var tag in post.Tags)
            {
                var existingTag =
                    await dataContext.Tags.SingleOrDefaultAsync(x =>
                        x.Name == tag.TagName);
                if (existingTag != null)
                    continue;

                await dataContext.Tags.AddAsync(new Tag
                { Name = tag.TagName, CreatedOn = DateTime.UtcNow, CreatorId = post.AppUserId });

            }
        }

        public async Task<bool> CreateTagAsync(Tag tag)
        {
            tag.Name = tag.Name.ToLower();
            var existingTag = await dataContext.Tags.SingleOrDefaultAsync(t => t.Name == tag.Name);
            if (existingTag != null)
                return true;

            await dataContext.Tags.AddAsync(tag);
            return await dataContext.SaveChangesAsync() > 0;
        }

        public async Task<Tag> GetTagByNameAsync(string tagName)
        {
            return await dataContext.Tags.SingleOrDefaultAsync(t => t.Name == tagName.ToLower());

        }

        public async Task<bool> DeleteTagAsync(string tagName)
        {
            var tag = await dataContext.Tags.SingleOrDefaultAsync(t => t.Name == tagName.ToLower());
            if (tag == null)
                return true;

            var postTags = await dataContext.PostTags.Where(pt => pt.TagName == tagName.ToLower()).ToListAsync();

            dataContext.RemoveRange(postTags);
            dataContext.Remove(tag);
            return await dataContext.SaveChangesAsync() > 0;

        }
    }
}
