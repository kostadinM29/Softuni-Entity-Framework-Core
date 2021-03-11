namespace FastFood.Core.Controllers
{
    using System.Linq;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Data;
    using FastFood.Models;
    using Microsoft.AspNetCore.Mvc;
    using ViewModels.Positions;

    public class PositionsController : Controller
    {
        private readonly FastFoodContext _context;
        private readonly IMapper _mapper;

        public PositionsController(FastFoodContext context, IMapper mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }

        public IActionResult Create()
        {
            return this.View();
        }

        [HttpPost]
        public IActionResult Create(CreatePositionInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Error", "Home");
            }

            var position = this._mapper.Map<Position>(model);

            this._context.Positions.Add(position);

            this._context.SaveChanges();

            return this.RedirectToAction("All", "Positions");
        }

        public IActionResult All()
        {
            var categories = this._context.Positions
                .ProjectTo<PositionsAllViewModel>(_mapper.ConfigurationProvider)
                .ToList();

            return this.View(categories);
        }
    }
}
