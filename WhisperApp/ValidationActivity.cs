using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Widget;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

namespace WhisperApp
{
    [Activity(Label = "Whisper")]
    public class ValidationActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.Validation);

            var email = Intent.GetStringExtra("email");
            var username = Intent.GetStringExtra("username");
            var name = Intent.GetStringExtra("name");
            var lastname = Intent.GetStringExtra("lastname");
            var password = Intent.GetStringExtra("password");

            var enteredCode = FindViewById<EditText>(Resource.Id.editText1);

            var button = FindViewById<Button>(Resource.Id.button1);


            var text = FindViewById<TextView>(Resource.Id.textView1);
            

            button.Click += delegate {
                if(this.CheckCode(email,enteredCode.Text))
                {
                    var passwordhash = this.ComputeSha256Hash(password);
                    if (this.AddUser(name,lastname,username, passwordhash, email))
                    {
                        Intent nextActivity = new Intent(this, typeof(WelcomeActivity));
                        nextActivity.PutExtra("email", email);
                        nextActivity.PutExtra("username", username);
                        nextActivity.PutExtra("name", name);
                        nextActivity.PutExtra("lastname", lastname);
                        StartActivity(nextActivity);
                    }
                }
                else
                {
                    text.SetTextColor(Color.Red);
                    text.Text += "/nIncorrect validation code";
                }
            };

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

        private bool CheckCode(string email, string code)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(
                 new MediaTypeWithQualityHeaderValue("application/json"));

            var response = client.GetAsync(string.Format("http://10.27.249.82:61366/api/Validation?email={0}&code={1}", email,code)).Result;
            var content = response.Content.ReadAsStringAsync().Result;

            return bool.Parse(content);
        }

        private bool DeleteCode(string email)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(
                 new MediaTypeWithQualityHeaderValue("application/json"));

            var response = client.DeleteAsync(string.Format("http://10.27.249.82:61366/api/Validation?email={0}",email)).Result;
            var content = response.Content.ReadAsStringAsync().Result;
            return bool.Parse(content);
        }

        private bool AddUser(string name, string lastname, string username, string password, string email)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(
                 new MediaTypeWithQualityHeaderValue("application/json"));
            var content1 = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("ID", "0"),
                new KeyValuePair<string, string>("Name", name),
                new KeyValuePair<string, string>("Lastname", lastname),
                new KeyValuePair<string, string>("Username", username),
                new KeyValuePair<string, string>("PasswordHash", password),
                new KeyValuePair<string, string>("Email", email)
            });
            var response = client.PostAsync("http://10.27.249.82:61366/api/Register", content1).Result;
            var content = response.Content.ReadAsStringAsync().Result;
            return bool.Parse(content.ToString());
        }
    }
}