using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Xamarin.Essentials;

namespace DrinkSafe.Android
{
    [Activity(Label = "DrinkSafe", Theme = "@style/MainTheme", MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : BaseActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Xamarin.FormsGoogleMaps.Init (this, savedInstanceState);
            LoadApplication(new App());

            GetPermissions(_permList).GetAwaiter();
        }
        
        public async Task GetPermissions(IList<Type> permList)
        {
            foreach (var perm in permList)
            {
                MethodInfo method = typeof(MainActivity).GetMethod("ObtainPermission");
                method = method?.MakeGenericMethod(perm);

                var tsk = (Task) method?.Invoke(this, null);

                if (tsk != null)
                {
                    await tsk.ConfigureAwait(false);
                }
            }
        }
        
        public async Task ObtainPermission<T>() where T : Permissions.BasePlatformPermission, new()
        {
            var status = await Permissions.CheckStatusAsync<T>();
            if (status != PermissionStatus.Granted && status != PermissionStatus.Disabled)
            {
                status = await Permissions.RequestAsync<T>();
            }
        }
        
        private readonly IList<Type> _permList = new List<Type>
        {
            typeof(Permissions.Maps),
            typeof(Permissions.LocationWhenInUse),
            typeof(Permissions.LocationAlways),
        };
    }
}