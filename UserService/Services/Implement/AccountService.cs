using UserService.Models;
using UserService.Repositories;
using UserService.Services.Interface;

namespace UserService.Services.Implement
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepo;
        public AccountService(IAccountRepository accountRepo)
        {
            _accountRepo = accountRepo;
        }
        public async Task<IEnumerable<Account>> GetListAllAccount()
        {
            return await _accountRepo.GetAll();
        }

        public async Task<IEnumerable<Account>> GetAllAccountAvailable() => await _accountRepo.GetAllAccountAvailable();

        public async Task<IEnumerable<Account>> GetListAccountByRoleId(int id) => await _accountRepo.GetListAccByRoleId(id);

        public async Task<Account> GetByIdAccount(int id) => await _accountRepo.GetById(id);

        public async Task<Account> GetByUsername(string username) => await _accountRepo.GetByUsername(username);

        public async Task AddAccount(Account item) => await _accountRepo.Add(item);
        public async Task UpdateAccount(Account item) => await _accountRepo.Update(item);
        public async Task DeleteAccount(Account item) => await _accountRepo.Delete(item);

        public async Task<Account?> GetAccountByEmail(string email) => await _accountRepo.GetAccountByEmail(email);

        public async Task<Account?> GetAccountByPhone(string phone) => await _accountRepo.GetAccountByPhone(phone);

        public async Task<int> GetTotalFarmerService() => await _accountRepo.GetTotalFarmerRepo();
        public async Task<int> GetTotalExpertService() => await _accountRepo.GetTotalExpertRepo();
        public async Task<Account> GetByIdFacebook(string fbId) => await _accountRepo.GetByFbId(fbId);
        public async Task CreateNewFacebookAccount(string fbId, string name, string email, string avatar)
        {
            Account newAcc = new Account
            {
                AccountId = 0,
                RoleId = 1,
                FacebookId = fbId,
                Username = email,
                Password = "1",
                FullName = name,
                Email = email,
                EmailConfirmed = 1,
                Phone = null,
                PhoneConfirmed = 0,
                Gender = null,
                DegreeUrl = null,
                Avatar = avatar,
                Major = null,
                Address = null,
                IsDeleted = false,
                Otp = null
            };
            await _accountRepo.Add(newAcc);
        }

        public async Task<string?> GetFullNameByUsername(string username) => await _accountRepo.GetFullnameByUsername(username);

        public async Task<Account?> GetAccountByIdentifier(string identifier)
        {
            if (string.IsNullOrEmpty(identifier))
                return null;

            // Kiểm tra theo Email
            var account = await _accountRepo.GetAccountByEmail(identifier);
            if (account != null)
                return account;

            // Kiểm tra theo Username
            account = await _accountRepo.GetByUsername(identifier);
            if (account != null)
                return account;

            // Kiểm tra theo PhoneNumber
            account = await _accountRepo.GetAccountByPhone(identifier);
            if (account != null)
                return account;

            return null;
        }
    }
}