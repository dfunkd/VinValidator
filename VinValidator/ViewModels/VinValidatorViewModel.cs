using System.Collections.ObjectModel;
using VinValidator.Core.Filter;
using VinValidator.Models;
using VinValidator.Services;

namespace VinValidator.ViewModels;

public class VinValidatorViewModel : BaseViewModel
{
    private IRuanMasterDataService ruanMasterDataService;

    #region Properties
    private bool isProcessing = false;
    public bool IsProcessing
    {
        get => isProcessing;
        set
        {
            if (isProcessing != value)
            {
                isProcessing = value;
                OnPropertyChanged();
            }
        }
    }

    private int assetCount = 0;
    public int AssetCount
    {
        get => assetCount;
        set
        {
            if (assetCount != value)
            {
                assetCount = value;
                OnPropertyChanged();
            }
        }
    }

    private int totalCount = 0;
    public int TotalCount
    {
        get => totalCount;
        set
        {
            if (totalCount != value)
            {
                totalCount = value;
                OnPropertyChanged();
            }
        }
    }

    private int unitCount = 0;
    public int UnitCount
    {
        get => unitCount;
        set
        {
            if (unitCount != value)
            {
                unitCount = value;
                OnPropertyChanged();
            }
        }
    }

    private int vehicleCount = 0;
    public int VehicleCount
    {
        get => vehicleCount;
        set
        {
            if (vehicleCount != value)
            {
                vehicleCount = value;
                OnPropertyChanged();
            }
        }
    }

    private string assetSearchString = string.Empty;
    public string AssetSearchString
    {
        get => assetSearchString;
        set
        {
            if (assetSearchString != value)
            {
                assetSearchString = value;
                OnPropertyChanged();
            }
        }
    }

    private string vinSearchString = string.Empty;
    public string VinSearchString
    {
        get => vinSearchString;
        set
        {
            if (vinSearchString != value)
            {
                vinSearchString = value;
                OnPropertyChanged();
            }
        }
    }

    private GroupFilter assetGroupFilter = new();
    public GroupFilter AssetGroupFilter
    {
        get => assetGroupFilter;
        set
        {
            if (assetGroupFilter != value)
            {
                assetGroupFilter = value;
                OnPropertyChanged();
            }
        }
    }

    private GroupFilter unitGroupFilter = new();
    public GroupFilter UnitGroupFilter
    {
        get => unitGroupFilter;
        set
        {
            if (unitGroupFilter != value)
            {
                unitGroupFilter = value;
                OnPropertyChanged();
            }
        }
    }

    private GroupFilter vehicleGroupFilter = new();
    public GroupFilter VehicleGroupFilter
    {
        get => vehicleGroupFilter;
        set
        {
            if (vehicleGroupFilter != value)
            {
                vehicleGroupFilter = value;
                OnPropertyChanged();
            }
        }
    }

    private ObservableCollection<AllData> allData = [];
    public ObservableCollection<AllData> AllData
    {
        get => allData;
        set
        {
            if (allData != value)
            {
                allData = value;
                OnPropertyChanged();
            }
        }
    }

    private ObservableCollection<EamAsset> filteredAssets = [];
    public ObservableCollection<EamAsset> FilteredAssets
    {
        get => filteredAssets;
        set
        {
            if (filteredAssets != value)
            {
                filteredAssets = value;
                OnPropertyChanged();
            }
        }
    }

    private ObservableCollection<Unit> filteredUnits = [];
    public ObservableCollection<Unit> FilteredUnits
    {
        get => filteredUnits;
        set
        {
            if (filteredUnits != value)
            {
                filteredUnits = value;
                OnPropertyChanged();
            }
        }
    }

    private ObservableCollection<VehicleSync> filteredVehicles = [];
    public ObservableCollection<VehicleSync> FilteredVehicles
    {
        get => filteredVehicles;
        set
        {
            if (filteredVehicles != value)
            {
                filteredVehicles = value;
                OnPropertyChanged();
            }
        }
    }

    private ObservableCollection<EamAsset> invalidAssets = [];
    public ObservableCollection<EamAsset> InvalidAssets
    {
        get => invalidAssets;
        set
        {
            if (invalidAssets != value)
            {
                invalidAssets = value;
                OnPropertyChanged();
            }
        }
    }

    private ObservableCollection<Unit> invalidUnits = [];
    public ObservableCollection<Unit> InvalidUnits
    {
        get => invalidUnits;
        set
        {
            if (invalidUnits != value)
            {
                invalidUnits = value;
                OnPropertyChanged();
            }
        }
    }

    private ObservableCollection<VehicleSync> invalidVehicles = [];
    public ObservableCollection<VehicleSync> InvalidVehicles
    {
        get => invalidVehicles;
        set
        {
            if (invalidVehicles != value)
            {
                invalidVehicles = value;
                OnPropertyChanged();
            }
        }
    }
    #endregion

    public VinValidatorViewModel()
    {
        ruanMasterDataService = new RuanMasterDataService();
    }

    #region Methods
    public async Task FilterAssets(CancellationToken cancellationToken = default)
    {
        if (InvalidAssets.Count == 0)
            await PopulateAssetsAsync(cancellationToken);

        FilteredAssets = InvalidAssets;

        if (AssetSearchString.Length > 0)
            FilteredAssets = [.. FilteredAssets.Where(w => w.Asset.Contains(AssetSearchString, StringComparison.InvariantCultureIgnoreCase))];

        if (VinSearchString.Length > 0)
            FilteredAssets = [.. FilteredAssets.Where(w => w.SerialNumber.Contains(VinSearchString, StringComparison.InvariantCultureIgnoreCase))];
    }

    public async Task FilterUnits(CancellationToken cancellationToken = default)
    {
        if (InvalidUnits.Count == 0)
            await PopulateUnitsAsync(cancellationToken);

        FilteredUnits = InvalidUnits;

        if (AssetSearchString.Length > 0)
            FilteredUnits = [.. FilteredUnits.Where(w => w.UnitCode.Contains(AssetSearchString, StringComparison.InvariantCultureIgnoreCase))];

        if (VinSearchString.Length > 0)
            FilteredUnits = [.. FilteredUnits.Where(w => w.ChassisSerialNumber.Contains(VinSearchString, StringComparison.InvariantCultureIgnoreCase))];
    }

    public async Task FilterVehicles(CancellationToken cancellationToken = default)
    {
        if (InvalidVehicles.Count == 0)
            await PopulateVehiclesAsync(cancellationToken);

        FilteredVehicles = InvalidVehicles;

        if (AssetSearchString.Length > 0)
            FilteredVehicles = [.. FilteredVehicles.Where(w => w.UnitId.Contains(AssetSearchString, StringComparison.InvariantCultureIgnoreCase))];

        if (VinSearchString.Length > 0)
            FilteredVehicles = [.. FilteredVehicles.Where(w => w.VIN.Contains(VinSearchString, StringComparison.InvariantCultureIgnoreCase))];
    }

    public async Task PopulateAssetsAsync(CancellationToken cancellationToken = default)
    {
        invalidAssets = [.. await ruanMasterDataService.GetAssetsAsync(cancellationToken)];

        AssetCount = InvalidAssets.Count;
    }

    public async Task PopulateUnitsAsync(CancellationToken cancellationToken = default)
    {
        invalidUnits = [.. await ruanMasterDataService.GetUnitsAsync(cancellationToken)];

        UnitCount = InvalidUnits.Count;
    }

    public async Task PopulateVehiclesAsync(CancellationToken cancellationToken = default)
    {
        invalidVehicles = [.. await ruanMasterDataService.GetVehicleSyncsAsync(cancellationToken)];

        VehicleCount = InvalidVehicles.Count;
    }
    #endregion
}
