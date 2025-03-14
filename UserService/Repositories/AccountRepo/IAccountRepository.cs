using UserService.Models;

namespace UserService.Repositories.AccountRepo
{
    public interface IAccountRepository
    {
        Task<IEnumerable<Account>> GetAll();
        Task<Account?> GetByUsername(string username);
        Task<Account?> GetAccountByEmail(string email);
        Task<IEnumerable<Account>> GetListAccByRoleId(int id);
        Task<Account> GetById(int? id);
        Task<Account> GetByFbId(string id);
        Task<Account> Add(Account item);
        Task Update(Account item);
        Task Delete(Account item);
        Task<int> GetTotalFarmerRepo();

        Task<int> GetTotalExpertRepo();

        Task<Account?> GetAccountByEmailForReset(string email);
        Task<string?> GetFullnameByUsername(string username);
        Task<Account?> GetAccountById(int? accountId);
        Task<List<Account>> GetAccountsByIds(List<int> ids);
    }
}
