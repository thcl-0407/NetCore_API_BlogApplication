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
    public class UnitTest_UpdateToken_UserService
    {
        private BlogApplicationDbContext db;
        private UserService userService;
        private TokenProviderOption tokenProviderOption;

        public UnitTest_UpdateToken_UserService()
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
        public void UpdateToken_NullParameter_ActualFalse()
        {
            //Arrange
            string UserID = null;
            TokenReadDTO tokenReadDTO = null;

            //Act
            var status = userService.UpdateToken(UserID, tokenReadDTO).Result.status;

            //Assert
            Assert.AreEqual(false, status);
        }
        #endregion

        #region Test User ID Value Length Equal 0
        [TestMethod]
        public void UpdateToken_UserIDValueLengthEqual0_ActualFalse()
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
            var status = userService.UpdateToken(UserID, tokenReadDTO).Result.status;

            //Assert
            Assert.AreEqual(false, status);
        }
        #endregion

        #region Test Token Not Exist
        [TestMethod]
        public void UpdateToken_TokenNotExist_ActualFalse()
        {
            //Arrange
            db.Users.RemoveRange(db.Users);
            db.Tokens.RemoveRange(db.Tokens);

            string UserID = Guid.NewGuid().ToString();
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

            User user = new User
            {
                UserID = new Guid(UserID),
                Email = "glen@icloud.com",
                UserName = "Glen",
                HashPassword = BCryptUtil.HashPassword("123456"),
                isAuthenticated = false
            };

            db.Users.Add(user);
            db.SaveChanges();

            Token token = new Token
            {
                AccessToken = tokenReadDTO.AccessToken,
                AccessTokenExpriesIn = tokenReadDTO.AccessTokenExpriesIn,
                RefreshToken = tokenReadDTO.RefreshToken,
                RefreshTokenExpriesIn = tokenReadDTO.RefreshTokenExpriesIn,
                UserID = user.UserID
            };

            db.Tokens.Add(token);
            db.SaveChanges();

            //Act
            var status = userService.UpdateToken(Guid.NewGuid().ToString(), tokenReadDTO).Result.status;

            //Assert
            Assert.AreEqual(false, status);
        }
        #endregion

        #region Test Update Token Success
        [TestMethod]
        public void UpdateToken_Success_ActualFalse()
        {
            //Arrange
            db.Users.RemoveRange(db.Users);
            db.Tokens.RemoveRange(db.Tokens);

            string UserID = Guid.NewGuid().ToString();
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

            User user = new User
            {
                UserID = new Guid(UserID),
                Email = "glen@icloud.com",
                UserName = "Glen",
                HashPassword = BCryptUtil.HashPassword("123456"),
                isAuthenticated = false
            };

            db.Users.Add(user);
            db.SaveChanges();

            Token token = new Token
            {
                AccessToken = tokenReadDTO.AccessToken,
                AccessTokenExpriesIn = tokenReadDTO.AccessTokenExpriesIn,
                RefreshToken = tokenReadDTO.RefreshToken,
                RefreshTokenExpriesIn = tokenReadDTO.RefreshTokenExpriesIn,
                UserID = user.UserID
            };
            db.Tokens.Add(token);
            db.SaveChanges();

            //Act
            var status = userService.UpdateToken(UserID, tokenReadDTO).Result.status;

            //Assert
            Assert.AreEqual(true, status);
        }
        #endregion
    }
}
