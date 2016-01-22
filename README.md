# Cross-Platform Architecture
* A Xamarin app has two types of code
	* **Shared**: used by all platforms (xplatform)
	* **Platform-specific**: used by one OS, but not both

* Top-down approach:
	* **Portable Class Libraries (PCLs)** are compilable into a single DLL used by multiple platforms
	* **Shared Projects** are recompiled in different platform contexts

* Bottom-up approach:
	* **conditional compilation** PS compilation demarcation around small blocks of code in a *Shared Project*
		* I assume this is like `#IFDEF __ANDROID__ #ENDIF`
		* not available in *PCL* since the dll is precompiled
		* *dependency injection* can be used to create platform-specific classes against a common interface
	* **file linking** is used to share specific files between projects

* **Divergence** describes the need for platform-specific (PS) implementations in a cross-platform app because PS differences cause their implementations to diverge from the main code in a solution.
	* eg. UI, networking/push notifications need local OS API access

## Handling Divergence
*"I am Divergent. I cannot be controlled."* /nocontext
* *project* level: use PS projects
* *file* level: file linking, sometimes with partial classes or methods
* *class* level: dependency injection in PCLs
* *method* level: partial methods
* *code* level: conditional compilation for individual lines of PS code

## Different Solutions
### 1. Xamarin.Forms
* **shared**: UI code in X.Forms project, DAL and BLL in Core Library
* **platform-specific**: startup code, custom renderers, services, notifications, sensors, or networking

### 2. Platform-Specific
* **shared**: app logic such as BLL and DAL using PCL or shared project
* **platform-specific**: platform-specific code for each platform (including but not limited to UI)

## Core Library
* dedicated project in solution where the DAL, BLL (Business Logic Layer) and other non-UI platform-independent code reside
* typically implemented using PCLs or Shared Projects
* a cross-platform catch-all for non-UI files, folders, and classes
* usually includes:
	* **DAL**: data access layer that may include SQLite access, data models, view models, repositories, cloud data access and web services
	* **BLL**: business logic that cuts across and is independent of platforms
		* *what does this even*
	* **misc**: utilities, interfaces, crossplatform resources, sundry necessities
* functions that need to be placed in PS projects:
	* certain types of local file access
	* OS services
	* if only a few lines of code, you can use conditional compilation or dependency injection

## Portable Class Libraries (PCL)
* code projects that provide a built-in subset of the .NET Framework
* can be compiled into a DLL once then run on all target platforms
* makes sense to make when it is to be distributed to other developers
* are configured at compile-time to target particular platforms using a profile
* once the DLL is compiled, PS customization requires an extra bit of work
* in a PCL, you cannot add or link files nor use partial classes, partial methods, or conditional compilation for platform-specific implementations
* customization is done using dependency injection

## Dependency Injection
* design principle that helps developers include PS functionality into an otherwise xplatform class using **Inversion of Control (IoC)**
* IoC patterns are framework calls to specific implementations of general classes provided by the application, the implementation is passed into a constructor/setter
* useful for PS functions like:
	* custom renderers
	* file handling
	* bg services
	* sensors
* how do:
	1. create an interface to define the methods and patterns to be implemented in each platform
	2. implement PS subclasses of the base class in each respective PS project
	3. inject PS implementations in shared code
* few ways to implement DI design principle:
	* interfaces
	* abstract classes
	* inheritance
	* `DependencyService` is built into X.Forms, implements DI using interfaces

### `DependencyService`
* allows you to create a base interface then build PS implementation to be invoked in shared code
* how do:
	1. **Interface**: An interface in the shared code declares the class for PS implementation.


	XFormsProj/**ICustomClass.cs**
	```
	public interface ICustomClass
	{
		string GoNative(string param);
	}
	```


	2. **Implementation**: PS implementations of the interface are registered using `[assembly]` tags.

		DroidProj/**CustomClass_Android.cs**
	```
	using XPlatformArchTut.Droid;

	[assembly: Xamarin.Forms.Dependency(typeof(CustomClass_Android))]
	namespace XPlatformArchTut.Droid
	{
		public class CustomClass_Android: ICustomClass
		{
			public CustomClass_Android ()
			{
			}

			public string GoNative(string param) {
				return "Android:: " + param;
			}
		}
	}
	```

	3. **Invocation**: PS code is invoked from the shared code using `DependencyService.Get<InterfaceName>.MethodName`.

	```
	var text = DepdendencyService.Get<ICustomClass>()
		.GoNative("platform-specific implementation complete!");
	```

## Shared Projects
* like the PCL, this is where a core library or X.Forms project can be housed for use in different platform contexts
	* `File` **>** `New Solution` in Xamarin Studio or Visual Studio 2013 Update 2
* useful when used by a single developer since it produces only a shareable code project, not a DLL
* handling divergence:
	* dependency injection (discussed above)
	* **conditional compilation**: compiler directives for small, code-level amounts of divergence
	* **file linking**: project file `include` for file or class-sized levels of divergence
	* **partial classes**: using `partial` keyword for class divergence 
	* **partial method**: using `partial` keyword for method divergence

### Conditional Compilation
* smallest granularity of divergence (line by line)

iOS:
```
#if __IOS__
// iOS-specific code
#endif
```

Android:
```
#if __ANDROID__
// Android-specific code
#endif
```

iOS | Android:
```
#if __MOBILE__
// code
#endif
```

Android API level:
```
#if __ANDROID_xx__
//code
#endif
```

### File Linking
* used for small or prototype PS apps where a lightweight alternative to a core lib is needed
* you can put all shared files into a single project and then link to those files from the other PS projects

### Partial Classes
* [better reference](https://msdn.microsoft.com/library/wa80x488%28v=vs.110%29.aspx)
* splits the definition of the class
* enables multiple programmers to work on it at the same time
* useful for extending a shared, xplatform class with PS functionality 
* work in Shared Projects and linked files but not PCLs
* how do:
	* create partial class in shared project:
	```
	public partial class Utility {
		public void DoCrossPlatformThing() {
		}
	}
	```

	* extend class in PS project
	```
	public partial class Utility {
		public void DoPlatformSpecificThing() {
		}
	}
	```

### Partial Methods
* partial methods don't provide xplatform implementation, only PS
* enable the implementer of one part of the class to define a method, similar to an event
	* the implementer of the other part of the class can decide whether or not to implement it or not
	* if not implemented, method signature and all calls to it are removed at compile time
* **must return `void`**
	* this makes sense because if other parts of the program rely on the return value of a method that may be removed at compile time, we're fucked
	* in the same vein, can have `ref` parameters but not `out`
* how do:
	* partial method in shared project, but do not provide implementation:
	```
	partial void DoThing();
	```

	* implement method in PS project
	```
	partial void DoThing() {
		// code pa more
	}
	```

## Version Divergence
* divergence is not only limited to device but also OS versions because screw you and your impossible standards, that's why
* detecting SDK level in Android:
```
if (((int) Android.OS.Build.Version.SdkInt) >= 2) {
	// stuff here	
}
```	
	* see also conditional compiling on Android
* on iOS:
```
if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
{
	// check if version 8.0 or newer
}
```