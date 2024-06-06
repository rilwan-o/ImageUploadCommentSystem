using Application.Features.Posts.Commands;
using Application.Features.Posts.Queries;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Post, PostVm>().ReverseMap();
            CreateMap<CreatePostCommand, Post>().ReverseMap();
            CreateMap<AddCommentCommand, Comment>().ReverseMap();


        }
    }
}
