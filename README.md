Movie Rating Application
A Windows Forms application built with C# .NET and MongoDB, implementing a movie rating system with user authentication, CRUD operations, friend management, and real-time notifications using a document-based database structure.
![Ekran görüntüsü 2025-03-01 004152](https://github.com/user-attachments/assets/72c93d28-3e42-4940-9801-e6bdc7cb81b1)


## Features

- **User Authentication**
  - Login/Register system
  - Role-based access (Admin/User)
  - Secure password hashing

- **Movie Management**
  - Browse movie catalog
  - View detailed movie information
  - Rate and comment on movies
  - Add movies to favorites

- **Social Features**
  - Send friend requests
  - Accept/Reject friend requests
  - Real-time notifications
  - Gift movies to friends

- **Admin Features**
  - Manage movie catalog
  - User management
  - System monitoring
 

## Technologies Used

- **Frontend:** Windows Forms (.NET)
- **Backend:** C# .NET
- **Database:** MongoDB
- **Authentication:** BCrypt
- **Image Handling:** Gravatar

1. Clone the repository
```bash
git clone [https://github.com/kalzimkholodros/WinformsDotnet]
```

2. Install MongoDB on your system
- Download and install MongoDB Community Server
- Create a database named "movieRatingDb"
- Create collections: "users", "movies"

3. Configure MongoDB Connection
- Update the connection string in `MongoDBService.cs`
- Default connection: `mongodb://localhost:27017`

4. Run the Application
- Open the solution in Visual Studio
- Build and run the project


## Database Structure

### Users Collection
```json
{
  "username": "string",
  "password": "string (hashed)",
  "role": "string",
  "balance": "decimal",
  "friends": "array",
  "friendRequests": "array",
  "notifications": "array",
  "favorites": "array",
  "ownedMovies": "array",
  "receivedGifts": "array"
}

### Movies Collection
```json
{
  "title": "string",
  "imageUrl": "string",
  "description": "string",
  "releaseYear": "integer",
  "director": "string",
  "genre": "string",
  "cast": "array",
  "price": "decimal",
  "isOnSale": "boolean",
  "discountRate": "decimal",
  "userRatings": "array",
  "averageUserRating": "double",
  "purchaseCount": "integer"
}

## Default Users

- **Admin Account**
  - Username: admin
  - Password: admin123
  - Role: admin

- **Test User Account**
  - Username: user1
  - Password: admin123
  - Role: user
## Acknowledgments

- Movie data and images sourced from IMDB
- Icons and UI elements from Microsoft Windows Forms
- Profile pictures handled by Gravatar

