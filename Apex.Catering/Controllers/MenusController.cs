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
    public class MenusController : ControllerBase
    {
        private readonly CateringDbContext _context;

        public MenusController(CateringDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MenuDto>>> GetMenus()
        {
            return await _context.Menus
                .Select(m => new MenuDto
                {
                    MenuId = m.MenuId,
                    MenuName = m.MenuName ?? ""
                })
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MenuDto>> GetMenu(int id)
        {
            var m = await _context.Menus.FindAsync(id);
            if (m == null) return NotFound();
            return new MenuDto { MenuId = m.MenuId, MenuName = m.MenuName ?? "" };
        }

        [HttpPost]
        public async Task<ActionResult<MenuDto>> PostMenu(MenuDto dto)
        {
            var menu = new Menu { MenuId = 0, MenuName = dto.MenuName };
            _context.Menus.Add(menu);
            await _context.SaveChangesAsync();

            dto.MenuId = menu.MenuId;
            return CreatedAtAction(nameof(GetMenu), new { id = menu.MenuId }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutMenu(int id, MenuDto dto)
        {
            var menu = await _context.Menus.FindAsync(id);
            if (menu == null) return NotFound();

            menu.MenuName = dto.MenuName;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMenu(int id)
        {
            var menu = await _context.Menus.FindAsync(id);
            if (menu == null) return NotFound();

            _context.Menus.Remove(menu);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
        
