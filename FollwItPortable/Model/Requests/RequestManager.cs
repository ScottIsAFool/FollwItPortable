namespace FollwItPortable.Model.Requests
{
    internal static class RequestManager
    {
        internal static string Username { get; set; }
        internal static string Password { get; set; }

        internal static TRequestType CreateRequestType<TRequestType>() where TRequestType : BaseRequest, new()
        {
            var item = new TRequestType
            {
                Username = Username,
                Password = Password
            };

            return item;
        }
    }
}
