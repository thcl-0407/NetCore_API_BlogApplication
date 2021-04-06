using System;

namespace DTO.ReadDTO
{
    public class UserReadDTO
    {
        public UserReadDTO(string userID, string userName, string email)
        {
            UserID = userID;
            UserName = userName;
            Email = email;
        }

        public UserReadDTO(){}

        public string UserID { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }

        public bool isValueNull()
        {
            if(this.UserID == null)
            {
                return true;
            }
            if (this.UserName == null)
            {
                return true;
            }
            if (this.Email == null)
            {
                return true;
            }

            return false;
        }
    }
}
