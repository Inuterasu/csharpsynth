//#define MONODEVELOP
//#define VISUALSTUDIO
#define UNITY
//#define XNA

using System;
using System.IO;
#if UNITY
using UnityEngine;
#endif

/* All CSharpSynth should call this delegate in order to get the streams on different platform or engines or development tool targets. */
 
namespace CSharpSynth
{
	static public class PlatformHelper
	{
		
#if !MONODEVELOP && !VISUALSTUDIO && !UNITY && !XNA
		static public Stream StreamLoad(string aPath){
			return File.Open(aPath, FileMode.Open);	
		}
		
		static public bool FileExists(string aPath){
			return File.Exists(aPath);	
		}
#endif
		
#if MONODEVELOP
		static public Stream StreamLoad(string aPath){
			return File.Open(aPath, FileMode.Open);
		}
		
		static public bool FileExists(string aPath){
			return File.Exists(aPath);	
		}
#endif
		
#if VISUALSTUDIO
		static public Stream StreamLoad(string aPath){
			return File.Open(aPath, FileMode.Open);	
		}
		
		static public bool FileExists(string aPath){
			return File.Exists(aPath);	
		}
#endif

#if UNITY
		static public Stream StreamLoad(string aPath){
			TextAsset ta = Resources.Load(aPath, typeof(TextAsset)) as TextAsset;
			if(ta != null){
				return new MemoryStream(ta.bytes);	
			}
			return null;	
		}
		
		static public bool FileExists(string aPath){
			return true;	
		}
#endif
		
#if XNA
		static public Stream StreamLoad(string aPath){
			return File.Open(aPath, FileMode.Open);		
		}
		
		static public bool FileExists(string aPath){
			return File.Exists(aPath);	
		}
#endif
		
	}
}

