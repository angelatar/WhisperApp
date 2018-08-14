using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
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

            searchButton.Click += delegate
             {
                 if (SearchUser(contactName.Text))
                 {
                     contactNameTextView.Enabled = true;
                     contactNameTextView.Click += delegate
                     {
                         QRGeneratorButton.Visibility = ViewStates.Visible;
                         scanQR.Visibility = ViewStates.Visible;
                     };
                     contactNameTextView.Text = contactName.Text;
                 }
             };

            QRGeneratorButton.Click += delegate
            {
                var keys = KeyGenerator();
                var qrtext = string.Format("Username : {0} , key : {1}", contactNameTextView.Text, keys["publicKey"]);
                Intent nextActivity = new Intent(this, typeof(QRCodeActivity));
                nextActivity.PutExtra("qrtext", qrtext);
                StartActivity(nextActivity);

            };

            scanQR.Click += delegate
            {
                Intent nextActivity = new Intent(this, typeof(QRCodeScannerActivity));
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

            var response = client.GetAsync("http://192.168.88.136:61366/api/users/" + contactusername).Result;
            var content = response.Content.ReadAsStringAsync().Result;
            var bee = JsonConvert.DeserializeObject(content);
            return bee.ToString();

        }

        public Bitmap GenerateQR(string text)
        {
            var bw = new ZXing.BarcodeWriter();
            var encOptions = new ZXing.Common.EncodingOptions() { Width = 400, Height = 400, Margin = 0 };
            bw.Options = encOptions;
            bw.Format = ZXing.BarcodeFormat.QR_CODE;
            var result = Bitmap.CreateBitmap(bw.Write(text));

            return result;
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