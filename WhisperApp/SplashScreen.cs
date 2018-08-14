using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace WhisperApp
{
    [Activity(Label = "Whisper", MainLauncher = true, Theme = "@style/Theme.Splash", NoHistory = true, Icon = "@drawable/Whisper_Icon_3")]
    public class SplashScreen : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here

            //Display Splash Screen for 4 Sec  
            Thread.Sleep(1000);
            //Start Activity1 Activity

            if (IsLogged())
                StartActivity(typeof(WelcomeActivity));
            else
                StartActivity(typeof(SignInActivity));

            //StartActivity(typeof(CommunicationActivity));
        }

        protected bool IsLogged()
        {
            //retreive 
            var prefs = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
            var username = prefs.GetString("username", null);
            var password = prefs.GetString("password", null);
            if (username != null && password != null)
                return true;
            return false;
            //Show a toast
            //RunOnUiThread(() => Toast.MakeText(this, somePref, ToastLength.Long).Show());

        }
    }
}