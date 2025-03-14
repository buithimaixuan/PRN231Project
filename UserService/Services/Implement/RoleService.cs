using UserService.Models;
using UserService.Repositories;
using UserService.Services.Interface;

namespace UserService.Services.Implement
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepo;
        public RoleService(IRoleRepository roleRepo)
        {
            _roleRepo = roleRepo;
        }
        public async Task<IEnumerable<Role>> GetListAllRole()
        {
            return await _roleRepo.GetListAll();
        }
        public async Task<Role?> GetByIdRole(int id)
        {
            return await _roleRepo.getById(id);
        }
      
    }

}
