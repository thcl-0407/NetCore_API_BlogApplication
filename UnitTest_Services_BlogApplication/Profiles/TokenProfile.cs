using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DTO.ReadDTO;
using DTO.WriteDTO;
using DAL.Entities;

namespace UnitTest_Services_BlogApplication.Profiles
{
    public class TokenProfile : Profile
    {
        public TokenProfile()
        {
            CreateMap<TokenReadDTO, Token>();
            CreateMap<TokenReadDTO, TokenWriteDTO>();
            CreateMap<Token, TokenWriteDTO>();
            CreateMap<Token, TokenReadDTO>();
        }
    }
}
