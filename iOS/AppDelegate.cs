using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

namespace GMPark.iOS
{
	[Register("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			global::Xamarin.Forms.Forms.Init();
			Xamarin.FormsGoogleMaps.Init("AIzaSyDNKV7LUQ_dTh1BGfM4ltK5LbAFzd8EBUY");

			LoadApplication(new App());

			return base.FinishedLaunching(app, options);
		}
	}
}
