using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Services;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authorization;
using API_BlogApplication.Enums;
using DTO.ReadDTO;
using DTO.WriteDTO;
using API_BlogApplication.Models;
using Microsoft.Net.Http.Headers;
using Utilities.JWT;

namespace API_BlogApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class CommentController : Controller
    {
        ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        #region Get Comments A Post
        /// <summary>
        ///     Lấy Danh Sách Comment Của Một Bài Viết
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        [Route("{PostID}")]
        public dynamic GetComments([FromRoute] int PostID)
        {
            Task<List<CommentReadDTO>> GetComments_Task = _commentService.GetComments(PostID);

            //Nếu Fetch Data Thành Công
            if (GetComments_Task.IsCompletedSuccessfully)
            {
                return new
                {
                    status = true,
                    code = ReturnCodes.DataGetSucceeded,
                    comments = GetComments_Task.Result
                };
            }
            else
            {
                return new
                {
                    status = false,
                    code = ReturnCodes.DataGetSucceeded,
                    message = "Có Lỗi Xảy Ra Khi Cố Gắng Truy Cập Danh Sách Comment"
                };
            }
        }
        #endregion

        #region Create A Post
        /// <summary>
        ///     Tạo Mới Một Comment
        /// </summary>
        /// <remarks>
        /// 
        ///     Sample Request:
        ///     
        ///         POST /api/Comment/Create 
        ///         {
        ///             commentContent: "abc...",
        ///             postID: 1,
        ///         }
        ///     
        /// </remarks>
        [HttpPost]
        [Authorize]
        public async Task<dynamic> Create([FromBody] CommentModel commentModel)
        {
            string accessToken = Request.Headers[HeaderNames.Authorization];
            string token = accessToken.Split("Bearer ")[1];
            UserReadDTO userRead = TokenUtil.GetSubFromToken(token);

            if (ModelState.IsValid)
            {
                CommentWriteDTO commentWrite = new CommentWriteDTO
                {
                    CommentContent = commentModel.CommentContent,
                    PostID = commentModel.PostID,
                    UserID = new Guid(userRead.UserID)
                };

                //Khởi Tạo Task Create Comment
                Services.Entities.CustomResponse CreateComment_Task = await _commentService.Create(commentWrite);

                //Nếu Task Done
                if (CreateComment_Task.status)
                {
                    return new
                    {
                        status = CreateComment_Task.status,
                        code = ReturnCodes.DataCreateSucceeded,
                        message = CreateComment_Task.message,
                        newComment = CreateComment_Task.commentReadDTO
                    };
                }
            }

            return new
            {
                status = false,
                code = ReturnCodes.ParameterError,
                message = "Dữ Liệu Không Hợp Lệ"
            };
        }
        #endregion

        #region Remove A Comment
        /// <summary>
        ///     Xoá Một Comment
        /// </summary>
        [HttpDelete]
        [Route("{CommentID}")]
        [AllowAnonymous]
        public dynamic Remove([FromRoute] string CommentID)
        {
            string accessToken = Request.Headers[HeaderNames.Authorization];
            string token = accessToken.Split("Bearer ")[1];
            UserReadDTO userRead = TokenUtil.GetSubFromToken(token);

            Task<Services.Entities.CustomResponse> remove_task = _commentService.Remove(CommentID, userRead.UserID);

            return new
            {
                status = remove_task.Result.status,
                code = (remove_task.Result.status ? ReturnCodes.DataRemoveSucceeded : ReturnCodes.DataRemoveFailed),
                message = remove_task.Result.message
            };
        }
        #endregion
    }
}
