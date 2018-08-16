using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using IdentityModel.Client;
using Newtonsoft.Json;

namespace WhisperApp
{
    [Activity(Label = "Whisper")]
    public class AddContactActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here

            SetContentView(Resource.Layout.AddContact);

            var contactName = FindViewById<EditText>(Resource.Id.searchusername);
            var searchButton = FindViewById<Button>(Resource.Id.searchbutton);
            var contactNameTextView = FindViewById<TextView>(Resource.Id.userTextView);
            var QRGeneratorButton = FindViewById<Button>(Resource.Id.qrgeneratorbutton);
            var scanQR = FindViewById<Button>(Resource.Id.scanqrButton);
            var enterpublickey = FindViewById<Button>(Resource.Id.enterpublickeyButton);
            Dictionary<string, string> keys = null;

            searchButton.Click += delegate
             {
                 if (SearchUser(contactName.Text))
                 {
                     contactNameTextView.Enabled = true;
                     contactNameTextView.Click += delegate
                     {
                         QRGeneratorButton.Visibility = ViewStates.Visible;
                         scanQR.Visibility = ViewStates.Visible;
                         enterpublickey.Visibility = ViewStates.Visible;
                         keys = KeyGenerator();
                     };
                     contactNameTextView.Text = contactName.Text;
                 }
             };

            QRGeneratorButton.Click += delegate
            {
                var qrtext = keys["publicKey"];
                Intent nextActivity = new Intent(this, typeof(QRCodeActivity));
                nextActivity.PutExtra("qrtext", qrtext);
                StartActivity(nextActivity);
            };

            scanQR.Click += delegate
            {
                Intent nextActivity = new Intent(this, typeof(QRCodeScannerActivity));
                StartActivity(nextActivity);
            };

            enterpublickey.Click += delegate
            {
                Intent nextActivity = new Intent(this, typeof(EnterPublicKeyActivity));
                nextActivity.PutExtra("username", contactNameTextView.Text);
                nextActivity.PutExtra("myPrivateKey", keys["privateKey"]);
                StartActivity(nextActivity);
            };
        }

        public bool SearchUser(string contactusername)
        {
            var prefs = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
            var username = prefs.GetString("username", null);
            var password = prefs.GetString("password", null);
            if (contactusername == username)
                return false;
            var result = FindUser(username, password, contactusername);
            if (result == "" || result == null)
                return false;
            else
                return true;
        }

        private static string FindUser(string usn, string pass, string contactusername)
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

            var response = client.GetAsync("http://10.27.249.82:61366/api/users/" + contactusername).Result;
            var content = response.Content.ReadAsStringAsync().Result;
            var bee = JsonConvert.DeserializeObject(content);
            return bee.ToString();

        }

        public Dictionary<string, string> KeyGenerator()
        {
            RSACryptoServiceProvider rSA = new RSACryptoServiceProvider();
            var privateKey = rSA.ToXmlString(true);
            var publicKey = rSA.ToXmlString(false);

            var privatekey = Convert.ToBase64String(Encoding.UTF8.GetBytes(privateKey));
            var publickey = Convert.ToBase64String(Encoding.UTF8.GetBytes(publicKey));

            var keys = new Dictionary<string, string>();

            keys.Add("publicKey", publickey);
            keys.Add("privateKey", privatekey);

            return keys;
        }
    }
}