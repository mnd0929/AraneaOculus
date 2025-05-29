using AraneaOculus.Core.Interfaces;
using Microsoft.Extensions.Logging;
using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;

namespace AraneaOculus.Agent.UI;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseBarcodeReader()
            .ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		builder.Services.AddSingleton<IBackgroundService>(_ => 
		{
#if ANDROID
			return new Services.Platforms.Android.AndroidBackgroundServiceImplementation();
#elif WINDOWS
			return new Services.Platforms.Windows.WindowsBackgroundServiceImplementation();
#else
			return null!;
#endif
		});

		return builder.Build();
	}
}
