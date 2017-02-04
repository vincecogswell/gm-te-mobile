using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace GMPark
{
	public partial class MapPage : ContentPage
	{
		public MapPage(string role, string building)
		{
			InitializeComponent();
			message.Text = "You are a" + role + ", and going to" + building ;

			//var map = new Map(
			//	MapSpan.FromCenterAndRadius(
			//			new Position(37, -122), Distance.FromMiles(0.3)))
			//{
			//	IsShowingUser = true,
			//	HeightRequest = 100,
			//	WidthRequest = 960,
			//	VerticalOptions = LayoutOptions.FillAndExpand
			//};
			//var stack = new StackLayout { Spacing = 0 };
			//stack.Children.Add(map);
			//Content = stack;
		}
	}
}
