using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace set
{
    public partial class login : Form
    {
        SqlConnection conn;
        SqlCommand cmd;
        SqlDataReader reader;
        System.Security.Cryptography.SHA256 HASH = SHA256.Create();
        private const string FixedSalt = "s8Yv9pqRvqflfXp2Ztjx1w=="; // 定義固定的 Salt 值

        public login()
        {
            InitializeComponent();
            this.Load += new System.EventHandler(this.login_Load); // 綁定 Load 事件處理程序
        }

        private void login_Load(object sender, EventArgs e)
        {
            // 在這裡可以進行表單載入時的初始化工作
        }

        private void button1_Click(object sender, EventArgs e)
        {
            byte[] salt = Encoding.UTF8.GetBytes(FixedSalt); // 使用固定的 Salt 值

            try
            {
                using (conn = new SqlConnection("DATA Source=192.168.233.1\\EXPRESS2022; Initial Catalog=orders; Integrated Security=false; user id=SQLDBA; password=654321"))
                {
                    conn.Open();

                    using (cmd = new SqlCommand("SELECT password FROM customers WHERE email = @Email", conn))
                    {
                        cmd.Parameters.Add("@Email", SqlDbType.NVarChar);
                        cmd.Parameters["@Email"].Value = textBox1.Text;

                        using (reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string storedHashedPassword = reader["password"].ToString();
                                reader.Close();

                                byte[] source = Encoding.UTF8.GetBytes(textBox2.Text); // textBox2 是輸入密碼的 TextBox
                                byte[] passandsalt = source.Concat(salt).ToArray();
                                byte[] crypto = HASH.ComputeHash(passandsalt);
                                string hashedPassword = Convert.ToBase64String(crypto);

                                // 調試訊息，檢查 hash 過程
                               // MessageBox.Show($"storedHashedPassword: {storedHashedPassword}\nhashedPassword: {hashedPassword}");

                                if (hashedPassword == storedHashedPassword)
                                {
                                    MessageBox.Show("登入成功！");
                                }
                                else
                                {
                                    MessageBox.Show("登入失敗，請檢查您的電子郵件和密碼。");
                                }
                            }
                            else
                            {
                                reader.Close();
                                Form1 registerForm = new Form1();
                                registerForm.Show();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("登入失敗: " + ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form1 registerForm = new Form1();
            registerForm.Show();
        }
    }
}

