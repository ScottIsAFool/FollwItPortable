using System;

namespace FollwItPortable.Model
{
    public class FollwItException : Exception
    {
        public FollwItException(string message, string response)
            : base(message)
        {
            Response = response;
        }

        public string Response { get; set; }
    }
}
