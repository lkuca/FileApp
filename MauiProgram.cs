namespace FileApp;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				
                fonts.AddFont("LTPerfume-2.ttf", "OpenSansSemibold");
			});

		return builder.Build();
	}
}
