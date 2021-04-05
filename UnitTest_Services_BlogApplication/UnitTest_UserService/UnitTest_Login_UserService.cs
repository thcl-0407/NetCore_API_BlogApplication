using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services;
using DAL;
using DAL.Entities;
using DTO.ReadDTO;
using DTO.WriteDTO;
using Utilities;
using System;
using UnitTest_Services_BlogApplication.Profiles;

namespace UnitTest_Services_BlogApplication.UnitTest_UserService
{
    [TestClass]
    public class UnitTest_Login_UserService
    {
        private BlogApplicationDbContext db;
        private UserService userService;

        public UnitTest_Login_UserService()
        {
            db = new BlogApplicationDbContext();

            var configuration = new AutoMapper.MapperConfiguration(cfg => {
                cfg.AddProfile(new UserProfile());
            });

            userService = new UserService(db, new AutoMapper.Mapper(configuration));
        }

        #region Test Email Null Reference
        [TestMethod]
        public void Login_EmailNullReference_ActualFalse()
        {
            //Arrange
            User user = new User
            {
                UserID = new System.Guid(),
                UserName = "Glen",
                Email = "glen@icloud.com",
                HashPassword = BCryptUtil.HashPassword("123456"),
                isAuthenticated = false
            };

            db.Users.RemoveRange(db.Users);

            db.Users.Add(user);
            db.SaveChanges();

            string Email = null;
            string Password = "123456";

            //Act
            var status = userService.Login(Email, Password).status;

            //Assert
            Assert.AreEqual(false, status);
        }
        #endregion

        #region Test Password Null Reference
        [TestMethod]
        public void Login_PasswordNullReference_ActualFalse()
        {
            //Arrange
            User user = new User
            {
                UserID = new System.Guid(),
                UserName = "Glen",
                Email = "glen@icloud.com",
                HashPassword = BCryptUtil.HashPassword("123456"),
                isAuthenticated = false
            };

            db.Users.RemoveRange(db.Users);

            db.Users.Add(user);
            db.SaveChanges();

            string Email = "glen@icloud.com";
            string Password = null;

            //Act
            var status = userService.Login(Email, Password).status;

            //Assert
            Assert.AreEqual(status, false);
        }
        #endregion

        #region Test Email Value Length = 0
        [TestMethod]
        public void Login_EmailValueLengthEquals0_ActualFalse()
        {
            //Arrange
            User user = new User
            {
                UserID = new System.Guid(),
                UserName = "Glen",
                Email = "glen@icloud.com",
                HashPassword = BCryptUtil.HashPassword("123456"),
                isAuthenticated = false
            };

            db.Users.RemoveRange(db.Users);

            db.Users.Add(user);
            db.SaveChanges();

            string Email = "";
            string Password = "123456";

            //Act
            var status = userService.Login(Email, Password).status;

            //Assert
            Assert.AreEqual(status, false);
        }
        #endregion

        #region Test Password Value Length = 0
        [TestMethod]
        public void Login_PasswordValueLengthEquals0_ActualFalse()
        {
            //Arrange
            User user = new User
            {
                UserID = new System.Guid(),
                UserName = "Glen",
                Email = "glen@icloud.com",
                HashPassword = BCryptUtil.HashPassword("123456"),
                isAuthenticated = false
            };

            db.Users.RemoveRange(db.Users);

            db.Users.Add(user);
            db.SaveChanges();

            string Email = "glen@icloud.com";
            string Password = "";

            //Act
            var status = userService.Login(Email, Password).status;

            //Assert
            Assert.AreEqual(status, false);
        }
        #endregion

        #region Test Email is not exist
        [TestMethod]
        public void Login_EmailIsNotExist_ActualFalse()
        {
            //Arrange
            User user = new User
            {
                UserID = new System.Guid(),
                UserName = "Glen",
                Email = "glen@icloud.com",
                HashPassword = BCryptUtil.HashPassword("123456"),
                isAuthenticated = false
            };

            db.Users.RemoveRange(db.Users);

            db.Users.Add(user);
            db.SaveChanges();

            string Email = "abc@icloud.com";
            string Password = "123456";

            //Act
            var status = userService.Login(Email, Password).status;

            //Assert
            Assert.AreEqual(status, false);
        }
        #endregion

        #region Test Password Wrong
        [TestMethod]
        public void Login_PasswordWrong_ActualFalse()
        {
            //Arrange
            User user = new User
            {
                UserID = new System.Guid(),
                UserName = "Glen",
                Email = "glen@icloud.com",
                HashPassword = BCryptUtil.HashPassword("123456"),
                isAuthenticated = false
            };

            db.Users.RemoveRange(db.Users);

            db.Users.Add(user);
            db.SaveChanges();

            string Email = "glen@icloud.com";
            string Password = "1234567";

            //Act
            var status = userService.Login(Email, Password).status;

            //Assert
            Assert.AreEqual(status, false);
        }
        #endregion

        #region Test Login Success
        [TestMethod]
        public void Login_Success_ActualTrue()
        {
            //Arrange
            User user = new User
            {
                UserID = new System.Guid(),
                UserName = "Glen",
                Email = "glen@icloud.com",
                HashPassword = BCryptUtil.HashPassword("123456"),
                isAuthenticated = false
            };

            db.Users.RemoveRange(db.Users);

            db.Users.Add(user);
            db.SaveChanges();

            var Email = "glen@icloud.com";
            var Password = "123456";

            //Act
            var status = userService.Login(Email, Password).status;

            //Assert
            Assert.AreEqual(status, true);
        }
        #endregion
    }
}
