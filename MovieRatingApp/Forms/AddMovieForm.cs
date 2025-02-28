using System;
using System.Drawing;
using System.Windows.Forms;
using MovieRatingApp.Services;
using MovieRatingApp.Models;
using System.Collections.Generic;

namespace MovieRatingApp.Forms
{
    public partial class AddMovieForm : Form
    {
        private readonly MongoDBService _mongoService;
        private TextBox txtTitle;
        private TextBox txtImageUrl;
        private NumericUpDown numPrice;
        private TextBox txtDescription;
        private NumericUpDown numReleaseYear;
        private TextBox txtDirector;
        private TextBox txtGenre;
        private TextBox txtCast;
        private CheckBox chkIsOnSale;
        private NumericUpDown numDiscountRate;
        private Button btnAdd;

        public AddMovieForm(MongoDBService mongoService)
        {
            _mongoService = mongoService;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(500, 600);
            this.Text = "Add New Movie";
            this.BackColor = Color.FromArgb(24, 24, 24);
            this.ForeColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            var labelStyle = new Action<Label>((lbl) => {
                lbl.AutoSize = true;
                lbl.Font = new Font("Segoe UI", 9);
            });

            var inputStyle = new Action<Control>((ctrl) => {
                ctrl.Width = 300;
                if (ctrl is TextBox)
                {
                    ((TextBox)ctrl).BackColor = Color.FromArgb(32, 32, 32);
                    ((TextBox)ctrl).ForeColor = Color.White;
                }
                else if (ctrl is NumericUpDown)
                {
                    ((NumericUpDown)ctrl).BackColor = Color.FromArgb(32, 32, 32);
                    ((NumericUpDown)ctrl).ForeColor = Color.White;
                }
            });

            int yPos = 20;
            int spacing = 50;

            // Title
            var lblTitle = new Label { Text = "Title:", Location = new Point(20, yPos) };
            labelStyle(lblTitle);
            txtTitle = new TextBox { Location = new Point(150, yPos) };
            inputStyle(txtTitle);

            // Image URL
            yPos += spacing;
            var lblImageUrl = new Label { Text = "Image URL:", Location = new Point(20, yPos) };
            labelStyle(lblImageUrl);
            txtImageUrl = new TextBox { Location = new Point(150, yPos) };
            inputStyle(txtImageUrl);

            // Price
            yPos += spacing;
            var lblPrice = new Label { Text = "Price ($):", Location = new Point(20, yPos) };
            labelStyle(lblPrice);
            numPrice = new NumericUpDown { Location = new Point(150, yPos), Minimum = 0, Maximum = 1000, DecimalPlaces = 2 };
            inputStyle(numPrice);

            // Description
            yPos += spacing;
            var lblDescription = new Label { Text = "Description:", Location = new Point(20, yPos) };
            labelStyle(lblDescription);
            txtDescription = new TextBox { Location = new Point(150, yPos), Multiline = true, Height = 60 };
            inputStyle(txtDescription);

            // Release Year
            yPos += spacing + 30;
            var lblReleaseYear = new Label { Text = "Release Year:", Location = new Point(20, yPos) };
            labelStyle(lblReleaseYear);
            numReleaseYear = new NumericUpDown { Location = new Point(150, yPos), Minimum = 1900, Maximum = DateTime.Now.Year };
            inputStyle(numReleaseYear);
            numReleaseYear.Value = DateTime.Now.Year;

            // Director
            yPos += spacing;
            var lblDirector = new Label { Text = "Director:", Location = new Point(20, yPos) };
            labelStyle(lblDirector);
            txtDirector = new TextBox { Location = new Point(150, yPos) };
            inputStyle(txtDirector);

            // Genre
            yPos += spacing;
            var lblGenre = new Label { Text = "Genre:", Location = new Point(20, yPos) };
            labelStyle(lblGenre);
            txtGenre = new TextBox { Location = new Point(150, yPos) };
            inputStyle(txtGenre);

            // Cast
            yPos += spacing;
            var lblCast = new Label { Text = "Cast (comma-separated):", Location = new Point(20, yPos) };
            labelStyle(lblCast);
            txtCast = new TextBox { Location = new Point(150, yPos) };
            inputStyle(txtCast);

            // Is On Sale
            yPos += spacing;
            chkIsOnSale = new CheckBox { Text = "On Sale", Location = new Point(150, yPos), AutoSize = true };
            chkIsOnSale.CheckedChanged += (s, e) => numDiscountRate.Enabled = chkIsOnSale.Checked;

            // Discount Rate
            yPos += spacing;
            var lblDiscountRate = new Label { Text = "Discount Rate (%):", Location = new Point(20, yPos) };
            labelStyle(lblDiscountRate);
            numDiscountRate = new NumericUpDown { Location = new Point(150, yPos), Minimum = 0, Maximum = 100, DecimalPlaces = 0, Enabled = false };
            inputStyle(numDiscountRate);

            // Add Button
            yPos += spacing;
            btnAdd = new Button
            {
                Text = "Add Movie",
                Location = new Point(150, yPos),
                Width = 300,
                Height = 40,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(40, 40, 40),
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            btnAdd.Click += btnAdd_Click;

            this.Controls.AddRange(new Control[] {
                lblTitle, txtTitle,
                lblImageUrl, txtImageUrl,
                lblPrice, numPrice,
                lblDescription, txtDescription,
                lblReleaseYear, numReleaseYear,
                lblDirector, txtDirector,
                lblGenre, txtGenre,
                lblCast, txtCast,
                chkIsOnSale,
                lblDiscountRate, numDiscountRate,
                btnAdd
            });
        }

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text) ||
                string.IsNullOrWhiteSpace(txtImageUrl.Text) ||
                string.IsNullOrWhiteSpace(txtDescription.Text) ||
                string.IsNullOrWhiteSpace(txtDirector.Text) ||
                string.IsNullOrWhiteSpace(txtGenre.Text))
            {
                MessageBox.Show("Please fill in all required fields!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var movie = new Movie
            {
                Title = txtTitle.Text.Trim(),
                ImageUrl = txtImageUrl.Text.Trim(),
                Price = numPrice.Value,
                Description = txtDescription.Text.Trim(),
                ReleaseYear = (int)numReleaseYear.Value,
                Director = txtDirector.Text.Trim(),
                Genre = txtGenre.Text.Trim(),
                Cast = new List<string>(txtCast.Text.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())),
                IsOnSale = chkIsOnSale.Checked,
                DiscountRate = chkIsOnSale.Checked ? (decimal)numDiscountRate.Value / 100 : 0,
                UserRatings = new List<UserRating>(),
                AverageUserRating = 0,
                PurchaseCount = 0
            };

            try
            {
                await _mongoService.AddMovie(movie);
                MessageBox.Show("Movie added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding movie: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
} 