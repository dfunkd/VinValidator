using Dapper;
using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Data;
using VinValidator.Models;

namespace VinValidator.Data.Repositories;

public interface IRuanMasterDataRepository
{
    Task<List<EamAsset>> GetAssetsAsync(CancellationToken cancellationToken = default);
    Task<List<Unit>> GetUnitsAsync(CancellationToken cancellationToken = default);
    Task<List<VehicleSync>> GetVehicleSyncsAsync(CancellationToken cancellationToken = default);
}

public class RuanMasterDataRepository : IRuanMasterDataRepository
{
    private readonly string? connectionString = string.Empty;

    public RuanMasterDataRepository()
    {
        connectionString = ConfigurationManager.ConnectionStrings["RuanMasterDataDb"].ConnectionString;
        if (connectionString is null)
            throw new Exception("Connection String was empty");
    }

    public async Task<List<EamAsset>> GetAssetsAsync(CancellationToken cancellationToken = default)
    {
        List<EamAsset> assets = [];

        const string sSql = @"
SELECT a.Asset, a.SerialNumber
FROM Eam.Asset AS a
WHERE LEN(a.SerialNumber) = 17
	AND a.SerialNumber LIKE '%[IOQ]%'
";
        using IDbConnection conn = new SqlConnection(connectionString);
        if (conn.State != ConnectionState.Open)
            conn.Open();

        CommandDefinition sCmd = new(sSql, null, null, 150, cancellationToken: cancellationToken);

        IEnumerable<EamAsset> res = await conn.QueryAsync<EamAsset>(sCmd);

        if (res.Any())
            assets.AddRange(res);

        return assets;
    }

    public async Task<List<Unit>> GetUnitsAsync(CancellationToken cancellationToken = default)
    {
        List<Unit> units = [];

        const string sSql = @"
SELECT u.UnitCode, u.ChassisSerialNumber
FROM Ruan.Unit AS u
WHERE LEN(u.ChassisSerialNumber) = 17
	AND u.ChassisSerialNumber LIKE '%[IOQ]%'
";

        using IDbConnection conn = new SqlConnection(connectionString);
        if (conn.State != ConnectionState.Open)
            conn.Open();

        CommandDefinition sCmd = new(sSql, null, null, 150, cancellationToken: cancellationToken);

        IEnumerable<Unit> res = await conn.QueryAsync<Unit>(sCmd);

        if (res.Any())
            units.AddRange(res);

        return units;
    }

    public async Task<List<VehicleSync>> GetVehicleSyncsAsync(CancellationToken cancellationToken = default)
    {
        List<VehicleSync> vehicles = [];

        const string sSql = @"
SELECT vs.UnitId, vs.VIN
FROM Lytx.VehicleSync AS vs
WHERE LEN(vs.VIN) = 17
	AND vs.VIN LIKE '%[IOQ]%'
";

        using IDbConnection conn = new SqlConnection(connectionString);
        if (conn.State != ConnectionState.Open)
            conn.Open();

        CommandDefinition sCmd = new(sSql, null, null, 150, cancellationToken: cancellationToken);

        IEnumerable<VehicleSync> res = await conn.QueryAsync<VehicleSync>(sCmd);

        if (res.Any())
            vehicles.AddRange(res);

        return vehicles;
    }
}
