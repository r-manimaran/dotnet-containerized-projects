using AppreciateAppApi.DTO;
using AppreciateAppApi.DTO.Appreciation;
using AppreciateAppApi.Models;
using AppreciateAppApi.Pagination;

namespace AppreciateAppApi.Services;

public interface IAppreciationService
{
    Task<BaseResponse<Appreciation?>> CreateAppreciationAsync(CreateAppreciationRequest request);
    Task<BaseResponse<AppreciationResponse>> GetAllAppreciation(int page, int pageSize, AppreciationType type);
    Task<BaseResponse<Appreciation?>> GetAppreciationByIdAsync(int id);
}
