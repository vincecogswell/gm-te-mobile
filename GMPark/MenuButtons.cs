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
	public class MenuButtons: Button
	{
		public MenuButtons(string name, Position pos)
		{
			Text = name;
			Font = Font.SystemFontOfSize(NamedSize.Large);
			TextColor = Color.White;
			BorderWidth = 1;
			BorderColor = Color.White;
			Margin = new Thickness(8, 8, 8, 8);
			BackgroundColor = Color.Transparent;
			FontFamily = Device.OnPlatform("AppleSDGothicNeo-UltraLight", "Droid Sans Mono", "Comic Sans MS");
			Command = new Command(o =>
			{
				App.MasterDetailPage.Detail = new NavigationPage(new Main(name,pos));
				App.MasterDetailPage.IsPresented = false;
			});
		}
	}
}
