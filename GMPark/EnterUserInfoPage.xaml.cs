using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace GMPark
{
	public partial class EnterUserInfoPage : ContentPage
	{
		public EnterUserInfoPage()
		{
			NavigationPage.SetBackButtonTitle(this, "");

			Title = "Preference";
			this.BackgroundColor = Color.FromRgb(104, 151, 243);
		}


	}
}
