using Npgsql;

namespace AdminAndManager
{
    public partial class Form1 : Form
    {
        private NpgsqlConnection connection;
        private string connectionString = "Host=localhost;Port=5432;Database=mangadm;Username=postgres;Password=1212321";
        public Form1()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Пожалуйста, введите имя пользователя и пароль");
                return;
            }

            bool loggedIn = Login(username, password);

            if (loggedIn)
            {
                string role = GetUserRole(username);
                OpenForm(role);
            }
            else
            {
                MessageBox.Show("Неверное имя пользователя или пароль");
            }
        }

        private bool Login(string username, string password)
        {
            try
            {
                using (connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT COUNT(*) FROM mnadm WHERE username = @username AND password = @password";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", password);

                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при подключении к базе данных: " + ex.Message);
                return false;
            }
        }

        private string GetUserRole(string username)
        {
            try
            {
                using (connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT role FROM mnadm WHERE username = @username";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        return cmd.ExecuteScalar().ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при получении роли пользователя: " + ex.Message);
                return string.Empty;
            }
        }

        private void OpenForm(string role)
        {
            switch (role)
            {
                case "admin":
                    AdminForm adminForm = new AdminForm();
                    adminForm.Show();
                    break;
                case "manager":
                    ManagerForm managerForm = new ManagerForm();
                    managerForm.Show();
                    break;
                default:
                    MessageBox.Show("Некорректная роль пользователя");
                    break;
            }
        }
    }
}