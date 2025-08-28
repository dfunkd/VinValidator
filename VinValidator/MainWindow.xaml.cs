using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VinValidator.Models;
using VinValidator.ViewModels;

namespace VinValidator;

public partial class MainWindow : Window
{
    #region Routed Commands
    #region ClearAssetSearch Command
    private static readonly RoutedCommand clearAssetSearchCommand = new();
    public static RoutedCommand ClearAssetSearchCommand = clearAssetSearchCommand;
    private void CanExecuteClearAssetSearchCommand(object sender, CanExecuteRoutedEventArgs e)
        => e.CanExecute = e.Source is Control && DataContext is VinValidatorViewModel vm && !string.IsNullOrWhiteSpace(vm.AssetSearchString);
    private void ExecutedClearAssetSearchCommand(object sender, ExecutedRoutedEventArgs e)
    {
        if (DataContext is VinValidatorViewModel vm)
            vm.AssetSearchString = string.Empty;
    }
    #endregion

    #region ClearVINSearch Command
    private static readonly RoutedCommand clearVINSearchCommand = new();
    public static RoutedCommand ClearVINSearchCommand = clearVINSearchCommand;
    private void CanExecuteClearVINSearchCommand(object sender, CanExecuteRoutedEventArgs e)
        => e.CanExecute = e.Source is Control && DataContext is VinValidatorViewModel vm &&  !string.IsNullOrWhiteSpace(vm.VinSearchString);
    private void ExecutedClearVINSearchCommand(object sender, ExecutedRoutedEventArgs e)
    {
        if (DataContext is VinValidatorViewModel vm)
            vm.VinSearchString = string.Empty;
    }
    #endregion

    #region Close Command
    private static readonly RoutedCommand closeCommand = new();
    public static RoutedCommand CloseCommand = closeCommand;
    private void CanExecuteCloseCommand(object sender, CanExecuteRoutedEventArgs e)
        => e.CanExecute = e.Source is Control;
    private void ExecutedCloseCommand(object sender, ExecutedRoutedEventArgs e)
        => Close();
    #endregion

    #region Export Command
    private static readonly RoutedCommand exportCommand = new();
    public static RoutedCommand ExportCommand = exportCommand;
    private void CanExecuteExportCommand(object sender, CanExecuteRoutedEventArgs e)
        => e.CanExecute = e.Source is Control && DataContext is VinValidatorViewModel vm && (vm.AssetCount > 0 || vm.UnitCount > 0 || vm.VehicleCount > 0);
    private void ExecutedExportCommand(object sender, ExecutedRoutedEventArgs e)
    {
        string filename = $@"C:\Users\dfunk\OneDrive - Ruan Transportation Management Systems\Desktop\{DateTime.Now:yyyy-MMMM-dd}VinValidation.xlsx";

        if (DataContext is VinValidatorViewModel vm)
        {
            List<AllData> allData = [];
            foreach (EamAsset asset in vm.FilteredAssets)
                allData.Add(new AllData()
                {
                    Asset = asset.Asset,
                    SerialNumber = asset.SerialNumber
                });

            foreach (Unit unit in vm.FilteredUnits)
            {
                if (allData.Any(a => a.Asset == unit.UnitCode && a.UnitCode == unit.UnitCode))
                    continue;

                AllData? existingAsset = allData.FirstOrDefault(f => f.Asset == unit.UnitCode);
                AllData? existingUnit = allData.FirstOrDefault(f => f.UnitCode == unit.UnitCode);
                if (existingAsset is null && existingUnit is null)
                    allData.Add(new AllData()
                    {
                        UnitCode = unit.UnitCode,
                        ChassisSerialNumber = unit.ChassisSerialNumber
                    });

                if (existingAsset is not null && existingUnit is null)
                    allData.Add(new AllData()
                    {
                        Asset = existingAsset.Asset,
                        SerialNumber = existingAsset.SerialNumber,
                        UnitCode = unit.UnitCode,
                        ChassisSerialNumber = unit.ChassisSerialNumber
                    });
            }

            foreach (VehicleSync vehicle in vm.FilteredVehicles)
            {
                if (allData.Any(a => (a.Asset == vehicle.UnitId  || a.UnitCode == vehicle.UnitId) && a.UnitId == vehicle.UnitId))
                    continue;

                AllData? existingAsset = allData.FirstOrDefault(f => f.Asset == vehicle.UnitId);
                AllData? existingUnit = allData.FirstOrDefault(f => f.UnitCode == vehicle.UnitId);
                AllData? existingVehicle = allData.FirstOrDefault(f => f.UnitId == vehicle.UnitId);
                if (existingAsset is null && existingUnit is null & existingVehicle is null)
                    allData.Add(new AllData()
                    {
                        UnitCode = vehicle.UnitId,
                        UnitId = vehicle.UnitId
                    });

                if (!(existingAsset is null || existingUnit is null) && existingVehicle is null)
                    allData.Add(new AllData()
                    {
                        Asset = existingAsset.Asset,
                        SerialNumber = existingAsset.SerialNumber,
                        UnitCode = existingUnit.UnitCode,
                        ChassisSerialNumber = existingUnit.ChassisSerialNumber,
                        UnitId = vehicle.UnitId,
                        VIN = vehicle.VIN
                    });
            }

            allData = [.. allData.OrderBy(o => o.Asset).ThenBy(t => t.UnitCode).ThenBy(t => t.UnitId)];

            allData.ExportToExcel(filename);
        }
    }
    #endregion

    #region Validate Command
    private static readonly RoutedCommand validateCommand = new();
    public static RoutedCommand ValidateCommand = validateCommand;
    private void CanExecuteValidateCommand(object sender, CanExecuteRoutedEventArgs e)
        => e.CanExecute = e.Source is Control && DataContext is VinValidatorViewModel vm && vm.IsProcessing == false;
    private async void ExecutedValidateCommand(object sender, ExecutedRoutedEventArgs e)
        => await PopulateLists(default);
    #endregion
    #endregion

    public MainWindow()
    {
        InitializeComponent();

        DataContext = new VinValidatorViewModel();
    }

    #region Events
    private void OnDrag(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
            DragMove();
    }

    private async void OnSearchChanged(object sender, TextChangedEventArgs e)
        => await PopulateLists(default);
    #endregion

    #region Functions
    private async Task PopulateLists(CancellationToken cancellationToken = default)
    {
        if (DataContext is VinValidatorViewModel vm)
        {

            await Task.Run(() =>
            {
                vm.IsProcessing = true;

                Dispatcher.Invoke(() =>
                {
                    lvAssets.Items.Clear();
                    lvUnits.Items.Clear();
                    lvVehicles.Items.Clear();
                });

                vm.AssetCount = 0;
                vm.UnitCount = 0;
                vm.VehicleCount = 0;
                vm.TotalCount = 0;

                List<Task> tasks = [vm.FilterAssets(cancellationToken), vm.FilterUnits(cancellationToken), vm.FilterVehicles(cancellationToken)];
                Task.WaitAll([.. tasks]);

                List<Task> populateTasks = [];
                populateTasks.Add(Task.Run(() =>
                {
                    foreach (EamAsset eam in vm.FilteredAssets)
                        Dispatcher.Invoke(() => lvAssets.Items.Add(eam));
                }));
                populateTasks.Add(Task.Run(() =>
                {
                    foreach (Unit unit in vm.FilteredUnits)
                        Dispatcher.Invoke(() => lvUnits.Items.Add(unit));
                }));
                populateTasks.Add(Task.Run(() =>
                {
                    foreach (VehicleSync vehicle in vm.FilteredVehicles)
                        Dispatcher.Invoke(() => lvVehicles.Items.Add(vehicle));
                }));
                Task.WaitAll([.. populateTasks]);

                vm.AssetCount = vm.FilteredAssets.Count;
                vm.UnitCount = vm.FilteredUnits.Count;
                vm.VehicleCount = vm.FilteredVehicles.Count;
                vm.TotalCount = vm.AssetCount + vm.UnitCount + vm.VehicleCount;

                vm.IsProcessing = false;

                return Task.CompletedTask;
            }, cancellationToken);
        }
    }
    #endregion
}
