//Set this in CompileSymbols in project properties //
//#define MONODEVELOP
//#define VISUALSTUDIO
//#define UNITY
//#define XNA

#define DEBUG_LEVEL_LOG
#define DEBUG_LEVEL_WARN
#define DEBUG_LEVEL_ERROR

using System;
using System.IO;
#if UNITY
using UnityEngine;
#endif

/* Different engines has different methods for log and debug information. We need to provide unified approach. */
 
namespace CSharpSynth
{
	public static class DBG
	{
	
#if VISUALSTUDIO || MONODEVELP || XNA
	[System.Diagnostics.Conditional( "DEBUG_LEVEL_LOG" )]
	[System.Diagnostics.Conditional( "DEBUG_LEVEL_WARN" )]
	[System.Diagnostics.Conditional( "DEBUG_LEVEL_ERROR" )]
	public static void log( object format, params object[] paramList )
	{
		if( format is string )
			 System.Diagnostics.Debug.WriteLine( string.Format( format as string, paramList ) );
		else
			System.Diagnostics.Debug.WriteLine( format );
	}
	
	
	[System.Diagnostics.Conditional( "DEBUG_LEVEL_WARN" )]
	[System.Diagnostics.Conditional( "DEBUG_LEVEL_ERROR" )]
	public static void warn( object format, params object[] paramList )
	{
		if( format is string )
			 System.Diagnostics.Debug.WriteLine( string.Format( format as string, paramList ) );
		else
			System.Diagnostics.Debug.WriteLine( format );
	}
	
	
	[System.Diagnostics.Conditional( "DEBUG_LEVEL_ERROR" )]
	public static void error( object format, params object[] paramList )
	{
		if( format is string )
			 System.Diagnostics.Debug.WriteLine( string.Format( format as string, paramList ) );
		else
			System.Diagnostics.Debug.WriteLine( format );
	}
	
	
	[System.Diagnostics.Conditional( "UNITY_EDITOR" )]
	[System.Diagnostics.Conditional( "DEBUG_LEVEL_LOG" )]
	public static void assert( bool condition )
	{
        System.Diagnostics.Debug.Assert(condition);
	}

	
	[System.Diagnostics.Conditional( "UNITY_EDITOR" )]
	[System.Diagnostics.Conditional( "DEBUG_LEVEL_LOG" )]
	public static void assert( bool condition, string assertString )
	{
        System.Diagnostics.Debug.Assert(condition, assertString);
	}

	
	[System.Diagnostics.Conditional( "UNITY_EDITOR" )]
	[System.Diagnostics.Conditional( "DEBUG_LEVEL_LOG" )]
	public static void assert( bool condition, string assertString, bool pauseOnFail )
	{
		//if( !condition )
		//{
		//	Debug.LogError( "assert failed! " + assertString );
		//	
		//	if( pauseOnFail )
		//		Debug.Break();
		//}
	}	
#endif

#if UNITY
	[System.Diagnostics.Conditional( "DEBUG_LEVEL_LOG" )]
	[System.Diagnostics.Conditional( "DEBUG_LEVEL_WARN" )]
	[System.Diagnostics.Conditional( "DEBUG_LEVEL_ERROR" )]
	public static void log( object format, params object[] paramList )
	{
		if( format is string )
			Debug.Log( string.Format( format as string, paramList ) );
		else
			Debug.Log( format );
	}
	
	
	[System.Diagnostics.Conditional( "DEBUG_LEVEL_WARN" )]
	[System.Diagnostics.Conditional( "DEBUG_LEVEL_ERROR" )]
	public static void warn( object format, params object[] paramList )
	{
		if( format is string )
			Debug.LogWarning( string.Format( format as string, paramList ) );
		else
			Debug.LogWarning( format );
	}
	
	
	[System.Diagnostics.Conditional( "DEBUG_LEVEL_ERROR" )]
	public static void error( object format, params object[] paramList )
	{
		if( format is string )
			Debug.LogError( string.Format( format as string, paramList ) );
		else
			Debug.LogError( format );
	}
	
	
	[System.Diagnostics.Conditional( "UNITY_EDITOR" )]
	[System.Diagnostics.Conditional( "DEBUG_LEVEL_LOG" )]
	public static void assert( bool condition )
	{
		assert( condition, string.Empty, true );
	}

	
	[System.Diagnostics.Conditional( "UNITY_EDITOR" )]
	[System.Diagnostics.Conditional( "DEBUG_LEVEL_LOG" )]
	public static void assert( bool condition, string assertString )
	{
		assert( condition, assertString, false );
	}

	
	[System.Diagnostics.Conditional( "UNITY_EDITOR" )]
	[System.Diagnostics.Conditional( "DEBUG_LEVEL_LOG" )]
	public static void assert( bool condition, string assertString, bool pauseOnFail )
	{
		if( !condition )
		{
			Debug.LogError( "assert failed! " + assertString );
			
			if( pauseOnFail )
				Debug.Break();
		}
	}
#endif
		
	}
}

