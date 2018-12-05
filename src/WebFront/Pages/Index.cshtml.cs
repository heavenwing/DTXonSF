using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebFront.Models;
using Contracts;

namespace WebFront.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IRemotingOrderService _remotingOrderService;

        [BindProperty]
        public OrderInput OrderInput { get; set; }

        public IndexModel(IRemotingOrderService remotingOrderService)
        {
            this._remotingOrderService = remotingOrderService;
        }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (OrderInput == null)
            {
                ModelState.AddModelError("", "you must input necessary data");
                return Page();
            }
            if (!ModelState.IsValid)
                return Page();

            var result = await this._remotingOrderService.CreateOrderAsync(new OrderDto()
            {
                ProductId = OrderInput.ProductId,
                Quantity = OrderInput.Quantity
            })
            if (!result)
            {
                ModelState.AddModelError("", "Create Order Failed");
                return Page();
            }
            return RedirectToPage("success");
        }
    }
}
