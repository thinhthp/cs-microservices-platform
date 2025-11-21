using BLL.DTOs.Dealer;
using BLL.Interfaces;
using DAL.Entities;
using DAL.Interfaces;
using System;

namespace BLL.Services;

public class DealerService : IDealerService
{
    private readonly IDealerRepository _dealerRepository;

    public DealerService(IDealerRepository dealerRepository)
    {
        _dealerRepository = dealerRepository;
    }

    public async Task<DealerResponse> AddAsync(DealerRequest dto)
    {
        var dealer = new Dealer
        {
            Code = dto.Code,
            Name = dto.Name,
            Region = dto.Region,
        };
        var createdDealer = await _dealerRepository.AddAsync(dealer);
        return MapToDto(createdDealer);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _dealerRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<DealerResponse>> GetAllAsync()
    {
        var dealers = await _dealerRepository.GetAllAsync();
        return dealers.Select(MapToDto);
    }

    public async Task<DealerResponse?> GetByIdAsync(Guid id)
    {
        var dealer = await _dealerRepository.GetByIdAsync(id);
        return dealer != null ? MapToDto(dealer) : null;
    }

    public async Task UpdateAsync(Guid id, DealerRequest dto)
    {
        var dealer = await _dealerRepository.GetByIdAsync(id);
        if (dealer == null)
            return;

        dealer.Code = dto.Code;
        dealer.Name = dto.Name;
        dealer.Region = dto.Region;

        await _dealerRepository.UpdateAsync(dealer);
    }

    private DealerResponse MapToDto(Dealer dealer)
    {
        return new DealerResponse
        {
            Id = dealer.Id,
            Code = dealer.Code,
            Name = dealer.Name,
            Region = dealer.Region,
        };
    }
}
