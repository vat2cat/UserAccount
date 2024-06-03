using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Data.SqlClient;

namespace set
{
    public partial class Form1 : Form
    {
        byte[] passandsalt;
        SqlConnection conn;
        SqlCommand cmd;
        SqlDataReader reader;
        System.Security.Cryptography.SHA256 HASH = SHA256.Create();
        private const string FixedSalt = "private const string FixedSalt = \"MySuperSecretSaltValue123!\";\r\n"; // 定義固定的 Salt 值

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // 初始化表單時的處理邏輯
        }

        private void button1_Click(object sender, EventArgs e)
        {
            byte[] salt = Encoding.UTF8.GetBytes(FixedSalt); // 使用固定的 Salt 值

            using (conn = new SqlConnection("DATA Source=192.168.233.1\\EXPRESS2022; Initial Catalog=orders; Integrated Security=false; user id=SQLDBA; password=654321"))
            {
                conn.Open();

                using (cmd = new SqlCommand("select password from customers where email = @Email", conn))
                {
                    cmd.Parameters.Add("@Email", SqlDbType.NVarChar);
                    cmd.Parameters["@Email"].Value = textBox1.Text;

                    using (reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            reader.Close();
                            byte[] source = Encoding.UTF8.GetBytes(textBox2.Text); // textBox2 是輸入密碼的 TextBox
                            passandsalt = source.Concat(salt).ToArray();
                            byte[] crypto = HASH.ComputeHash(passandsalt);
                            string password = Convert.ToBase64String(crypto);

                            using (SqlCommand insertCmd = new SqlCommand("insert into customers (email, password) values (@Email, @password)", conn))
                            {
                                insertCmd.Parameters.Add("@Email", SqlDbType.NVarChar);
                                insertCmd.Parameters["@Email"].Value = textBox1.Text;
                                insertCmd.Parameters.Add("@password", SqlDbType.NVarChar);
                                insertCmd.Parameters["@password"].Value = password;
                                insertCmd.ExecuteNonQuery();
                                MessageBox.Show("註冊成功！");
                            }
                        }
                        else
                        {
                            MessageBox.Show("該電子郵件地址已被註冊。");
                        }
                    }
                }
            }
        }
    }
}