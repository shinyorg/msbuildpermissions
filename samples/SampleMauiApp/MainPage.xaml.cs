using SampleMauiApp.Permissions;

namespace SampleMauiApp;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    async void OnRequestCamera(object? sender, EventArgs e)
        => await CheckAndDisplayAsync<Microsoft.Maui.ApplicationModel.Permissions.Camera>("Camera");

    async void OnRequestBluetooth(object? sender, EventArgs e)
        => await CheckAndDisplayAsync<BluetoothLEPermission>("Bluetooth LE");

    async void OnRequestLocation(object? sender, EventArgs e)
        => await CheckAndDisplayAsync<Microsoft.Maui.ApplicationModel.Permissions.LocationWhenInUse>("Location");

    async void OnRequestPush(object? sender, EventArgs e)
        => await CheckAndDisplayAsync<PushNotificationPermission>("Push Notifications");

    async void OnRequestBiometric(object? sender, EventArgs e)
        => await CheckAndDisplayAsync<BiometricPermission>("Biometric");

    async void OnRequestContacts(object? sender, EventArgs e)
        => await CheckAndDisplayAsync<Microsoft.Maui.ApplicationModel.Permissions.ContactsRead>("Contacts");

    async Task CheckAndDisplayAsync<TPermission>(string name) where TPermission : Microsoft.Maui.ApplicationModel.Permissions.BasePermission, new()
    {
        var status = await Microsoft.Maui.ApplicationModel.Permissions.CheckStatusAsync<TPermission>();

        if (status != PermissionStatus.Granted)
        {
            status = await Microsoft.Maui.ApplicationModel.Permissions.RequestAsync<TPermission>();
        }

        await DisplayAlertAsync(name, $"Permission status: {status}", "OK");
    }
}
