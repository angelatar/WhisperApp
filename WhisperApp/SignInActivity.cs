using Android.App;
using Android.Widget;
using Android.OS;
using IdentityModel.Client;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using Android.Content;
using System;
using Newtonsoft.Json;

namespace WhisperApp
{
    [Activity(Label = "Whisper", Icon = "@drawable/Whisper_Icon_3")]
    public class SignInActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.SignIn);

            var signupTxt = FindViewById<TextView>(Resource.Id.textViewSignUp);

            signupTxt.Clickable = true;

            signupTxt.Click += delegate {
                Intent nextActivity = new Intent(this, typeof(SignUpActivity));
                StartActivity(nextActivity);
            };

            var signinbtn = FindViewById<Button>(Resource.Id.buttonSignIn);

            var username = FindViewById<EditText>(Resource.Id.editTextUsername);
            var password = FindViewById<EditText>(Resource.Id.editTextPassword);


            signinbtn.Click += delegate
            {
                if (username.Text != "" && password.Text != "")
                {
                    var userInfo = CheckUser(username.Text, password.Text);
                    if (userInfo != null)
                    {
                        saveset(username.Text, password.Text);
                        Intent nextActivity = new Intent(this, typeof(WelcomeActivity));
                        nextActivity.PutExtra("Username", userInfo);
                        StartActivity(nextActivity);
                    }
                    else
                    {
                        FindViewById<TextView>(Resource.Id.InvalidtextView).Visibility = Android.Views.ViewStates.Visible;
                    }
                }
            };
        }

        protected void saveset(string username,string password)
        {
            //store
            var prefs = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
            var prefEditor = prefs.Edit();
            prefEditor.PutString("username", username);
            prefEditor.PutString("password", password);
            prefEditor.Commit();
        }


        private static string CheckUser(string usn, string pass)
        {
            var discoveryClient = new DiscoveryClient("http://192.168.88.136:59447"); //discover the IdentityServer
            discoveryClient.Policy.RequireHttps = false;

            var identityServer = discoveryClient.GetAsync().Result;

            if (identityServer.IsError)
            {
                return null;
            }

            var username = usn;
            var password = pass;


            //Get the token
            var tokenClient = new TokenClient(identityServer.TokenEndpoint, "ChatClient", "secret");
            var tokenResponse = tokenClient.RequestResourceOwnerPasswordAsync(username, password, "UserAPI").Result;
            //Call the API

            HttpClient client = new HttpClient();
            client.SetBearerToken(tokenResponse.AccessToken);
            client.DefaultRequestHeaders.Accept.Add(
                 new MediaTypeWithQualityHeaderValue("application/json"));

            var response = client.GetAsync("http://192.168.88.136:61366/api/users/"+username).Result;
            var content = response.Content.ReadAsStringAsync().Result;
            var bee = JsonConvert.DeserializeObject(content);
            if (bee == null)
                return null;
            return bee.ToString();

        }
    }
}

