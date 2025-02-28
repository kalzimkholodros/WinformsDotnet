using MongoDB.Bson;
using System.Collections.Generic;

namespace MovieRatingApp.Models
{
    public class User
    {
        public ObjectId Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public List<ObjectId> Favorites { get; set; } = new List<ObjectId>();
        public decimal Balance { get; set; } = 1000M;
        public List<ObjectId> Friends { get; set; } = new List<ObjectId>();
        public List<ObjectId> FriendRequests { get; set; } = new List<ObjectId>();
        public List<ObjectId> OwnedMovies { get; set; } = new List<ObjectId>();
        public List<Gift> ReceivedGifts { get; set; } = new List<Gift>();
        public string ProfilePicture { get; set; } = "https://www.gravatar.com/avatar/default?d=mp&f=y";
    }

    public class Gift
    {
        public ObjectId MovieId { get; set; }
        public ObjectId SenderUserId { get; set; }
        public string Message { get; set; }
        public bool IsAccepted { get; set; }
        public System.DateTime SentDate { get; set; }
    }
} 