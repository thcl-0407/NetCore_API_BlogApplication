using DTO.ReadDTO;
using DTO.WriteDTO;
using Services.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public interface ICommentService
    {
        Task<CustomResponse> Create(CommentWriteDTO commentWrite);
        Task<List<CommentReadDTO>> GetComments(int PostID);
        Task<CustomResponse> Remove(string CommentID, string UserID);
    }
}