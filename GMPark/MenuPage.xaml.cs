/* Authot : Phyllis Jin
 * Content page thats contains all elements such as logo and campus
 */
using System;
using System.Net.Http;
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
		HttpClient client;
		StackLayout stack;
		ScrollView scroll;
		GMTEMap map;

		public MenuPage()
		{
			
			InitializeComponent();

			client = new HttpClient();
			client.MaxResponseContentBufferSize = 256000;

			if (Application.Current.Properties.ContainsKey("map"))
			{
				map = (GMTEMap)Application.Current.Properties["map"];
			}

			else
			{
				map = new GMTEMap();
			}

			scroll = new ScrollView();
			stack = new StackLayout { 
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

			scroll.Content = stack;
			Content = scroll;
			Title = "None";
			BackgroundColor = Color.FromRgb(104, 151, 243);
			Icon = Device.OS == TargetPlatform.iOS ? "menu.png" : null;
		}

		// call API to get all campus info
		public async Task<ServerJSON> GetCampuses()
		{
			var uri = new Uri("http://35.9.22.105/campuses");
			var response = await client.GetAsync(uri);
			if (response.IsSuccessStatusCode)
			{
				var content = await response.Content.ReadAsStringAsync();
				return JsonConvert.DeserializeObject<ServerJSON>(content);
			}
			else
			{
				return null;
			}
		}

		// add all campus which come from API and show them in the menu page
		public void AddButtons()
		{
			if (Application.Current.Properties.ContainsKey("map"))
			{
				map = (GMTEMap)Application.Current.Properties["map"];
			}

			for (int i = stack.Children.Count - 1; i >= 2; i--)
			{
				stack.Children.RemoveAt(i);
			}

			var campusList = map.GetCampusList();

			foreach (string name in campusList)
			{
				var click = new MenuButtons(name);
				stack.Children.Add(click);
			}
		}

	}
}
