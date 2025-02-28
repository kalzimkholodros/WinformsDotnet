using System;
using System.Windows.Forms;
using MovieRatingApp.Services;
using MovieRatingApp.Models;

namespace MovieRatingApp.Forms
{
    public partial class LoginForm : Form
    {
        private readonly MongoDBService _mongoService;
        public User LoggedInUser { get; private set; }

        public LoginForm(MongoDBService mongoService)
        {
            _mongoService = mongoService;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.txtUsername = new TextBox();
            this.txtPassword = new TextBox();
            this.btnLogin = new Button();
            this.btnRegister = new Button();
            this.label1 = new Label();
            this.label2 = new Label();

            // txtUsername
            this.txtUsername.Location = new System.Drawing.Point(120, 50);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(200, 23);

            // txtPassword
            this.txtPassword.Location = new System.Drawing.Point(120, 90);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(200, 23);

            // label1
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 53);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 15);
            this.label1.Text = "Username:";

            // label2
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 93);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 15);
            this.label2.Text = "Password:";

            // btnLogin
            this.btnLogin.Location = new System.Drawing.Point(120, 130);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(95, 30);
            this.btnLogin.Text = "Login";
            this.btnLogin.Click += new EventHandler(btnLogin_Click);

            // btnRegister
            this.btnRegister.Location = new System.Drawing.Point(225, 130);
            this.btnRegister.Name = "btnRegister";
            this.btnRegister.Size = new System.Drawing.Size(95, 30);
            this.btnRegister.Text = "Register";
            this.btnRegister.Click += new EventHandler(btnRegister_Click);

            // LoginForm
            this.ClientSize = new System.Drawing.Size(350, 200);
            this.Controls.AddRange(new Control[] {
                this.label1,
                this.label2,
                this.txtUsername,
                this.txtPassword,
                this.btnLogin,
                this.btnRegister
            });
            this.Name = "LoginForm";
            this.Text = "Movie Rating App - Login";
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            var username = txtUsername.Text;
            var password = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter username and password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            LoggedInUser = await _mongoService.AuthenticateUser(username, password);
            if (LoggedInUser != null)
            {
                var mainForm = new MovieListForm(_mongoService, LoggedInUser);
                this.Hide();
                mainForm.ShowDialog();
                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid username or password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            var registerForm = new RegisterForm(_mongoService);
            registerForm.ShowDialog();
        }

        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private Button btnRegister;
        private Label label1;
        private Label label2;
    }
} 