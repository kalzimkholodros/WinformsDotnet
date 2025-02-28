using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace MovieRatingApp.Models
{
    public class Movie
    {
        [BsonId]
        public ObjectId Id { get; set; }
        
        [BsonElement("title")]
        public string Title { get; set; }
        
        [BsonElement("imageUrl")]
        public string ImageUrl { get; set; }
        
        [BsonElement("price")]
        public decimal Price { get; set; } = 29.99M;
        
        [BsonElement("description")]
        public string Description { get; set; }
        
        [BsonElement("releaseYear")]
        public int ReleaseYear { get; set; }
        
        [BsonElement("userRatings")]
        public List<UserRating> UserRatings { get; set; } = new List<UserRating>();
        
        [BsonElement("averageUserRating")]
        public double AverageUserRating { get; set; }
        
        [BsonElement("genre")]
        public string Genre { get; set; }
        
        [BsonElement("director")]
        public string Director { get; set; }
        
        [BsonElement("cast")]
        public List<string> Cast { get; set; } = new List<string>();
        
        [BsonElement("purchaseCount")]
        public int PurchaseCount { get; set; }
        
        [BsonElement("isOnSale")]
        public bool IsOnSale { get; set; }
        
        [BsonElement("discountRate")]
        public decimal DiscountRate { get; set; }
    }

    public class UserRating
    {
        [BsonElement("userId")]
        public ObjectId UserId { get; set; }
        
        [BsonElement("username")]
        public string Username { get; set; }
        
        [BsonElement("rating")]
        public double Rating { get; set; }
        
        [BsonElement("comment")]
        public string Comment { get; set; }
        
        [BsonElement("ratedDate")]
        public System.DateTime RatedDate { get; set; }
        
        [BsonElement("likedBy")]
        public List<ObjectId> LikedBy { get; set; } = new List<ObjectId>();
    }
} 