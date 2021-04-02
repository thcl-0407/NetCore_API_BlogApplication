using System;
using System.Collections.Generic;
using System.Text;
using DTO.ReadDTO;

namespace Services.Entities
{
    public class CustomResponse
    {
        public CustomResponse(bool status, string message)
        {
            this.status = status;
            this.message = message;
        }

        public CustomResponse(UserReadDTO userReadDTO, bool status, string message)
        {
            this.userReadDTO = userReadDTO;
            this.status = status;
            this.message = message;
        }

        public CustomResponse(CommentReadDTO commentReadDTO, bool status, string message)
        {
            this.commentReadDTO = commentReadDTO;
            this.status = status;
            this.message = message;
        }

        public UserReadDTO userReadDTO { get; set; }
        public CommentReadDTO commentReadDTO { get; set; }
        public bool status { get; set; }
        public string message { get; set; }
    }
}
