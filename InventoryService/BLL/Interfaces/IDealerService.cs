using BLL.DTOs.Dealer;

namespace BLL.Interfaces;

public interface IDealerService
{
    Task<IEnumerable<DealerResponse>> GetAllAsync();
    Task<DealerResponse?> GetByIdAsync(long id);
    Task<DealerResponse> AddAsync(DealerRequest dto);
    Task UpdateAsync(long id, DealerRequest dto);
    Task DeleteAsync(long id);
}
