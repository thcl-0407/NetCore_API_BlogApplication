using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Json;
using System.Threading.Tasks;
using Utilities;
using Utilities.JWT;
using DAL.Entities;
using Newtonsoft.Json;
using Services;
using AutoMapper;
using DTO.WriteDTO;
using DTO.ReadDTO;
using Microsoft.Net.Http.Headers;
using API_BlogApplication.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Swashbuckle.AspNetCore.Annotations;
using API_BlogApplication.Swagger.Filters;
using API_BlogApplication.Models;

namespace API_BlogApplication.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [SwaggerSchemaFilter(typeof(UserWriteDTOSchemaFilter))]
    public class UserController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public UserController(IMapper mapper, IUserService userService)
        {
            _mapper = mapper;
            _userService = userService;
        }

        #region Login
        /// <summary>
        ///     Đăng Nhập Vào Hệ Thống
        /// </summary>
        /// <remarks>
        /// 
        ///     Sample Request:
        ///     
        ///         POST /api/User/Login
        ///         {
        ///             "email": "abc@gmail.com",
        ///             "password":"123456"
        ///         }
        ///         
        ///</remarks>
        [HttpPost]
        public dynamic Login([FromBody] UserLoginModel user)
        {
            if (ModelState.IsValid)
            {
                Services.Entities.CustomResponse Login_Response = _userService.Login(user.Email, user.Password);

                if (Login_Response.status)
                {
                    Token token_current = _userService.GetToken(Login_Response.userReadDTO.UserID);

                    //Lần Đầu Tiên Đăng Nhập
                    if (token_current == null)
                    {
                        Utilities.JWT.TokenResponse tokens = TokenUtil.GenerateTokens(Login_Response.userReadDTO, Startup.userTokenOption);

                        DTO.ReadDTO.TokenReadDTO tokenReadDTO = new DTO.ReadDTO.TokenReadDTO
                        {
                            AccessToken = tokens.AccessToken,
                            AccessTokenExpriesIn = tokens.AccessTokenExpiresIn,
                            RefreshToken = tokens.RefreshToken,
                            RefreshTokenExpriesIn = tokens.RefreshTokenExpiresIn,
                        };

                        //Lưu Token Mới Vào Database
                        _userService.AddToken(Login_Response.userReadDTO.UserID, tokenReadDTO);

                        return new
                        {
                            status = true,
                            code = ReturnCodes.DataGetSucceeded,
                            message = "Đăng Nhập Thành Công",
                            data = Login_Response.userReadDTO,
                            tokens = tokens
                        };
                    }

                    //Nếu Token Trong Database Đã Hết Hạn
                    if (!TokenUtil.isExpiredTime(token_current.AccessToken))
                    {
                        Utilities.JWT.TokenResponse tokens = TokenUtil.GenerateTokens(Login_Response.userReadDTO, Startup.userTokenOption);

                        DTO.ReadDTO.TokenReadDTO tokenReadDTO = new DTO.ReadDTO.TokenReadDTO
                        {
                            AccessToken = tokens.AccessToken,
                            AccessTokenExpriesIn = tokens.AccessTokenExpiresIn,
                            RefreshToken = tokens.RefreshToken,
                            RefreshTokenExpriesIn = tokens.RefreshTokenExpiresIn,
                        };

                        _userService.UpdateToken(Login_Response.userReadDTO.UserID, tokenReadDTO);
                    }

                    return new
                    {
                        status = true,
                        code = ReturnCodes.DataGetSucceeded,
                        message = "Đăng Nhập Thành Công",
                        data = Login_Response.userReadDTO,
                        tokens = _mapper.Map<TokenReadDTO>(token_current)
                    };
                }
            }

            return new
            {
                status = false,
                code = ReturnCodes.DataGetFailed,
                message = "Đăng Nhập Không Thành Công"
            };
        }
        #endregion

        #region Registry
        /// <summary>
        ///     Tạo Tài Khoản
        /// </summary>
        /// <remarks>
        /// 
        ///     Sample Request:
        /// 
        ///         POST /api/User/Registry
        ///         {
        ///             "username": "abc",            
        ///             "email": "abc@gmail.com",
        ///             "password":"123456"
        ///         }
        ///         
        ///</remarks>
        [HttpPost]
        [AllowAnonymous]
        public dynamic Registry([FromBody] UserRegistryModel userRegistry)
        {
            if (ModelState.IsValid)
            {
                UserWriteDTO user = new UserWriteDTO();

                user.UserID = Guid.NewGuid();
                user.UserName = userRegistry.UserName;
                user.Email = userRegistry.Email;
                user.Password = BCryptUtil.HashPassword(userRegistry.Password);

                Task<Services.Entities.CustomResponse> task = _userService.RegistryAsync(user);

                if (task.Result.status)
                {
                    UserReadDTO user_return = new UserReadDTO(user.UserID.ToString(), user.UserName, user.Email);

                    return new
                    {
                        status = true,
                        code = ReturnCodes.DataCreateFailed,
                        message = "Đăng Ký Thành Công",
                        data = user_return
                    };
                }
            }

            return new
            {
                status = false,
                code = ReturnCodes.DataCreateFailed,
                message = "Đăng Ký Không Thành Công"
            };
        }
        #endregion

        #region Check Token Expired
        /// <summary>
        ///     Kiểm Tra Một Token Đã Hết Hạn Chưa 
        /// </summary>
        [HttpGet]
        [Authorize]
        public dynamic ExpiredTime()
        {
            string UserHeaderBearer = Request.Headers[HeaderNames.Authorization];
            string AccessToken = UserHeaderBearer.Split("Bearer ")[1];

            if (AccessToken.Length <= 0)
            {
                return BadRequest();
            }

            return new
            {
                isExpiredTime = TokenUtil.isExpiredTime(AccessToken)
            };
        }
        #endregion

        #region Update User Name
        /// <summary>
        ///     Update Thông Tin Người Dùng
        /// </summary>
        /// <remarks>
        /// 
        ///     Sample Request:
        ///     
        ///         Patch /api/User/Update
        ///         {
        ///             userName: "abc..."
        ///         }
        ///         
        ///</remarks>
        [HttpPatch]
        [Authorize]
        [Route("{UserID}")]
        public async Task<dynamic> Update([FromBody] UserUpdateModel userUpdateModel)
        {
            string accessToken = Request.Headers[HeaderNames.Authorization];
            string token = accessToken.Split("Bearer ")[1];
            UserReadDTO userRead = TokenUtil.GetSubFromToken(token);

            if (ModelState.IsValid)
            {
                userRead.UserName = userUpdateModel.UserName;
                Services.Entities.CustomResponse updateUserInforTask = await _userService.UpdateUserInfor(userRead);

                if (updateUserInforTask.status)
                {
                    return new
                    {
                        status = true,
                        code = ReturnCodes.DataUpdateSucceeded,
                        message = updateUserInforTask.message,
                        data = updateUserInforTask.userReadDTO
                    };
                }
            }

            return new
            {
                status = false,
                code = ReturnCodes.DataUpdateFailed,
                message = "Có Lỗi Xảy Ra Khi Cố Gắng Cập Nhật Dữ Liệu"
            };
        }
        #endregion

        #region Update Password
        /// <summary>
        ///     Thay Đổi Mật Khẩu Người Dùng
        /// </summary>
        /// <remarks>
        /// 
        ///     Sample Request:
        ///     
        ///         Patch /api/User/ChangePassword
        ///         {
        ///             oldPassword: "abc...",
        ///             newPassword: "xyz...",
        ///         }
        ///         
        ///</remarks>
        [HttpPatch]
        [Authorize]
        public async Task<dynamic> ChangePassword([FromBody] UserChangePasswordModel userChangePasswordModel)
        {
            string accessToken = Request.Headers[HeaderNames.Authorization];
            string token = accessToken.Split("Bearer ")[1];
            UserReadDTO userRead = TokenUtil.GetSubFromToken(token);

            if (ModelState.IsValid)
            {
                Services.Entities.CustomResponse UpdatePassword_Task = await _userService.UpdatePassword(userRead.UserID, userChangePasswordModel.OldPassword, userChangePasswordModel.NewPassword);

                return new
                {
                    status = UpdatePassword_Task.status,
                    code = 200,
                    message = UpdatePassword_Task.message
                };
            }

            return new
            {
                status = false,
                code = ReturnCodes.DataUpdateFailed,
                message = "Có Lỗi Xảy Ra Khi Cố Gắng Thay Đổi Mật Khẩu"
            };
        }
        #endregion
    }
}
