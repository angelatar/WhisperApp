using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using IdentityModel.Client;

namespace WhisperApp
{
    [Activity(Label = "Whisper")]
    public class CommunicationActivity : Activity
    {
        TextView message;

        TextView mymessage;

        Button send;

        EditText enteredmessage;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here

            SetContentView(Resource.Layout.communication);

            //Android.Media.AudioSource.VoiceCall

            var contname = FindViewById<TextView>(Resource.Id.textView1);
            contname.Text = Intent.GetStringExtra("name");

            message = FindViewById<TextView>(Resource.Id.textView2);

            mymessage = FindViewById<TextView>(Resource.Id.textView3);

            send = FindViewById<Button>(Resource.Id.button1);

            enteredmessage = FindViewById<EditText>(Resource.Id.editText1);


            var identityClient = new DiscoveryClient("http://10.27.249.82:59447"); //discover the IdentityServer
            identityClient.Policy.RequireHttps = false;

            var identityServer = identityClient.GetAsync().Result;

            if (identityServer.IsError)
            {
                return;
            }
            //Get the token
            var prefs = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
            var username = prefs.GetString("username", null);
            var password = prefs.GetString("password", null);

            var tokenClient = new TokenClient(identityServer.TokenEndpoint, "ChatClient", "secret");
            var tokenResponse = tokenClient.RequestResourceOwnerPasswordAsync(username, password, "CallingRequestAPI").Result;


            new System.Threading.Thread(new System.Threading.ThreadStart(() =>
            {
                while (true)
                {
                    var client33 = new HttpClient();
                    client33.SetBearerToken(tokenResponse.AccessToken);
                    client33.DefaultRequestHeaders.Accept.Add(
                         new MediaTypeWithQualityHeaderValue("application/json"));

                    var response22 = client33.GetAsync(string.Format("http://10.27.249.82:63653/api/CallProcess?senderID={0}&receiverID={1}", 8, 1)).Result;
                    var content22 = response22.Content.ReadAsStringAsync().Result;

                    if (content22 != null && content22 != "" && content22 != "[]" && content22 != "null")
                        try
                        {
                            var message1 = FindViewById<TextView>(Resource.Id.textView2);
                            message1.Text = content22.Split(',')[2].Substring(content22.Split(',')[2].LastIndexOf(':')).Replace("\"", "").Replace(":", "").Replace("}", "");
                            var response28 = client33.DeleteAsync(string.Format("http://10.27.249.82:63653/api/CallProcess?senderID={0}&receiverID={1}", 8, 1)).Result;
                            var content28 = response28.Content.ReadAsStringAsync().Result;
                        }
                        catch (System.Exception ex)
                        {

                        }
                }
            })).Start();


            send.Click += delegate
            {

                if (enteredmessage.Text != "")
                {
                    mymessage.Text = enteredmessage.Text;

                    var client8 = new HttpClient();
                    client8.SetBearerToken(tokenResponse.AccessToken);
                    client8.DefaultRequestHeaders.Accept.Add(
                         new MediaTypeWithQualityHeaderValue("application/json"));
                    var content8 = new FormUrlEncodedContent(new[]
                    {
                            new KeyValuePair<string, string>("SenderID", "1"),
                            new KeyValuePair<string, string>("ReceiverID", "8"),
                            new KeyValuePair<string, string>("Traffic", enteredmessage.Text),
                        });
                    var response8 = client8.PostAsync("http://10.27.249.82:63653/api/CallProcess", content8).Result;
                    var content9 = response8.Content.ReadAsStringAsync().Result;

                    enteredmessage.Text = "";
                }
            };

        }

        private Dictionary<string,string> GetKeys(string username)
        {
            return null;
        }

        private string Encrypt(string message,string publickey)
        {
            byte[] mess;
            byte[] encrypted;
            using (var rsa = new RSACryptoServiceProvider())
            {
                //rsa.PersistKeyInCsp = false;
                //rsa.ImportParameters(new RSAParameters(){ p publickey);
                //encrypted = rsa.Encrypt(mess, true);
            }
                return null;
        }

        private string Decrypt()
        {
            return null;
        }
    }
}