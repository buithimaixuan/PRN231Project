using UserService.Models;

namespace UserService.DTOs
{
    public class PersonalPageDTO
    {
        public AccountDTO accountDTO { get; set; }
        public IEnumerable<FriendRequest> friendRequestReceivers { get; set; }
        public IEnumerable<FriendRequest> friendRequestSenders { get; set; }
        public IEnumerable<FriendRequest> listFriends { get; set; }
        public IEnumerable<PostDTO> listPostDTOs { get; set; }

        public PersonalPageDTO(AccountDTO accountDTO, IEnumerable<FriendRequest> friendRequestReceivers, IEnumerable<FriendRequest> friendRequestSenders, IEnumerable<FriendRequest> listFriends, IEnumerable<PostDTO> listPostDTOs)
        {
            this.accountDTO = accountDTO;
            this.friendRequestReceivers = friendRequestReceivers;
            this.friendRequestSenders = friendRequestSenders;
            this.listFriends = listFriends;
            this.listPostDTOs = listPostDTOs;
        }
    }
}
