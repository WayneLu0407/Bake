using Bake.Data;
using Bake.Models.Social;
using Bake.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Contracts;

namespace Bake.Controllers
{
    public class PostArticleController : Controller
    {
        private readonly BakeContext _bakeContext;
        public PostArticleController(BakeContext bakeContext)
        {
            _bakeContext = bakeContext;
        }

        private int CurrentUserId
        {
            get
            {
                var claimValue = User.FindFirst("UserId")?.Value
                              ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(claimValue, out int id))
                {
                    return id;
                }
                return 0;
            }
        }
        public IActionResult CreatePost()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetPostList() 
        {
            var list = (await _bakeContext.Posts.OrderByDescending(p => p.CreatedAt).ToListAsync())
                .Select(p => new
                {
                    p.PostId,
                    p.Title,
                    p.IsPublished,
                    Date = p.CreatedAt?.ToString("yyyy/MM/dd") ?? "無日期"
                });

            return Json(list);
        }

        [HttpGet]
        public async Task<IActionResult> GetPostDetail(int id)
        {
            var post = await _bakeContext.Posts
                .Include(p=>p.PostAttachments)
                .FirstOrDefaultAsync(p=>p.PostId==id);
            if (post == null) return NotFound();

            var coverImg = post.PostAttachments.FirstOrDefault(a => a.IsCover == true)?.FileUrl;
            return Json(new
            {
                post.PostId,
                post.Title,
                post.Content,
                post.IsPublished,
                PostTypeId = post.TypeId,
                ImageUrl = coverImg
            });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateInsertPost([FromForm] PostArticleViewModel viewModel)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            if (!ModelState.IsValid)
                //return Json(new { success = false, message = "請填寫標題與內容" });
                return Json(new { success = false, message = string.Join(" | ", errors) });

            try
            {
                //文章初始值
                Post post;
                PostAttachment attachment;

                //新增文章
                if (viewModel.PostId == 0)
                {
                    post = new Post
                    {
                        AuthorId = CurrentUserId,
                        TypeId = 0,
                        CreatedAt = DateTime.Now,
                        LikesCount = 0,
                        ViewCount = 0,
                        FavoriteCount = 0
                    };
                    _bakeContext.Posts.Add(post);
                }
                //更新文章
                else
                {
                    post = _bakeContext.Posts.FirstOrDefault(p => p.PostId == viewModel.PostId && p.AuthorId == CurrentUserId);
                    if(post == null)
                        return Json(new { success = false, message = "找不到文章或無權限編輯" });
                }

                //更新內容
                post.Title = viewModel.Title;
                post.Content = viewModel.Content;
                post.IsPublished = viewModel.IsPublished;
                await _bakeContext.SaveChangesAsync(); //若是新增文章，先儲存取得postId

                //處理圖片上傳
                if (viewModel.Images != null && viewModel.Images.Count > 0) 
                {
                    // 先看有無舊圖片，有的話就刪除舊圖片，釋放資源
                    if (viewModel.PostId != 0)
                    {
                        var oldAttachments = _bakeContext.PostAttachments.Where(a => a.PostId == post.PostId).ToList();

                        foreach (var oldFile in oldAttachments) 
                        {
                            var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", oldFile.FileUrl.TrimStart('/'));
                            if (System.IO.File.Exists(oldFilePath)) 
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }
                        _bakeContext.PostAttachments.RemoveRange(oldAttachments);
                    }

                    // 新圖片上傳
                    string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/PostImage/article");
                    if(!Directory.Exists(uploadPath))
                        Directory.CreateDirectory(uploadPath);

                    bool isFirstImage = true;
                    foreach (var image in viewModel.Images) 
                    {
                        //產生唯一檔名
                        string fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                        string filePath = Path.Combine(uploadPath, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }

                        //儲存圖片資訊到資料庫
                        var postAttachment = new PostAttachment
                        {
                            PostId = post.PostId,
                            FileUrl = "/PostImage/article/"+fileName,
                            IsCover = (image.FileName == viewModel.CoverImgName) || (isFirstImage && string.IsNullOrEmpty(viewModel.CoverImgName))
                        };

                        _bakeContext.PostAttachments.Add(postAttachment);
                    }

                    await _bakeContext.SaveChangesAsync();
                }

                return Json(new { success = true, message = "文章已成功儲存", newPostId = post.PostId});
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "發生錯誤: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeletePost(int id) 
        {
            var post = await _bakeContext.Posts.FindAsync(id);
            if(post == null) return Json(new { success = false, message = "找不到文章" });

            _bakeContext.Posts.Remove(post);
            await _bakeContext.SaveChangesAsync();

            return Json(new { success = true, message = "文章已刪除" });
        }
    }
}