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