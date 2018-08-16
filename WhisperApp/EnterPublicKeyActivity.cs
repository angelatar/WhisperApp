using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;

namespace WhisperApp
{
    [Activity(Label = "Whisper")]
    public class EnterPublicKeyActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here

            SetContentView(Resource.Layout.enterpublickey);

            var enterbutton = FindViewById<Button>(Resource.Id.enterbutton);
            var publickey = FindViewById<EditText>(Resource.Id.publickeyeditText);

            enterbutton.Click += delegate
             {
                 if (publickey.Text != "")
                 {
                     var username = Intent.GetStringExtra("username");
                     var myPrivateKey = Intent.GetStringExtra("myPrivateKey");
                     saveset(username, publickey.Text, myPrivateKey);
                     Intent nextActivity = new Intent(this, typeof(WelcomeActivity));
                     StartActivity(nextActivity);
                 }
             };

        }

        protected void saveset(string username, string contpublickey, string myprivatekey)
        {
            //store
            var prefs = Application.Context.GetSharedPreferences("Contacts", FileCreationMode.Private);
            var count = prefs.GetInt("Count", 0);
            var prefEditor = prefs.Edit();
            prefEditor.PutString(string.Format("cont{0}",count+1), username);
            prefEditor.PutString(string.Format("cont{0} public key",count+1), contpublickey);
            prefEditor.PutString(string.Format("my private key with cont{0}",count+1), myprivatekey);
            prefEditor.PutInt("Count", count + 1);
            prefEditor.Commit();
        }
    }
}