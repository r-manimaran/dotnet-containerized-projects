using AppreciateAppApi.DTO.Appreciation;
using AppreciateAppApi.DTO;
using AppreciateAppApi.Models;
using AutoMapper;

namespace AppreciateAppApi.MappingProfile;

public class AppreciationProfile : Profile
{
    public AppreciationProfile()
    {
        CreateMap<AppreciationItem, Item>().ReverseMap();   
        CreateMap<CreateCategoryRequest, Category>().ReverseMap();
    }
}

