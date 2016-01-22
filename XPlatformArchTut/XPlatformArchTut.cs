using System;

using Xamarin.Forms;

namespace XPlatformArchTut
{
	public class App : Application
	{
		public App ()
		{
			Label l = new Label {
				Text = "",
				VerticalOptions = LayoutOptions.Fill,
				HorizontalOptions = LayoutOptions.Fill,
				BackgroundColor = Color.Black,
				TextColor = Color.White
			};

			Button b = new Button {
				Text = "Go Native!",
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
			};
			b.Clicked += (sender, e) => {
				var text = DependencyService.Get<ICustomClass>().GoNative("IT'S ALIVE");
				l.Text = text;
			};

			// The root page of your application
			MainPage = new ContentPage {
				Content = new StackLayout {
					VerticalOptions = LayoutOptions.Center,
					Children = {
						b, l			
					}
				},
				Padding = new Thickness(0, Device.OnPlatform(20, 0, 0), 0, 0),
			};
		}

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}

