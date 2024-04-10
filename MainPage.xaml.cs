namespace FileApp;

public partial class MainPage : ContentPage
{
	int count = 0;


	public MainPage()
	{
		//InitializeComponent();
		CarouselView  carouselView = new CarouselView()
		{
			VerticalOptions = LayoutOptions.Center,
		};
		carouselView.ItemsSource = new List<Product>
		{
			new Product {Name="Name1", Description="Dis1", Image ="dotnet_bot.svg"},

            new Product {Name="Name2", Description="Dis2", Image ="dotnet_bot.svg"},

            new Product {Name="Name3", Description="Dis3", Image ="dotnet_bot.svg"}

        };
		carouselView.ItemTemplate = new DataTemplate(() =>
		{
			Label header = new Label
			{
				FontAttributes = FontAttributes.Bold,
				HorizontalTextAlignment = TextAlignment.Center,
				FontSize = 18
			};
			header.SetBinding(Label.TextProperty, "Name");

			Image image = new Image { WidthRequest = 150, HeightRequest = 150 };
			image.SetBinding(Image.SourceProperty, "Image");

			Label description = new Label { HorizontalTextAlignment = TextAlignment.Center };
			description.SetBinding(Label.TextProperty, "Description");

			StackLayout stacLayout= new StackLayout() {header, image, description};
			Frame frame = new Frame();
			frame.Content = stacLayout;
			return frame;
		});
		Content = carouselView;
	}

	private void OnCounterClicked(object sender, EventArgs e)
	{
		count++;

		if (count == 1)
			CounterBtn.Text = $"Clicked {count} time";
		else
			CounterBtn.Text = $"Clicked {count} times";

		SemanticScreenReader.Announce(CounterBtn.Text);
	}
}

