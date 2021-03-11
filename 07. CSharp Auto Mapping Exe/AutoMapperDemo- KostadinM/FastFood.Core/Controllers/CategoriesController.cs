using System.Linq;
using AutoMapper.QueryableExtensions;
using FastFood.Models;

namespace FastFood.Core.Controllers
{
    using System;
    using AutoMapper;
    using Data;
    using Microsoft.AspNetCore.Mvc;
    using ViewModels.Categories;

    public class CategoriesController : Controller
    {
        private readonly FastFoodContext _context;
        private readonly IMapper _mapper;

        public CategoriesController(FastFoodContext context, IMapper mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }

        public IActionResult Create()
        {
            return this.View();
        }

        [HttpPost]
        public IActionResult Create(CreateCategoryInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Error", "Home");
            }
            var category = this._mapper.Map<Category>(model);

            this._context.Categories.Add(category);

            this._context.SaveChanges();

            return this.RedirectToAction("All", "Categories");
        }

        public IActionResult All()
        {
            var categories = this._context
                .Categories
                .ProjectTo<CategoryAllViewModel>(_mapper.ConfigurationProvider)
                .ToList();

            return this.View(categories);
        }
    }
}
