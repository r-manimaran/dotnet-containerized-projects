using AppreciateAppApi.DTO;
using AppreciateAppApi.DTO.Appreciation;
using AppreciateAppApi.Models;

namespace AppreciateAppApi.Services;

public interface IAppreciationService
{
    Task<BaseResponse<Appreciation?>> CreateAppreciationAsync(CreateAppreciationRequest request);
    Task<BaseResponse<AppreciationResponse>> GetAllAppreciation(int page, int pageSize, AppreciationType type);
}
