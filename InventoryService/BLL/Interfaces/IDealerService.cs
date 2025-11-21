using BLL.DTOs.Dealer;
using System;

namespace BLL.Interfaces;

public interface IDealerService
{
    Task<IEnumerable<DealerResponse>> GetAllAsync();
    Task<DealerResponse?> GetByIdAsync(Guid id);
    Task<DealerResponse> AddAsync(DealerRequest dto);
    Task UpdateAsync(Guid id, DealerRequest dto);
    Task DeleteAsync(Guid id);
}
