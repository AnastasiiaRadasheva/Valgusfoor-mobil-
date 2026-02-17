using Microsoft.Maui.Controls;
using System.Threading;
using System.Threading.Tasks;

namespace Valgusfoor;

public partial class ValgusfoorPage : ContentPage
{
    Grid grid;
    Image bg;

    VerticalStackLayout vsl;
    HorizontalStackLayout hsl;

    BoxView punane, kollane, roheline;

    Button sisseBtn, valjaBtn, ooBtn, autoBtn;
    Label statusLabel;

    bool isOn = false;
    bool isNight = false;
    bool blinkOn = false;

    bool isAutoRunning = false;
    CancellationTokenSource? autoToken;

    readonly Color OFF = Colors.Gray;

    public ValgusfoorPage()
    {
        Title = "Valgusfoor";

        bg = new Image
        {
            Source = "day.png",
            Aspect = Aspect.AspectFill,
            Opacity = 0.7
        };

        statusLabel = new Label
        {
            Text = "Vali valgus",
            FontSize = 26,
            FontAttributes = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.Center
        };

        punane = TeeRing();
        kollane = TeeRing();
        roheline = TeeRing();

        sisseBtn = new Button
        {
            Text = "Sisse",
            FontSize = 18,
            FontFamily = "Impact",
            BackgroundColor = Colors.AliceBlue,
            TextColor = Colors.Black,
            HeightRequest = 50,
            CornerRadius = 10
        };
        sisseBtn.Clicked += (s, e) => TurnOn();

        valjaBtn = new Button
        {
            Text = "Välja",
            FontSize = 18,
            FontFamily = "Impact",
            BackgroundColor = Colors.AliceBlue,
            TextColor = Colors.Black,
            HeightRequest = 50,
            CornerRadius = 10
        };
        valjaBtn.Clicked += (s, e) => TurnOff();

        ooBtn = new Button
        {
            Text = "Ööreþiim",
            FontSize = 18,
            FontFamily = "Impact",
            BackgroundColor = Colors.AliceBlue,
            TextColor = Colors.Black,
            HeightRequest = 50,
            CornerRadius = 10
        };
        ooBtn.Clicked += (s, e) => ToggleNight();

        autoBtn = new Button
        {
            Text = "Automaat",
            FontSize = 18,
            FontFamily = "Impact",
            BackgroundColor = Colors.AliceBlue,
            TextColor = Colors.Black,
            HeightRequest = 50,
            CornerRadius = 10,
            HorizontalOptions = LayoutOptions.Center
        };
        autoBtn.Clicked += AutoStart;

        hsl = new HorizontalStackLayout
        {
            Spacing = 15,
            HorizontalOptions = LayoutOptions.Center,
            Children = { sisseBtn, valjaBtn, ooBtn }
        };

        vsl = new VerticalStackLayout
        {
            Padding = 20,
            Spacing = 18,
            HorizontalOptions = LayoutOptions.Center,
            Children =
            {
                statusLabel,
                punane,
                kollane,
                roheline,
                hsl,
                autoBtn
            }
        };

        grid = new Grid();
        grid.Children.Add(bg);
        grid.Children.Add(vsl);

        Content = grid;

        TurnOff();
    }

    BoxView TeeRing()
    {
        double size = 120;

        return new BoxView
        {
            WidthRequest = size,
            HeightRequest = size,
            CornerRadius = (float)(size / 2),
            Color = OFF,
            HorizontalOptions = LayoutOptions.Center
        };
    }

    void TurnOn()
    {
        isOn = true;

        if (isNight)
        {
            punane.Color = Color.FromRgb(120, 0, 0);
            roheline.Color = Color.FromRgb(0, 120, 0);
            kollane.Color = Colors.Yellow;
            StartBlink();
        }
        else
        {
            StopBlink();
            punane.Color = Colors.Red;
            kollane.Color = Colors.Yellow;
            roheline.Color = Colors.LimeGreen;
        }

        statusLabel.Text = "Vali valgus";
        LisaTapid();
    }

    void TurnOff()
    {
        isOn = false;

        StopAuto();
        StopBlink();

        punane.Color = OFF;
        kollane.Color = OFF;
        roheline.Color = OFF;

        statusLabel.Text = "Lülita esmalt foor sisse";

        punane.GestureRecognizers.Clear();
        kollane.GestureRecognizers.Clear();
        roheline.GestureRecognizers.Clear();
    }

    void ToggleNight()
    {
        isNight = !isNight;

        StopAuto();

        bg.Source = isNight ? "night.png" : "day.png";

        if (isOn) TurnOn();
    }

    void LisaTapid()
    {
        punane.GestureRecognizers.Clear();
        kollane.GestureRecognizers.Clear();
        roheline.GestureRecognizers.Clear();

        punane.GestureRecognizers.Add(new TapGestureRecognizer
        {
            Command = new Command(async () => await Vajutus("Seisa", punane))
        });

        kollane.GestureRecognizers.Add(new TapGestureRecognizer
        {
            Command = new Command(async () => await Vajutus("Valmista", kollane))
        });

        roheline.GestureRecognizers.Add(new TapGestureRecognizer
        {
            Command = new Command(async () => await Vajutus("Sõida", roheline))
        });
    }

    async Task Vajutus(string tekst, VisualElement ring)
    {
        if (!isOn)
        {
            statusLabel.Text = "Lülita esmalt foor sisse";
            return;
        }

        await Task.WhenAll(ring.ScaleTo(1.15, 150), ring.FadeTo(0.6, 150));
        await Task.WhenAll(ring.ScaleTo(1.0, 150), ring.FadeTo(1.0, 150));

        statusLabel.Text = tekst;
    }

    void StartBlink()
    {
        Device.StartTimer(TimeSpan.FromMilliseconds(600), () =>
        {
            if (!isOn || !isNight) return false;

            blinkOn = !blinkOn;
            kollane.Color = blinkOn ? Colors.Yellow : OFF;

            return true;
        });
    }

    void StopBlink()
    {
        blinkOn = false;
        if (isOn && isNight) kollane.Color = Colors.Yellow;
    }

    async void AutoStart(object? sender, EventArgs e)
    {
        if (!isOn || isNight) return;
        if (isAutoRunning) return;

        isAutoRunning = true;
        autoToken = new CancellationTokenSource();

        try
        {
            while (!autoToken.Token.IsCancellationRequested)
            {
                await ShowLightAsync(punane, Colors.Red, "Seisa", autoToken.Token);
                await Task.Delay(2000, autoToken.Token);

                await ShowLightAsync(kollane, Colors.Yellow, "Valmista", autoToken.Token);
                await BlinkYellowAsync(1000, autoToken.Token);

                await ShowLightAsync(roheline, Colors.LimeGreen, "Sõida", autoToken.Token);
                await Task.Delay(2000, autoToken.Token);

                await ShowLightAsync(kollane, Colors.Yellow, "Valmista", autoToken.Token);
                await BlinkYellowAsync(1000, autoToken.Token);
            }
        }
        catch (TaskCanceledException) { }

        isAutoRunning = false;
    }

    async Task ShowLightAsync(BoxView onBox, Color onColor, string text, CancellationToken token)
    {
        punane.Color = OFF;
        kollane.Color = OFF;
        roheline.Color = OFF;

        onBox.Color = onColor;
        statusLabel.Text = text;

        await Task.Yield();
    }

    async Task BlinkYellowAsync(int ms, CancellationToken token)
    {
        int step = 200;
        int count = ms / step;

        for (int i = 0; i < count; i++)
        {
            token.ThrowIfCancellationRequested();
            kollane.Color = (i % 2 == 0) ? OFF : Colors.Yellow;
            await Task.Delay(step, token);
        }

        kollane.Color = Colors.Yellow;
    }

    void StopAuto()
    {
        if (autoToken != null)
        {
            autoToken.Cancel();
            autoToken = null;
        }
        isAutoRunning = false;
    }

}
