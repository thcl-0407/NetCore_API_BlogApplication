using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services;
using DAL;
using DAL.Entities;
using DTO.ReadDTO;
using DTO.WriteDTO;
using Utilities;
using UnitTest_Services_BlogApplication.Profiles;
using System;

namespace UnitTest_Services_BlogApplication.UnitTest_UserService
{
    [TestClass]
    public class UnitTest_GetToken_UserService
    {
        private BlogApplicationDbContext db;
        private UserService userService;

        public UnitTest_GetToken_UserService()
        {
            db = new BlogApplicationDbContext();

            var configuration = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new UserProfile());
            });

            userService = new UserService(db, new AutoMapper.Mapper(configuration));
        }

        #region Test Null Paremeter
        [TestMethod]
        public void GetToken_NullParameter_ThrowNullReferenceException()
        {
            //Arrange
            string UserId = null;

            //Assert
            Action action = new Action(() => {
                userService.GetToken(UserId);
            });

            //Assert
            Assert.ThrowsException<NullReferenceException>(action);
        }
        #endregion

        #region Test UserID Value Length = 0
        [TestMethod]
        public void GetToken_UserIDValueLengthEqual0_ThrowArgumentException()
        {
            //Arrange
            string UserId = " ";

            //Assert
            Action action = new Action(() => {
                userService.GetToken(UserId);
            });

            //Assert
            Assert.ThrowsException<ArgumentException>(action);
        }
        #endregion

        #region Test UserID Not Exist
        [TestMethod]
        public void GetToken_UserIDNotExist_Actual()
        {
            //Arrange
            var UserId = new Guid().ToString();

            //Assert
            var Token = userService.GetToken(UserId);

            //Assert
            Assert.AreEqual(null, Token);
        }
        #endregion

        #region Test UserID Exist But User isn't FirstTime Login
        [TestMethod]
        public void GetToken_UserIDExistNotLoginFirstTime_Actual()
        {
            //Arrange
            db.Users.RemoveRange(db.Users);

            User user = new User {
                UserID = new Guid(),
                UserName = "Glen",
                Email = "glen@icloud.com",
                HashPassword = BCryptUtil.HashPassword("123456"),
                isAuthenticated = false
            };

            db.Users.Add(user);
            db.SaveChanges();

            var UserId = user.UserID.ToString();

            //Assert
            var Token = userService.GetToken(UserId);

            //Assert
            Assert.AreEqual(null, Token);
        }
        #endregion
    }
}
