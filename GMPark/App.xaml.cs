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
		public App()
		{
			InitializeComponent();

			//MainPage = new NavigationPage(new SplashScreen());
			MasterDetailPage = new MasterDetailPage
			{
				Master = new MenuPage(),
				Detail = new NavigationPage(new Main("Warren Tech Center",new Position(42.515062, -83.038084))),
			};
			MainPage = MasterDetailPage;

		}

		protected override void OnStart()
		{
			// Handle when your app starts
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}
	}
}
