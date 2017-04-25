using System;

using Xamarin.Forms;

namespace GMPark
{
	public class temp : ContentPage
	{
		public temp()
		{
			Content = new StackLayout
			{
				Children = {
					new Label { Text = "Hello ContentPage" }
				}
			};
		}
	}
}

