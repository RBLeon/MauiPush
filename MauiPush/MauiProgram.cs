using Microsoft.AspNetCore.Components.WebView.Maui;
using MauiPush.Data;
using Microsoft.Maui.LifecycleEvents;
using Plugin.Firebase.Auth;
using Plugin.Firebase.Bundled.Shared;
using Plugin.Firebase.CloudMessaging;
using Plugin.Firebase.Crashlytics;
#if IOS
using Plugin.Firebase.Bundled.Platforms.iOS;
#elif ANDROID
using Plugin.Firebase.Bundled.Platforms.Android;
#endif

namespace MauiPush;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.RegisterFirebaseServices()
			.ConfigureFonts(fonts => { fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"); });

		builder.Services.AddMauiBlazorWebView();
#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
#endif

		builder.Services.AddSingleton<WeatherForecastService>();

		return builder.Build();
	}
	
	private static MauiAppBuilder RegisterFirebaseServices(this MauiAppBuilder builder)
	{
		builder.ConfigureLifecycleEvents(events => {
#if IOS
			events.AddiOS(iOS => iOS.FinishedLaunching((app, launchOptions) => {
				CrossFirebase.Initialize(CreateCrossFirebaseSettings());
				CrossFirebaseCrashlytics.Current.SetCrashlyticsCollectionEnabled(true);
				return false;
			}));
#elif ANDROID
            events.AddAndroid(android => android.OnCreate((activity, _) =>
                CrossFirebase.Initialize(activity, CreateCrossFirebaseSettings())));
                CrossFirebaseCrashlytics.Current.SetCrashlyticsCollectionEnabled(true);
#endif
		});

		builder.Services.AddSingleton(_ => CrossFirebaseCloudMessaging.Current);
		return builder;
	}

	private static CrossFirebaseSettings CreateCrossFirebaseSettings()
	{
		return new CrossFirebaseSettings(
			isCloudMessagingEnabled: true,
			isAnalyticsEnabled: true);
	}
}

