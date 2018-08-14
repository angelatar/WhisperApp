using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;

namespace WhisperApp
{
    [Activity(Label = "Whisper")]
    public class SignUpActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here

            SetContentView(Resource.Layout.SignUp);

            var name = FindViewById<EditText>(Resource.Id.nameEditText);
            var lastname = FindViewById<EditText>(Resource.Id.lastnameEditText);
            var username = FindViewById<EditText>(Resource.Id.usernameEditText);
            var email = FindViewById<EditText>(Resource.Id.emailEditText);
            var emailTextView = FindViewById<TextView>(Resource.Id.emailTextView);
            var password = FindViewById<EditText>(Resource.Id.passwordEditText);
            var passwordTextView = FindViewById<TextView>(Resource.Id.passwordTextView);


            email.TextChanged += delegate
            {
                var emailvalid = Android.Util.Patterns.EmailAddress.Matcher(email.Text).Matches();

                if (!emailvalid)
                    emailTextView.SetTextColor(Color.Red);
                else
                    emailTextView.SetTextColor(Color.LightGray);
            };

            password.TextChanged += delegate
            {
                if (password.Text.Length < 8)
                    passwordTextView.SetTextColor(Color.Red);
                else
                    passwordTextView.SetTextColor(Color.LightGray);
            };


            var signupbtn = FindViewById<Button>(Resource.Id.signUpbutton);
            signupbtn.Click += delegate
            {
                var emailvalid = Android.Util.Patterns.EmailAddress.Matcher(email.Text).Matches();

                if (username.Text != "" && password.Text != "" && name.Text != "" && lastname.Text != "" && (email.Text != "" && emailvalid))
                {
                    this.SendValidationCode(email.Text);
                    Intent nextActivity = new Intent(this, typeof(ValidationActivity));
                    nextActivity.PutExtra("email", email.Text);
                    nextActivity.PutExtra("username", username.Text);
                    nextActivity.PutExtra("name", name.Text);
                    nextActivity.PutExtra("lastname", lastname.Text);
                    nextActivity.PutExtra("password", username.Text);
                    StartActivity(nextActivity);
                }
            };
        }

        private bool SendValidationCode(string receiver)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(
                 new MediaTypeWithQualityHeaderValue("application/json"));
            var paramContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("email", receiver)
            });

            var response = client.PostAsync("http://192.168.88.136:61366/api/Validation", paramContent).Result;
            var content = response.Content.ReadAsStringAsync().Result;
            return bool.Parse(content);
        }

   
    }
}