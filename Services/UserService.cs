using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Utilities;
using DAL;
using DAL.Entities;
using Services.Entities;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using DTO.ReadDTO;
using DTO.WriteDTO;

namespace Services
{
    public class UserService : IUserService
    {
        private readonly BlogApplicationDbContext db;
        private readonly IMapper _mapper;

        public UserService(BlogApplicationDbContext dbContext, IMapper mapper)
        {
            db = dbContext;
            _mapper = mapper;
        }

        /*Tạo Tài Khoản Người Dùng*/
        public async Task<CustomResponse> RegistryAsync(UserWriteDTO user)
        {
            //Check User parameter is Null
            if (user == null)
            {
                return new CustomResponse(false, "User is Null");
            }

            //Check Value in User's Object is Null
            if (user.isValueNull())
            {
                return new CustomResponse(false, "Dữ Liệu Không Hợp Lệ");
            }

            //Check UserName value Length == 0
            if (user.UserName.Trim().Length == 0)
            {
                return new CustomResponse(false, "Dữ Liệu Không Hợp Lệ");
            }

            //Check Email, Password invalid type format
            if (!CValidUtil.isValidEmail(user.Email) || CValidUtil.isValidPassword(user.Password))
            {
                return new CustomResponse(false, "Dữ Liệu Không Hợp Lệ");
            }

            try
            {
                User user_mapper = _mapper.Map<User>(user);

                if (GetUser_EmailAsync(user_mapper.Email).Result == null)
                {
                    await db.Users.AddAsync(user_mapper);
                    await db.SaveChangesAsync();

                    return new CustomResponse(true, "Tạo Tài Khoản Thành Công");
                }
            }
            catch (Exception e)
            {
                return new CustomResponse(false, e.Message);
            }

            return new CustomResponse(false, "Tài Khoản Đã Tồn Tại");
        }

        /*Lấy Thông Tin Người Dùng Với Email*/
        public async Task<User> GetUser_EmailAsync(string Email)
        {
            if(Email == null)
            {
                throw new NullReferenceException();
            }

            if (!CValidUtil.isValidEmail(Email))
            {
                throw new ArgumentException();
            }

            return await db.Users.Where(u => u.Email.Equals(Email)).FirstOrDefaultAsync();
        }

        /*Đăng Nhập*/
        public CustomResponse Login(string Email, string Password)
        {
            if(Email == null || Password == null)
            {
                return new CustomResponse(false, "Dữ Liệu Không Hợp Lệ");
            }

            if (Email.Length <= 0 || Password.Length <= 0)
            {
                return new CustomResponse(false, "Dữ Liệu Không Hợp Lệ");
            }

            try
            {
                User UserResult = GetUser_EmailAsync(Email).Result;

                if (UserResult != null)
                {
                    if (BCryptUtil.VerifyPassword(Password, UserResult.HashPassword))
                    {
                        return new CustomResponse(_mapper.Map<UserReadDTO>(UserResult), true, "Đăng Nhập Thành Công");
                    }
                }
            }
            catch (Exception e)
            {
                return new CustomResponse(false, e.Message);
            }

            return new CustomResponse(false, "Đăng Nhập Thất Bại");
        }

        /*Get Token's Access User*/
        public Token GetToken(string UserID)
        {
            if (UserID == null)
            {
                throw new NullReferenceException();
            }

            if(UserID.Trim().Length == 0)
            {
                throw new ArgumentException();
            }

            return db.Tokens.Where(t => t.UserID.Equals(new Guid(UserID))).FirstOrDefault();
        }

        /*Add Token For User*/
        public async Task<CustomResponse> AddToken(string UserID, TokenReadDTO tokenReadDTO)
        {
            try
            {
                Token token = _mapper.Map<Token>(tokenReadDTO);
                token.UserID = new Guid(UserID);

                await db.Tokens.AddAsync(token);
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return new CustomResponse(false, e.Message);
            }

            return new CustomResponse(true, "Thêm Token Thành Công");
        }

        /*Update Current Token*/
        public async Task<CustomResponse> UpdateToken(string UserID, TokenReadDTO tokenReadDTO)
        {
            try
            {
                Token token = GetToken(UserID);

                if (token != null)
                {
                    if(tokenReadDTO == null)
                    {
                        return new CustomResponse(false, "TokenReadDTO is Null");
                    }

                    if (tokenReadDTO.isNullValue())
                    {
                        return new CustomResponse(false, "TokenReadDTO Value is Null");
                    }

                    token.AccessToken = tokenReadDTO.AccessToken;
                    token.AccessTokenExpriesIn = tokenReadDTO.AccessTokenExpriesIn;
                    token.RefreshToken = tokenReadDTO.RefreshToken;
                    token.RefreshTokenExpriesIn = tokenReadDTO.RefreshTokenExpriesIn;

                    await db.SaveChangesAsync();
                }
                else
                {
                    return new CustomResponse(false, "User ID Not Exist");
                }
            }
            catch (Exception e)
            {
                return new CustomResponse(false, e.Message);
            }

            return new CustomResponse(true, "Thêm Token Thành Công");
        }

        /*Update User Infor*/
        public async Task<CustomResponse> UpdateUserInfor(UserReadDTO userReadDTO)
        {
            if(userReadDTO == null)
            {
                return new CustomResponse(false, "User Read DTO is null");
            }

            if (userReadDTO.isValueNull())
            {
                return new CustomResponse(false, "User Read DTO value is null");
            }

            try
            {
                User userWillUpdate = db.Users.Where(u => u.UserID.Equals(new Guid(userReadDTO.UserID))).FirstOrDefault();

                if (userWillUpdate != null)
                {
                    userWillUpdate.UserName = userReadDTO.UserName;

                    await db.SaveChangesAsync();
                }
                else
                {
                    return new CustomResponse(false, "User is not exist");
                }
            }
            catch (Exception e)
            {
                return new CustomResponse(false, e.Message);
            }

            return new CustomResponse(userReadDTO, true, "Update Thành Công");
        }

        /*Update Password*/
        public async Task<CustomResponse> UpdatePassword(string UserID, string OldPassword, string NewPassword)
        {
            if(UserID == null || UserID.Trim().Length == 0)
            {
                return new CustomResponse(false, "User ID invalid");
            }

            if (OldPassword == null || OldPassword.Trim().Length == 0)
            {
                return new CustomResponse(false, "Old Password invalid");
            }

            if (NewPassword == null || NewPassword.Trim().Length == 0)
            {
                return new CustomResponse(false, "New Password invalid");
            }

            try
            {
                User userWillUpdate = db.Users.Where(u => u.UserID.Equals(new Guid(UserID))).FirstOrDefault();

                if (userWillUpdate != null)
                {
                    if(BCryptUtil.VerifyPassword(OldPassword, userWillUpdate.HashPassword))
                    {
                        userWillUpdate.HashPassword = BCryptUtil.HashPassword(NewPassword);

                        await db.SaveChangesAsync();
                    }
                    else
                    {
                        return new CustomResponse(false, "Mật Khẩu Không Chính Xác");
                    }
                }
                else
                {
                    return new CustomResponse(false, "User Not Exist");
                }
            }
            catch (Exception e)
            {
                return new CustomResponse(false, e.Message);
            }

            return new CustomResponse(true, "Update Mật Khẩu Thành Công");
        }
    }
}
