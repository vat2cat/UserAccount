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
                        cmd.Parameters.Add("@Email", SqlDbType.NVarChar);  // 定義 SQL 查詢中的參數
                        cmd.Parameters["@Email"].Value = textBox1.Text;  // 從 textBox1 獲取用戶輸入的電子郵件地址

                        using (reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string storedHashedPassword = reader["password"].ToString();  // 從資料庫中獲取存儲的HASH密碼
                                reader.Close();

                                byte[] source = Encoding.UTF8.GetBytes(textBox2.Text); // textBox2 是輸入密碼的 TextBox // 獲取用戶輸入的密碼
                                byte[] passandsalt = source.Concat(salt).ToArray();   // 將密碼和 Salt 合併
                                byte[] crypto = HASH.ComputeHash(passandsalt);   // 對合併後的字節數組進行HASH
                                string hashedPassword = Convert.ToBase64String(crypto);  // 將HASH結果轉換為 Base64 字符串

                                // 調試訊息，檢查 hash 過程
                               // MessageBox.Show($"storedHashedPassword: {storedHashedPassword}\nhashedPassword: {hashedPassword}");

                                if (hashedPassword == storedHashedPassword)  // 比較HASH Value是否匹配
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
                                registerForm.Show();  // 如果電子郵件地址未被註冊，顯示註冊表單
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("登入失敗: " + ex.Message);  // 如果發生異常，顯示錯誤信息
            }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form1 registerForm = new Form1();
            registerForm.Show();  // 顯示註冊表單
        }
        }
    }
}
