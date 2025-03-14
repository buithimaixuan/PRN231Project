using PostService.Models;
using PostService.Repositories.Interface;
using PostService.Services.Interface;

namespace PostService.Services.Implement
{
    public class CategoryPostService : ICategoryPostService
    {
        private readonly ICategoryPostRepository _categoryPostRepository;

        public CategoryPostService(ICategoryPostRepository categoryPostRepository)
        {
            this._categoryPostRepository = categoryPostRepository;
        }

        public async Task<int> AddCatPost(CategoryPost item)
        {
            return await _categoryPostRepository.Add(item);
        }

        public async Task<int> DeleteCatPost(int id)
        {
            return await _categoryPostRepository.Delete(id);
        }

        public async Task<CategoryPost> FindById(int id)
        {
            return await _categoryPostRepository.FindById(id);
        }

        public async Task<IEnumerable<CategoryPost>> GetAllCategory()
        {
            return await _categoryPostRepository.GetAllCategory();
        }

        public async Task<int> UpdateCatPost(CategoryPost item)
        {
            return await _categoryPostRepository.Update(item);
        }
    }
}
