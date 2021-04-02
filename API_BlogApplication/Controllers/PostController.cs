using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API_BlogApplication.Models;
using Services;
using API_BlogApplication.Enums;
using Microsoft.AspNetCore.Authorization;
using DTO.ReadDTO;
using DTO.WriteDTO;
using Services.Entities;
using Microsoft.Net.Http.Headers;
using System.Json;
using Utilities.JWT;

namespace API_BlogApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class PostController : Controller
    {
        private readonly IPostService _postService;

        public PostController(IPostService postService)
        {
            _postService = postService;
        }

        #region Get All Public Posts
        /// <summary>
        ///     Lấy Danh Sách Tất Cả Bài Viết Public
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public dynamic All()
        {
            Task<List<PostReadDTO>> GetAll_Task = _postService.GetAll();

            if (GetAll_Task.IsCompleted)
            {
                return new
                {
                    status = true,
                    code = ReturnCodes.DataGetSucceeded,
                    blogs = GetAll_Task.Result
                };
            }
            else
            {
                return new
                {
                    status = false,
                    code = ReturnCodes.DataGetFailed,
                    message = "Có Lỗi Xảy Ra. Không Thể Get Danh Sách Bài Viết Vào Lúc Này"
                };
            }
        }
        #endregion

        #region Get Post
        /// <summary>
        ///     Lấy Thông Tin Của Một Bài Viết
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        [Route("{PostID}")]
        public dynamic GetPost([FromRoute] int PostID)
        {
            Task<PostReadDTO> GetPost_Task = _postService.GetPost(PostID);

            if (GetPost_Task.IsCompleted)
            {
                return new
                {
                    status = true,
                    code = ReturnCodes.DataGetSucceeded,
                    blogs = GetPost_Task.Result
                };
            }
            else
            {
                return new
                {
                    status = false,
                    code = ReturnCodes.DataGetFailed,
                    message = $"Có Lỗi Xảy Ra. Không Thể Get Bài Viết {PostID.ToString()} Vào Lúc Này"
                };
            }
        }
        #endregion

        #region Get Posts A User
        /// <summary>
        ///     Lấy Danh Sách Tất Cả Bài Viết Của Một User
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        [Route("{UserID}")]
        public dynamic GetPosts([FromRoute] string UserID)
        {
            return new
            {
                status = true,
                code = ReturnCodes.DataGetSucceeded,
                blogs = _postService.GetPosts(UserID).Result
            };
        }
        #endregion

        #region Get Posts By Title
        /// <summary>
        ///     Tìm Kiếm Danh Sách Bài Viết Theo Tiêu Đề
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        [Route("{Title}")]
        public async Task<dynamic> GetPostsByTitle([FromRoute] string Title)
        {

            var task = await _postService.GetPostsByTitle(Title);

            return new
            {
                status = task == null ? false : true,
                code = task == null ? ReturnCodes.DataGetFailedWithNoData : ReturnCodes.DataGetSucceeded,
                blogs = task
            };
        }
        #endregion

        #region Get Posts A User By Token
        /// <summary>
        ///     Lấy Danh Sách Tất Cả Bài Viết Của User Đăng Nhập
        /// </summary>
        [HttpGet]
        [Authorize]
        public dynamic GetPosts()
        {
            string accessToken = Request.Headers[HeaderNames.Authorization];
            string token = accessToken.Split("Bearer ")[1];
            UserReadDTO userRead = TokenUtil.GetSubFromToken(token);

            return new
            {
                status = true,
                code = ReturnCodes.DataGetSucceeded,
                blogs = _postService.GetPosts(userRead.UserID).Result
            };
        }
        #endregion

        #region Get Base64Image
        /// <summary>
        ///     Lấy Base64Image Của Một Bài Viết
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        [Route("{PostID}")]
        public dynamic Base64Image([FromRoute] int PostID)
        {
            try
            {
                //Nếu Chuỗi Base64 Code Bị Empty
                if (_postService.GetBase64ImageAsync(PostID).Result.Equals(""))
                {
                    return new
                    {
                        status = false,
                        code = ReturnCodes.DataGetFailedWithNoData,
                        message = "Không Có Ảnh Để Hiển Thị"
                    };
                }

                //Chuỗi Base64Image Code Có Dữ Liệu
                return new
                {
                    status = true,
                    code = ReturnCodes.DataGetSucceeded,
                    data = _postService.GetBase64ImageAsync(PostID).Result
                };
            }
            catch (Exception e)
            {
                return new
                {
                    status = true,
                    code = ReturnCodes.DataGetFailed,
                    message = e.Message
                };
            }
        }

        #endregion

        #region Create A New Post
        /// <summary>
        ///     Tạo Mới Một Bài Viết
        /// </summary>
        /// <remarks>
        /// 
        ///     Sample Request:
        ///     
        ///         POST /api/Post/Create 
        ///         {
        ///             Title: "abc...",
        ///             Summary: "abc...",
        ///             Content: "bla bla bla...",
        ///             EncodeImage: "data:image/jpeg;base64,/9j/4AAQSkZJRgABAQA...",
        ///         }
        ///     
        /// </remarks>
        [HttpPost]
        [Authorize]
        public dynamic Create(PostModel postModel)
        {
            string accessToken = Request.Headers[HeaderNames.Authorization];
            string token = accessToken.Split("Bearer ")[1];
            UserReadDTO userRead = TokenUtil.GetSubFromToken(token);

            if (ModelState.IsValid)
            {
                PostWriteDTO postWrite = new PostWriteDTO
                {
                    TitlePost = postModel.Title,
                    SummaryPost = postModel.Summary,
                    ContentPost = postModel.Content,
                    EncodeImage = postModel.EncodeImage,
                    UserID = userRead.UserID
                };

                if (_postService.Create(postWrite).Result.status)
                {
                    return new
                    {
                        status = true,
                        code = ReturnCodes.DataCreateSucceeded,
                        message = "Thêm Post Thành Công"
                    };
                }
            }

            return new
            {
                status = false,
                code = ReturnCodes.DataCreateFailed,
                message = "Có Lỗi Xảy Ra"
            };
        }
        #endregion

        #region Update A Post
        /// <summary>
        ///     Cập Nhật Một Bài Viết
        /// </summary>
        /// <remarks>
        /// 
        ///     Sample Request:
        ///     
        ///         Put /api/Post/Update/1 
        ///         {
        ///             title: "abc...",
        ///             summary: "abc...",
        ///             content: "bla bla bla...",
        ///             encodeImage: "data:image/jpeg;base64,/9j/4AAQSkZJRgABAQA...",
        ///         }
        ///     
        /// </remarks>
        [HttpPut]
        [Authorize]
        [DisableRequestSizeLimit]
        [Route("{PostID}")]
        public dynamic Update([FromRoute] int PostID, [FromBody] PostModel postModel)
        {
            string accessToken = Request.Headers[HeaderNames.Authorization];
            string token = accessToken.Split("Bearer ")[1];
            UserReadDTO userRead = TokenUtil.GetSubFromToken(token);

            PostWriteDTO postWrite = new PostWriteDTO { 
                PostID = PostID,
                TitlePost = postModel.Title,
                ContentPost = postModel.Content,
                SummaryPost = postModel.Summary,
                EncodeImage = postModel.EncodeImage,
                UserID = userRead.UserID
            };

            Task<CustomResponse> remove_task = _postService.Update(postWrite, userRead.UserID);

            return new
            {
                status = remove_task.Result.status,
                code = (remove_task.Result.status ? ReturnCodes.DataRemoveSucceeded : ReturnCodes.DataRemoveFailed),
                message = remove_task.Result.message
            };
        }
        #endregion

        #region Remove A Post
        /// <summary>
        ///     Xoá Một Bài Viết
        /// </summary>
        /// <remarks>
        /// 
        ///     Sample Request:
        ///     
        ///         DELETE /api/Post/Remove/1 
        ///         
        /// </remarks>
        [HttpDelete]
        [Authorize]
        [Route("{PostID}")]
        public dynamic Remove([FromRoute] int PostID)
        {
            string accessToken = Request.Headers[HeaderNames.Authorization];
            string token = accessToken.Split("Bearer ")[1];
            UserReadDTO userRead = TokenUtil.GetSubFromToken(token);

            Task<CustomResponse> remove_task = _postService.Remove(PostID, userRead.UserID);

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
