using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace WhisperApp.Resources
{
    public class ViewHolder:Java.Lang.Object
    {
        public TextView txtUsername { set; get; }
    }

    public class CustomAdapter : BaseAdapter
    {
        private Activity activity;
        private List<Contact> contacts;

        public CustomAdapter(Activity activity,List<Contact> contact)
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

            txtUsername.Click += delegate
            {
                communicatebutton.Visibility = ViewStates.Visible;
                communicatebutton.Click += delegate
                 {
                     Intent nextActivity = new Intent(view.Context, typeof(CommunicationActivity));
                     view.Context.StartActivity(nextActivity);
                 };
            };

            txtUsername.Text = contacts[position].Username;
            return view;
        }
    }
}