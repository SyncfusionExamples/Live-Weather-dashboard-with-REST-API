namespace WeatherDashBoardDemo;

public partial class WeatherDetails : ContentPage
{
	public WeatherDetails()
	{
		InitializeComponent();
	}

    private void Button_Clicked(object sender, EventArgs e)
    {
        if (App.Current != null)
        {
            if (App.Current.RequestedTheme == AppTheme.Light)
            {
                App.Current.UserAppTheme = AppTheme.Dark;
            }
            else
            {
                App.Current.UserAppTheme = AppTheme.Light;
            }
        }
    }
}