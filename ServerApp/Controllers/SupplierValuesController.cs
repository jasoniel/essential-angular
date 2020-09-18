using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServerApp.Models;
using ServerApp.Models.BindingTargets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerApp.Controllers
{
    [Route("api/suppliers")]
    [Authorize(Roles = "Administrator")]
    [ApiController]
    public class SupplierValuesController : Controller
    {
        private DataContext context;

        public SupplierValuesController(DataContext ctx)
        {
            context = ctx;
        }

        [HttpGet]
        public IEnumerable<Supplier> GetSuppliers()
        {
            return context.Supplier;
        }

        [HttpPost]
        public IActionResult CreateSupplier([FromBody] SupplierData sdata)
        {

            if (ModelState.IsValid)
            {
                Supplier s = sdata.Supplier;
                context.Add(s);
                context.SaveChanges();
                return Ok(s.SupplierId);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpPut("{id}")]
        public IActionResult ReplaceSupplier(long id, [FromBody] SupplierData sdata)
        {
            if (ModelState.IsValid)
            {
                Supplier s = sdata.Supplier;
                s.SupplierId = id;
                context.Update(s);
                context.SaveChanges();
                return Ok();
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteSupplier(long id)
        {
            var supplier = context.Supplier.Find(id);

            if (supplier != null)
            {
                context.Supplier.Remove(new Supplier { SupplierId = id });
                context.SaveChanges();
                return Ok();
            }

            return NotFound();

        }

    }
}
