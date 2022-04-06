using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AngularBlogCoreApi.Models;
using AngularBlogCoreApi.Responses;
using System.Globalization;
using System.IO;

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
        public async Task<ActionResult<IEnumerable<ArticleResponse>>> GetArticle()
        {
            var articles= await _context.Article.Include(x=>x.Comment).OrderByDescending(x=>x.PublishDate).Select(x=>new ArticleResponse {
                Id = x.Id,
                Title = x.Title,
                Picture = x.Picture,
                Category = new CategoryResponse { Id=x.CategoryId,CategoryName = x.Category.CategoryName},
                CommentCount = x.Comment.Count(),
                ViewCount = x.ViewCount,
                PublishDate = x.PublishDate
            
            }).ToListAsync();

            return Ok(articles);

        }
        
        [HttpGet]
        [Route("GetArticlesWithCategory/{categoryId}/{page}/{pageSize}")]
        public IActionResult GetArticlesWithCategory(int categoryId, int page=1, int pageSize = 5)
        {
            IQueryable<Article> query = _context.Article.Include(r => r.Category).Include(r => r.Comment).Where(r => r.CategoryId == categoryId).OrderByDescending(r => r.PublishDate);
            var queryResult = ArticlesPagination(query,page,pageSize);
            var result = new
            {
                TotalCount = queryResult.Item2,
                Articles = queryResult.Item1
            };
            return Ok(result);
        }
        [HttpGet]
        [Route("SearchArticles/{queryText}/{page}/{pageSize}")]
        public IActionResult SearchArticles(string queryText, int page = 1, int pageSize = 5)
        {
            IQueryable<Article> query = _context.Article.Include(r => r.Category).Include(r => r.Comment).Where(r => r.Title.Contains(queryText)).OrderByDescending(r=>r.PublishDate);
            var resultQuery = ArticlesPagination(query, page, pageSize);
            var result = new
            {
                Articles = resultQuery.Item1,
                TotalCount = resultQuery.Item2
            };
            
            return Ok(result);
        }

        [HttpGet]
        [Route("GetArticlesByMostViewed")]
        public IActionResult GetArticlesByMostViewed()
        {
            System.Threading.Thread.Sleep(2000);
            var article = _context.Article.OrderByDescending(x=>x.ViewCount).Take(5).Select(x=> new ArticleResponse {
                Title = x.Title,
                Id=x.Id
            
            });
            return Ok(article);

        }
        [HttpGet]
        [Route("GetArticlesArchive")]
        public IActionResult GetArticlesArchive()
        {
            System.Threading.Thread.Sleep(1000);
            var query = _context.Article.GroupBy(x => new
            {
                x.PublishDate.Year,
                x.PublishDate.Month
            }).Select(z => new
            {
                year = z.Key.Year,
                month = z.Key.Month,
                count = z.Count(),
                mounthName = new DateTime(z.Key.Year, z.Key.Month, 1).ToString("MMMM")
            });
            return Ok(query);
        }
        [HttpGet]
        [Route("GetArticleArchiveList/{year}/{month}/{page}/{pageSize}")]
        public IActionResult GetArticleArchiveList(int year, int month, int page, int pageSize)
        {
            System.Threading.Thread.Sleep(1700);
            var query = _context.Article.Include(x => x.Category).Include(x => x.Comment).Where(x => x.PublishDate.Year == year && x.PublishDate.Month == month).OrderByDescending(x => x.PublishDate);
            var resultQuery = ArticlesPagination(query, page, pageSize);
            var result = new
            {
                Articles = resultQuery.Item1,
                TotalCount = resultQuery.Item2
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
            var firstArticle = _context.Article.Find(id);
            firstArticle.Title = article.Title;
            firstArticle.ContentMain = article.ContentMain;
            firstArticle.ContentSummary = article.ContentSummary;
            firstArticle.CategoryId = article.Category.Id;
            firstArticle.Picture = article.Picture;


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

            return Ok();
        }

        // POST: api/Articles
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult> PostArticle(Article article)
        {
            if (article.Category!=null)
            {
                article.CategoryId = article.Category.Id;
            }
            article.Category = null;
            article.ViewCount = 0;
            article.PublishDate = DateTime.Now;
            _context.Article.Add(article);
            await _context.SaveChangesAsync();

            return Ok();
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
            System.Threading.Thread.Sleep(2000);
            int totalCount = query.Count();

            var articlesResponse = query.Skip((pageSize * (page - 1))).Take(pageSize).ToList().Select(x => new ArticleResponse
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

        [HttpGet]
        [Route("ArticleViewCountUp/{id}")]
        public IActionResult ArticleViewCountUp(int id)
        {

            var article = _context.Article.Find(id);
            article.ViewCount += 1;
            _context.SaveChanges();
            return Ok();

        }

        [HttpPost]
        [Route("SaveArticlePicture")]
        public async Task<IActionResult> SaveArticlePicture(IFormFile Picture)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Picture.FileName);
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/ArticlePictures", fileName);

            using (var stream = new FileStream(path,FileMode.Create))
            {
                await Picture.CopyToAsync(stream);
            }
            var result = new
            {
                path = "https://" + Request.Host + "/ArticlePictures/" + fileName
            };
            return Ok(result);
        }



    }
}
