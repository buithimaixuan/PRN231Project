using UserService.Models;

namespace UserService.Services.Interface
{
    public interface IRoleService
    {
        Task<IEnumerable<Role>> GetListAllRole();
        Task<Role> GetByIdRole(int id);
    }
}
