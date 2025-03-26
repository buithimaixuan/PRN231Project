using Client.Models;
using Client.ViewModel;

namespace Client.DTOs
{
    public class PersonalPageDTO
    {
        public AccountDTO accountDTO { get; set; }
        public IEnumerable<FriendRequestDTO> friendRequestReceivers { get; set; }
        public IEnumerable<FriendRequestDTO> friendRequestSenders { get; set; }
        public IEnumerable<FriendRequestDTO> listFriends { get; set; }
        public IEnumerable<PostDTO> listPostDTOs { get; set; }

        public PersonalPageDTO(AccountDTO accountDTO, IEnumerable<FriendRequestDTO> friendRequestReceivers, IEnumerable<FriendRequestDTO> friendRequestSenders, IEnumerable<FriendRequestDTO> listFriends, IEnumerable<PostDTO> listPostDTOs)
        {
            this.accountDTO = accountDTO;
            this.friendRequestReceivers = friendRequestReceivers;
            this.friendRequestSenders = friendRequestSenders;
            this.listFriends = listFriends;
            this.listPostDTOs = listPostDTOs;
        }
    }
}
