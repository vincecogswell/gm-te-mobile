using System;
using System.Net;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plugin.Geolocator;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace GMPark
{
	public partial class MenuPage : ContentPage
	{
		public MenuPage()
		{
			InitializeComponent();

			var scroll = new ScrollView();
			var stack = new StackLayout { 
				VerticalOptions = LayoutOptions.Center,
				Children =
				{
					new Image
					{
						VerticalOptions = LayoutOptions.Center,
						HorizontalOptions = LayoutOptions.Center,
						Source = "splash_logo.jpg",
						Scale = 0.8
					},

					new Label
					{
						Text = "Transportation Experience",
						TextColor=Color.White,
						FontSize = Device.GetNamedSize(NamedSize.Large,typeof(Label)),
						FontFamily = Device.OnPlatform("AppleSDGothicNeo-UltraLight", "Droid Sans Mono", "Comic Sans MS"),
						HorizontalOptions=LayoutOptions.Center,


					},
					new Label
					{
						Text = " "
					},
				}
			};
			var campuses = new List<Tuple<string, Position>>()
			{
				Tuple.Create( "Warren Tech Center", new Position(42.515062, -83.038084) ),
				Tuple.Create( "Michigan State University", new Position(42.723363, -84.477996) ),
			};

			foreach (Tuple<string, Position> campus in campuses)
			{
				var click = new MenuButtons(campus.Item1,campus.Item2);
				stack.Children.Add(click);


			}
			scroll.Content = stack;
			Content = scroll;

			Title = "None";
			BackgroundColor = Color.FromRgb(104, 151, 243);
			Icon = Device.OS == TargetPlatform.iOS ? "menu.png" : null;
		}

	}
}
