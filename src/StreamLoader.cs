//#define MONODEVELOP
//#define VISUALSTUDIO
//#define UNITY
//#define XNA

using System;
using System.IO;
#if UNITY
using UnityEngine;
#endif

/* All CSharpSynth should call this delegate in order to get the streams on different platform or engines or development tool targets. */
 
namespace CSharpSynth
{
	static public class StreamLoader
	{
		
#if !MONODEVELOP && !VISUALSTUDIO && !UNITY && !XNA
		static public Stream StreamLoaderDelegate(string aPath){
			return null;	
		}
#endif
		
#if MONODEVELOP
		static public Stream StreamLoaderDelegate(string aPath){
			return null;	
		}
#endif
		
#if VISUALSTUDIO
		static public Stream StreamLoaderDelegate(string aPath){
			return null;	
		}
#endif

#if UNITY
		static public Stream StreamLoaderDelegate(string aPath){
			return null;	
		}
#endif
		
#if XNA
		static public Stream StreamLoaderDelegate(string aPath){
			return null;	
		}
#endif
		
	}
}

