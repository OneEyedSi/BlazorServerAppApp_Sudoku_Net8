﻿using Microsoft.EntityFrameworkCore;
using SudokuDataAccess;
using SudokuDataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuClassLibrary.DataServices
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IDbContextFactory<DataContext> _dbContextFactory;
        public UserProfileService(IDbContextFactory<DataContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public UserProfile? GetDefaultUserProfile()
        {
            return GetUserProfileById(UserProfile.DefaultId);
        }

        public UserProfile? GetUserProfileById(int userProfileId)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();

            if (dbContext == null)
            {
                return null;
            }

            // Don't catch any exceptions - we want them to bubble up so we can see what's going on.
            return dbContext.UserProfiles
                .Include(up => up.Icon)
                .FirstOrDefault(up => up.UserProfileId == userProfileId);
        }

        public async Task<int?> AddOrUpdateUserProfileAsync(int userProfileId, string name, int iconId)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();

            if (dbContext == null)
            {
                return null;
            }

            var userProfile = new UserProfile() { UserProfileId = userProfileId, Name = name, IconId = iconId };

            // Update will create the record if the Id field is not set (ie is 0).
            dbContext.Update(userProfile);
            await dbContext.SaveChangesAsync();
            return userProfile.UserProfileId;
        }

        public async Task<bool> RemoveUserProfileAsync(int userProfileId)
        {
            var userProfile = new UserProfile() { UserProfileId = userProfileId };
            return await RemoveUserProfileAsync(userProfile);
        }

        public async Task<bool> RemoveUserProfileAsync(UserProfile userProfile)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();

            if (dbContext == null)
            {
                return false;
            }

            dbContext.Remove(userProfile);
            await dbContext.SaveChangesAsync();
            return true;
        }
    }
}
