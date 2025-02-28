using MongoDB.Driver;
using MovieRatingApp.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using System.Windows.Forms;

namespace MovieRatingApp.Services
{
    public class MongoDBService
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<Movie> _movies;

        public MongoDBService()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            _database = client.GetDatabase("movieRatingDb");
            _users = _database.GetCollection<User>("users");
            _movies = _database.GetCollection<Movie>("movies");
        }

        public static async Task<MongoDBService> CreateAsync()
        {
            var service = new MongoDBService();
            await service.InitializeDatabase();
            await service.UpdateMoviesWithCommentFeature();
            return service;
        }

        private async Task InitializeDatabase()
        {
            // Admin kullanıcısını kontrol et ve yoksa oluştur
            var adminExists = await _users.Find(u => u.Username == "admin").AnyAsync();
            if (!adminExists)
            {
                await RegisterUser("admin", "admin123", "admin");
            }

            // Filmleri kontrol et ve yoksa ekle
            var moviesExist = await _movies.Find(_ => true).AnyAsync();
            if (!moviesExist)
            {
                var movies = new List<Movie>
                {
                    new Movie
                    {
                        Title = "The Shawshank Redemption",
                        ImageUrl = "https://m.media-amazon.com/images/M/MV5BNDE3ODcxYzMtY2YzZC00NmNlLWJiNDMtZDViZWM2MzIxZDYwXkEyXkFqcGdeQXVyNjAwNDUxODI@._V1_.jpg",
                        Description = "Two imprisoned men bond over a number of years, finding solace and eventual redemption through acts of common decency.",
                        ReleaseYear = 1994,
                        Director = "Frank Darabont",
                        Genre = "Drama",
                        Cast = new List<string> { "Tim Robbins", "Morgan Freeman", "Bob Gunton" },
                        Price = 14.99M,
                        IsOnSale = false,
                        DiscountRate = 0,
                        UserRatings = new List<UserRating>(),
                        AverageUserRating = 0,
                        PurchaseCount = 0
                    },
                    new Movie
                    {
                        Title = "The Godfather",
                        ImageUrl = "https://m.media-amazon.com/images/M/MV5BM2MyNjYxNmUtYTAwNi00MTYxLWJmNWYtYzZlODY3ZTk3OTFlXkEyXkFqcGdeQXVyNzkwMjQ5NzM@._V1_.jpg",
                        Description = "The aging patriarch of an organized crime dynasty transfers control of his clandestine empire to his reluctant son.",
                        ReleaseYear = 1972,
                        Director = "Francis Ford Coppola",
                        Genre = "Crime, Drama",
                        Cast = new List<string> { "Marlon Brando", "Al Pacino", "James Caan" },
                        Price = 19.99M,
                        IsOnSale = true,
                        DiscountRate = 0.2M,
                        UserRatings = new List<UserRating>(),
                        AverageUserRating = 0,
                        PurchaseCount = 0
                    },
                    new Movie
                    {
                        Title = "The Dark Knight",
                        ImageUrl = "https://m.media-amazon.com/images/M/MV5BMTMxNTMwODM0NF5BMl5BanBnXkFtZTcwODAyMTk2Mw@@._V1_.jpg",
                        Description = "When the menace known as the Joker wreaks havoc and chaos on the people of Gotham, Batman must accept one of the greatest psychological and physical tests of his ability to fight injustice.",
                        ReleaseYear = 2008,
                        Director = "Christopher Nolan",
                        Genre = "Action, Crime, Drama",
                        Cast = new List<string> { "Christian Bale", "Heath Ledger", "Aaron Eckhart" },
                        Price = 24.99M,
                        IsOnSale = false,
                        DiscountRate = 0,
                        UserRatings = new List<UserRating>(),
                        AverageUserRating = 0,
                        PurchaseCount = 0
                    },
                    new Movie
                    {
                        Title = "Pulp Fiction",
                        ImageUrl = "https://m.media-amazon.com/images/M/MV5BNGNhMDIzZTUtNTBlZi00MTRlLWFjM2ItYzViMjE3YzI5MjljXkEyXkFqcGdeQXVyNzkwMjQ5NzM@._V1_.jpg",
                        Description = "The lives of two mob hitmen, a boxer, a gangster and his wife, and a pair of diner bandits intertwine in four tales of violence and redemption.",
                        ReleaseYear = 1994,
                        Director = "Quentin Tarantino",
                        Genre = "Crime, Drama",
                        Cast = new List<string> { "John Travolta", "Uma Thurman", "Samuel L. Jackson" },
                        Price = 24.99M,
                        IsOnSale = false,
                        DiscountRate = 0,
                        UserRatings = new List<UserRating>(),
                        AverageUserRating = 0,
                        PurchaseCount = 0
                    },
                    new Movie
                    {
                        Title = "Inception",
                        ImageUrl = "https://m.media-amazon.com/images/M/MV5BMjAxMzY3NjcxNF5BMl5BanBnXkFtZTcwNTI5OTM0Mw@@._V1_.jpg",
                        Description = "A thief who steals corporate secrets through the use of dream-sharing technology is given the inverse task of planting an idea into the mind of a C.E.O.",
                        ReleaseYear = 2010,
                        Director = "Christopher Nolan",
                        Genre = "Action, Adventure, Sci-Fi",
                        Cast = new List<string> { "Leonardo DiCaprio", "Joseph Gordon-Levitt", "Ellen Page" },
                        Price = 29.99M,
                        IsOnSale = true,
                        DiscountRate = 0.15M,
                        UserRatings = new List<UserRating>(),
                        AverageUserRating = 0,
                        PurchaseCount = 0
                    },
                    new Movie
                    {
                        Title = "Interstellar",
                        ImageUrl = "https://m.media-amazon.com/images/M/MV5BZjdkOTU3MDktN2IxOS00OGEyLWFmMjktY2FiMmZkNWIyODZiXkEyXkFqcGdeQXVyMTMxODk2OTU@._V1_.jpg",
                        Description = "A team of explorers travel through a wormhole in space in an attempt to ensure humanity's survival.",
                        ReleaseYear = 2014,
                        Director = "Christopher Nolan",
                        Genre = "Adventure, Drama, Sci-Fi",
                        Cast = new List<string> { "Matthew McConaughey", "Anne Hathaway", "Jessica Chastain" },
                        Price = 34.99M,
                        IsOnSale = true,
                        DiscountRate = 0.1M,
                        UserRatings = new List<UserRating>(),
                        AverageUserRating = 0,
                        PurchaseCount = 0
                    },
                    new Movie
                    {
                        Title = "The Matrix",
                        ImageUrl = "https://m.media-amazon.com/images/M/MV5BNzQzOTk3OTAtNDQ0Zi00ZTVkLWI0MTEtMDllZjNkYzNjNTc4L2ltYWdlXkEyXkFqcGdeQXVyNjU0OTQ0OTY@._V1_.jpg",
                        Description = "A computer programmer discovers that reality as he knows it is a simulation created by machines, and joins a rebellion to break free from it.",
                        ReleaseYear = 1999,
                        Director = "Lana Wachowski, Lilly Wachowski",
                        Genre = "Action, Sci-Fi",
                        Cast = new List<string> { "Keanu Reeves", "Laurence Fishburne", "Carrie-Anne Moss" },
                        Price = 24.99M,
                        IsOnSale = false,
                        DiscountRate = 0,
                        UserRatings = new List<UserRating>(),
                        AverageUserRating = 0,
                        PurchaseCount = 0
                    },
                    new Movie
                    {
                        Title = "Fight Club",
                        ImageUrl = "https://m.media-amazon.com/images/M/MV5BNDIzNDU0YzEtYzE5Ni00ZjlkLTk5ZjgtNjM3NWE4YzA3Nzk3XkEyXkFqcGdeQXVyMjUzOTY1NTc@._V1_.jpg",
                        Description = "An insomniac office worker and a devil-may-care soap maker form an underground fight club that evolves into much more.",
                        ReleaseYear = 1999,
                        Director = "David Fincher",
                        Genre = "Drama",
                        Cast = new List<string> { "Brad Pitt", "Edward Norton", "Helena Bonham Carter" },
                        Price = 19.99M,
                        IsOnSale = false,
                        DiscountRate = 0,
                        UserRatings = new List<UserRating>(),
                        AverageUserRating = 0,
                        PurchaseCount = 0
                    },
                    new Movie
                    {
                        Title = "Forrest Gump",
                        ImageUrl = "https://m.media-amazon.com/images/M/MV5BNWIwODRlZTUtY2U3ZS00Yzg1LWJhNzYtMmZiYmEyNmU1NjMzXkEyXkFqcGdeQXVyMTQxNzMzNDI@._V1_.jpg",
                        Description = "The presidencies of Kennedy and Johnson, the Vietnam War, the Watergate scandal and other historical events unfold from the perspective of an Alabama man with an IQ of 75, whose only desire is to be reunited with his childhood sweetheart.",
                        ReleaseYear = 1994,
                        Director = "Robert Zemeckis",
                        Genre = "Drama, Romance",
                        Cast = new List<string> { "Tom Hanks", "Robin Wright", "Gary Sinise" },
                        Price = 19.99M,
                        IsOnSale = true,
                        DiscountRate = 0.25M,
                        UserRatings = new List<UserRating>(),
                        AverageUserRating = 0,
                        PurchaseCount = 0
                    }
                };

                await _movies.InsertManyAsync(movies);
            }
        }

        // User işlemleri
        public async Task<User> AuthenticateUser(string username, string password)
        {
            var user = await _users.Find(u => u.Username == username).FirstOrDefaultAsync();
            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password))
                return user;
            return null;
        }

        public async Task<bool> RegisterUser(string username, string password, string role = "user")
        {
            if (await _users.Find(u => u.Username == username).AnyAsync())
                return false;

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            var user = new User
            {
                Username = username,
                Password = hashedPassword,
                Role = role,
                Favorites = new List<ObjectId>()
            };

            await _users.InsertOneAsync(user);
            return true;
        }

        // Film işlemleri
        public async Task<List<Movie>> GetAllMovies()
        {
            return await _movies.Find(_ => true).ToListAsync();
        }

        public async Task AddMovie(Movie movie)
        {
            await _movies.InsertOneAsync(movie);
        }

        public async Task AddRating(ObjectId movieId, ObjectId userId, double rating)
        {
            var movie = await _movies.Find(m => m.Id == movieId).FirstOrDefaultAsync();
            if (movie == null) return;

            var user = await _users.Find(u => u.Id == userId).FirstOrDefaultAsync();
            if (user == null) return;

            // Kullanıcının daha önce puan verip vermediğini kontrol et
            var existingRating = movie.UserRatings.FirstOrDefault(r => r.UserId == userId);
            if (existingRating != null)
            {
                MessageBox.Show("You have already rated this movie. You can only rate a movie once.", 
                    "Rating Exists", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var userRating = new UserRating 
            { 
                UserId = userId,
                Username = user.Username,
                Rating = rating,
                RatedDate = DateTime.Now,
                LikedBy = new List<ObjectId>()
            };

            var update = Builders<Movie>.Update.Push(m => m.UserRatings, userRating);
            await _movies.UpdateOneAsync(m => m.Id == movieId, update);

            // Ortalama puanı güncelle
            var updatedMovie = await _movies.Find(m => m.Id == movieId).FirstOrDefaultAsync();
            double averageRating = 0;
            if (updatedMovie.UserRatings.Count > 0)
            {
                double totalRating = 0;
                foreach (var ur in updatedMovie.UserRatings)
                {
                    totalRating += ur.Rating;
                }
                averageRating = totalRating / updatedMovie.UserRatings.Count;
            }

            var updateAverage = Builders<Movie>.Update.Set(m => m.AverageUserRating, averageRating);
            await _movies.UpdateOneAsync(m => m.Id == movieId, updateAverage);
        }

        public async Task ToggleFavorite(ObjectId userId, ObjectId movieId)
        {
            var user = await _users.Find(u => u.Id == userId).FirstOrDefaultAsync();
            if (user == null) return;

            if (user.Favorites.Contains(movieId))
            {
                var update = Builders<User>.Update.Pull(u => u.Favorites, movieId);
                await _users.UpdateOneAsync(u => u.Id == userId, update);
            }
            else
            {
                var update = Builders<User>.Update.Push(u => u.Favorites, movieId);
                await _users.UpdateOneAsync(u => u.Id == userId, update);
            }
        }

        public async Task<List<Movie>> GetFavoriteMovies(ObjectId userId)
        {
            var user = await _users.Find(u => u.Id == userId).FirstOrDefaultAsync();
            if (user == null || user.Favorites.Count == 0)
                return new List<Movie>();

            return await _movies.Find(m => user.Favorites.Contains(m.Id)).ToListAsync();
        }

        // Kullanıcı bakiye işlemleri
        public async Task<bool> UpdateBalance(ObjectId userId, decimal amount)
        {
            var update = Builders<User>.Update.Inc(u => u.Balance, amount);
            var result = await _users.UpdateOneAsync(u => u.Id == userId && u.Balance + amount >= 0, update);
            return result.ModifiedCount > 0;
        }

        // Film satın alma
        public async Task<bool> PurchaseMovie(ObjectId userId, ObjectId movieId)
        {
            var movie = await _movies.Find(m => m.Id == movieId).FirstOrDefaultAsync();
            if (movie == null) return false;

            var user = await _users.Find(u => u.Id == userId).FirstOrDefaultAsync();
            if (user == null) return false;

            decimal finalPrice = movie.IsOnSale ? movie.Price * (1 - movie.DiscountRate) : movie.Price;
            if (user.Balance < finalPrice) return false;

            // Kullanıcının bakiyesini düşür
            var balanceUpdate = Builders<User>.Update.Inc(u => u.Balance, -finalPrice)
                                                   .Push(u => u.OwnedMovies, movieId);
            var balanceResult = await _users.UpdateOneAsync(u => u.Id == userId, balanceUpdate);

            // Filmin satın alınma sayısını artır
            var movieUpdate = Builders<Movie>.Update.Inc(m => m.PurchaseCount, 1);
            var movieResult = await _movies.UpdateOneAsync(m => m.Id == movieId, movieUpdate);

            return balanceResult.ModifiedCount > 0 && movieResult.ModifiedCount > 0;
        }

        // Hediye gönderme
        public async Task<bool> SendGift(ObjectId senderId, ObjectId receiverId, ObjectId movieId, string message)
        {
            var gift = new Gift
            {
                MovieId = movieId,
                SenderUserId = senderId,
                Message = message,
                IsAccepted = false,
                SentDate = DateTime.Now
            };

            var update = Builders<User>.Update.Push(u => u.ReceivedGifts, gift);
            var result = await _users.UpdateOneAsync(u => u.Id == receiverId, update);
            return result.ModifiedCount > 0;
        }

        // Arkadaşlık isteği gönderme
        public async Task<bool> SendFriendRequest(ObjectId senderId, ObjectId receiverId)
        {
            // Alıcı kullanıcının var olup olmadığını kontrol et
            var receiver = await _users.Find(u => u.Id == receiverId).FirstOrDefaultAsync();
            if (receiver == null)
            {
                MessageBox.Show("User not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Kullanıcının kendine arkadaşlık isteği göndermesini engelle
            if (senderId == receiverId)
            {
                MessageBox.Show("You cannot send a friend request to yourself.", 
                    "Invalid Request", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Zaten arkadaş olup olmadıklarını kontrol et
            var sender = await _users.Find(u => u.Id == senderId).FirstOrDefaultAsync();
            if (sender.Friends != null && sender.Friends.Contains(receiverId))
            {
                MessageBox.Show("You are already friends with this user.", 
                    "Already Friends", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            // Zaten istek gönderilmiş mi kontrol et
            if (receiver.FriendRequests != null && receiver.FriendRequests.Contains(senderId))
            {
                MessageBox.Show("You have already sent a friend request to this user.", 
                    "Request Exists", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            var update = Builders<User>.Update.AddToSet(u => u.FriendRequests, senderId);
            var result = await _users.UpdateOneAsync(u => u.Id == receiverId, update);

            if (result.ModifiedCount > 0)
            {
                MessageBox.Show("Friend request sent successfully!", 
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }

            return false;
        }

        // Arkadaşlık isteğini kabul etme
        public async Task<bool> AcceptFriendRequest(ObjectId userId, ObjectId friendId)
        {
            var userUpdate = Builders<User>.Update.AddToSet(u => u.Friends, friendId)
                                                .Pull(u => u.FriendRequests, friendId);
            var friendUpdate = Builders<User>.Update.AddToSet(u => u.Friends, userId);

            var userResult = await _users.UpdateOneAsync(u => u.Id == userId, userUpdate);
            var friendResult = await _users.UpdateOneAsync(u => u.Id == friendId, friendUpdate);

            return userResult.ModifiedCount > 0 && friendResult.ModifiedCount > 0;
        }

        // Kullanıcı yorumu beğenme
        public async Task<bool> LikeComment(ObjectId movieId, ObjectId userId, ObjectId commentUserId)
        {
            var update = Builders<Movie>.Update.AddToSet("userRatings.$[rating].likedBy", userId);
            var arrayFilters = new[] 
            { 
                new BsonDocumentArrayFilterDefinition<BsonDocument>(
                    new BsonDocument("rating.userId", commentUserId)
                )
            };
            var updateOptions = new UpdateOptions { ArrayFilters = arrayFilters };

            var result = await _movies.UpdateOneAsync(
                m => m.Id == movieId,
                update,
                updateOptions
            );

            return result.ModifiedCount > 0;
        }

        // Film yorumu ekleme/güncelleme
        public async Task AddComment(ObjectId movieId, ObjectId userId, string comment)
        {
            var movie = await _movies.Find(m => m.Id == movieId).FirstOrDefaultAsync();
            if (movie == null) return;

            var existingRating = movie.UserRatings.FirstOrDefault(r => r.UserId == userId);
            if (existingRating == null)
            {
                MessageBox.Show("Please rate the movie before adding a comment.", 
                    "Rating Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!string.IsNullOrEmpty(existingRating.Comment))
            {
                MessageBox.Show("You have already commented on this movie. You can only comment once.", 
                    "Comment Exists", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var filter = Builders<Movie>.Filter.And(
                Builders<Movie>.Filter.Eq(m => m.Id, movieId),
                Builders<Movie>.Filter.ElemMatch(m => m.UserRatings, r => r.UserId == userId)
            );

            var update = Builders<Movie>.Update.Set(
                "userRatings.$[rating].comment", comment
            );

            var arrayFilters = new[]
            {
                new BsonDocumentArrayFilterDefinition<BsonDocument>(
                    new BsonDocument("rating.userId", userId)
                )
            };

            var updateOptions = new UpdateOptions { ArrayFilters = arrayFilters };

            await _movies.UpdateOneAsync(filter, update, updateOptions);
        }

        // Arkadaş listesini getirme
        public async Task<List<User>> GetFriends(ObjectId userId)
        {
            var user = await _users.Find(u => u.Id == userId).FirstOrDefaultAsync();
            if (user?.Friends == null || !user.Friends.Any())
                return new List<User>();

            return await _users.Find(u => user.Friends.Contains(u.Id))
                             .Project<User>(Builders<User>.Projection
                                 .Include(u => u.Id)
                                 .Include(u => u.Username)
                                 .Include(u => u.ProfilePicture))
                             .ToListAsync();
        }

        // Popüler filmleri getirme
        public async Task<List<Movie>> GetPopularMovies(int limit = 10)
        {
            return await _movies.Find(_ => true)
                              .Sort(Builders<Movie>.Sort.Descending(m => m.PurchaseCount))
                              .Limit(limit)
                              .ToListAsync();
        }

        // İndirimdeki filmleri getirme
        public async Task<List<Movie>> GetMoviesOnSale()
        {
            return await _movies.Find(m => m.IsOnSale)
                              .Sort(Builders<Movie>.Sort.Descending(m => m.DiscountRate))
                              .ToListAsync();
        }

        public async Task<Movie> GetMovie(ObjectId movieId)
        {
            return await _movies.Find(m => m.Id == movieId).FirstOrDefaultAsync();
        }

        public async Task UpdateMoviesWithCommentFeature()
        {
            var update = Builders<Movie>.Update
                .SetOnInsert(m => m.UserRatings, new List<UserRating>())
                .SetOnInsert(m => m.AverageUserRating, 0.0)
                .SetOnInsert(m => m.PurchaseCount, 0);

            await _movies.UpdateManyAsync(
                Builders<Movie>.Filter.Empty,
                update,
                new UpdateOptions { IsUpsert = true }
            );
        }

        public async Task UpdateRatingComment(ObjectId movieId, ObjectId userId, string comment)
        {
            var filter = Builders<Movie>.Filter.And(
                Builders<Movie>.Filter.Eq(m => m.Id, movieId),
                Builders<Movie>.Filter.ElemMatch(m => m.UserRatings, r => r.UserId == userId)
            );

            var update = Builders<Movie>.Update.Set(
                "userRatings.$[rating].comment", comment
            );

            var arrayFilters = new[]
            {
                new BsonDocumentArrayFilterDefinition<BsonDocument>(
                    new BsonDocument("rating.userId", userId)
                )
            };

            var updateOptions = new UpdateOptions { ArrayFilters = arrayFilters };

            await _movies.UpdateOneAsync(filter, update, updateOptions);
        }
    }
} 