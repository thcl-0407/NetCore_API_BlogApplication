using DTO.ReadDTO;
using DTO.WriteDTO;
using Services.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public interface IPostService
    {
        Task<CustomResponse> Create(PostWriteDTO postWrite);
        Task<List<PostReadDTO>> GetAll();
        Task<List<PostReadDTO>> GetPostsByTitle(string Key_Title);
        Task<string> GetBase64ImageAsync(int? PostID);
        Task<PostReadDTO> GetPost(int? PostID);
        Task<List<PostReadDTO>> GetPosts(string UserID);
        Task<CustomResponse> Remove(int postID, string UserID);
        Task<CustomResponse> Update(PostWriteDTO postWrite, string UserID);
    }
}