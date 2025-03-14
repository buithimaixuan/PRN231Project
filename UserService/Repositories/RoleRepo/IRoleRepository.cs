using UserService.Models;

namespace UserService.Repositories.RoleRepo
{
    public interface IRoleRepository
    {
        Task<IEnumerable<Role>> GetListAll();
        Task<Role?> getById(int id);
    }
}
