using UserService.Models;

namespace UserService.Services.Interface
{
    public interface IAccountService
    {
        Task<IEnumerable<Account>> GetListAllAccount();
        Task<IEnumerable<Account>> GetAllAccountAvailable();
        Task<IEnumerable<Account>> GetListAccountByRoleId(int role_id);
        Task<Account> GetByIdAccount(int id);
        Task<Account> GetByUsername(string username);
        Task<Account> GetByIdFacebook(string fbId);
        Task AddAccount(Account item);
        Task CreateNewFacebookAccount(string fbId, string name, string email, string avatar);
        Task UpdateAccount(Account item);
        Task DeleteAccount(Account item);
        Task<Account?> GetAccountByEmail(string email);
        Task<int> GetTotalFarmerService();
        Task<string?> GetFullNameByUsername(string username);
        Task<int> GetTotalExpertService();
    }
}
