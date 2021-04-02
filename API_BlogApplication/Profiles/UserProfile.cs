using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DAL.Entities;
using DTO.WriteDTO;
using DTO.ReadDTO;

namespace API_BlogApplication.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserReadDTO, User>();
            CreateMap<User, UserReadDTO>();
            CreateMap<User, UserWriteDTO>().ForMember(uw => uw.Password, options => options.MapFrom(u => u.HashPassword));
            CreateMap<UserWriteDTO, User>().ForMember(u => u.HashPassword, options => options.MapFrom(uw => uw.Password));
        }
    }
}
