using AppreciateAppApi.DTO.Appreciation;
using AppreciateAppApi.DTO;
using AppreciateAppApi.Models;
using AutoMapper;

namespace AppreciateAppApi.MappingProfile;

public class AppreciationProfile : Profile
{
    public AppreciationProfile()
    {
        CreateMap<AppreciationItem, Item>();
        CreateMap<Item, AppreciationItem>()
            .ForMember(dest => dest.AppreciationType, opt =>
                    opt.MapFrom((src, dest, destMember, context) =>
                        context.Items.ContainsKey("AppreciationType")
                         ? (AppreciationType)context.Items["AppreciationType"]
                         : AppreciationType.Received));
        CreateMap<CreateCategoryRequest, Category>().ReverseMap();
        CreateMap<Item, Appreciation>();
        
        // Map Id from Employee to EmployeeId in sender
        CreateMap<Employee, Sender>().ForMember(dest=> dest.EmployeeId,
                opt => opt.MapFrom(src=>src.Id));

        CreateMap<Employee, Receiver>().ForMember(dest => dest.EmployeeId,
                opt => opt.MapFrom(src => src.Id));

    }
}

