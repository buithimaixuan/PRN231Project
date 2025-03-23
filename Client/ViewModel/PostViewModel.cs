namespace Client.ViewModel
{
    public class PostViewModel
    {
        public int AccountId { get; set; }
        public string ContentPost { get; set; }
        public string CategoryId { get; set; }
        public List<IFormFile> Images { get; set; } // Để nhận nhiều ảnh từ form


        public PostViewModel(int accountId, string contentPost, string categoryId, List<IFormFile> images)
        {
            AccountId = accountId;
            this.ContentPost = contentPost;
            this.CategoryId = categoryId;
            Images = images;
        }

        public PostViewModel()
        {
        }
    }
}
