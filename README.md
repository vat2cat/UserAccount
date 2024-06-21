# 介紹
帳號的登入、註冊頁面，透過C# 來實現一個登入、註冊的 Windows Forms 應用程式，使用 SHA256 進行密碼HASH以及固定的 Salt 值來增強安全性。
# Demo
註冊成功：![螢幕擷取畫面 2024-06-21 231446](https://github.com/vat2cat/UserAccount/assets/160461838/30942c5b-5aed-4b37-b0c9-5157785c5a58)

(https://github.com/vat2cat/UserAccount/assets/160461838/dda155a6-579d-41fd-ae18-4135274a39e8)
重複註冊：
(https://github.com/vat2cat/UserAccount/assets/160461838/47a19141-ae85-440c-9307-a2cebfee67cf)
登入失敗：
(https://github.com/vat2cat/UserAccount/assets/160461838/78d18ca2-6380-4c53-87bb-32cdc4b3c031)
登入成功：
(https://github.com/vat2cat/UserAccount/assets/160461838/b5dde03c-142d-4852-ac3b-e56ff31b4f73)

# 註冊頁面(Form1)
功能介紹：(1)初始化與 SQL Server 的連接。(2)檢查電子郵件是否已經被註冊。(3)如果未註冊，將電子郵件和加密後的密碼插入到資料庫中。(4)顯示註冊成功或失敗的消息。
# 登入頁面(login)
功能介紹：(1)初始化與 SQL Server 的連接。(2)檢查輸入的電子郵件是否已經註冊。(3)比較用戶輸入的密碼經過HASH處理後與數據庫中的HASH Value是否匹配。(4)顯示登入成功或失敗的消息。
