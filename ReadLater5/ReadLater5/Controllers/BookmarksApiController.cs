using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Data;
using Entity;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using ReadLater5.DTO;
using Microsoft.AspNetCore.Identity;
using Services;

namespace ReadLater5.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BookmarksApiController : ControllerBase
    {
        IBookmarkService _bookmarkService;
        private readonly SignInManager<IdentityUser> _userManager;
        private readonly IJWTService _jwtService;
        public BookmarksApiController(IBookmarkService bookmarkService, IJWTService jwtService, SignInManager<IdentityUser> userManager)
        {
            _bookmarkService = bookmarkService;
            _userManager = userManager;
            _jwtService = jwtService;
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserModel login)
        {
            IActionResult response = Unauthorized();
            var authentication = await _userManager.PasswordSignInAsync(login.Username, login.Password, false, lockoutOnFailure: false);
            if (authentication.Succeeded)
            {
                var tokenString = _jwtService.GenerateJSONWebToken();
                response = Ok(new { token = tokenString });
            }

            return response;
        }

        // GET: api/BookmarksApi
        [HttpGet]
        public ActionResult<IEnumerable<Bookmark>> GetBookmark()
        {
            return  _bookmarkService.GetBookmarks();
        }

        // GET: api/BookmarksApi/5
        [HttpGet("{id}")]
        public ActionResult<Bookmark> GetBookmark(int id)
        {
            var bookmark = _bookmarkService.GetBookmark(id);

            if (bookmark == null)
            {
                return NotFound();
            }

            return bookmark;
        }

        // PUT: api/BookmarksApi/5
        [HttpPut("{id}")]
        public  IActionResult PutBookmark(int id, Bookmark bookmark)
        {
            if (id != bookmark.ID)
            {
                return BadRequest();
            }

            try
            {
                _bookmarkService.UpdateBookmark(bookmark);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_bookmarkService.BookmarkExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/BookmarksApi
        [HttpPost]
        public ActionResult<Bookmark> PostBookmark(Bookmark bookmark)
        {
            _bookmarkService.CreateBookmark(bookmark);

            return CreatedAtAction("GetBookmark", new { id = bookmark.ID }, bookmark);
        }

        // DELETE: api/BookmarksApi/5
        [HttpDelete("{id}")]
        public  IActionResult DeleteBookmark(int id)
        {
            var bookmark = _bookmarkService.GetBookmark(id);
            if (bookmark == null)
            {
                return NotFound();
            }
            _bookmarkService.DeleteBookmark(bookmark);

            return NoContent();
        }
    }
}
