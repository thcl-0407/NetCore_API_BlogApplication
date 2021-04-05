using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services;
using DAL;
using DAL.Entities;
using DTO.ReadDTO;
using DTO.WriteDTO;
using Utilities;
using UnitTest_Services_BlogApplication.Profiles;

namespace UnitTest_Services_BlogApplication.UnitTest_UserService
{
    [TestClass]
    public class UnitTest_Registry_UserService
    {
        private BlogApplicationDbContext db;
        private UserService userService;

        public UnitTest_Registry_UserService()
        {
            db = new BlogApplicationDbContext();

            var configuration = new AutoMapper.MapperConfiguration(cfg => {
                cfg.AddProfile(new UserProfile());
            });

            userService = new UserService(db, new AutoMapper.Mapper(configuration));
        }

        #region Test Null Parameter
        [TestMethod]
        public void Registry_UserWriteDTONullReference_ActualFalse()
        {
            //Arrange
            UserWriteDTO user = null;

            //Act
            var status = userService.RegistryAsync(user).Result.status;

            //Assert
            Assert.AreEqual(status, false);
        }
        #endregion

        #region Test UserName is Null
        [TestMethod]
        public void Registry_UserNameNull_ActualFalse()
        {
            //Arrange
            UserWriteDTO user = new UserWriteDTO {
                UserID = new System.Guid(),
                UserName = null,
                Email = "glen@icloud.com",
                Password = "123456",
                isAuthenticated = false
            };

            //Act
            var status = userService.RegistryAsync(user).Result.status;

            //Assert
            Assert.AreEqual(status, false);
        }
        #endregion

        #region Test Email is Null
        [TestMethod]
        public void Registry_EmailNull_ActualFalse()
        {
            //Arrange
            UserWriteDTO user = new UserWriteDTO
            {
                UserID = new System.Guid(),
                UserName = "Glen",
                Email = null,
                Password = "123456",
                isAuthenticated = false
            };

            //Act
            var status = userService.RegistryAsync(user).Result.status;

            //Assert
            Assert.AreEqual(status, false);
        }
        #endregion

        #region Test Password is Null
        [TestMethod]
        public void Registry_PasswordNull_ActualFalse()
        {
            //Arrange
            UserWriteDTO user = new UserWriteDTO
            {
                UserID = new System.Guid(),
                UserName = "Glen",
                Email = "glen@icloud.com",
                Password = null,
                isAuthenticated = false
            };

            //Act
            var status = userService.RegistryAsync(user).Result.status;

            //Assert
            Assert.AreEqual(status, false);
        }
        #endregion

        #region Test UserName value length = 0
        [TestMethod]
        public void Registry_UserNameValueLengthEquals0_ActualFalse()
        {
            //Arrange
            UserWriteDTO user = new UserWriteDTO
            {
                UserID = new System.Guid(),
                UserName = "",
                Email = "glen@icloud.com",
                Password = "123456",
                isAuthenticated = false
            };

            //Act
            var status = userService.RegistryAsync(user).Result.status;

            //Assert
            Assert.AreEqual(status, false);
        }
        #endregion

        #region Test Email value length = 0
        [TestMethod]
        public void Registry_EmailValueLengthEquals0_ActualFalse()
        {
            //Arrange
            UserWriteDTO user = new UserWriteDTO
            {
                UserID = new System.Guid(),
                UserName = "Glen",
                Email = "",
                Password = "123456",
                isAuthenticated = false
            };

            //Act
            var status = userService.RegistryAsync(user).Result.status;

            //Assert
            Assert.AreEqual(status, false);
        }
        #endregion

        #region Test Password value length = 0
        [TestMethod]
        public void Registry_PasswordValueLengthEquals0_ActualFalse()
        {
            //Arrange
            UserWriteDTO user = new UserWriteDTO
            {
                UserID = new System.Guid(),
                UserName = "Glen",
                Email = "glen@icloud.com",
                Password = "",
                isAuthenticated = false
            };

            //Act
            var status = userService.RegistryAsync(user).Result.status;

            //Assert
            Assert.AreEqual(status, false);
        }
        #endregion

        #region Test Account is Exist
        [TestMethod]
        public void Registry_AccountIsExist_ActualFalse()
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

            db.Users.Add(user);
            db.SaveChanges();

            UserWriteDTO userWriteDTO = new UserWriteDTO
            {
                UserID = new System.Guid(),
                UserName = "Glen",
                Email = "glen@icloud.com",
                Password = BCryptUtil.HashPassword("123456"),
                isAuthenticated = false
            };

            //Act
            var status = userService.RegistryAsync(userWriteDTO).Result.status;

            //Assert
            Assert.AreEqual(status, false);
        }
        #endregion

        #region Test Registry Success
        [TestMethod]
        public void Registry_Success_ActualTrue()
        {
            //Arrange
            db.Users.RemoveRange(db.Users);
            db.SaveChanges();

            UserWriteDTO userWriteDTO = new UserWriteDTO
            {
                UserID = new System.Guid(),
                UserName = "Glen",
                Email = "glen@icloud.com",
                Password = BCryptUtil.HashPassword("123456"),
                isAuthenticated = false
            };

            //Act
            var status = userService.RegistryAsync(userWriteDTO).Result.status;

            //Assert
            Assert.AreEqual(status, true);
        }
        #endregion
    }
}
