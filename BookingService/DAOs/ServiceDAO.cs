using BookingService.Models;
using Microsoft.EntityFrameworkCore;

namespace RequestService.DAOs
{
    public class ServiceDAO : SingletonBase<ServiceDAO>
    {
        // Lấy danh sách tất cả dịch vụ
        public async Task<IEnumerable<Service>> GetAll()
        {
            return await _context.Services.ToListAsync();
        }

        // Lấy danh sách dịch vụ chưa xóa
        public async Task<IEnumerable<Service>> GetAllServiceAvailable()
        {
            return await _context.Services.Where(c => c.IsDeleted == false && c.IsEnable == true).ToListAsync();
        }

        // Lấy dịch vụ theo Id
        public async Task<Service> GetById(int id)
        {
            var item = await _context.Services.FirstOrDefaultAsync(c => c.ServiceId == id);
            if (item == null) return null;
            return item;
        }

        // Lấy dịch vụ theo phân trang
        public async Task<IEnumerable<Service>> GetServicesPaged(int pageNumber, int pageSize)
        {
            return await _context.Services
                .Where(s => s.IsDeleted == false && s.IsEnable == true) // Adjust based on your business logic
                .OrderBy(s => s.ServiceId) // Ensure consistent ordering
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        // Đếm tổng số dịch vụ khả dụng
        public async Task<int> GetTotalServicesCount()
        {
            return await _context.Services.CountAsync(s => s.IsDeleted == false && s.IsEnable == true);
        }

        // Lấy danh sách dịch vụ theo accountId
        public async Task<IEnumerable<Service>> GetAllServiceByAccId(int id)
        {
            return await _context.Services.Where(c => c.CreatorId == id && c.IsDeleted == false).ToListAsync();
        }

        // Tạo dịch vụ
        public async Task<Service> AddService(Service item)
        {
            _context.Services.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        // Chỉnh sửa dịch vụ
        public async Task<Service> UpdateService(Service item)
        {
            var existingItem = await GetById(item.ServiceId);
            if (existingItem == null) return null;

            _context.Entry(existingItem).CurrentValues.SetValues(item);
            await _context.SaveChangesAsync();
            return item;

        }

        // Xóa dịch vụ
        public async Task DeleteService(int id)
        {
            var item = await GetById(id);
            if (item == null) return;

            item.DeletedAt = DateTime.Now;
            item.IsDeleted = true;

            _context.Entry(item).CurrentValues.SetValues(item);
            //_context.Services.Remove(item);
            await _context.SaveChangesAsync();
        }
    }
}
