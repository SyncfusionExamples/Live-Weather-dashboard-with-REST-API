namespace WeatherDashBoardDemo
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1JEaF5cXmRCd0xyWmFZfVtgcV9FaVZURWYuP1ZhSXxWdk1jXH9bc31QQ2FVVUd9XEI=");

            return new Window(new WeatherDetails());
        }
    }
}