using System;
using System.Net;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Plugin.Geolocator;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
namespace GMPark
{
	public partial class App : Application
	{
		public static MasterDetailPage MasterDetailPage;
		public static MenuPage Menu;
		public GMTEMap map;

		public App()
		{
			InitializeComponent();

			Menu = new MenuPage();

			MasterDetailPage = new MasterDetailPage
			{
				Master = Menu,
				Detail = new NavigationPage(new Main("")),
			};

			MainPage = MasterDetailPage;

		}

		protected override void OnStart()
		{
			// Handle when your app starts
		}

		protected override void OnSleep()
		{
			/*if (Application.Current.Properties.ContainsKey("map"))
			{
				map = (GMTEMap)Application.Current.Properties["map"];
			}
			    
			CrossGeolocator.Current.PositionChanged += (o, args) =>
			{
				if (!Application.Current.Properties.ContainsKey("notification"))
				{
					Application.Current.Properties["notification"] = 0;
				}

				if ((map.CheckInGeofences(args.Position))
					&& (onCampus == false) && ((int)Application.Current.Properties["notification"] == 0))
				{
					Device.BeginInvokeOnMainThread(() =>
					{
						mCurrentCampus = map.InWhichGeofences(args.Position);
						DisplayAlert("Welcome to " + mCurrentCampus + "!", "We hope you find your way around!", "Okay");
						onCampus = true;
						Application.Current.Properties["notification"] = 1;
					});
				}

				else if ((map.CheckInGeofences(args.Position) == false)
						 && (onCampus == true) && ((int)Application.Current.Properties["notification"] == 1))
				{
					Device.BeginInvokeOnMainThread(() =>
					{
						DisplayAlert("Now leaving " + mCurrentCampus, "Did you mean to do that?", "Maybe?");
						onCampus = false;
						mCurrentCampus = "";
						Application.Current.Properties["notification"] = 0;
					});
				}
			};*/
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}
	}
}
