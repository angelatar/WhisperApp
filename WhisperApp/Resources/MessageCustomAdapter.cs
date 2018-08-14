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
    public class MessageViewHolder : Java.Lang.Object
    {
        public TextView txtmMessage { set; get; }
    }

    class MessageCustomAdapter : BaseAdapter
    {
        private Activity mainActivity;
        private List<string> messages;


        public MessageCustomAdapter(Activity activity, List<string> messages)
        {
            this.mainActivity = activity;
            this.messages = messages;
        }

        public override int Count
        {
            get
            {
                return messages.Count;
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return position;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {

            //LayoutInflater inflater = (LayoutInflater)mainActivity.BaseContext.GetSystemService(Context.la);
            //View itemView = inflater.Inflate(Resource.Layout.message_list_item,null);

            //TextView message_user, message_time, message_contnent;
            //message_user = itemView.FindViewById<TextView>(Resource.Id.message_user);
            //message_contnent = itemView.FindViewById<TextView>(Resource.Id.message_text);
            //message_time = itemView.FindViewById<TextView>(Resource.Id.message_time);

            var view = convertView ?? mainActivity.
                LayoutInflater.Inflate(Resource.Layout.list_view_dataTemplate, parent, false);

            var txtUsername = view.FindViewById<TextView>(Resource.Id.textView1);

            txtUsername.Text = messages[position];
            return view;
        }
    }
}