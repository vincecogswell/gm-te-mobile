using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace GMPark
{
	public partial class SplashScreen : ContentPage
	{
		public SplashScreen()
		{
			InitializeComponent();
			this.BackgroundColor = Color.FromRgb(104,151,243);
			var button = new Button
			{
				Text = "Get Started",
				TextColor = Color.White,
				FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Button)),
				FontFamily = Device.OnPlatform("AppleSDGothicNeo-UltraLight", "Droid Sans Mono", "Comic Sans MS"),
				HorizontalOptions = LayoutOptions.Center,
				BackgroundColor = Color.Transparent,
			};
			button.Clicked += OnClicked;
			var stack = new StackLayout
			{
				VerticalOptions = LayoutOptions.Center,
				HorizontalOptions = LayoutOptions.Center,
				Children = 
				{
					new Image
					{
						VerticalOptions = LayoutOptions.Center,
						HorizontalOptions = LayoutOptions.Center,
						Source = "splash_logo.jpg",
					},
					new Label
					{
						Text = " "
					},
					new Label
					{
						Text = "Transportation Experience",
						TextColor=Color.White,
						FontSize = Device.GetNamedSize(NamedSize.Large,typeof(Label)),
						FontFamily = Device.OnPlatform("AppleSDGothicNeo-UltraLight", "Droid Sans Mono", "Comic Sans MS"),
						HorizontalOptions=LayoutOptions.Center,


					},
					button
				}
			};

			Content = stack;
			NavigationPage.SetHasNavigationBar(this, false);

		}

		async void OnClicked(object sender, EventArgs args)
		{
			await Navigation.PushAsync(new ChooseCampus());
		}
	}
}
