using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using IdentityModel.Client;
using Java.Lang;

namespace WhisperApp.Resources
{
    public class ViewHolder : Java.Lang.Object
    {
        public TextView txtUsername { set; get; }
    }

    public class CustomAdapter : BaseAdapter
    {
        private Activity activity;
        private List<Contact> contacts;

        public CustomAdapter(Activity activity, List<Contact> contact)
        {
            this.activity = activity;
            this.contacts = contact;
        }

        public override int Count
        {
            get
            {
                return contacts.Count;
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            return 0;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView ?? activity.
                LayoutInflater.Inflate(Resource.Layout.list_view_dataTemplate, parent, false);

            var txtUsername = view.FindViewById<TextView>(Resource.Id.textView1);
            var communicatebutton = view.FindViewById<Button>(Resource.Id.button1);

            txtUsername.Text = contacts[position].Username;

            txtUsername.Click += delegate
            {
                communicatebutton.Visibility = ViewStates.Visible;
                communicatebutton.Click += delegate
                 {
                     Intent nextActivity = new Intent(view.Context, typeof(CommunicationActivity));

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


                     var client = new HttpClient();
                     client.SetBearerToken(tokenResponse.AccessToken);
                     client.DefaultRequestHeaders.Accept.Add(
                          new MediaTypeWithQualityHeaderValue("application/json"));
                     var content1 = new FormUrlEncodedContent(new[]
                     {
                        new KeyValuePair<string, string>("SenderID", "1"),
                        new KeyValuePair<string, string>("ReceiverID", "8"),
                        new KeyValuePair<string, string>("State", "1"),
                     });
                     var response = client.PostAsync("http://10.27.249.82:63653/api/CallingRequest", content1).Result;
                     var content = response.Content.ReadAsStringAsync().Result;

                     while (true)
                     {
                         Thread.Sleep(1000);
                         var client1 = new HttpClient();
                         client1.SetBearerToken(tokenResponse.AccessToken);
                         client1.DefaultRequestHeaders.Accept.Add(
                              new MediaTypeWithQualityHeaderValue("application/json"));

                         var response1 = client1.GetAsync(string.Format("http://10.27.249.82:63653/api/CallingRequest?id={0}", 1)).Result;
                         var content2 = response1.Content.ReadAsStringAsync().Result;
                         Console.WriteLine(content2);
                         if (content2 != null && content2 != "" && content2 != "[]")
                         {
                             var response56 = client1.DeleteAsync(string.Format("http://10.27.249.82:63653/api/CallingRequest?senderID={0}&receiverID={1}", 8, 1)).Result;
                             var content56 = response56.Content.ReadAsStringAsync().Result;

                             break;
                         }
                     }

                     nextActivity.PutExtra("name", txtUsername.Text);
                     view.Context.StartActivity(nextActivity);
                 };
            };

            return view;
        }
    }
}