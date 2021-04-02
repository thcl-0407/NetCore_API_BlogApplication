using DAL.Entities;
using Services.Entities;
using System.Threading.Tasks;
using DTO.ReadDTO;
using DTO.WriteDTO;

namespace Services
{
    public interface IUserService
    {
        Task<User> GetUser_EmailAsync(string Email);
        Task<CustomResponse> RegistryAsync(UserWriteDTO user);
        CustomResponse Login(string Email, string Password);
        Token GetToken(string UserID);
        Task<CustomResponse> AddToken(string UserID, TokenReadDTO tokenReadDTO);
        Task<CustomResponse> UpdateToken(string UserID, TokenReadDTO tokenReadDTO);
        Task<CustomResponse> UpdateUserInfor(UserReadDTO userReadDTO);
        Task<CustomResponse> UpdatePassword(string UserID, string OldPassword, string NewPassword);
    }
}