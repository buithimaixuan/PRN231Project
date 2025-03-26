using Microsoft.EntityFrameworkCore;
using UserService.Models;

namespace UserService.DAOs
{
    public class AccountDAO : SingletonBase<AccountDAO>
    {
        public async Task<IEnumerable<Account>> getAll()
        {
            return await _context.Accounts.ToListAsync();
        }

        public async Task<IEnumerable<Account>> getAllAccountAvailable()
        {
            return await _context.Accounts.Where(a => a.IsDeleted == false).ToListAsync();
        }

        public async Task<Account?> getByUsername(string username)
        {
            var account = await _context.Accounts.SingleOrDefaultAsync(acc => acc.Username.Equals(username));
            if (account == null) return null;
            return account;
        }
        public async Task<IEnumerable<Account>> GetListAccountByRoleId(int role_id)
        {
            var item = await _context.Accounts.Where(c => c.RoleId == role_id && c.IsDeleted == false).ToListAsync();
            if (item == null) return null;
            return item;
        }
        public async Task<Account?> GetAccountByEmail(string email)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(acc => acc.Email.Equals(email));
            if (account == null) return null;
            return account;
        }

        public async Task<Account?> GetAccountByPhone(string phone)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(acc => acc.Phone.Equals(phone));
            if (account == null) return null;
            return account;
        }

        public async Task<Account> GetById(int? id)
        {
            var item = await _context.Accounts.FirstOrDefaultAsync(c => c.AccountId == id && c.IsDeleted == false);
            if (item == null) return null;
            return item;
        }

        public async Task<Account> Add(Account item)
        {
            _context.Accounts.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }


        public async Task Update(Account item)
        {
            var existingItem = await GetById(item.AccountId);
            if (existingItem == null) return;

            _context.Entry(existingItem).CurrentValues.SetValues(item);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Account item)
        {
            var existingItem = await GetById(item.AccountId);
            if (existingItem == null) return;

            existingItem.IsDeleted = true;

            _context.Entry(existingItem).CurrentValues.SetValues(item);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetTotalExpertCountAsync()
        {
            return await _context.Accounts.CountAsync(n => n.IsDeleted != true && n.RoleId == 3);
        }

        public async Task<int> GetTotalFarmerCountAsync()
        {
            return await _context.Accounts.CountAsync(n => n.IsDeleted != true && n.RoleId == 2);
        }



        public async Task<Account> GetByFbId(string fbId)
        {
            var item = await _context.Accounts.FirstOrDefaultAsync(acc => acc.FacebookId.Equals(fbId));
            if (item == null) return null;
            return item;
        }


        // Lấy account bằng gmail để reset password
        public async Task<Account?> GetAccountByEmailForReset(string email)
        {
            var account = await _context.Accounts.SingleOrDefaultAsync(acc => acc.Email.Equals(email) && acc.IsDeleted == false && acc.FacebookId == null);
            if (account == null) return null;
            return account;
        }

        public async Task<string?> GetFullNameByUsername(string username)
        {
            return await _context.Accounts.Where(a => a.Username.Equals(username)).Select(f => f.FullName).FirstOrDefaultAsync();
        }

        public async Task<Account?> GetAccountById(int? accountId)
        {
            return await _context.Accounts.FirstOrDefaultAsync(a => a.AccountId == accountId);
        }


        public async Task<List<Account>> GetAccountsByIds(List<int> ids)
        {
            return await _context.Accounts
                .Where(account => ids.Contains(account.AccountId))
                .ToListAsync();
        }

        public async Task<List<Account>> GetAccountsByRoleId(int roleId)
        {
            return await _context.Accounts
                .Where(a => a.RoleId == roleId)
                .ToListAsync();
        }




    }
}
