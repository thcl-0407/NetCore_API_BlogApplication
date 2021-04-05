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
    public class UnitTest_AddToken_UserService
    {
        private BlogApplicationDbContext db;
        private UserService userService;
        private TokenProviderOption tokenProviderOption;

        public UnitTest_AddToken_UserService()
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

        #region Test Null UserID
        [TestMethod]
        public void AddToken_NullUserID_ActualFalse()
        {
            //Arrange
            string UserID = null;
            UserReadDTO userReadDTO = new UserReadDTO
            {
                UserID = UserID,
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
            var status = userService.AddToken(UserID, tokenReadDTO).Result.status;

            //Assert
            Assert.AreEqual(status, false);
        }
        #endregion

        #region Test Null Parameter
        [TestMethod]
        public void AddToken_NullParameter_ActualFalse()
        {
            //Arrange
            string UserID = null;
            //UserReadDTO userReadDTO = new UserReadDTO
            //{
            //    UserID = UserID,
            //    Email = "glen@icloud.com",
            //    UserName = "Glen"
            //};

            //Utilities.JWT.TokenResponse tokens = TokenUtil.GenerateTokens(userReadDTO, tokenProviderOption);

            DTO.ReadDTO.TokenReadDTO tokenReadDTO = null;

            //Act
            var status = userService.AddToken(UserID, tokenReadDTO).Result.status;

            //Assert
            Assert.AreEqual(status, false);
        }
        #endregion

        #region Test UserID Value Length = 0
        [TestMethod]
        public void AddToken_UserIDValueLengthEqual0_ActualFalse()
        {
            //Arrange
            string UserID = "";
            UserReadDTO userReadDTO = new UserReadDTO
            {
                UserID = UserID,
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
            var status = userService.AddToken(UserID, tokenReadDTO).Result.status;

            //Assert
            Assert.AreEqual(status, false);
        }
        #endregion

        #region Test Successful
        [TestMethod]
        public void AddToken_Successful_ActualTrue()
        {
            //Arrange
            db.Users.RemoveRange(db.Users);
            
            User user = new User();
            user.UserID = new Guid();
            user.UserName = "Glen";
            user.HashPassword = BCryptUtil.HashPassword("123456");
            user.Email = "glen@icloud.com";
            user.isAuthenticated = false;

            db.Users.Add(user);
            db.SaveChanges();

            UserReadDTO userReadDTO = new UserReadDTO
            {
                UserID = user.UserID.ToString(),
                Email = user.Email,
                UserName = user.UserName
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
            var status = userService.AddToken(user.UserID.ToString(), tokenReadDTO).Result.status;

            //Assert
            Assert.AreEqual(status, true);
        }
        #endregion
    }
}
