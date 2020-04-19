using System;
using System.Net.Http;

namespace TurnipTracker.Services
{
    public static  class FriendService
    {
        public static HttpClient Client { get; }
        static FriendService()
        {
            Client = new HttpClient();
        }


    }
}
