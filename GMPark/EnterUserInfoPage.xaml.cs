/* Author : Phyllis Jin
 * Preference page
 */
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
	public partial class EnterUserInfoPage : ContentPage
	{
		// variables
		private string campusName = "False";
		private string role = "False";
		private string building = "False";
		GMTEMap map;
		private Position pos;

		// constructor
		public EnterUserInfoPage(string campus)
		{
			// initialize map
			map = (GMTEMap)Application.Current.Properties["map"];

			// UI
			NavigationPage.SetBackButtonTitle(this, "");
			this.campusName = campus;
			Title = "Preference";
			this.BackgroundColor = Color.FromRgb(104, 151, 243);
			var scroll = new ScrollView();
			var grid = new Grid { 
				HorizontalOptions = LayoutOptions.CenterAndExpand
			};
			grid.RowDefinitions.Add(new RowDefinition { Height = 80 });
			grid.RowDefinitions.Add(new RowDefinition { Height = 80 });
			grid.RowDefinitions.Add(new RowDefinition { Height = 80 });
			grid.ColumnDefinitions.Add(new ColumnDefinition { Width = 80 });
			grid.ColumnDefinitions.Add(new ColumnDefinition { Width = 200 });

			string font;

			switch (Device.RuntimePlatform)
			{
				case "iOS":
					font = "AppleSDGothicNeo-UltraLight";
					break;
				case "Android": 
					font = "Droid Sans Mono";
					break;
				default:
					font = "Comic Sans MS";
					break;
			}
			// create pickers
			var labelC = new Label
			{
				Text = "Campus: ",
				TextColor = Color.White,
				FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
				FontAttributes = FontAttributes.Bold,
				FontFamily = font,
				VerticalOptions = LayoutOptions.Center,
			};
			grid.Children.Add(labelC, 0, 0);
			var labelN = new Label
			{
				Text = this.campusName,
				TextColor = Color.White,
				FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
				FontAttributes = FontAttributes.Bold,
				FontFamily = font,
				VerticalOptions = LayoutOptions.Center,
			};
			grid.Children.Add(labelN, 1, 0);
			var labelR = new Label
			{
				Text = "Role: ",
				TextColor = Color.White,
				FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
				FontAttributes = FontAttributes.Bold,
				FontFamily = font,
				VerticalOptions = LayoutOptions.Center,
			};
			grid.Children.Add(labelR, 0, 1);
			var rolePicker = new Picker
			{
				Title = "Select Your Role",
				VerticalOptions = LayoutOptions.CenterAndExpand
			};

			var roles = map.GetRoles(campusName);

			foreach (Role r in roles)
			{
				rolePicker.Items.Add(r.GetName());
			}
			rolePicker.SelectedIndexChanged += (sender, args) =>
				{
					if (rolePicker.SelectedIndex == -1)
					{
						this.role = "False";
					}
					else
					{
						this.role = rolePicker.Items[rolePicker.SelectedIndex];
					}
				};
			grid.Children.Add(rolePicker, 1, 1);
			var labelB = new Label
			{
				Text = "Building: ",
				TextColor = Color.White,
				FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
				FontAttributes = FontAttributes.Bold,
				FontFamily = font,
				VerticalOptions = LayoutOptions.Center,
			};
			grid.Children.Add(labelB, 0, 2);
			var buildingPicker = new Picker
			{
				Title = "Select Your Building",
				VerticalOptions = LayoutOptions.CenterAndExpand
			};

			var buildings = map.GetBuildingList(campusName);

			foreach (string b in buildings)
			{
				buildingPicker.Items.Add(b);
			}

			// display selected role if users already saved their preference
			if (Application.Current.Properties.ContainsKey(campus + "role"))
			{
				int i = 0;
				foreach (string item in rolePicker.Items)
				{
					if (item == (string)Application.Current.Properties[campus + "role"])
					{
						rolePicker.SelectedIndex = i;
					}
					i += 1;
				}
			}
			if (Application.Current.Properties.ContainsKey(campus + "building"))
			{
				int i = 0;
				foreach (string item in buildingPicker.Items)
				{
					if (item == (string)Application.Current.Properties[campus + "building"])
					{
						buildingPicker.SelectedIndex = i;
					}
					i += 1;
				}
			}
			buildingPicker.SelectedIndexChanged += (sender, args) =>
				{
					if (buildingPicker.SelectedIndex == -1)
					{
						this.building = "False";
					}
					else
					{
						this.building = buildingPicker.Items[buildingPicker.SelectedIndex];
					}
				};
			grid.Children.Add(buildingPicker, 1, 2);

			// button for saving all user's preference
			Button saveButton = new Button { 
				Text = "Save",
				Font = Font.SystemFontOfSize(NamedSize.Large),
				TextColor = Color.White,
				BackgroundColor = Color.Transparent,
				FontFamily = font,
				BorderWidth = 1,
				BorderColor = Color.White,
				Margin = new Thickness(4,4,4,4)
			};
			saveButton.Clicked += OnClicked;
			var stack = new StackLayout{ 
				HorizontalOptions = LayoutOptions.Center,
				Children = {
					grid,
					new Label {Text=" "},
					saveButton
				}

			};
			scroll.Content = stack;
			Content = scroll;
		}

		async void OnClicked(object sender, EventArgs args)
		{
			// user must select all fields to save their preference successfully
			if (this.campusName != "False" && this.role != "False" && this.building != "False")
			{
				Application.Current.Properties[campusName + "campus"] = this.campusName;
				Application.Current.Properties[campusName + "role"] = this.role;
				Application.Current.Properties[campusName + "building"] = this.building;
				App.MasterDetailPage.Detail = new NavigationPage(new Main(campusName));
				App.MasterDetailPage.IsPresented = false;
			}
			// else display warnning
			else
			{
				await DisplayAlert("Must Enter All Fields", "Please enter all fields", "OK");
			}

		}
	}
}
