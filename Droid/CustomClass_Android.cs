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

