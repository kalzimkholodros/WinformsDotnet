using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using MovieRatingApp.Models;
using MovieRatingApp.Services;
using MongoDB.Bson;
using System.Linq;

namespace MovieRatingApp.Forms
{
    public class MovieDetailForm : Form
    {
        private Movie _movie;
        private readonly User _currentUser;
        private readonly MongoDBService _mongoService;
        private FlowLayoutPanel commentsPanel;

        public MovieDetailForm(Movie movie, User currentUser, MongoDBService mongoService)
        {
            _movie = movie;
            _currentUser = currentUser;
            _mongoService = mongoService;
            InitializeComponent();
            LoadComments();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(1200, 800);
            this.Text = _movie.Title;
            this.BackColor = Color.FromArgb(18, 18, 18);
            this.ForeColor = Color.White;
            this.StartPosition = FormStartPosition.CenterParent;

            // Movie Poster
            var posterBox = new PictureBox
            {
                Size = new Size(300, 420),
                Location = new Point(20, 20),
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.None
            };
            try { posterBox.Load(_movie.ImageUrl); }
            catch { posterBox.BackColor = Color.Gray; }

            // Movie Details Panel
            var detailsPanel = new Panel
            {
                Location = new Point(340, 20),
                Size = new Size(450, 420),
                BackColor = Color.FromArgb(24, 24, 24)
            };

            var titleLabel = new Label
            {
                Text = _movie.Title,
                Location = new Point(15, 15),
                Size = new Size(420, 35),
                Font = new Font("Segoe UI", 18, FontStyle.Bold)
            };

            var yearLabel = new Label
            {
                Text = $"Release Year: {_movie.ReleaseYear}",
                Location = new Point(15, 60),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 11)
            };

            var directorLabel = new Label
            {
                Text = $"Director: {_movie.Director}",
                Location = new Point(15, 90),
                Size = new Size(420, 25),
                Font = new Font("Segoe UI", 11)
            };

            var genreLabel = new Label
            {
                Text = $"Genre: {_movie.Genre}",
                Location = new Point(15, 120),
                Size = new Size(420, 25),
                Font = new Font("Segoe UI", 11)
            };

            var castLabel = new Label
            {
                Text = $"Cast: {string.Join(", ", _movie.Cast)}",
                Location = new Point(15, 150),
                Size = new Size(420, 50),
                Font = new Font("Segoe UI", 11)
            };

            var ratingLabel = new Label
            {
                Text = $"Average Rating: {_movie.AverageUserRating:F1}/10 ({_movie.UserRatings.Count} ratings)",
                Location = new Point(15, 205),
                Size = new Size(420, 25),
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.Gold
            };

            decimal finalPrice = _movie.IsOnSale ? _movie.Price * (1 - _movie.DiscountRate) : _movie.Price;
            var priceLabel = new Label
            {
                Text = _movie.IsOnSale ? 
                    $"Price: ${finalPrice:F2} (${_movie.Price:F2})" : 
                    $"Price: ${_movie.Price:F2}",
                Location = new Point(15, 235),
                Size = new Size(420, 25),
                Font = new Font("Segoe UI", 11),
                ForeColor = _movie.IsOnSale ? Color.LightGreen : Color.White
            };

            var descriptionLabel = new Label
            {
                Text = "Description:",
                Location = new Point(15, 270),
                Size = new Size(100, 25),
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };

            var descriptionText = new TextBox
            {
                Text = _movie.Description,
                Location = new Point(15, 300),
                Size = new Size(420, 105),
                Multiline = true,
                ReadOnly = true,
                BackColor = Color.FromArgb(32, 32, 32),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 11)
            };

            detailsPanel.Controls.AddRange(new Control[] {
                titleLabel, yearLabel, directorLabel, genreLabel,
                castLabel, ratingLabel, priceLabel,
                descriptionLabel, descriptionText
            });

            // Comments Section
            var commentsHeaderLabel = new Label
            {
                Text = "Comments",
                Location = new Point(810, 20),
                Size = new Size(360, 30),
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White
            };

            commentsPanel = new FlowLayoutPanel
            {
                Location = new Point(810, 60),
                Size = new Size(360, 380),
                AutoScroll = true,
                BackColor = Color.FromArgb(24, 24, 24),
                Padding = new Padding(5)
            };

            // Rating and Comment Input Section
            var inputPanel = new Panel
            {
                Location = new Point(20, 460),
                Size = new Size(1150, 170),
                BackColor = Color.FromArgb(24, 24, 24)
            };

            var userRatingLabel = new Label
            {
                Text = "Your Rating:",
                Location = new Point(15, 15),
                Size = new Size(80, 25),
                Font = new Font("Segoe UI", 11)
            };

            var ratingNumeric = new NumericUpDown
            {
                Location = new Point(100, 15),
                Size = new Size(70, 25),
                Minimum = 1,
                Maximum = 10,
                DecimalPlaces = 1,
                Value = 5,
                Increment = 0.5M,
                BackColor = Color.FromArgb(32, 32, 32),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11)
            };

            var addRatingButton = new Button
            {
                Text = "Rate",
                Location = new Point(180, 15),
                Size = new Size(70, 25),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(40, 40, 40),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10),
                Cursor = Cursors.Hand
            };

            var commentLabel = new Label
            {
                Text = "Your Comment:",
                Location = new Point(15, 55),
                Size = new Size(120, 25),
                Font = new Font("Segoe UI", 11)
            };

            var commentBox = new TextBox
            {
                Location = new Point(15, 85),
                Size = new Size(1020, 70),
                Multiline = true,
                BackColor = Color.FromArgb(32, 32, 32),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11)
            };

            var addCommentButton = new Button
            {
                Text = "Add Comment",
                Location = new Point(1045, 85),
                Size = new Size(90, 70),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(40, 40, 40),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10),
                Cursor = Cursors.Hand
            };

            addRatingButton.Click += async (s, e) =>
            {
                await _mongoService.AddRating(_movie.Id, _currentUser.Id, (double)ratingNumeric.Value);
                _movie = await _mongoService.GetMovie(_movie.Id);
                LoadComments();
                MessageBox.Show("Rating added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            addCommentButton.Click += async (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(commentBox.Text)) 
                {
                    MessageBox.Show("Please enter a comment.", "Comment Required", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var existingRating = _movie.UserRatings.FirstOrDefault(r => r.UserId == _currentUser.Id);
                if (existingRating == null)
                {
                    MessageBox.Show("Please rate the movie before adding a comment.", "Rating Required", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                await _mongoService.AddComment(_movie.Id, _currentUser.Id, commentBox.Text);
                _movie = await _mongoService.GetMovie(_movie.Id);
                LoadComments();
                commentBox.Text = "";
                MessageBox.Show("Comment added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            inputPanel.Controls.AddRange(new Control[] {
                userRatingLabel, ratingNumeric, addRatingButton,
                commentLabel, commentBox, addCommentButton
            });

            this.Controls.AddRange(new Control[] {
                posterBox, detailsPanel, commentsHeaderLabel, commentsPanel, inputPanel
            });
        }

        private void LoadComments()
        {
            commentsPanel.Controls.Clear();
            foreach (var rating in _movie.UserRatings.OrderByDescending(r => r.RatedDate))
            {
                var commentPanel = CreateCommentPanel(rating);
                commentsPanel.Controls.Add(commentPanel);
            }
        }

        private Panel CreateCommentPanel(UserRating rating)
        {
            var panel = new Panel
            {
                Size = new Size(330, 100),
                Margin = new Padding(5),
                BackColor = Color.FromArgb(32, 32, 32)
            };

            var usernameLabel = new Label
            {
                Text = rating.Username ?? "Anonymous",
                Location = new Point(10, 10),
                Size = new Size(150, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White
            };

            var ratingLabel = new Label
            {
                Text = $"Rating: {rating.Rating:F1}/10",
                Location = new Point(170, 10),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Gold
            };

            var dateLabel = new Label
            {
                Text = rating.RatedDate.ToString("dd/MM/yyyy"),
                Location = new Point(250, 10),
                Size = new Size(70, 20),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Gray
            };

            var commentLabel = new Label
            {
                Text = rating.Comment ?? "",
                Location = new Point(10, 35),
                Size = new Size(230, 55),
                Font = new Font("Segoe UI", 10)
            };

            var likeButton = new Button
            {
                Text = $"👍 {rating.LikedBy?.Count ?? 0}",
                Location = new Point(250, 65),
                Size = new Size(70, 25),
                FlatStyle = FlatStyle.Flat,
                BackColor = rating.LikedBy?.Contains(_currentUser.Id) ?? false ? 
                    Color.FromArgb(60, 60, 60) : Color.FromArgb(40, 40, 40),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9),
                Cursor = Cursors.Hand
            };

            likeButton.Click += async (s, e) =>
            {
                await _mongoService.LikeComment(_movie.Id, _currentUser.Id, rating.UserId);
                _movie = await _mongoService.GetMovie(_movie.Id);
                LoadComments();
            };

            panel.Controls.AddRange(new Control[] {
                usernameLabel, ratingLabel, dateLabel, commentLabel, likeButton
            });

            return panel;
        }
    }
} 