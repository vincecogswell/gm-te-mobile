/* Authot : Phyllis Jin
 * Interface for function text to speech in share code
 */
using System;
namespace GMPark
{
	public interface ITextToSpeech
	{
		void Speak(string text);
	}
}
