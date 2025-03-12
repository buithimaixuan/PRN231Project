using Microsoft.EntityFrameworkCore;
using UserService.Models;

namespace UserService.DAOs
{
    public class RoleDAO : SingletonBase<RoleDAO>
    {
        public async Task<IEnumerable<Role>> GetAll()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task<Role?> FindById(int id)
        {
            var item = await _context.Roles.FirstOrDefaultAsync(obj => obj.RoleId == id);
            if (item == null) return null;
            return item;
        }
        public async Task Add(Role item)
        {
            _context.Roles.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Role item)
        {
            var existingItem = await FindById(item.RoleId);
            if (existingItem == null) return;

            _context.Entry(existingItem).CurrentValues.SetValues(item);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var item = await FindById(id);
            if (item == null) return;

            _context.Roles.Remove(item);
            await _context.SaveChangesAsync();
        }

    }
}
