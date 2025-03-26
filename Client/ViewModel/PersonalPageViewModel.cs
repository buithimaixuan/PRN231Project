using Client.DTOs;
using Client.Models;

namespace Client.ViewModel
{
    public class PersonalPageViewModel
    {
        public PersonalPageDTO? PersonalPageDTO { get; set; }
        public List<CategoryPost>? categoryPosts { get; set; }
        public PostViewModel? PostViewModel { get; set; }

    }
}
