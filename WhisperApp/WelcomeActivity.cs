using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.Design.Widget;
using Android.Widget;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Java.Lang;

namespace WhisperApp
{
    [Activity(Label = "WelcomeActivity", Theme = "@style/MyTheme")]
    public class WelcomeActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        private Android.Support.V4.Widget.DrawerLayout drawerLayout;
        private NavigationView navView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Welcome);

            // Create your application here 
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            //SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetHomeAsUpIndicator(null);//(Resource.Drawable.icon);
            drawerLayout = FindViewById<Android.Support.V4.Widget.DrawerLayout>(Resource.Id.drawer_layout);
            navView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navView.SetNavigationItemSelectedListener(this);

            var username = FindViewById<TextView>(Resource.Id.textViewUsername);

            username.Text = GetUsername();

            var getRequest = new Task<string>(() =>
            {

                //Console.WriteLine("Waiting for request!");
                while (true)
                {
                    Thread.Sleep(1000);
                    var client = new HttpClient();
                    client.DefaultRequestHeaders.Accept.Add(
                         new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = client.GetAsync(string.Format("http://localhost:63653/api/CallingRequest?id={0}", 8)).Result;
                    var content = response.Content.ReadAsStringAsync().Result;
                    //Console.WriteLine(content);
                    if (content != "" || content != null || content != "[]")
                        return content;
                }
            }
);

            getRequest.Start();
        }

        public bool OnNavigationItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.nav_add_contact:
                    Intent addContAcitvity = new Intent(this, typeof(AddContactActivity));
                    StartActivity(addContAcitvity);
                    return true;
                case Resource.Id.nav_sign_out:
                    saveset();
                    Intent signOutActivity = new Intent(this, typeof(SignInActivity));
                    StartActivity(signOutActivity);
                    return true;
                case Resource.Id.nav_contacts:
                    Intent contlist = new Intent(this, typeof(ContactListActivity));
                    StartActivity(contlist);
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        protected void saveset()
        {
            //store
            var prefs = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
            var prefEditor = prefs.Edit();
            prefEditor.PutString("username", null);
            prefEditor.PutString("password", null);
            prefEditor.Commit();
        }
        protected string GetUsername()
        {
            var prefs = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
            var username = prefs.GetString("username", null);
            return username;
        }
    }
}