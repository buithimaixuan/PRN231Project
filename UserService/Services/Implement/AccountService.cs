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

        public async Task<IEnumerable<Account>> GetListAccountByRoleId(int id)
        {
            return await _accountRepo.GetListAccByRoleId(id);
        }
        public async Task<Account> GetByIdAccount(int id)
        {
            var getAcc = await _accountRepo.GetById(id);
            return getAcc;
        }

        public async Task<Account> GetByUsername(string username)
        {
            return await _accountRepo.GetByUsername(username);
        }

        public async Task AddAccount(Account item)
        {
            await _accountRepo.Add(item);
        }
        public async Task UpdateAccount(Account item)
        {
            await _accountRepo.Update(item);
        }
        public async Task DeleteAccount(Account item)
        {
            await _accountRepo.Delete(item);
        }

        public async Task<Account?> GetAccountByEmail(string email)
        {
            return await _accountRepo.GetAccountByEmail(email);
        }


        public async Task<int> GetTotalFarmerService()
        {
            return await _accountRepo.GetTotalFarmerRepo();
        }
        public async Task<int> GetTotalExpertService()
        {
            return await _accountRepo.GetTotalExpertRepo();
        }
        public async Task<Account> GetByIdFacebook(string fbId)
        {
            return await _accountRepo.GetByFbId(fbId);
        }
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

        public async Task<string?> GetFullNameByUsername(string username)
        {
            return await _accountRepo.GetFullnameByUsername(username);
        }
    }
}