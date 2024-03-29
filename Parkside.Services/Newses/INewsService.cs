﻿using Parkside.Models.ViewModels;

namespace Parkside.Services.Newss
{
    public interface INewsService
    {
        Task<NewsDetailsViewModel> GetNews(int id);
        PagingViewModel<NewsViewModel> GetNewses(
            string? NameSearch, string? OrderBy, string? PublishedDate,
            bool? IsPublished, bool? IsPrimary,
            int PageNumber, int PageSize);
        IQueryable<NewsDetailsViewModel> GetHomePageNewses();
        IQueryable<NewsViewModel> GetLatestNormalNewses();
        IQueryable<NewsViewModel> GetLatestPrimaryNewses();
        Task AddNews(NewsCreateViewModel model);
        Task DeleteNews(int id);
        Task VirtualDeleteNews(int id);
        Task UpdateNews(int id, NewsUpdateViewModel model);

    }
}
