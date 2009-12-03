using System;

namespace StructureMap
{
    public interface IMessageSender
    {
        void SendMessage(string text, string sender, string receiver);
    }

    public class FluentMessageSender
    {
        private readonly IMessageSender _messageSender;

        public FluentMessageSender(IMessageSender sender)
        {
            _messageSender = sender;
        }

        public SendExpression SendText(string text)
        {
            return new SendExpression(text, _messageSender);
        }

        public class SendExpression : ToExpression
        {
            private readonly string _text;
            private readonly IMessageSender _messageSender;
            private string _sender;

            public SendExpression(string text, IMessageSender messageSender)
            {
                _text = text;
                _messageSender = messageSender;
            }

            public ToExpression From(string sender)
            {
                _sender = sender;
                return this;
            }

            void ToExpression.To(string receiver)
            {
                _messageSender.SendMessage(_text, _sender, receiver);
            }
        }

        public interface ToExpression
        {
            void To(string receiver);
        }
    }

    public class SendMessageRequest
    {
        public string Text { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
    }

    public class ParameterObjectMessageSender
    {
        public void Send(SendMessageRequest request)
        {
            // send the message
        }
    }

    public class APIConsumer
    {
    // Snippet from a class that uses IMessageSender
    public void SendMessage(IMessageSender sender)
    {
        // Is this right?
        sender.SendMessage("the message body", "PARTNER001", "PARTNER002");

        // or this?
        sender.SendMessage("PARTNER001", "the message body", "PARTNER002");

        // or this?
        sender.SendMessage("PARTNER001", "PARTNER002", "the message body");
    }

    public void SendMessageFluently(FluentMessageSender sender)
    {
        sender
            .SendText("the message body")
            .From("PARTNER001").To("PARTNER002");
    }

    public void SendMessageAsParameter(ParameterObjectMessageSender sender)
    {
        sender.Send(new SendMessageRequest()
        {
            Text = "the message body",
            Receiver = "PARTNER001",
            Sender = "PARTNER002"
        });
    }
    }
}