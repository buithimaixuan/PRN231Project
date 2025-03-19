using System.Net.Http;
using System.Text.Json;
using UserService.DTOs;
using UserService.Models;
using UserService.Repositories;
using UserService.Repositories.AccountRepo;
using UserService.Services.Interface;

namespace UserService.Services.Implement
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepo;
        private readonly IFriendRequestRepository _friendRequestRepository;
        private readonly HttpClient _postServiceClient;
        public AccountService(IAccountRepository accountRepo, IHttpClientFactory httpClientFactory, IFriendRequestRepository friendRequestRepository)
        {
            _accountRepo = accountRepo;
            _postServiceClient = httpClientFactory.CreateClient("PostService"); ;
            _friendRequestRepository = friendRequestRepository;
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
        public async Task CreateNewFarmerAccount(string username, string password, string fullName, string email, string phone, string address, string avatar)
        {
            Account newFarmer = new Account
            {
                AccountId = 0,
                RoleId = 2,  // 2 là Farmer
                Username = username,
                Password = password,
                FullName = fullName,
                Email = email,
                EmailConfirmed = 0,
                Phone = phone,
                PhoneConfirmed = 0,
                Gender = null,
                DateOfBirth = null,
                ShortBio = null,
                EducationUrl = null,
                YearOfExperience = 0,
                DegreeUrl = null,
                Avatar = avatar,
                Major = null,
                Address = address,
                IsDeleted = false,
                Otp = null,
                FacebookId = null,
                Role = null  // Gán null để tránh lỗi validation
            };

            await _accountRepo.Add(newFarmer);
        }

        // mai xuan create expert
        public async Task CreateNewExpertAccount(AccountDTO account)
        {

            Account newFarmer = new Account
            {
                AccountId = 0,
                RoleId = 3,  // expert
                Username = account.Username,
                Password = account.Password,
                FullName = account.FullName,
                Email = account.Email,
                EmailConfirmed = 0,
                Phone = account.Phone,
                PhoneConfirmed = 0,
                Gender = account.Gender,
                DateOfBirth = account.DateOfBirth,
                ShortBio = account.ShortBio,
                EducationUrl = account.EducationUrl,
                YearOfExperience = account.YearOfExperience,
                DegreeUrl = account.DegreeUrl,
                Avatar = account.Avatar,
                Major = account.Major,
                Address = account.Address,
                IsDeleted = false,
                Otp = null,
                FacebookId = null,
                Role = null  // Gán null để tránh lỗi validation
            };

            await _accountRepo.Add(newFarmer);
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

        public Task<List<Account>> GetAccountsByRoleId(int roleId)
        {
            return _accountRepo.GetAccountsByRoleId(roleId);
        }

        public async Task<Account?> GetTopFarmer()
        {
            var allFarmers = await _accountRepo.GetAccountsByRoleId(1);
            var postCounts = await _accountRepo.GetPostCounts();

            return allFarmers
                .Where(f => postCounts.ContainsKey(f.AccountId))
                .OrderByDescending(f => postCounts[f.AccountId])
                .FirstOrDefault();
        }
        public async Task<IEnumerable<FriendRequestDTO>> GetFriendRequestReceivers(int accountId)
        {
            return await _friendRequestRepository.GetFriendRequestReceivers(accountId);
        }

        public async Task<IEnumerable<FriendRequestDTO>> GetFriendRequestSenders(int accountId)
        {
            return await _friendRequestRepository.GetFriendRequestSenders(accountId);
        }

        public async Task<IEnumerable<FriendRequestDTO>> GetListFriends(int accountId)
        {
            return await _friendRequestRepository.GetListFriends(accountId);
        }
        public async Task<PersonalPageDTO> GetPersonalPageDTO(int accountId)
        {
            // Lấy thông tin tài khoản
            var account = await _accountRepo.GetById(accountId);
            if (account == null)
                throw new Exception("Account not found or deleted.");

            var accountDTO = new AccountDTO
            {
                RoleId = account.RoleId,
                Username = account.Username,
                Password = "", // Lưu ý: Không nên trả password thực tế
                Email = account.Email,
                Phone = account.Phone,
                Gender = account.Gender,
                FullName = account.FullName,
                DateOfBirth = account.DateOfBirth,
                ShortBio = account.ShortBio,
                EducationUrl = account.EducationUrl,
                YearOfExperience = account.YearOfExperience,
                DegreeUrl = account.DegreeUrl,
                Avatar = account.Avatar,
                Major = account.Major,
                Address = account.Address
            };

            // Lấy danh sách bạn bè, yêu cầu gửi/nhận (giả định đã có logic trong repository)
            var friendRequestReceivers = await _friendRequestRepository.GetFriendRequestReceivers(accountId);
            var friendRequestSenders = await _friendRequestRepository.GetFriendRequestSenders(accountId);
            var listFriends = await _friendRequestRepository.GetListFriends(accountId);

            // Gọi API của PostService để lấy danh sách bài viết
            var postApiUrl = $"all/by-account?accountId={accountId}";
            var response = await _postServiceClient.GetAsync(postApiUrl);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Failed to fetch posts: {response.ReasonPhrase}");

            var content = await response.Content.ReadAsStringAsync();
            var postDTOs = JsonSerializer.Deserialize<IEnumerable<PostDTO>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<PostDTO>();

            // Tạo và trả về UserProfileWithPostsDTO
            return new PersonalPageDTO(accountDTO, friendRequestReceivers, friendRequestSenders, listFriends, postDTOs);
        }

        public async Task<FriendRequest> SendFriendRequest(int senderId, int receiverId)
        {
            // Kiểm tra đầu vào
            if (senderId <= 0 || receiverId <= 0)
                throw new ArgumentException("SenderId and ReceiverId must be positive.");
            if (senderId == receiverId)
                throw new ArgumentException("Cannot send friend request to yourself.");

            return await _friendRequestRepository.SendFriendRequest(senderId, receiverId);
        }

        public async Task UpdateFriendRequestStatus(int senderId, int receiverId, string status)
        {
            if (senderId <= 0 || receiverId <= 0)
                throw new ArgumentException("SenderId and ReceiverId must be positive.");
            if (status != "accepted" && status != "rejected")
                throw new ArgumentException("Status must be either 'accepted' or 'rejected'.");

            await _friendRequestRepository.UpdateFriendRequestStatus(senderId, receiverId, status);
        }
        public async Task<AccountPhotosDTO> GetAccountPhotos(int accountId)
        {
            // Kiểm tra tài khoản có tồn tại không
            var account = await _accountRepo.GetById(accountId);
            if (account == null)
                throw new Exception("Account not found or deleted.");

            // Gọi API của PostService để lấy danh sách bài viết
            var postApiUrl = $"all/by-account?accountId={accountId}";
            var response = await _postServiceClient.GetAsync(postApiUrl);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Failed to fetch posts: {response.ReasonPhrase}");

            var content = await response.Content.ReadAsStringAsync();
            var postDTOs = JsonSerializer.Deserialize<IEnumerable<PostDTO>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<PostDTO>();
            // Trích xuất tất cả ảnh từ danh sách bài viết
            var photos = postDTOs
                .SelectMany(postDTO => postDTO.postImages ?? new List<PostDTO.PostImage>())
                .Where(image => !image.IsDeleted.GetValueOrDefault()) // Chỉ lấy ảnh chưa bị xóa
                .ToList();

            int countPhotos = photos.Count();

            return new AccountPhotosDTO(accountId, countPhotos, photos);
        }
    }
}