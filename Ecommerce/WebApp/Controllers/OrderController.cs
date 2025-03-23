using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Viewmodels;

namespace WebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OrderController : Controller
    {
        private readonly EcommerceDwaContext _context;

        public OrderController(EcommerceDwaContext context)
        {
            _context = context;
        }

        // GET: OrderController
        public ActionResult Index()
        {
            List<OrderVM> orders = new List<OrderVM>();

            _context.Orders.ToList().ForEach(x => orders.Add(new OrderVM { Id = x.IdOrder, CustomerId = x.CustomerId, PaymentMethodId = x.PaymentMethodId, CreatedAt = x.CreatedAt, Total = x.Total }));

            return View(orders);
        }

        // GET: OrderController/Details/5
        public ActionResult Details(int id)
        {
            try
            {
                var order = _context.Orders.FirstOrDefault(x => x.IdOrder == id);

                if (order == null)
                {
                    return NotFound();
                }

                var orderVM = new OrderVM
                {
                    Id = order.IdOrder,
                    CustomerId = order.CustomerId,
                    PaymentMethodId = order.PaymentMethodId,
                    CreatedAt = order.CreatedAt,
                    Total = order.Total
                };

                return View(orderVM);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        // GET: OrderController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: OrderController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(OrderVM order)
        {
            try
            {
                var newOrder = new Order
                {
                    CustomerId = order.CustomerId,
                    PaymentMethodId= order.PaymentMethodId,
                    CreatedAt = DateTime.Now,
                    Total = order.Total
                };

                _context.Orders.Add(newOrder);

                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: OrderController/Edit/5
        public ActionResult Edit(int id)
        {
            try
            {
                var order = _context.Orders.FirstOrDefault(x => x.IdOrder == id);

                if (order == null)
                {
                    return NotFound();
                }

                var orderVM = new OrderVM
                {
                    Id = order.IdOrder,
                    CustomerId = order.CustomerId,
                    PaymentMethodId = order.PaymentMethodId,
                    CreatedAt = order.CreatedAt,
                    Total = order.Total
                };

                return View(orderVM);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        // POST: OrderController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, OrderVM order)
        {
            try
            {
                var ordertoedit = _context.Orders.FirstOrDefault(x => x.IdOrder == id);

                if (order == null)
                {
                    return NotFound();
                }

                ordertoedit.CustomerId = order.CustomerId;
                ordertoedit.PaymentMethodId = order.PaymentMethodId;
                ordertoedit.CreatedAt = order.CreatedAt;
                ordertoedit.Total = order.Total;

                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: OrderController/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                var order = _context.Orders.FirstOrDefault(x => x.IdOrder == id);

                if (order == null)
                {
                    return NotFound();
                }

                var orderVM = new OrderVM
                {
                    Id = order.IdOrder,
                    CustomerId = order.CustomerId,
                    PaymentMethodId = order.PaymentMethodId,
                    CreatedAt = order.CreatedAt,
                    Total = order.Total
                };

                return View(orderVM);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        // POST: OrderController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                var ordertodelete = _context.Orders.FirstOrDefault(x => x.IdOrder == id);

                if (ordertodelete == null)
                {
                    return NotFound();
                }

                _context.Orders.Remove(ordertodelete);

                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
