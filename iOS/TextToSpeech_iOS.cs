/* Authot : Phyllis Jin
 * class for function text to speech that works on iOS platform
 */
using System;
using AVFoundation;
using Xamarin.Forms;
using GMPark.iOS;

[assembly: Dependency(typeof(TextToSpeech_IOS))]

namespace GMPark.iOS
{
	public class TextToSpeech_IOS : ITextToSpeech
	{
		public TextToSpeech_IOS()
		{
		}
		// Overwirte Speak method in share code
		public void Speak(string text)
		{
			var speechSynthesizer = new AVSpeechSynthesizer();

			var speechUtterance = new AVSpeechUtterance(text)
			{
				Rate = AVSpeechUtterance.MaximumSpeechRate / 2,
				Voice = AVSpeechSynthesisVoice.FromLanguage("en-US"),
				Volume = 0.5f,
				PitchMultiplier = 1.0f
			};

			speechSynthesizer.SpeakUtterance(speechUtterance);
		}
	}
}
