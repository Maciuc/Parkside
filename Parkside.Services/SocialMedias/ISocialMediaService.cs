﻿using Parkside.Models.ViewModels;

namespace Parkside.Services.SocialMedias
{
    public interface ISocialMediaService
    {
        Task<SocialMediaViewModel> GetSocialMedia(int id);
        PagingViewModel<SocialMediaViewModel> GetSocialMedias(
            string? NameSearch, string? OrderBy, string? Platform, int PageNumber, int PageSize);
        IQueryable<string?> GetPlatformsDropDown();
        IQueryable<SocialMediaViewModel> GetHomePageSocialMedia();
        Task AddSocialMedia(SocialMediaCreateViewModel model);
        Task DeleteSocialMedia(int id);
        Task VirtualDeleteSocialMedia(int id);
        Task UpdateSocialMedia(int id, SocialMediaUpdateViewModel model);

    }
}
