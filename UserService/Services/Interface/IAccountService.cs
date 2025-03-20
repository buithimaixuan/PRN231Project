using UserService.DTOs;
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
        Task<Account?> GetAccountByPhone(string phone);
        Task<Account?> GetAccountByIdentifier(string identifier); // Username, Email hoặc Phone
        Task<int> GetTotalFarmerService();
        Task<string?> GetFullNameByUsername(string username);
        Task<int> GetTotalExpertService();
        Task CreateNewFarmerAccount(string username, string password, string fullName, string email, string phone, string address, string avatar);
        Task CreateNewExpertAccount(AccountDTO account);
        Task<List<Account>> GetAccountsByRoleId(int roleId);
        Task<Account?> GetTopFarmer();
        Task<IEnumerable<FriendRequestDTO>> GetFriendRequestReceivers(int accountId);
        Task<IEnumerable<FriendRequestDTO>> GetFriendRequestSenders(int accountId);
        Task<IEnumerable<FriendRequestDTO>> GetListFriends(int accountId);
        Task<PersonalPageDTO> GetPersonalPageDTO(int accountId);
        Task<AccountPhotosDTO> GetAccountPhotos(int accountId);
        Task<FriendRequest> SendFriendRequest(int senderId, int receiverId);
        Task UpdateFriendRequestStatus(int senderId, int receiverId, string status);
        Task Unfriend(int userId1, int userId2);
    }
}
