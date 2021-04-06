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
    public class UnitTest_UpdatePassword_UserService
    {
        private BlogApplicationDbContext db;
        private UserService userService;
        private TokenProviderOption tokenProviderOption;

        public UnitTest_UpdatePassword_UserService()
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

        #region Test Null User ID
        [TestMethod]
        public void UpdatePassword_NullUserID_ActualFalse() {
            //Arrange
            string UserID = null;
            string OldPassword = "123456";
            string NewPassword = "12345678";

            //Act
            var status = userService.UpdatePassword(UserID, OldPassword, NewPassword).Result.status;

            //Assert
            Assert.AreEqual(false, status);
        }
        #endregion

        #region Test Null Old Password
        [TestMethod]
        public void UpdatePassword_NullOldPassword_ActualFalse()
        {
            //Arrange
            string UserID = Guid.NewGuid().ToString();
            string OldPassword = null;
            string NewPassword = "12345678";

            //Act
            var status = userService.UpdatePassword(UserID, OldPassword, NewPassword).Result.status;

            //Assert
            Assert.AreEqual(false, status);
        }
        #endregion

        #region Test Null New Password
        [TestMethod]
        public void UpdatePassword_NullNewPassword_ActualFalse()
        {
            //Arrange
            string UserID = Guid.NewGuid().ToString();
            string OldPassword = "123456";
            string NewPassword = null;

            //Act
            var status = userService.UpdatePassword(UserID, OldPassword, NewPassword).Result.status;

            //Assert
            Assert.AreEqual(false, status);
        }
        #endregion

        #region Test User ID Empty
        [TestMethod]
        public void UpdatePassword_UserIDEmpty_ActualFalse()
        {
            //Arrange
            string UserID = "";
            string OldPassword = "123456";
            string NewPassword = "1234567";

            //Act
            var status = userService.UpdatePassword(UserID, OldPassword, NewPassword).Result.status;

            //Assert
            Assert.AreEqual(false, status);
        }
        #endregion

        #region Test Old Password Empty
        [TestMethod]
        public void UpdatePassword_OldPasswordEmpty_ActualFalse()
        {
            //Arrange
            string UserID = Guid.NewGuid().ToString();
            string OldPassword = "";
            string NewPassword = "1234567";

            //Act
            var status = userService.UpdatePassword(UserID, OldPassword, NewPassword).Result.status;

            //Assert
            Assert.AreEqual(false, status);
        }
        #endregion

        #region Test New Password Empty
        [TestMethod]
        public void UpdatePassword_NewPasswordEmpty_ActualFalse()
        {
            //Arrange
            string UserID = Guid.NewGuid().ToString();
            string OldPassword = "123456";
            string NewPassword = "";

            //Act
            var status = userService.UpdatePassword(UserID, OldPassword, NewPassword).Result.status;

            //Assert
            Assert.AreEqual(false, status);
        }
        #endregion

        #region Test Users Not Exist
        [TestMethod]
        public void UpdatePassword_UserNotExist_ActualFalse()
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

            string UserID = Guid.NewGuid().ToString();
            string OldPassword = "123456789";
            string NewPassword = "123456";

            //Act
            var status = userService.UpdatePassword(UserID, OldPassword, NewPassword).Result.status;

            //Assert
            Assert.AreEqual(false, status);
        }
        #endregion

        #region Test Wrong Old Password
        [TestMethod]
        public void UpdatePassword_WrongOldPassword_ActualFalse()
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

            string UserID = user.UserID.ToString();
            string OldPassword = "123456789";
            string NewPassword = "123456";

            //Act
            var status = userService.UpdatePassword(UserID, OldPassword, NewPassword).Result.status;

            //Assert
            Assert.AreEqual(false, status);
        }
        #endregion

        #region Test Success
        [TestMethod]
        public void UpdatePassword_Success_ActualFalse()
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

            string UserID = user.UserID.ToString();
            string OldPassword = "123456";
            string NewPassword = "123456789";

            //Act
            var status = userService.UpdatePassword(UserID, OldPassword, NewPassword).Result.status;

            //Assert
            Assert.AreEqual(true, status);
        }
        #endregion
    }
}
