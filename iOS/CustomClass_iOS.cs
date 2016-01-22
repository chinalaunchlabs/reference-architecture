using XPlatformArchTut.iOS;

[assembly: Xamarin.Forms.Dependency(typeof(CustomClass_iOS))]
namespace XPlatformArchTut.iOS
{
	public class CustomClass_iOS: ICustomClass
	{
		public CustomClass_iOS ()
		{
		}

		public string GoNative(string param) {
			return "iOS:: " + param;
		}
	}
}

