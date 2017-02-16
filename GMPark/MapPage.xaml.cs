using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace GMPark
{
	public partial class MapPage : ContentPage
	{
		public MapPage(string role, Building building)
		{
			InitializeComponent();
			//message.Text = "You are a" + role + ", and going to" + building "
			Device.BeginInvokeOnMainThread(() =>
			{
				DisplayAlert("Welcome", "You Are In MSU Campus", "OK");
			});
			var map = new Xamarin.Forms.GoogleMaps.Map(
				MapSpan.FromCenterAndRadius(
						new Position(0, 0), Distance.FromMiles(0.3)))
			{
				IsShowingUser = true,
				HeightRequest = 100,
				WidthRequest = 960,
				VerticalOptions = LayoutOptions.FillAndExpand
			};

			GetPositions(building, map);
			var button = new Button()
			{
				Text = "Start Directions!",
				Font = Font.SystemFontOfSize(NamedSize.Large),
			};

			var stack = new StackLayout { Spacing = 0, VerticalOptions = LayoutOptions.FillAndExpand };
			stack.Children.Add(map);
			stack.Children.Add(button);
			this.Content = stack;

		}


		public async void GetPositions(Building building, Map map)
		{
			var geocoder = new Xamarin.Forms.GoogleMaps.Geocoder();
			var positions = await geocoder.GetPositionsForAddressAsync(building.Address);

			if (positions.Count() > 0)
			{
				map.MoveToRegion(MapSpan.FromCenterAndRadius(positions.First(), Distance.FromMeters(150)));
				var reg = map.VisibleRegion;

				Pin pin = new Pin
				{
					Type = PinType.Place,
					Label = building.Name,
					Address = building.Address,
					Position = positions.First()
				};

				map.Pins.Add(pin);
				//var format = "0.00";
				//this.Text = $"Center = {reg.Center.Latitude.ToString(format)}, {reg.Center.Longitude.ToString(format)}";
			}
			else
			{
				await this.DisplayAlert("Not found", "Geocoder returns no results", "Close");
			}
		}

	}
}
