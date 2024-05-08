namespace FileApp;

public partial class NewPage2 : ContentPage
{
    private bool isFront = true;
    private int currentWordIndex = 0;
    private List<(string, string)> wordPairs = new List<(string, string)>();
    private string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "sonapaarid.txt");
    public Label FrontLabel, BackLabel, WordCountLabel;
    public Frame CardFrame;
    TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer();
    public NewPage2()
	{
        //InitializeComponent();
        var carusel = new CarouselView();
        carusel.ItemsSource = filePath;
        carusel.ItemTemplate = new DataTemplate(() =>{

            WordCountLabel = new Label()
            {
                WidthRequest= 200,
                HeightRequest= 400,
                IsVisible = true,
                Text = "fdsfsdf"
            };
            FrontLabel = new Label()
            {
                WidthRequest= 200,
                HeightRequest= 400,
                IsVisible = true,
            };
            BackLabel = new Label() { WidthRequest = 200, HeightRequest = 200, IsVisible = true };
            
            CardFrame = new Frame()
            {
                WidthRequest = 200,
                HeightRequest = 400,
                IsVisible = true,
                GestureRecognizers = { tapGestureRecognizer },
                Content= new StackLayout { Children= { WordCountLabel,FrontLabel,BackLabel}}
            };

            return CardFrame;
        });
        
        Button lisa_kaart = new Button()
        {
            WidthRequest = 100,
            HeightRequest = 100,
            IsVisible = true,


        };
        Button kustuta_kaart = new Button() { WidthRequest = 100, HeightRequest = 100, IsVisible = true };
        Button muuda_kaart = new Button() { WidthRequest = 100, HeightRequest = 100, IsVisible = true };

        
        Content = new StackLayout
        {
            Spacing = 10,
            Children = { carusel,lisa_kaart, kustuta_kaart, muuda_kaart }
        };
        
        //Content = carusel;

	}
    private async Task sõnafailist()
    {
        try
        {
            if (File.Exists(filePath))
            {
                string[] lines = await File.ReadAllLinesAsync(filePath);
                wordPairs = lines.Select(line => line.Split(';')).Select(parts => (parts[0], parts[1])).ToList();
                UpdateWordCountLabel();
            }
            else
            {
                Console.WriteLine("Faili ei ole olemas.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Viga faili lugemisel: {ex.Message}");
        }
    }
    private async Task<List<(string, string)>> ReadWordPairsFromFile()
    {
        try
        {
            if (File.Exists(filePath))
            {
                string[] lines = await File.ReadAllLinesAsync(filePath);
                return lines.Select(line => line.Split(';')).Select(parts => (parts[0], parts[1])).ToList();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Viga faili lugemisel: {ex.Message}");
        }
        return new List<(string, string)>();
    }
    private async Task WriteWordPairsToFile(List<(string, string)> wordPairs)
    {
        try
        {
            string[] lines = wordPairs.Select(pair => $"{pair.Item1};{pair.Item2}").ToArray();
            await File.WriteAllLinesAsync(filePath, lines);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Viga faili kirjutamisel: {ex.Message}");
        }
    }
    private async void FlipCard_Clicked(object sender, EventArgs e)
    {
        await FlipCardAnimation();
        isFront = !isFront;
    }

    private async void SetWordPair(string eestisona, string venesona)
    {
        FrontLabel.Text = eestisona;
        BackLabel.Text = venesona;
    }

    private async Task FlipCardAnimation()
    {
        await CardFrame.RotateYTo(-115, 250, Easing.Linear);
        if (isFront)
        {
            FrontLabel.IsVisible = false;
            BackLabel.IsVisible = true;
        }
        else
        {
            FrontLabel.IsVisible = true;
            BackLabel.IsVisible = false;
        }
        await CardFrame.RotateYTo(0, 250, Easing.Linear);
    }
    private void UpdateWordCountLabel() { WordCountLabel.Text = $"Sõna {currentWordIndex + 1}/{wordPairs.Count}"; }
    private async void AddWord_Clicked(object sender, EventArgs e)
    {
        string newEestisona = await DisplayPromptAsync("Lisa Sõna", "Sisesta uus eestikeelne sõna:", "OK", "Loobu");
        if (!string.IsNullOrWhiteSpace(newEestisona))
        {
            string newvenesona = await DisplayPromptAsync("Lisa Sõna", "Sisestage tõlge vene keeles:", "OK", "Loobu");
            if (!string.IsNullOrWhiteSpace(newvenesona))
            {
                wordPairs.Add((newEestisona, newvenesona));
                await WriteWordPairsToFile(wordPairs);
                UpdateWordCountLabel();
            }
        }
    }

    private async void ChangeWord_Clicked(object sender, EventArgs e)
    {
        if (wordPairs.Count == 0)
        {
            await DisplayAlert("Viga", "Sõnu pole saadaval.", "OK");
            return;
        }

        string[] voimalused = wordPairs.Select(pair => $"{pair.Item1} - {pair.Item2}").ToArray();
        string selectedsona = await DisplayActionSheet("Muuda Sõna", "Loobu", null, voimalused);
        if (selectedsona != null && selectedsona != "Loobu")
        {
            int indexToChange = Array.IndexOf(voimalused, selectedsona);
            string oldEestisona = wordPairs[indexToChange].Item1;
            string oldvenesona = wordPairs[indexToChange].Item2;

            string newEestisona = await DisplayPromptAsync("Muuda Sõna", "Sisesta uus eestikeelne sõna:", "OK", "Loobu", oldEestisona);
            if (!string.IsNullOrWhiteSpace(newEestisona))
            {
                string newvenesona = await DisplayPromptAsync("Muuda Sõna", "Sisestage uus tõlge vene keeles:", "OK", "Loobu", oldvenesona);
                if (!string.IsNullOrWhiteSpace(newvenesona))
                {
                    wordPairs[indexToChange] = (newEestisona, newvenesona);
                    await WriteWordPairsToFile(wordPairs);

                    if (indexToChange == currentWordIndex)
                    {
                        SetWordPair(newEestisona, newvenesona);
                    }
                    UpdateWordCountLabel();
                }
            }
        }
    }


    private async void RemoveWord_Clicked(object sender, EventArgs e)
    {

        if (wordPairs.Count == 0)
        {
            await DisplayAlert("Viga", "Pole sõnu, mida eemaldada.", "OK");
            return;
        }

        string[] voimalused = wordPairs.Select(pair => $"{pair.Item1} - {pair.Item2}").ToArray();
        string selectedsona = await DisplayActionSheet("Kustuta Sõna", "Loobu", null, voimalused);
        if (selectedsona != null && selectedsona != "Loobu")
        {
            int indexToRemove = Array.IndexOf(voimalused, selectedsona);
            wordPairs.RemoveAt(indexToRemove);
            await WriteWordPairsToFile(wordPairs);

            if (indexToRemove == currentWordIndex) { currentWordIndex = currentWordIndex - 1; }
            SetWordPair(wordPairs[currentWordIndex].Item1, wordPairs[currentWordIndex].Item2);
            UpdateWordCountLabel();
        }
    }





}