using System;

/*
 * Using DependencyService requires an interface of the functionality
 * you want to implement.
 * 
 * Interfaces help to create a consistent architecture for specifying
 * xplatform feature sets with platform-specific implementations.
 */
namespace XPlatformArchTut
{
	public interface ICustomClass
	{
		string GoNative(string param);
	}
}

