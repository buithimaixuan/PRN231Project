using System.ComponentModel.DataAnnotations;
using Client.DTOs;
using Client.Models;

namespace Client.ViewModel
{
    public class UpdatePostPageViewModel
    {
        //DỮ LIỆU CŨ
        public PostDTO PostDTO { get; set; }
        public List<CategoryPost> CategoryPosts { get; set; }
        public UpdatePostViewModel UpdatePostViewModel { get; set; }
    }
}
