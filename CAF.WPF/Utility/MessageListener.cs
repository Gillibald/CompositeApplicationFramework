#region Usings

using System.Diagnostics;
using System.Windows;
using CompositeApplicationFramework.Helper;

#endregion

namespace CompositeApplicationFramework.Utility
{
    public class MessageListener : DependencyObject
    {
        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(
            "Message",
            typeof (string),
            typeof (MessageListener),
            new UIPropertyMetadata(null));

        private static MessageListener _instance;

        private MessageListener()
        {
        }

        public static MessageListener Instance => _instance ?? (_instance = new MessageListener());

        public string Message
        {
            get => (string) GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }

        public void ReceiveMessage(string message)
        {
            Message = message;
            Debug.WriteLine(Message);
            DispatcherHelper.DoEvents();
        }
    }
}