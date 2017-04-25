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
			if (Application.Current.Properties.ContainsKey("map"))
			{
				map = (GMTEMap)Application.Current.Properties["map"];
			}

			else
			{
				map = new GMTEMap()
				{
					IsShowingUser = true,
					HeightRequest = 100,
					WidthRequest = 960,
					VerticalOptions = LayoutOptions.FillAndExpand,
					HasZoomEnabled = true
				};
			}

			List<int> mLotOrder;
			int mGoingTo;
			bool onCampus = false;
			string mCurrentCampus = "";
			string mCampusName, mBuildingName;
			string mCurrentLot = "";
			string mLotParked = "";
			string mRole = "";
			bool mTimerStarted = false;
			double mTimerLength = 0;
			bool mParked = false;
			bool inLot = false;
			bool mShown = false;
			double MPH = 2.2352;
			double TimerMax = 90;
			    
			CrossGeolocator.Current.PositionChanged += (o, args) =>
			{
				if ((map.CheckInGeofences(args.Position))
				    && (onCampus == false))
				{
					Device.BeginInvokeOnMainThread(() =>
					{
						mCurrentCampus = map.InWhichGeofences(args.Position);
                        //DisplayAlert("Welcome to " + mCurrentCampus + "!", "We hope you find your way around!", "Okay");
					});
					onCampus = true;
				}

				else if ((map.CheckInGeofences(args.Position) == false)
				         && (onCampus == true))
				{
					Device.BeginInvokeOnMainThread(() =>
					{
						//DisplayAlert("Now leaving " + mCurrentCampus, "Did you mean to do that?", "Maybe?");
onCampus = false;
						mCurrentCampus = "";
					});
				}

				if (map.CheckInLotGeofences(args.Position, mCurrentCampus) && (mParked == false) && (inLot == false))
				{
					Device.BeginInvokeOnMainThread(() =>
					{
						mCurrentLot = map.InWhichLot(args.Position, mCurrentCampus);
						//DisplayAlert("You are in lot " + mCurrentLot + "!", "We hope you find a spot!", "Okay");
					});

					inLot = true;
				}

				if ((mParked) && (mShown == false))
				{
					Device.BeginInvokeOnMainThread(() =>
					{
						//DisplayAlert("You Parked!", "We detected that you parked in " + mLotParked, "Okay");
					});
					mShown = true;
				}

				if ((map.InWhichLot(args.Position, mCurrentCampus) != mCurrentLot) && (mCurrentLot != "")
				         && (mTimerStarted == false) && (mParked == false))
				{
					Device.BeginInvokeOnMainThread(() =>
					{
						//DisplayAlert("Now leaving " + mCurrentLot, "Start parking-detection algorithm", "Start");
					});
					mLotParked = mCurrentLot;
					mCurrentLot = "";
					mTimerStarted = true;

					if (map.CheckInLotGeofences(args.Position, mCurrentCampus))
					{
						inLot = true;
					}

					else
					{
						inLot = false;
					}

					//Device.StartTimer(TimeSpan.FromSeconds(.5), new Func<bool>(() => CheckSpeed(args.Position)));
				}

				if ((map.CheckInLotGeofences(args.Position, mCurrentCampus) == false) && (mParked == false))
				{
					mCurrentLot = "";
					inLot = false;
				}
			};
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}
	}
}
