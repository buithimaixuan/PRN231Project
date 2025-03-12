using UserService.DAOs;
using UserService.Models;

namespace UserService.Repositories.RoleRepo
{
    public class RoleRepository : IRoleRepository
    {
        private RoleDAO roleDAO;
        public RoleRepository(RoleDAO roleDAO)
        {
            this.roleDAO = roleDAO;
        }
        public async Task<IEnumerable<Role>> GetListAll()
        {
            return await roleDAO.GetAll();
        }

        public async Task<Role?> getById(int id)
        {
            return await roleDAO.FindById(id);
        }
    }
}

