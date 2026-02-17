using Microsoft.Extensions.DependencyInjection;

namespace Valgusfoor
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var startPage = new ValgusfoorPage();

            return new Window(startPage);
        }
    }
}