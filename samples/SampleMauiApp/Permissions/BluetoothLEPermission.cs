#if ANDROID
using Android;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.Content;
#endif

namespace SampleMauiApp.Permissions;

public class BluetoothLEPermission : Microsoft.Maui.ApplicationModel.Permissions.BasePlatformPermission
{
#if ANDROID
    public override (string androidPermission, bool isRuntime)[] RequiredPermissions
    {
        get
        {
            var permissions = new List<(string, bool)>();

            if (OperatingSystem.IsAndroidVersionAtLeast(31))
            {
                permissions.Add((Manifest.Permission.BluetoothConnect, true));
                permissions.Add((Manifest.Permission.BluetoothScan, true));
            }
            else
            {
                permissions.Add((Manifest.Permission.Bluetooth, true));
                permissions.Add((Manifest.Permission.BluetoothAdmin, true));
                permissions.Add((Manifest.Permission.AccessFineLocation, true));
            }

            return permissions.ToArray();
        }
    }
#elif IOS
    public override Task<PermissionStatus> CheckStatusAsync()
    {
        var status = CoreBluetooth.CBManager.Authorization switch
        {
            CoreBluetooth.CBManagerAuthorization.AllowedAlways => PermissionStatus.Granted,
            CoreBluetooth.CBManagerAuthorization.Denied => PermissionStatus.Denied,
            CoreBluetooth.CBManagerAuthorization.Restricted => PermissionStatus.Restricted,
            _ => PermissionStatus.Unknown
        };
        return Task.FromResult(status);
    }

    public override Task<PermissionStatus> RequestAsync()
    {
        // iOS triggers the Bluetooth permission dialog on first CBCentralManager usage.
        // Return current status since there is no explicit request API.
        return CheckStatusAsync();
    }
#endif
}
