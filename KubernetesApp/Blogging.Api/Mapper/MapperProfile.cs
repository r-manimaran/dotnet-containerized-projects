using AutoMapper;
using Blogging.Api.Dtos;
using Blogging.Api.Models;

namespace Blogging.Api.Mapper;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        // Map from Post to PostResponse
        CreateMap<Post, PostResponse>()
            .ForMember(dest => dest.Category, opt=> opt.MapFrom(src=>src.Category.Name))
            .ForMember(dest => dest.CreatedOn, opt=> opt.MapFrom(src => src.CreatedDate));

        // Reverse Map: Map from PostResponse to Post
        CreateMap<PostResponse, Post>()
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedOn))
            .ForMember(dest => dest.Category, opt => opt.Ignore())
            .ForMember(dest => dest.CategoryId, opt => opt.Ignore());

        // Mapping for CreatePostRequest to Post
        CreateMap<CreatePostRequest, Post>()
            .ForMember(dest=>dest.CreatedDate, opt=>opt.MapFrom(_=> DateTime.UtcNow));
    }
}
