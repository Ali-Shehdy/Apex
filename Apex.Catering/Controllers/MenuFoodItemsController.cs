using Apex.Catering.Data;
using Apex.Catering.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apex.Catering.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuFoodItemController : ControllerBase
    {
        private readonly CateringDbContext _context;

        public MenuFoodItemController(CateringDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MenuFoodItemDto>>> GetMenuFoodItems()
        {
            return await _context.MenuFoodItems
                .Select(mf => new MenuFoodItemDto
                {
                    MenuId = mf.MenuId,
                    FoodItemId = mf.FoodItemId
                })
                .ToListAsync();
        }

        [HttpGet("{menuId}/{foodItemId}")]
        public async Task<ActionResult<MenuFoodItemDto>> GetMenuFoodItem(int menuId, int foodItemId)
        {
            var mf = await _context.MenuFoodItems.FindAsync(menuId, foodItemId);
            if (mf == null) return NotFound();

            return new MenuFoodItemDto { MenuId = mf.MenuId, FoodItemId = mf.FoodItemId };
        }

        [HttpPost]
        public async Task<ActionResult<MenuFoodItemDto>> PostMenuFoodItem(MenuFoodItemDto dto)
        {
            var mf = new MenuFoodItem { MenuId = dto.MenuId, FoodItemId = dto.FoodItemId };
            _context.MenuFoodItems.Add(mf);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMenuFoodItem), new { menuId = dto.MenuId, foodItemId = dto.FoodItemId }, dto);
        }

        [HttpDelete("{menuId}/{foodItemId}")]
        public async Task<IActionResult> DeleteMenuFoodItem(int menuId, int foodItemId)
        {
            var mf = await _context.MenuFoodItems.FindAsync(menuId, foodItemId);
            if (mf == null) return NotFound();

            _context.MenuFoodItems.Remove(mf);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
//         public async Task<IActionResult> DeleteMenu(int id)

