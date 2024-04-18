using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.Data.SqlClient;

namespace CarShowroom
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string connectionString = ClassSQL.GetConnSQL();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnPassword_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("К сожалению вы можете узнать свои данные от учетной записи, только у администратора");
        }
        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Программа разработана для упрощения работы персонала на предприяти \"Автосалон\".");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string query = "SELECT COUNT(1) FROM Sotrudniki WHERE S_LOGIN = @log AND S_PASSWORD = @pas";

            try
            {
                using (SqlConnection sqlcon = new SqlConnection(connectionString))
                {
                    using (SqlCommand sqlcmd = new SqlCommand(query, sqlcon))
                    {
                        sqlcmd.CommandType = CommandType.Text;
                        sqlcmd.Parameters.AddWithValue("@log", txtLog.Text);
                        sqlcmd.Parameters.AddWithValue("@pas", password.Password);

                        sqlcon.Open();
                        int count = Convert.ToInt32(sqlcmd.ExecuteScalar());

                        if (count == 1)
                        {
                            string postQuery = "SELECT S_POSTID FROM Sotrudniki WHERE S_LOGIN = @log";
                            SqlCommand postCmd = new SqlCommand(postQuery, sqlcon);
                            postCmd.Parameters.AddWithValue("@log", txtLog.Text);
                            int postId = Convert.ToInt32(postCmd.ExecuteScalar());

                            if (postId == 1)
                            {
                                Admin admin = new Admin();
                                admin.Show();
                            }
                            else if (postId == 2)
                            {
                                Prodavec prod = new Prodavec();
                                prod.Show();
                            }
                            else
                            {
                                MessageBox.Show("Должность не определена.");
                            }

                            Close();
                        }
                        else
                        {
                            MessageBox.Show("Не правильно внесены данные");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
