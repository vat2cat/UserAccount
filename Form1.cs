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
        byte[] passandsalt;  // 用來存儲密碼和 Salt 的字節數組
        SqlConnection conn;  // 用來管理與 SQL Server 的連接
        SqlCommand cmd;   // 用來執行 SQL 命令
        SqlDataReader reader;  // 用來讀取 SQL 查詢的結果
        System.Security.Cryptography.SHA256 HASH = SHA256.Create();  / 初始化 SHA256 HASH
        private const string FixedSalt = "s8Yv9pqRvqflfXp2Ztjx1w=="; // 定義固定的 Salt 值

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
                    cmd.Parameters.Add("@Email", SqlDbType.NVarChar);  // 定義 SQL 查詢中的參數
                    cmd.Parameters["@Email"].Value = textBox1.Text;  // 從 textBox1 獲取用戶輸入的電子郵件地址

                    using (reader = cmd.ExecuteReader())  // 執行查詢並讀取結果
                    {
                        if (!reader.Read())  // 如果沒有讀到結果，說明該電子郵件地址未被註冊
                        {
                            reader.Close();
                            byte[] source = Encoding.UTF8.GetBytes(textBox2.Text); // textBox2 是輸入密碼的 TextBox
                            passandsalt = source.Concat(salt).ToArray();  // 將密碼和 Salt 合併
                            byte[] crypto = HASH.ComputeHash(passandsalt);  // 對合併後的字節數組進行HASH
                            string password = Convert.ToBase64String(crypto);  // 將HASH結果轉換為 Base64 字符串

                            using (SqlCommand insertCmd = new SqlCommand("insert into customers (email, password) values (@Email, @password)", conn))
                            {
                                insertCmd.Parameters.Add("@Email", SqlDbType.NVarChar);  // 定義插入命令中的參數
                                insertCmd.Parameters["@Email"].Value = textBox1.Text;
                                insertCmd.Parameters.Add("@password", SqlDbType.NVarChar);
                                insertCmd.Parameters["@password"].Value = password;  // 將加密後的密碼存儲到數據庫中
                                insertCmd.ExecuteNonQuery();  // 執行插入操作
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
