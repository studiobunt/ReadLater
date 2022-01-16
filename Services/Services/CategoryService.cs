using Data;
using Entity;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class CategoryService : ICategoryService
    {
        private ReadLaterDataContext _ReadLaterDataContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private string userId;
        public CategoryService(ReadLaterDataContext readLaterDataContext, IHttpContextAccessor httpContextAccessor) 
        {
            _ReadLaterDataContext = readLaterDataContext;
            _httpContextAccessor = httpContextAccessor;
            userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        }

        public Category CreateCategory(Category category)
        {
            category.UserId = userId;
            _ReadLaterDataContext.Add(category);
            _ReadLaterDataContext.SaveChanges();
            return category;
        }

        public void UpdateCategory(Category category)
        {
            _ReadLaterDataContext.Update(category);
            _ReadLaterDataContext.SaveChanges();
        }

        public List<Category> GetCategories()
        {
            return _ReadLaterDataContext.Categories.Where(c => c.UserId == userId).ToList();
        }

        public Category GetCategory(int Id)
        {
            return _ReadLaterDataContext.Categories.Where(c => c.ID == Id && c.UserId == userId).FirstOrDefault();
        }

        public Category GetCategory(string Name)
        {
            return _ReadLaterDataContext.Categories.Where(c => c.Name == Name && c.UserId == userId).FirstOrDefault();
        }

        public void DeleteCategory(Category category)
        {
            _ReadLaterDataContext.Categories.Remove(category);
            _ReadLaterDataContext.SaveChanges();
        }
    }
}
