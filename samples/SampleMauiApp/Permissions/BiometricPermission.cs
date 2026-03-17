#if ANDROID
using Android;
#endif
#if IOS
using LocalAuthentication;
#endif

namespace SampleMauiApp.Permissions;

public class BiometricPermission : Microsoft.Maui.ApplicationModel.Permissions.BasePlatformPermission
{
#if ANDROID
    public override (string androidPermission, bool isRuntime)[] RequiredPermissions =>
        OperatingSystem.IsAndroidVersionAtLeast(28)
            ? [(Manifest.Permission.UseBiometric, true)]
            : [];
#elif IOS
    public override Task<PermissionStatus> CheckStatusAsync()
    {
        var context = new LAContext();
        var canEvaluate = context.CanEvaluatePolicy(LAPolicy.DeviceOwnerAuthenticationWithBiometrics, out var error);

        if (canEvaluate)
            return Task.FromResult(PermissionStatus.Granted);

        if (error?.Code == (long)LAStatus.BiometryNotAvailable)
            return Task.FromResult(PermissionStatus.Restricted);

        if (error?.Code == (long)LAStatus.BiometryLockout)
            return Task.FromResult(PermissionStatus.Denied);

        return Task.FromResult(PermissionStatus.Unknown);
    }

    public override Task<PermissionStatus> RequestAsync()
    {
        // iOS has no explicit biometric permission dialog.
        // The Face ID usage description is shown on first authentication attempt.
        return CheckStatusAsync();
    }
#endif
}
