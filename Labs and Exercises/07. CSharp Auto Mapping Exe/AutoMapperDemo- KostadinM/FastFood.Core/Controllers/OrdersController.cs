using AutoMapper.QueryableExtensions;
using FastFood.Models;

namespace FastFood.Core.Controllers
{
    using System;
    using System.Linq;
    using AutoMapper;
    using Data;
    using Microsoft.AspNetCore.Mvc;
    using ViewModels.Orders;

    public class OrdersController : Controller
    {
        private readonly FastFoodContext _context;
        private readonly IMapper _mapper;

        public OrdersController(FastFoodContext context, IMapper mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }

        public IActionResult Create()
        {
            var viewOrder = new CreateOrderViewModel
            {
                Items = this._context.Items.Select(x => x.Id).ToList(),
                Employees = this._context.Employees.Select(x => x.Id).ToList(),
            };

            return this.View(viewOrder);
        }

        [HttpPost]
        public IActionResult Create(CreateOrderInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return this.RedirectToAction("Error", "Home");
            }

            var order = this._mapper.Map<Order>(model);
            var orderItem = this._mapper.Map<OrderItem>(model);
            orderItem.Order = order;

            this._context.Orders.Add(order);
            this._context.OrderItems.Add(orderItem);

            this._context.SaveChanges();


            return this.RedirectToAction("All", "Orders");
        }

        public IActionResult All()
        {
            var orders = this._context
                .Orders
                .ProjectTo<OrderAllViewModel>(this._mapper.ConfigurationProvider)
                .ToList();

            return this.View(orders);
        }
    }
}
