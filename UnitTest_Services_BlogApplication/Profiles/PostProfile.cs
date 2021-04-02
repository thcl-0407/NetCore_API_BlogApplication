using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DTO.ReadDTO;
using DTO.WriteDTO;
using Services.Entities;

namespace UnitTest_Services_BlogApplication.Profiles
{
    public class PostProfile : Profile
    {
        public PostProfile()
        {
            CreateMap<PostReadDTO, PostWriteDTO>();
            CreateMap<PostWriteDTO, PostReadDTO>();
        }
    }
}
