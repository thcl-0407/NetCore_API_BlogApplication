using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DTO.ReadDTO;
using DTO.WriteDTO;
using Services.Entities;
using API_BlogApplication.Models;

namespace API_BlogApplication.Profiles
{
    public class PostProfile : Profile
    {
        public PostProfile()
        {
            CreateMap<PostReadDTO, PostWriteDTO>();
            CreateMap<PostWriteDTO, PostReadDTO>();
            CreateMap<PostReadDTO, PostModel>();
        }
    }
}
