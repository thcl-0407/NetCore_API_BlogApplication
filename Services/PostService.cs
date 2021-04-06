using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Services.Entities;
using DTO.ReadDTO;
using DTO.WriteDTO;


namespace Services
{
    public class PostService : IPostService
    {
        private BlogApplicationDbContext db;

        public PostService(BlogApplicationDbContext dbContext)
        {
            db = dbContext;
        }

        /*Thêm Một Post Mới*/
        public async Task<CustomResponse> Create(PostWriteDTO postWrite)
        {
            if (postWrite == null)
            {
                return new CustomResponse(false, "Post is Null");
            }

            if (postWrite.isPropertiesNull())
            {
                return new CustomResponse(false, "Post properties is Null");
            }

            if (postWrite.isPropertiesEmpty())
            {
                return new CustomResponse(false, "Post properties is Empty");
            }

            try
            {
                //Init a Post
                Post post = new Post();
                post.TitlePost = postWrite.TitlePost;
                post.SummaryPost = postWrite.SummaryPost;
                post.ContentPost = postWrite.ContentPost;
                post.DateCreate = DateTime.Now;
                post.DateUpdate = DateTime.Now;
                post.UserID = new Guid(postWrite.UserID);

                //Init ID cho Image
                Guid NewImageID = Guid.NewGuid();
                post.ImageID = NewImageID;

                //Init a Image
                ImageGallery image = new ImageGallery();
                image.Base64Code = postWrite.EncodeImage;
                image.ImageID = NewImageID;

                bool task_add_image = db.ImageGalleries.AddAsync(image).IsCompleted;

                if (task_add_image)
                {
                    await db.Posts.AddAsync(post);
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                return new CustomResponse(false, e.Message);
            }

            return new CustomResponse(true, "Thêm Post Thành Công");
        }

        /*Cập Nhật Một Bài Post*/
        public async Task<CustomResponse> Update(PostWriteDTO postWrite, string UserID)
        {
            if (postWrite == null)
            {
                return new CustomResponse(false, "Post is Null");
            }

            postWrite.UserID = UserID;

            if (postWrite.isPropertiesNull())
            {
                return new CustomResponse(false, "Post properties is Null");
            }

            if (postWrite.isPropertiesEmpty())
            {
                return new CustomResponse(false, "Post properties is Empty");
            }

            try
            {
                Post CurPost = db.Posts.FirstOrDefaultAsync(p => p.PostID == postWrite.PostID).Result;

                //Tìm Thấy Post Cần Cập Nhật
                if (CurPost != null)
                {
                    //Fix Post Không Chính Chủ
                    if (!CurPost.UserID.Equals(new Guid(UserID)))
                    {
                        return new CustomResponse(false, "Bạn Không Có Quyền Cập Nhật Post Này");
                    }

                    //Có Cập Nhật Hình Ảnh
                    if (postWrite.EncodeImage.Length > 0)
                    {
                        ImageGallery new_image = new ImageGallery
                        {
                            ImageID = Guid.NewGuid(),
                            Base64Code = postWrite.EncodeImage
                        };

                        bool add_image_task = db.ImageGalleries.AddAsync(new_image).IsCompleted;

                        if (add_image_task)
                        {
                            CurPost.TitlePost = postWrite.TitlePost;
                            CurPost.SummaryPost = postWrite.SummaryPost;
                            CurPost.ContentPost = postWrite.ContentPost;
                            CurPost.DateUpdate = DateTime.Now;
                            CurPost.ImageID = new_image.ImageID;
                        }
                        else
                        {
                            return new CustomResponse(false, "Không Thể Cập Nhật Hình Ảnh Vào Lúc Này");
                        }
                    }
                    //Không Cập Nhật Hình Ảnh
                    else
                    {
                        CurPost.TitlePost = postWrite.TitlePost;
                        CurPost.SummaryPost = postWrite.SummaryPost;
                        CurPost.ContentPost = postWrite.ContentPost;
                        CurPost.DateUpdate = DateTime.Now;
                    }


                    //Done and Save Change All Modifies
                    await db.SaveChangesAsync();
                }
                //Tìm Không Thấy Post Cần Cập Nhật
                else
                {
                    return new CustomResponse(false, "Post Không Tồn Tại");
                }
            }
            catch (Exception e)
            {
                return new CustomResponse(false, e.Message);
            }

            return new CustomResponse(true, "Cập Nhật Thành Công");
        }

        /*Xoá Một Bài Post*/
        public async Task<CustomResponse> Remove(int postID, string UserID)
        {
            if (postID == null)
            {
                return new CustomResponse(false, "PostID is Null");
            }

            try
            {
                IQueryable<Comment> Comments = from PostComment in db.PostComments
                                               join Comment in db.Comments on PostComment.CommentID equals Comment.CommentID
                                               where PostComment.PostID == postID
                                               select Comment;

                Post post = db.Posts.FirstOrDefaultAsync(p => p.PostID == postID).Result;

                if (post != null)
                {
                    //Post Chính Chủ => Được Xoá
                    if (post.UserID.Equals(new Guid(UserID)))
                    {
                        db.RemoveRange(Comments);
                        db.Posts.Remove(post);
                        await db.SaveChangesAsync();
                    }
                    //Post Vãng Lai
                    else
                    {
                        return new CustomResponse(false, "Bạn Không Có Quyền Xoá Post Này");
                    }

                    return new CustomResponse(true, "Xoá Post Thành Công");
                }
                else
                {
                    return new CustomResponse(false, "Post Không Tồn Tại");
                }
            }
            catch (Exception e)
            {
                return new CustomResponse(false, e.Message);
            }

           
        }

        /*Get A Post*/
        public async Task<PostReadDTO> GetPost(int? PostID)
        {
            if (PostID == null)
            {
                throw new ArgumentNullException();
            }

            if(PostID > int.MaxValue || PostID < int.MinValue)
            {
                throw new ArgumentOutOfRangeException();
            }

            PostReadDTO post_result =  (from ImageGallery in db.ImageGalleries
                                             join Post in db.Posts on ImageGallery.ImageID equals Post.ImageID
                                             join User in db.Users on Post.UserID equals User.UserID
                                             where Post.PostID == PostID
                                             select new PostReadDTO
                                             {
                                                 PostID = Post.PostID,
                                                 TitlePost = Post.TitlePost,
                                                 ContentPost = Post.ContentPost,
                                                 SummaryPost = Post.SummaryPost,
                                                 ImageID = ImageGallery.ImageID.ToString(),
                                                 EncodeImage = ImageGallery.Base64Code,
                                                 DateCreated = Post.DateCreate,
                                                 UserID = User.UserID.ToString(),
                                                 UserName = User.UserName,
                                             }).FirstOrDefaultAsync().Result;

            return post_result;
        }

        /*Get Base64Image A Post*/
        public async Task<string> GetBase64ImageAsync(int? PostID)
        {
            if (PostID == null)
            {
                throw new ArgumentNullException();
            }

            try
            {
                Guid ImageID = db.Posts.Where(p => p.PostID == PostID).FirstOrDefault().ImageID;
                Task<ImageGallery> task_image = db.ImageGalleries.Where(ig => ig.ImageID.Equals(ImageID)).FirstOrDefaultAsync();

                return task_image.Result.Base64Code;
            }
            catch (Exception e)
            {
                return String.Empty;
            }

            return String.Empty;
        }

        /*Get Tất Cả Bài Post*/
        public async Task<List<PostReadDTO>> GetAll()
        {
            List<PostReadDTO> post_result = (from ImageGallery in db.ImageGalleries
                                                   join Post in db.Posts on ImageGallery.ImageID equals Post.ImageID
                                                   join User in db.Users on Post.UserID equals User.UserID
                                                   select new PostReadDTO
                                                   {
                                                       PostID = Post.PostID,
                                                       TitlePost = Post.TitlePost,
                                                       SummaryPost = Post.SummaryPost,
                                                       EncodeImage = ImageGallery.Base64Code,
                                                       ImageID = ImageGallery.ImageID.ToString(),
                                                       DateCreated = Post.DateCreate,
                                                       UserID = User.UserID.ToString(),
                                                       UserName = User.UserName,
                                                   }).ToListAsync<PostReadDTO>().Result;

            return post_result;
        }

        /*Get Danh Sách Bài Viết Của Một User*/
        public Task<List<PostReadDTO>> GetPosts(string UserID)
        {
            if (UserID == null)
            {
                throw new NullReferenceException();
            }

            if (UserID.Trim().Length == 0)
            {
                throw new ArgumentException();
            }

            Task<List<PostReadDTO>> post_result = (from ImageGallery in db.ImageGalleries
                                             join Post in db.Posts on ImageGallery.ImageID equals Post.ImageID
                                             join User in db.Users on Post.UserID equals User.UserID
                                             where Post.UserID.ToString().Equals(UserID)
                                             select new PostReadDTO
                                             {
                                                 PostID = Post.PostID,
                                                 TitlePost = Post.TitlePost,
                                                 ContentPost = Post.ContentPost,
                                                 SummaryPost = Post.SummaryPost,
                                                 ImageID = ImageGallery.ImageID.ToString(),
                                                 EncodeImage = ImageGallery.Base64Code,
                                                 DateCreated = Post.DateCreate,
                                                 UserID = User.UserID.ToString(),
                                                 UserName = User.UserName,
                                             }).ToListAsync();

            return post_result;
        }

        public Task<List<PostReadDTO>> GetPostsByTitle(string Key_Title)
        {
            if (Key_Title == null)
            {
                throw new NullReferenceException();
            }

            if (Key_Title.Trim().Length == 0)
            {
                throw new ArgumentException();
            }

            Task<List<PostReadDTO>> post_result = (from ImageGallery in db.ImageGalleries
                                                   join Post in db.Posts on ImageGallery.ImageID equals Post.ImageID
                                                   join User in db.Users on Post.UserID equals User.UserID
                                                   where Post.TitlePost.Contains(Key_Title)
                                                   select new PostReadDTO
                                                   {
                                                       PostID = Post.PostID,
                                                       TitlePost = Post.TitlePost,
                                                       ContentPost = Post.ContentPost,
                                                       SummaryPost = Post.SummaryPost,
                                                       ImageID = ImageGallery.ImageID.ToString(),
                                                       EncodeImage = ImageGallery.Base64Code,
                                                       DateCreated = Post.DateCreate,
                                                       UserID = User.UserID.ToString(),
                                                       UserName = User.UserName,
                                                   }).ToListAsync();

            return post_result;
        }
    }
}
