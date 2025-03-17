using System.Net.Http;
using System.Text.Json;
using UserService.DAOs;
using UserService.Models;

namespace UserService.Repositories.AccountRepo
{
    public class AccountRepository : IAccountRepository
    {
        private readonly AccountDAO _accountDAO;
        private readonly HttpClient _httpClient;
        private readonly FriendRequestDAO _friendRequestDAO;

        public AccountRepository(AccountDAO accountDAO, HttpClient httpClient, FriendRequestDAO friendRequestDAO)
        {
            _accountDAO = accountDAO;
            _httpClient = httpClient;
            _friendRequestDAO = friendRequestDAO;
        }

        public async Task<IEnumerable<Account>> GetAll()
        {
            return await _accountDAO.getAll();
        }

        public async Task<IEnumerable<Account>> GetAllAccountAvailable()
        {
            return await _accountDAO.getAllAccountAvailable();
        }

        public async Task<Account?> GetByUsername(string username) => await _accountDAO.getByUsername(username);

        public async Task<IEnumerable<Account>> GetListAccByRoleId(int role_id) => await _accountDAO.GetListAccountByRoleId(role_id);
        public async Task<Account> GetById(int? id) => await _accountDAO.GetById(id);
        public async Task<Account?> GetAccountById(int? accountId) => await _accountDAO.GetAccountById(accountId);
        public async Task<Account> Add(Account account) => await _accountDAO.Add(account);

        public async Task Update(Account account) => await _accountDAO.Update(account);
        public async Task Delete(Account account) => await _accountDAO.Delete(account);
        public async Task<string?> GetFullnameByUsername(string username) => await _accountDAO.GetFullNameByUsername(username);
        public async Task<Account?> GetAccountByEmail(string email) => await _accountDAO.GetAccountByEmail(email);
        public async Task<Account?> GetAccountByPhone(string phone) => await _accountDAO.GetAccountByPhone(phone);
        public async Task<int> GetTotalFarmerRepo() => await _accountDAO.GetTotalFarmerCountAsync();
        
        public async Task<int> GetTotalExpertRepo() => await _accountDAO.GetTotalExpertCountAsync();
        
        public async Task<Account> GetByFbId(string fbId) => await _accountDAO.GetByFbId(fbId);
        
        public async Task<Account?> GetAccountByEmailForReset(string email) => await _accountDAO.GetAccountByEmailForReset(email);

        public async Task<List<Account>> GetAccountsByIds(List<int> ids) => await _accountDAO.GetAccountsByIds(ids);
        public async Task<List<Account>> GetAccountsByRoleId(int roleId)
        {
            return await _accountDAO.GetAccountsByRoleId(roleId);
        }

        public async Task<Dictionary<int, int>> GetPostCounts()
        {
            var response = await _httpClient.GetAsync("http://postservice/api/post/post-count-by-account");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Dictionary<int, int>>(json);
        }

        public async Task<IEnumerable<FriendRequest>> GetFriendRequestReceivers(int accountId) => await _friendRequestDAO.GetFriendRequestReceivers(accountId);
        public async Task<IEnumerable<FriendRequest>> GetFriendRequestSenders(int accountId) => await _friendRequestDAO.GetFriendRequestSenders(accountId);
        public async Task<IEnumerable<FriendRequest>> GetListFriends(int accountId) => await _friendRequestDAO.GetListFriends(accountId);
    }
}
