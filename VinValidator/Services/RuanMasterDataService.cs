using VinValidator.Data.Repositories;
using VinValidator.Models;

namespace VinValidator.Services;

public interface IRuanMasterDataService
{
    Task<List<EamAsset>> GetAssetsAsync(CancellationToken cancellationToken = default);
    Task<List<Unit>> GetUnitsAsync(CancellationToken cancellationToken = default);
    Task<List<VehicleSync>> GetVehicleSyncsAsync(CancellationToken cancellationToken = default);
}

public class RuanMasterDataService : IRuanMasterDataService
{
    private IRuanMasterDataRepository repo;

    public RuanMasterDataService()
    {
        repo = new RuanMasterDataRepository();
    }

    public async Task<List<EamAsset>> GetAssetsAsync(CancellationToken cancellationToken = default)
        => await repo.GetAssetsAsync(cancellationToken);

    public async Task<List<Unit>> GetUnitsAsync(CancellationToken cancellationToken = default)
        => await repo.GetUnitsAsync(cancellationToken);

    public async Task<List<VehicleSync>> GetVehicleSyncsAsync(CancellationToken cancellationToken = default)
        => await repo.GetVehicleSyncsAsync(cancellationToken);
}
