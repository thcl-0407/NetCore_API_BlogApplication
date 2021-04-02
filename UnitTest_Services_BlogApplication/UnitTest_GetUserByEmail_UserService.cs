using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services;
using DAL;
using DAL.Entities;
using DTO.ReadDTO;
using DTO.WriteDTO;
using Utilities;
using UnitTest_Services_BlogApplication.Profiles;
using System;

namespace UnitTest_Services_BlogApplication
{
    [TestClass]
    public class UnitTest_GetUserByEmail_UserService
    {
        private BlogApplicationDbContext db;
        private UserService userService;

        public UnitTest_GetUserByEmail_UserService()
        {
            db = new BlogApplicationDbContext();

            var configuration = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new UserProfile());
            });

            userService = new UserService(db, new AutoMapper.Mapper(configuration));
        }

        #region Test Null Parameter
        [TestMethod]
        public void GetUserByEmail_NullParameter_ActualThrowNullReferenceException()
        {
            //Arrange
            string Email = null;

            //Act
            var user = userService.GetUser_EmailAsync(Email);

            //Assert
            Assert.ThrowsExceptionAsync<NullReferenceException>(() => userService.GetUser_EmailAsync(Email));
        }
        #endregion

        #region Test Email value length = 0
        [TestMethod]
        public void GetUserByEmail_EmailValueLengthEquals0_ActualNull()
        {
            //Arrange
            var Email = "";

            //Act
            var user = userService.GetUser_EmailAsync(Email);

            //Assert
            Assert.ThrowsExceptionAsync<ArgumentException>(() => userService.GetUser_EmailAsync(Email));
        }
        #endregion

        #region Test Email Invalid Format
        [TestMethod]
        public void GetUserByEmail_EmailInvalidFormat_ActualNull()
        {
            //Arrange
            var Email = "abc*321sd`-1";

            //Act
            var user = userService.GetUser_EmailAsync(Email);

            //Assert
            Assert.ThrowsExceptionAsync<ArgumentException>(() => userService.GetUser_EmailAsync(Email));
        }
        #endregion
    }
}
