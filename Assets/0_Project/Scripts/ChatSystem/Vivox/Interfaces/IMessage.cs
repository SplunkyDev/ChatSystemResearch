using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chat
{
    public interface IMessage
    {
        public string Channel { get; }
        public string Sender { get; }
        public string Message { get; }
    }
}
