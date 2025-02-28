using System;
using System.Drawing;
using System.Windows.Forms;
using MovieRatingApp.Services;
using MovieRatingApp.Models;
using System.Collections.Generic;
using MongoDB.Bson;
using System.Threading.Tasks;

namespace MovieRatingApp.Forms
{
    public partial class MovieListForm : Form
    {
        private readonly MongoDBService _mongoService;
        private readonly User _currentUser;
        private List<Movie> _movies;
        private FlowLayoutPanel moviePanel;
        private Panel topPanel;
        private Button btnAddMovie;
        private Button btnViewFavorites;
        private Button btnPopular;
        private Button btnOnSale;
        private Button btnFriends;
        private Label lblBalance;
        private Label lblUsername;
        private PictureBox profilePicture;

        public MovieListForm(MongoDBService mongoService, User currentUser)
        {
            _mongoService = mongoService;
            _currentUser = currentUser;
            InitializeComponent();
            LoadMovies();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(1200, 800);
            this.Text = "Movie Store";
            this.BackColor = Color.FromArgb(18, 18, 18);
            this.ForeColor = Color.White;

            // Top Panel
            topPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(24, 24, 24)
            };

            // Profile Section
            profilePicture = new PictureBox
            {
                Size = new Size(40, 40),
                Location = new Point(10, 10),
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle
            };
            try { profilePicture.Load(_currentUser.ProfilePicture); }
            catch { profilePicture.BackColor = Color.Gray; }

            lblUsername = new Label
            {
                Text = _currentUser.Username,
                Location = new Point(60, 10),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            lblBalance = new Label
            {
                Text = $"Balance: ${_currentUser.Balance:F2}",
                Location = new Point(60, 30),
                AutoSize = true,
                Font = new Font("Segoe UI", 9)
            };

            // Buttons
            var buttonStyle = new Action<Button>((btn) => {
                btn.FlatStyle = FlatStyle.Flat;
                btn.BackColor = Color.FromArgb(40, 40, 40);
                btn.ForeColor = Color.White;
                btn.Size = new Size(100, 35);
                btn.Font = new Font("Segoe UI", 9);
                btn.Cursor = Cursors.Hand;
            });

            btnAddMovie = new Button
            {
                Text = "Add Movie",
                Location = new Point(200, 13),
                Visible = _currentUser.Role == "admin"
            };
            buttonStyle(btnAddMovie);
            btnAddMovie.Click += btnAddMovie_Click;

            btnViewFavorites = new Button
            {
                Text = "Favorites",
                Location = new Point(310, 13)
            };
            buttonStyle(btnViewFavorites);
            btnViewFavorites.Click += btnViewFavorites_Click;

            btnPopular = new Button
            {
                Text = "Popular",
                Location = new Point(420, 13)
            };
            buttonStyle(btnPopular);
            btnPopular.Click += async (s, e) => await LoadPopularMovies();

            btnOnSale = new Button
            {
                Text = "On Sale",
                Location = new Point(530, 13)
            };
            buttonStyle(btnOnSale);
            btnOnSale.Click += async (s, e) => await LoadMoviesOnSale();

            btnFriends = new Button
            {
                Text = "Friends",
                Location = new Point(640, 13)
            };
            buttonStyle(btnFriends);
            btnFriends.Click += ShowFriendsDialog;

            // Movie Panel
            moviePanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.FromArgb(18, 18, 18),
                Padding = new Padding(20)
            };

            // Add controls
            topPanel.Controls.AddRange(new Control[] {
                profilePicture, lblUsername, lblBalance,
                btnAddMovie, btnViewFavorites, btnPopular,
                btnOnSale, btnFriends
            });

            this.Controls.Add(topPanel);
            this.Controls.Add(moviePanel);
        }

        private async void LoadMovies(bool showFavorites = false)
        {
            moviePanel.Controls.Clear();
            _movies = showFavorites ? 
                await _mongoService.GetFavoriteMovies(_currentUser.Id) :
                await _mongoService.GetAllMovies();

            foreach (var movie in _movies)
            {
                var movieCard = CreateMovieCard(movie);
                moviePanel.Controls.Add(movieCard);
            }
        }

        private Panel CreateMovieCard(Movie movie)
        {
            var card = new Panel
            {
                Width = 250,
                Height = 450,
                Margin = new Padding(15),
                BackColor = Color.FromArgb(32, 32, 32)
            };

            var pictureBox = new PictureBox
            {
                Width = 230,
                Height = 330,
                Location = new Point(10, 10),
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.None
            };

            try { pictureBox.Load(movie.ImageUrl); }
            catch { pictureBox.BackColor = Color.Gray; }
            
            pictureBox.Cursor = Cursors.Hand;
            pictureBox.Click += (s, e) =>
            {
                var detailForm = new MovieDetailForm(movie, _currentUser, _mongoService);
                detailForm.ShowDialog();
            };

            var titleLabel = new Label
            {
                Text = movie.Title,
                Location = new Point(10, 345),
                Width = 230,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.White
            };

            decimal finalPrice = movie.IsOnSale ? movie.Price * (1 - movie.DiscountRate) : movie.Price;
            var priceLabel = new Label
            {
                Text = movie.IsOnSale ? 
                    $"${finalPrice:F2} (${movie.Price:F2})" : 
                    $"${movie.Price:F2}",
                Location = new Point(10, 370),
                Width = 110,
                ForeColor = movie.IsOnSale ? Color.LightGreen : Color.White,
                Font = new Font("Segoe UI", 10)
            };

            var ratingLabel = new Label
            {
                Text = $"Rating: {movie.AverageUserRating:F1}",
                Location = new Point(130, 370),
                Width = 110,
                ForeColor = Color.Gold,
                Font = new Font("Segoe UI", 10)
            };

            var buttonStyle = new Action<Button>((btn) => {
                btn.FlatStyle = FlatStyle.Flat;
                btn.BackColor = Color.FromArgb(40, 40, 40);
                btn.ForeColor = Color.White;
                btn.Height = 30;
                btn.Font = new Font("Segoe UI", 9);
                btn.Cursor = Cursors.Hand;
            });

            var purchaseButton = new Button
            {
                Text = _currentUser.OwnedMovies?.Contains(movie.Id) ?? false ? "Owned" : "Purchase",
                Location = new Point(10, 400),
                Width = 75,
                Enabled = !(_currentUser.OwnedMovies?.Contains(movie.Id) ?? false)
            };
            buttonStyle(purchaseButton);
            purchaseButton.Click += async (s, e) => await PurchaseMovie(movie, purchaseButton);

            var rateButton = new Button
            {
                Text = "Rate",
                Location = new Point(90, 400),
                Width = 75
            };
            buttonStyle(rateButton);
            rateButton.Click += (s, e) => ShowRatingDialog(movie);

            var giftButton = new Button
            {
                Text = "Gift",
                Location = new Point(170, 400),
                Width = 70,
                Enabled = _currentUser.OwnedMovies?.Contains(movie.Id) ?? false
            };
            buttonStyle(giftButton);
            giftButton.Click += (s, e) => ShowGiftDialog(movie);

            card.Controls.AddRange(new Control[] {
                pictureBox, titleLabel, priceLabel,
                ratingLabel, purchaseButton, rateButton, giftButton
            });

            return card;
        }

        private async void ShowRatingDialog(Movie movie)
        {
            using (var ratingForm = new Form())
            {
                ratingForm.Text = $"Rate {movie.Title}";
                ratingForm.Size = new Size(300, 150);
                ratingForm.StartPosition = FormStartPosition.CenterParent;
                ratingForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                ratingForm.MaximizeBox = false;
                ratingForm.MinimizeBox = false;

                var numericUpDown = new NumericUpDown
                {
                    Location = new Point(20, 20),
                    Size = new Size(100, 25),
                    Minimum = 1,
                    Maximum = 10,
                    DecimalPlaces = 1,
                    Increment = 0.1M
                };

                var submitButton = new Button
                {
                    Text = "Submit",
                    Location = new Point(20, 60),
                    DialogResult = DialogResult.OK
                };

                ratingForm.Controls.AddRange(new Control[] { numericUpDown, submitButton });

                if (ratingForm.ShowDialog() == DialogResult.OK)
                {
                    await _mongoService.AddRating(movie.Id, _currentUser.Id, (double)numericUpDown.Value);
                    LoadMovies();
                }
            }
        }

        private void btnAddMovie_Click(object sender, EventArgs e)
        {
            if (_currentUser.Role != "admin")
            {
                MessageBox.Show("Only administrators can add movies.");
                return;
            }

            var addMovieForm = new AddMovieForm(_mongoService);
            if (addMovieForm.ShowDialog() == DialogResult.OK)
            {
                LoadMovies();
            }
        }

        private void btnViewFavorites_Click(object sender, EventArgs e)
        {
            var button = (Button)sender;
            if (button.Text == "Favorites")
            {
                LoadMovies(true);
                button.Text = "All Movies";
            }
            else
            {
                LoadMovies(false);
                button.Text = "Favorites";
            }
        }

        private async Task PurchaseMovie(Movie movie, Button purchaseButton)
        {
            decimal finalPrice = movie.IsOnSale ? movie.Price * (1 - movie.DiscountRate) : movie.Price;
            
            if (_currentUser.Balance < finalPrice)
            {
                MessageBox.Show("Insufficient balance!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var result = await _mongoService.PurchaseMovie(_currentUser.Id, movie.Id);
            if (result)
            {
                _currentUser.Balance -= finalPrice;
                lblBalance.Text = $"Balance: ${_currentUser.Balance:F2}";
                purchaseButton.Text = "Owned";
                purchaseButton.Enabled = false;
                MessageBox.Show("Purchase successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Purchase failed!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowGiftDialog(Movie movie)
        {
            using (var giftForm = new Form())
            {
                giftForm.Text = $"Gift {movie.Title}";
                giftForm.Size = new Size(400, 300);
                giftForm.StartPosition = FormStartPosition.CenterParent;
                giftForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                giftForm.MaximizeBox = false;
                giftForm.MinimizeBox = false;
                giftForm.BackColor = Color.FromArgb(24, 24, 24);
                giftForm.ForeColor = Color.White;

                var friendsList = new ComboBox
                {
                    Location = new Point(20, 20),
                    Width = 340,
                    DropDownStyle = ComboBoxStyle.DropDownList
                };

                var messageBox = new TextBox
                {
                    Location = new Point(20, 60),
                    Width = 340,
                    Height = 100,
                    Multiline = true,
                    BackColor = Color.FromArgb(32, 32, 32),
                    ForeColor = Color.White
                };

                var sendButton = new Button
                {
                    Text = "Send Gift",
                    Location = new Point(20, 180),
                    Width = 340,
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.FromArgb(40, 40, 40),
                    ForeColor = Color.White
                };

                // Arkadaş listesini yükle
                Task.Run(async () =>
                {
                    var friends = await _mongoService.GetFriends(_currentUser.Id);
                    giftForm.Invoke((MethodInvoker)(() =>
                    {
                        friendsList.Items.AddRange(friends.ToArray());
                        friendsList.DisplayMember = "Username";
                    }));
                });

                sendButton.Click += async (s, e) =>
                {
                    if (friendsList.SelectedItem == null)
                    {
                        MessageBox.Show("Please select a friend!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    var friend = (User)friendsList.SelectedItem;
                    var result = await _mongoService.SendGift(_currentUser.Id, friend.Id, movie.Id, messageBox.Text);
                    
                    if (result)
                    {
                        MessageBox.Show("Gift sent successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        giftForm.DialogResult = DialogResult.OK;
                    }
                    else
                    {
                        MessageBox.Show("Failed to send gift!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                };

                giftForm.Controls.AddRange(new Control[] { friendsList, messageBox, sendButton });
                giftForm.ShowDialog();
            }
        }

        private void ShowFriendsDialog(object sender, EventArgs e)
        {
            using (var friendsForm = new Form())
            {
                friendsForm.Text = "Friends";
                friendsForm.Size = new Size(400, 500);
                friendsForm.StartPosition = FormStartPosition.CenterParent;
                friendsForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                friendsForm.MaximizeBox = false;
                friendsForm.MinimizeBox = false;
                friendsForm.BackColor = Color.FromArgb(24, 24, 24);
                friendsForm.ForeColor = Color.White;

                var friendsList = new ListView
                {
                    View = View.Details,
                    FullRowSelect = true,
                    GridLines = true,
                    Location = new Point(20, 20),
                    Size = new Size(340, 300),
                    BackColor = Color.FromArgb(32, 32, 32),
                    ForeColor = Color.White
                };
                friendsList.Columns.Add("Username", 200);
                friendsList.Columns.Add("Status", 100);

                var addFriendBox = new TextBox
                {
                    Location = new Point(20, 340),
                    Width = 240,
                    BackColor = Color.FromArgb(32, 32, 32),
                    ForeColor = Color.White
                };

                var addButton = new Button
                {
                    Text = "Add Friend",
                    Location = new Point(270, 339),
                    Width = 90,
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.FromArgb(40, 40, 40),
                    ForeColor = Color.White
                };

                // Arkadaş listesini yükle
                Task.Run(async () =>
                {
                    var friends = await _mongoService.GetFriends(_currentUser.Id);
                    friendsForm.Invoke((MethodInvoker)(() =>
                    {
                        foreach (var friend in friends)
                        {
                            var item = new ListViewItem(friend.Username);
                            item.SubItems.Add("Friend");
                            friendsList.Items.Add(item);
                        }
                    }));
                });

                addButton.Click += async (s, e) =>
                {
                    var username = addFriendBox.Text.Trim();
                    if (string.IsNullOrEmpty(username))
                    {
                        MessageBox.Show("Please enter a username!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // TODO: Implement friend request logic
                    MessageBox.Show("Friend request sent!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    addFriendBox.Text = "";
                };

                friendsForm.Controls.AddRange(new Control[] { friendsList, addFriendBox, addButton });
                friendsForm.ShowDialog();
            }
        }

        private async Task LoadPopularMovies()
        {
            _movies = await _mongoService.GetPopularMovies();
            RefreshMovieList();
        }

        private async Task LoadMoviesOnSale()
        {
            _movies = await _mongoService.GetMoviesOnSale();
            RefreshMovieList();
        }

        private void RefreshMovieList()
        {
            moviePanel.Controls.Clear();
            foreach (var movie in _movies)
            {
                var movieCard = CreateMovieCard(movie);
                moviePanel.Controls.Add(movieCard);
            }
        }
    }
} 