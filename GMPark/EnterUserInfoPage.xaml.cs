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
		private string campusName = "False";
		private string role = "False";
		private string building = "False";
		// changes commit
		private Position pos;
		public EnterUserInfoPage(Campus campus, Position pos)
		{
			NavigationPage.SetBackButtonTitle(this, "");
			this.campusName = campus.Name;
			this.pos = pos;
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
			var labelC = new Label
			{
				Text = "Campus: ",
				TextColor = Color.White,
				FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
				FontAttributes = FontAttributes.Bold,
				FontFamily = Device.OnPlatform("AppleSDGothicNeo-UltraLight", "Droid Sans Mono", "Comic Sans MS"),
				VerticalOptions = LayoutOptions.Center,
			};
			grid.Children.Add(labelC, 0, 0);
			var labelN = new Label
			{
				Text = this.campusName,
				TextColor = Color.White,
				FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
				FontAttributes = FontAttributes.Bold,
				FontFamily = Device.OnPlatform("AppleSDGothicNeo-UltraLight", "Droid Sans Mono", "Comic Sans MS"),
				VerticalOptions = LayoutOptions.Center,
			};
			grid.Children.Add(labelN, 1, 0);
			var labelR = new Label
			{
				Text = "Role: ",
				TextColor = Color.White,
				FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
				FontAttributes = FontAttributes.Bold,
				FontFamily = Device.OnPlatform("AppleSDGothicNeo-UltraLight", "Droid Sans Mono", "Comic Sans MS"),
				VerticalOptions = LayoutOptions.Center,
			};
			grid.Children.Add(labelR, 0, 1);
			var rolePicker = new Picker
			{
				Title = "Select Your Role",
				VerticalOptions = LayoutOptions.CenterAndExpand
			};
			foreach (string r in campus.Roles)
			{
				rolePicker.Items.Add(r);
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
				FontFamily = Device.OnPlatform("AppleSDGothicNeo-UltraLight", "Droid Sans Mono", "Comic Sans MS"),
				VerticalOptions = LayoutOptions.Center,
			};
			grid.Children.Add(labelB, 0, 2);
			var buildingPicker = new Picker
			{
				Title = "Select Your Building",
				VerticalOptions = LayoutOptions.CenterAndExpand
			};
			foreach (Building b in campus.Buildings)
			{
				buildingPicker.Items.Add(b.Name);
			}
			if (Application.Current.Properties.ContainsKey("role"))
			{
				int i = 0;
				foreach (string item in rolePicker.Items)
				{
					if (item == (string)Application.Current.Properties["role"])
					{
						rolePicker.SelectedIndex = i;
					}
					i += 1;
				}
			}
			if (Application.Current.Properties.ContainsKey("building"))
			{
				int i = 0;
				foreach (string item in buildingPicker.Items)
				{
					if (item == (string)Application.Current.Properties["building"])
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
			//Button saveButton = new SaveButton(this.campusName,this.pos,this.role,this.building);
			Button saveButton = new Button { 
				Text = "Save",
				Font = Font.SystemFontOfSize(NamedSize.Large),
				TextColor = Color.White,
				BackgroundColor = Color.Transparent,
				FontFamily = Device.OnPlatform("AppleSDGothicNeo-UltraLight", "Droid Sans Mono", "Comic Sans MS"),
				BorderWidth = 1,
				BorderColor = Color.White,
				Margin = new Thickness(4,4,4,4)
			};
			saveButton.Clicked += OnClicked;
			//grid.Children.Add(saveButton, 0, 3);
			//Grid.SetColumnSpan(saveButton, 2);
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
			if (this.campusName != "False" && this.role != "False" && this.building != "False")
			{
				Application.Current.Properties["campus"] = this.campusName;
				Application.Current.Properties["role"] = this.role;
				Application.Current.Properties["building"] = this.building;
				App.MasterDetailPage.Detail = new NavigationPage(new Main(this.campusName, this.pos));
				App.MasterDetailPage.IsPresented = false;
			}
			else
			{
				await DisplayAlert("Alert!", "You missed something", "OK");
			}

		}
	}
}
