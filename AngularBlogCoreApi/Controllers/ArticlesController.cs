using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AngularBlogCoreApi.Models;
using AngularBlogCoreApi.Responses;

namespace AngularBlogCoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly AngularBlogContext _context;

        public ArticlesController(AngularBlogContext context)
        {
            _context = context;
        }
        
        // GET: api/Articles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Article>>> GetArticle()
        {
            return await _context.Article.ToListAsync();
        }
        
        [HttpGet]
        [Route("GetArticlesWithCategory/{categoryId}/{page}/{pageSize}")]
        public IActionResult GetArticlesWithCategory(int id, int page=1, int pageSize = 5)
        {
            IQueryable<Article> query = _context.Article.Include(r => r.Category).Include(r => r.Comment).Where(r => r.CategoryId == id).OrderByDescending(r => r.PublishDate);
            var queryResult = ArticlesPagination(query,page,pageSize);
            var result = new
            {
                TotalCount = queryResult.Item2,
                Articles = queryResult.Item1
            };
            return Ok(result);
        }

        [HttpGet("{page}/{pageSize}")]
        public IActionResult GetArticle(int page=1,int pageSize = 5)
        {
            System.Threading.Thread.Sleep(3000);
            try
            {
                IQueryable<Article> query;
                query = _context.Article.Include(r => r.Category).Include(r => r.Comment).OrderByDescending(r => r.PublishDate);
                int totalCount = query.Count();
                var articlesResponse = query.Skip((pageSize * (page- 1))).Take(5).ToList().Select(x => new ArticleResponse
                {
                    Id = x.Id,
                    Title = x.Title,
                    ContentMain = x.ContentMain,
                    ContentSummary = x.ContentSummary,
                    Picture = x.Picture,
                    ViewCount = x.ViewCount,
                    CommentCount = x.Comment.Count,
                    Category = new CategoryResponse() { Id = x.CategoryId, CategoryName = x.Category.CategoryName }


                });
                var result = new
                {
                    totalCount = totalCount,
                    Articles = articlesResponse

                };
                return Ok(result);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }


        // GET: api/Articles/5
        [HttpGet("{id}")]
        public IActionResult GetArticle(int id)
        {
            System.Threading.Thread.Sleep(2000);
            var article = _context.Article.Include(r => r.Category).Include(r => r.Comment).FirstOrDefault(r => r.Id == id);
            if (article == null)
            {
                return NotFound();
            }
            ArticleResponse articleResponse = new ArticleResponse()
            {
                Id = article.Id,
                ContentMain = article.ContentMain,
                ContentSummary = article.ContentSummary,
                Picture = article.Picture,
                PublishDate = article.PublishDate,
                Title = article.Title,
                CommentCount = article.Comment.Count(),
                Category = new CategoryResponse { Id = article.Category.Id, CategoryName = article.Category.CategoryName }

            };

            return Ok(articleResponse);
        }

        // PUT: api/Articles/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutArticle(int id, Article article)
        {
            if (id != article.Id)
            {
                return BadRequest();
            }

            _context.Entry(article).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArticleExists(id))
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

        // POST: api/Articles
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Article>> PostArticle(Article article)
        {
            _context.Article.Add(article);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetArticle", new { id = article.Id }, article);
        }

        // DELETE: api/Articles/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Article>> DeleteArticle(int id)
        {
            var article = await _context.Article.FindAsync(id);
            if (article == null)
            {
                return NotFound();
            }

            _context.Article.Remove(article);
            await _context.SaveChangesAsync();

            return article;
        }

        private bool ArticleExists(int id)
        {
            return _context.Article.Any(e => e.Id == id);
        }

        public System.Tuple<IEnumerable<ArticleResponse>,int> ArticlesPagination(IQueryable<Article>query, int page, int pageSize)
        {
            int totalCount = query.Count();

            var articlesResponse = query.Skip((pageSize * (page - 1))).Take(5).ToList().Select(x => new ArticleResponse
            {
                Id = x.Id,
                Title = x.Title,
                ContentMain = x.ContentMain,
                ContentSummary = x.ContentSummary,
                Picture = x.Picture,
                ViewCount = x.ViewCount,
                CommentCount = x.Comment.Count,
                Category = new CategoryResponse() { Id = x.CategoryId, CategoryName = x.Category.CategoryName }


            });

            return new System.Tuple<IEnumerable<ArticleResponse>, int>(articlesResponse, totalCount);

        }

    }
}
