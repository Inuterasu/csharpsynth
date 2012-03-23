using System;
using System.IO;
/*
 *	Every solution has to have its own c# file StreamLoader.cs and in it StreamLoaderDelegate in order to fill the needs for CSharpSyth stream needs. 
 *	All CSharpSynth should call this delegate in order to get the streams on different platform or engines or development tool targets.
 * 
 * 
 * 
 */
namespace CSharpSynth
{
	static public class StreamLoader
	{
		static public Stream StreamLoaderDelegate(string aPath){
			return null;	
		}
	}
}

