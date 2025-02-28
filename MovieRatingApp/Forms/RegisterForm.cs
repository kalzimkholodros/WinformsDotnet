using System;
using System.Windows.Forms;
using MovieRatingApp.Services;

namespace MovieRatingApp.Forms
{
    public partial class RegisterForm : Form
    {
        private readonly MongoDBService _mongoService;

        public RegisterForm(MongoDBService mongoService)
        {
            InitializeComponent();
            _mongoService = mongoService;
        }

        private void InitializeComponent()
        {
            this.txtUsername = new TextBox();
            this.txtPassword = new TextBox();
            this.txtConfirmPassword = new TextBox();
            this.btnRegister = new Button();
            this.label1 = new Label();
            this.label2 = new Label();
            this.label3 = new Label();

            // txtUsername
            this.txtUsername.Location = new System.Drawing.Point(150, 50);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(200, 23);

            // txtPassword
            this.txtPassword.Location = new System.Drawing.Point(150, 90);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(200, 23);

            // txtConfirmPassword
            this.txtConfirmPassword.Location = new System.Drawing.Point(150, 130);
            this.txtConfirmPassword.Name = "txtConfirmPassword";
            this.txtConfirmPassword.PasswordChar = '*';
            this.txtConfirmPassword.Size = new System.Drawing.Size(200, 23);

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

            // label3
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 133);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(107, 15);
            this.label3.Text = "Confirm Password:";

            // btnRegister
            this.btnRegister.Location = new System.Drawing.Point(150, 170);
            this.btnRegister.Name = "btnRegister";
            this.btnRegister.Size = new System.Drawing.Size(200, 30);
            this.btnRegister.Text = "Register";
            this.btnRegister.Click += new EventHandler(btnRegister_Click);

            // RegisterForm
            this.ClientSize = new System.Drawing.Size(380, 230);
            this.Controls.AddRange(new Control[] {
                this.label1,
                this.label2,
                this.label3,
                this.txtUsername,
                this.txtPassword,
                this.txtConfirmPassword,
                this.btnRegister
            });
            this.Name = "RegisterForm";
            this.Text = "Movie Rating App - Register";
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private async void btnRegister_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || 
                string.IsNullOrWhiteSpace(txtPassword.Text) || 
                string.IsNullOrWhiteSpace(txtConfirmPassword.Text))
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }

            if (txtPassword.Text != txtConfirmPassword.Text)
            {
                MessageBox.Show("Passwords do not match.");
                return;
            }

            bool success = await _mongoService.RegisterUser(txtUsername.Text, txtPassword.Text);
            
            if (success)
            {
                MessageBox.Show("Registration successful! You can now login.");
                this.Close();
            }
            else
            {
                MessageBox.Show("Username already exists. Please choose a different username.");
            }
        }

        private TextBox txtUsername;
        private TextBox txtPassword;
        private TextBox txtConfirmPassword;
        private Button btnRegister;
        private Label label1;
        private Label label2;
        private Label label3;
    }
} 