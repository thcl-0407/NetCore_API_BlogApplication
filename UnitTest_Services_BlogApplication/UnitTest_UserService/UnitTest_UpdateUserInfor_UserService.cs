using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services;
using DAL;
using DAL.Entities;
using DTO.ReadDTO;
using DTO.WriteDTO;
using Utilities;
using UnitTest_Services_BlogApplication.Profiles;
using System;
using Utilities.JWT;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace UnitTest_Services_BlogApplication.UnitTest_UserService
{
    [TestClass]
    public class UnitTest_UpdateUserInfor_UserService
    {
        private BlogApplicationDbContext db;
        private UserService userService;
        private TokenProviderOption tokenProviderOption;

        public UnitTest_UpdateUserInfor_UserService()
        {
            db = new BlogApplicationDbContext();

            var configuration = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new UserProfile());
                cfg.AddProfile(new TokenProfile());
            });

            userService = new UserService(db, new AutoMapper.Mapper(configuration));

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("blogapp!api!this_is@my@secret_key!"));

            tokenProviderOption = new TokenProviderOption
            {
                Audience = "ConsumerUser",
                Issuer = "BackEnd",
                SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256),
            };
        }

        #region Test Null Parameter
        [TestMethod]
        public void UpdateUserInfor_NullParameters_ActualFalse()
        {
            //Arrange
            UserReadDTO userReadDTO = null;

            Utilities.JWT.TokenResponse tokens = TokenUtil.GenerateTokens(userReadDTO, tokenProviderOption);

            DTO.ReadDTO.TokenReadDTO tokenReadDTO = new DTO.ReadDTO.TokenReadDTO
            {
                AccessToken = tokens.AccessToken,
                AccessTokenExpriesIn = tokens.AccessTokenExpiresIn,
                RefreshToken = tokens.RefreshToken,
                RefreshTokenExpriesIn = tokens.RefreshTokenExpiresIn,
            };

            //Act
            var status = userService.UpdateUserInfor(userReadDTO).Result.status;

            //Assert
            Assert.AreEqual(status, false);
        }
        #endregion

        #region Test Null UserID
        [TestMethod]
        public void UpdateUserInfor_NullUserID_ActualFalse()
        {
            //Arrange
            UserReadDTO userReadDTO = new UserReadDTO { 
                UserID = null,
                Email = "thcongloc0407@gmail.com",
                UserName = "thcongloc"
            };

            Utilities.JWT.TokenResponse tokens = TokenUtil.GenerateTokens(userReadDTO, tokenProviderOption);

            DTO.ReadDTO.TokenReadDTO tokenReadDTO = new DTO.ReadDTO.TokenReadDTO
            {
                AccessToken = tokens.AccessToken,
                AccessTokenExpriesIn = tokens.AccessTokenExpiresIn,
                RefreshToken = tokens.RefreshToken,
                RefreshTokenExpriesIn = tokens.RefreshTokenExpiresIn,
            };

            //Act
            var status = userService.UpdateUserInfor(userReadDTO).Result.status;

            //Assert
            Assert.AreEqual(status, false);
        }
        #endregion

        #region Test Null Email
        [TestMethod]
        public void UpdateUserInfor_NullEmail_ActualFalse()
        {
            //Arrange
            UserReadDTO userReadDTO = new UserReadDTO
            {
                UserID = Guid.NewGuid().ToString(),
                Email = null,
                UserName = "thcongloc"
            };

            Utilities.JWT.TokenResponse tokens = TokenUtil.GenerateTokens(userReadDTO, tokenProviderOption);

            DTO.ReadDTO.TokenReadDTO tokenReadDTO = new DTO.ReadDTO.TokenReadDTO
            {
                AccessToken = tokens.AccessToken,
                AccessTokenExpriesIn = tokens.AccessTokenExpiresIn,
                RefreshToken = tokens.RefreshToken,
                RefreshTokenExpriesIn = tokens.RefreshTokenExpiresIn,
            };

            //Act
            var status = userService.UpdateUserInfor(userReadDTO).Result.status;

            //Assert
            Assert.AreEqual(status, false);
        }
        #endregion

        #region Test Null UserName
        [TestMethod]
        public void UpdateUserInfor_NullUserName_ActualFalse()
        {
            //Arrange
            UserReadDTO userReadDTO = new UserReadDTO
            {
                UserID = Guid.NewGuid().ToString(),
                Email = "glen@icloud.com",
                UserName = null
            };

            Utilities.JWT.TokenResponse tokens = TokenUtil.GenerateTokens(userReadDTO, tokenProviderOption);

            DTO.ReadDTO.TokenReadDTO tokenReadDTO = new DTO.ReadDTO.TokenReadDTO
            {
                AccessToken = tokens.AccessToken,
                AccessTokenExpriesIn = tokens.AccessTokenExpiresIn,
                RefreshToken = tokens.RefreshToken,
                RefreshTokenExpriesIn = tokens.RefreshTokenExpiresIn,
            };

            //Act
            var status = userService.UpdateUserInfor(userReadDTO).Result.status;

            //Assert
            Assert.AreEqual(status, false);
        }
        #endregion

        #region Test User Is Not Exist
        [TestMethod]
        public void UpdateUserInfor_UserIsNotExist_ActualFalse()
        {
            //Arrange
            UserReadDTO userReadDTO = new UserReadDTO
            {
                UserID = Guid.NewGuid().ToString(),
                Email = "glen@icloud.com",
                UserName = "Glen"
            };

            Utilities.JWT.TokenResponse tokens = TokenUtil.GenerateTokens(userReadDTO, tokenProviderOption);

            DTO.ReadDTO.TokenReadDTO tokenReadDTO = new DTO.ReadDTO.TokenReadDTO
            {
                AccessToken = tokens.AccessToken,
                AccessTokenExpriesIn = tokens.AccessTokenExpiresIn,
                RefreshToken = tokens.RefreshToken,
                RefreshTokenExpriesIn = tokens.RefreshTokenExpiresIn,
            };

            //Act
            var status = userService.UpdateUserInfor(userReadDTO).Result.status;

            //Assert
            Assert.AreEqual(status, false);
        }
        #endregion

        #region Test User Update Success
        [TestMethod]
        public void UpdateUserInfor_Success_ActualFalse()
        {
            //Arrange
            db.Users.RemoveRange(db.Users);
            db.SaveChanges();

            User user = new User();
            user.UserID = Guid.NewGuid();
            user.UserName = "Glen";
            user.Email = "thcongloc0407@gmail.com";
            user.HashPassword = BCryptUtil.HashPassword("123456");
            user.isAuthenticated = false;

            db.Users.Add(user);
            db.SaveChanges();
            
            UserReadDTO userReadDTO = new UserReadDTO
            {
                UserID = user.UserID.ToString(),
                Email = "glen@icloud.com",
                UserName = "Glen"
            };

            Utilities.JWT.TokenResponse tokens = TokenUtil.GenerateTokens(userReadDTO, tokenProviderOption);

            DTO.ReadDTO.TokenReadDTO tokenReadDTO = new DTO.ReadDTO.TokenReadDTO
            {
                AccessToken = tokens.AccessToken,
                AccessTokenExpriesIn = tokens.AccessTokenExpiresIn,
                RefreshToken = tokens.RefreshToken,
                RefreshTokenExpriesIn = tokens.RefreshTokenExpiresIn,
            };

            //Act
            var status = userService.UpdateUserInfor(userReadDTO).Result.status;

            //Assert
            Assert.AreEqual(status, true);
        }
        #endregion
    }
}
