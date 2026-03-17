#if ANDROID
using Android;
#endif
#if IOS
using UserNotifications;
#endif

namespace SampleMauiApp.Permissions;

public class PushNotificationPermission : Microsoft.Maui.ApplicationModel.Permissions.BasePlatformPermission
{
#if ANDROID
    public override (string androidPermission, bool isRuntime)[] RequiredPermissions
    {
        get
        {
            if (OperatingSystem.IsAndroidVersionAtLeast(33))
                return [(Manifest.Permission.PostNotifications, true)];

            return [];
        }
    }
#elif IOS
    public override async Task<PermissionStatus> CheckStatusAsync()
    {
        var settings = await UNUserNotificationCenter.Current.GetNotificationSettingsAsync();
        return settings.AuthorizationStatus switch
        {
            UNAuthorizationStatus.Authorized => PermissionStatus.Granted,
            UNAuthorizationStatus.Provisional => PermissionStatus.Granted,
            UNAuthorizationStatus.Denied => PermissionStatus.Denied,
            _ => PermissionStatus.Unknown
        };
    }

    public override async Task<PermissionStatus> RequestAsync()
    {
        var (granted, _) = await UNUserNotificationCenter.Current.RequestAuthorizationAsync(
            UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound);

        return granted ? PermissionStatus.Granted : PermissionStatus.Denied;
    }
#endif
}
