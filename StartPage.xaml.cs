
using System.Threading.Tasks;

namespace Valgusfoor;

public partial class StartPage : ContentPage
{
    VerticalStackLayout vst;
    ScrollView sv;
    public List<ContentPage> Lehed = new List<ContentPage>() { new ValgusfoorPage() };
    public List<string> LeheNimed = new List<string>() { "Joonis" };


    public StartPage()
    {
        Title = "Avaleht";
        vst = new VerticalStackLayout { Padding = 20, Spacing = 15 };
        for (int i = 0; i < Lehed.Count; i++)
        {
            Button nupp = new Button
            {
                Text = LeheNimed[i],
                FontSize = 18,
                FontFamily = "Impact",
                BackgroundColor = Colors.AliceBlue,
                TextColor = Colors.Black,
                ZIndex = i,
                HeightRequest = 50,
                CornerRadius = 10
            };
            vst.Add(nupp);
            nupp.Clicked += (sender, e) =>
            {
                var valik = Lehed[nupp.ZIndex];
                Navigation.PushAsync(valik);
            };
        }
        sv = new ScrollView { Content = vst };
        Content = sv;


    }

}