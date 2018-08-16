using Android.App;
using Android.Widget;
using Android.OS;
using IdentityModel.Client;
using System.Net.Http;
using System.Net.Http.Headers;
using Android.Content;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

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
                    var passwordhash = this.ComputeSha256Hash(password.Text);
                    var userInfo = CheckUser(username.Text, passwordhash);
                    if (userInfo != null)
                    {
                        saveset(username.Text, passwordhash);
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
            var discoveryClient = new DiscoveryClient("http://10.27.249.82:59447"); //discover the IdentityServer
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

            var response = client.GetAsync("http://10.27.249.82:61366/api/users/"+username).Result;
            var content = response.Content.ReadAsStringAsync().Result;
            var bee = JsonConvert.DeserializeObject(content);
            if (bee == null)
                return null;
            return bee.ToString();

        }

        private string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}

