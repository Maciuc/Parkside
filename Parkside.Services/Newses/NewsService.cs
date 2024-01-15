﻿using exp.NET6.Services.DBServices;
using Parkside.Infrastructure.Entities;
using Parkside.Infrastructure.Repositories.Newses;
using Parkside.Models.Helpers;
using Parkside.Models.ViewModels;

namespace Parkside.Services.Newss
{
    public class NewsService : INewsService
    {
        private readonly INewsRepo _newsRepo;
        private readonly IGenericService _genericService;
        public NewsService(INewsRepo newsRepo, IGenericService genericService)
        {
            _newsRepo = newsRepo;
            _genericService = genericService;
        }

        public async Task<NewsViewModel> GetNews(int id)
        {
            var news = await _newsRepo.GetAsync(id);

            if (news == null)
                throw new NotFoundException("News not found!");

            var finalNews = new NewsViewModel
            {
                Id = news.Id,
                Name = news.Name,
                Description = news.Description,
                PublishedDate = news.PublishedDate,
                IsPublished = news.IsPublished,
                ImageBase64 = _genericService.GetImgBase64(news.ImageUrl)

            };

            return finalNews;
        }

        public PagingViewModel<NewsViewModel> GetNewses(
            string? nameSearch, string? columnToSort, int pageNumber, int pageSize)
        {
            var newses = _newsRepo.GetAllQuerable();

            if (!string.IsNullOrWhiteSpace(nameSearch))
            {
                nameSearch = nameSearch.Trim();
                newses = newses.Where(c => c.Name.Contains(nameSearch));
            }

            switch (columnToSort)
            {

                case ("name"):
                    newses = newses.OrderBy(c => c.Name);
                    break;

                case ("name_desc"):
                    newses = newses.OrderByDescending(c => c.Name);
                    break;

                case ("publishedDate"):
                    newses = newses.OrderBy(c => c.PublishedDate);
                    break;

                case ("publishedDate_desc"):
                    newses = newses.OrderByDescending(c => c.PublishedDate);
                    break;

                default:
                    newses = newses.OrderByDescending(c => c.PublishedDate);
                    break;
            }

            var numberOfNewss = newses.Count();

            var newsesPerPage = newses.Skip(pageSize * (pageNumber - 1))
              .Take(pageSize).Select(news => new NewsViewModel
              {
                  Id = news.Id,
                  Name = news.Name,
                  Description = news.Description,
                  PublishedDate = news.PublishedDate,
                  IsPublished = news.IsPublished,
                  ImageBase64 = _genericService.GetImgBase64(news.ImageUrl)
              })
              .ToList();

            var paginingList = new PagingViewModel<NewsViewModel>
            {
                Count = numberOfNewss,
                NumberOfPages = (int)Math.Ceiling(numberOfNewss / (double)pageSize),
                Items = newsesPerPage
            };

            return paginingList;
        }

        public async Task AddNews(NewsCreateViewModel model)
        {

            var finalNews = new News()
            {
                Name = model.Name,
                Description = model.Description,
                PublishedDate = model.PublishedDate,
                IsPublished = model.IsPublished,
                ImageUrl = _genericService.GetImagePath(model.ImageBase64, null, "Newses")
            };

            await _newsRepo.Add(finalNews);
        }
        public async Task DeleteNews(int id)
        {
            var news = await _newsRepo.GetAsync(id);
            if (news == null)
                throw new NotFoundException("News not found!");

            await _newsRepo.Delete(id);
        }

        public async Task VirtualDeleteNews(int id)
        {
            var news = await _newsRepo.GetAsync(id);
            if (news == null)
                throw new NotFoundException("News not found!");

            news.IsDeleted = true;

            await _newsRepo.Update(news);
        }

        public async Task UpdateNews(int id, NewsUpdateViewModel model)
        {
            var news = await _newsRepo.GetAsync(id);
            if (news == null)
                throw new NotFoundException("News not found!");

            news.Name = model.Name;
            news.Description = model.Description;
            news.PublishedDate = model.PublishedDate;
            news.IsPublished = model.IsPublished;
            news.ImageUrl = _genericService.GetImagePath(model.ImageBase64, null, "Newses");

            await _newsRepo.Update(news);
        }

        public IQueryable<NewsViewModel> GetHomePageNewses()
        {
            var newses = _newsRepo.GetAllQuerable().Where(x => x.IsPublished == true); ;

            var finalNewses = newses.Select(news => new NewsViewModel
            {
                Id = news.Id,
                Name = news.Name,
                Description = news.Description,
                PublishedDate = news.PublishedDate,
                IsPublished = news.IsPublished,
                ImageBase64 = _genericService.GetImgBase64(news.ImageUrl)
            });


            return finalNewses;
        }

        public IQueryable<NewsViewModel> GetLatestNewses()
        {
            var newses = _newsRepo.GetAllQuerable().Where(x => x.IsPublished == true); ;

            var finalNewses = newses.OrderByDescending(c => c.PublishedDate).Take(6)
                .Select(news => new NewsViewModel
                {
                    Id = news.Id,
                    Name = news.Name,
                    Description = news.Description,
                    PublishedDate = news.PublishedDate,
                    IsPublished = news.IsPublished,
                    ImageBase64 = _genericService.GetImgBase64(news.ImageUrl)
                });

            return finalNewses;
        }
    }
}
