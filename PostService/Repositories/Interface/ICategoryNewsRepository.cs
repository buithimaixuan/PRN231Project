using Microsoft.EntityFrameworkCore;
using PostService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostService.Repositories.Interface
{
    public interface ICategoryNewsRepository
    {
        Task<CategoryNews> GetCategoryNewsById(int id);
        Task<IEnumerable<CategoryNews>> GetAllCategoryNews();
        Task<IEnumerable<CategoryNews>> GetCategoriesHaveNews();
        
        Task Add(CategoryNews item);
        Task Update(CategoryNews item);
    }
}
