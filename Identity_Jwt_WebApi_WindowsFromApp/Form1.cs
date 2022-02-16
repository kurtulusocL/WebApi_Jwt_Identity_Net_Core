using Identity_Jwt_WebApi_WindowsFromApp.Forms;
using Identity_Jwt_WebApi_WindowsFromApp.Models.AuthModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Identity_Jwt_WebApi_WindowsFromApp
{
    public partial class Form1 : Form
    {
        //kurtulusocal@gmail.com => kurtulusocL_1972
        //kurtulusocal@outlook.com => ocL_1972
        //kurtulusocal@mail.com => ocL_12345678
        HttpClient client = new HttpClient();
        public Form1()
        {
            InitializeComponent();
        }
        void Clear()
        {
            txtLoginPassword.Clear();
            txtLoginUsername.Clear();
            txtRegisterConfirmPassword.Clear();
            txtRegisterPassword.Clear();
            txtRegisterUsername.Clear();
        }
               
        private async void btnRegister_Click(object sender, EventArgs e)
        {
            var model = new RegisterDto
            {
                Email = txtRegisterUsername.Text,
                Password = txtRegisterPassword.Text,
                ConfirmPassword = txtRegisterConfirmPassword.Text
            };

            var jsonData = JsonConvert.SerializeObject(model);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("http://localhost:50707/api/accounts/register", content);
            var responseBody = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<UserManagerResponseDto>(responseBody);
            if (responseObject.IsSuccess)
            {
                MessageBox.Show("Your user account created successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Clear();
            }
            else
            {
                MessageBox.Show("Your user account could not create", "Warning", MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning);
            }
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            var model = new LoginDto
            {
                Email = txtLoginUsername.Text,
                Password = txtLoginPassword.Text,
            };

            var jsonData = JsonConvert.SerializeObject(model);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("http://localhost:50707/api/accounts/login", content);
            var responseBody = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<UserManagerResponseDto>(responseBody);
            if (responseObject.IsSuccess)
            {                
                MessageBox.Show("It's successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Clear();
            }
            else
            {
                MessageBox.Show("There was a mistake.", "Warning", MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning);
            }
        }
    }
}
