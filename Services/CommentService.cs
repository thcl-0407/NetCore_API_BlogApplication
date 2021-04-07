using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO.ReadDTO;
using DTO.WriteDTO;
using DAL;
using DAL.Entities;
using Services.Entities;

namespace Services
{
    public class CommentService : ICommentService
    {
        private readonly BlogApplicationDbContext db;
        public CommentService(BlogApplicationDbContext _db)
        {
            this.db = _db;
        }

        /*Get Comment A Post*/
        public async Task<List<CommentReadDTO>> GetComments(int PostID) 
        {
            if(PostID >= Int32.MaxValue || PostID <= Int32.MinValue)
            {
                throw new ArgumentOutOfRangeException();
            }

            List<CommentReadDTO> GetComment_Task = new List<CommentReadDTO>();

            try
            {
                GetComment_Task = (from Comment in db.Comments
                                   join PostComment in db.PostComments on Comment.CommentID equals PostComment.CommentID
                                   where PostComment.PostID == PostID
                                   select new CommentReadDTO
                                   {
                                       CommentID = Comment.CommentID,
                                       CommentContent = Comment.ContentComment,
                                       DateCreated = Comment.DateCreate,
                                       PostID = PostComment.PostID,
                                       UserID = PostComment.UserID,
                                       UserName = db.Users.FirstOrDefault(u => u.UserID.Equals(PostComment.UserID)).UserName
                                   }).ToListAsync().Result;

                return GetComment_Task;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /*Create Comment A Post*/
        public async Task<CustomResponse> Create(CommentWriteDTO commentWrite)
        {
            CommentReadDTO commentReadDTO;

            if (commentWrite == null || commentWrite.isNullValue())
            {
                return new CustomResponse(false, "Comment is Null");
            }

            if (commentWrite.UserID.ToString().Trim().Length == 0)
            {
                return new CustomResponse(false, "User ID is Empty");
            }

            if(commentWrite.CommentContent.Trim().Length == 0)
            {
                return new CustomResponse(false, "Comment Content is Empty");
            }

            try
            {
                Comment newComment = new Comment
                {
                    CommentID = Guid.NewGuid(),
                    ContentComment = commentWrite.CommentContent,
                    DateCreate = DateTime.Now,
                };

                //Check Task's Done When Add a New Comment
                bool AddComment_Task = db.Comments.AddAsync(newComment).IsCompleted;

                //Nếu Done
                if (AddComment_Task)
                {
                    PostComment postComment = new PostComment
                    {
                        CommentID = newComment.CommentID,
                        PostID = commentWrite.PostID,
                        UserID = commentWrite.UserID
                    };

                    await db.PostComments.AddAsync(postComment);
                    //Done and Save Change
                    await db.SaveChangesAsync();

                    commentReadDTO = new CommentReadDTO();
                    commentReadDTO.CommentID = newComment.CommentID;
                    commentReadDTO.CommentContent = newComment.ContentComment;
                    commentReadDTO.DateCreated = newComment.DateCreate;
                    commentReadDTO.UserID = commentWrite.UserID;
                    commentReadDTO.PostID = commentWrite.PostID;
                    commentReadDTO.UserName = db.Users.FirstOrDefault(u => u.UserID.Equals(commentWrite.UserID)).UserName;
                }
                //Ngược Lại Có Lỗi Xảy Ra
                else
                {
                    return new CustomResponse(false, "Có Lỗi Xảy Ra Khi Thêm Comment");
                }
            }
            catch (Exception e)
            {
                return new CustomResponse(false, e.Message);
            }

            return new CustomResponse(commentReadDTO, true, "Thêm Comment Thành Công");
        }

        /*Remove Comment A Post*/
        public async Task<CustomResponse> Remove(string CommentID, string UserID)
        {
            if(CommentID == null || UserID == null)
            {
                return new CustomResponse(false, "Parameter is null");
            }

            if(CommentID.Trim().Length == 0 || UserID.Trim().Length == 0)
            {
                return new CustomResponse(false, "Property is Empty");
            }
            
            try
            {
                var PostComment = db.PostComments.FirstOrDefault(pc => pc.CommentID.Equals(new Guid(CommentID)) && pc.UserID.Equals(new Guid(UserID)));

                //Không Có Quyền Xoá Comment
                if (PostComment == null)
                {
                    return new CustomResponse(false, "Bạn Không Có Quyền Xoá Comment");
                }

                db.Comments.Remove(db.Comments.FirstOrDefault(c => c.CommentID.Equals(PostComment.CommentID)));
                await db.SaveChangesAsync();

                return new CustomResponse(true, "Xoá Thành Công");
            }
            catch (Exception e)
            {
                return new CustomResponse(false, e.Message);
            }
        }
    }
}
