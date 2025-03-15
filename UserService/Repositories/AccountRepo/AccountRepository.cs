﻿using UserService.DAOs;
using UserService.Models;

namespace UserService.Repositories.AccountRepo
{
    public class AccountRepository : IAccountRepository
    {
        private readonly AccountDAO _accountDAO;

        public AccountRepository(AccountDAO accountDAO)
        {
            _accountDAO = accountDAO;
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
        
        public async Task<int> GetTotalExpertRepo() => await _accountDAO.GetTotalFarmerCountAsync();
        
        public async Task<Account> GetByFbId(string fbId) => await _accountDAO.GetByFbId(fbId);
        
        public async Task<Account?> GetAccountByEmailForReset(string email) => await _accountDAO.GetAccountByEmailForReset(email);

        public async Task<List<Account>> GetAccountsByIds(List<int> ids) => await _accountDAO.GetAccountsByIds(ids);
    }
}
