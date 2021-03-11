using FastFood.Models;

namespace FastFood.Core.Controllers
{
    using System;
    using System.Linq;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Data;
    using Microsoft.AspNetCore.Mvc;
    using ViewModels.Items;

    public class ItemsController : Controller
    {
        private readonly FastFoodContext _context;
        private readonly IMapper _mapper;

        public ItemsController(FastFoodContext context, IMapper mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }

        public IActionResult Create()
        {
            var categories = this._context.Categories
                .ProjectTo<CreateItemViewModel>(_mapper.ConfigurationProvider)
                .ToList();

            return this.View(categories);
        }

        [HttpPost]
        public IActionResult Create(CreateItemInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Error", "Home");
            }

            var item = this._mapper.Map<Item>(model);

            this._context.Items.Add(item);

            this._context.SaveChanges();

            return RedirectToAction("All", "Items");
        }

        public IActionResult All()
        {
            var items = this._context
                .Items
                .ProjectTo<ItemsAllViewModels>(this._mapper.ConfigurationProvider)
                .ToList();

            return this.View(items);
        }
    }
}
