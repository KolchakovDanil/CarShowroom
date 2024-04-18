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
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Data;

namespace CarShowroom
{
    /// <summary>
    /// Логика взаимодействия для RedSotrudniki.xaml
    /// </summary>
    public partial class RedSotrudniki : Window
    {
        public RedSotrudniki()
        {
            InitializeComponent();
            SotrudnikID();
            PostName();
        }
        private void insur(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^а-яА-Я]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void inname(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^а-яА-Я]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void inpatr(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^а-яА-Я]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void inphone(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]");
            e.Handled = regex.IsMatch(e.Text);
        }
        public void SotrudnikID()
        {
            combID.Items.Clear();
            string connectionString = ClassSQL.GetConnSQL();
            string query = "SELECT S_ID FROM Sotrudniki";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int sotrudnikId = reader.GetInt32(0);
                        combID.Items.Add(sotrudnikId);
                    }
                }
            }
        }
        public void PostName()
        {
            combPost.Items.Clear();
            string connectionString = ClassSQL.GetConnSQL();
            string query = "SELECT P_NAME FROM Post";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string postdName = reader.GetString(0);
                        combPost.Items.Add(postdName);
                    }
                }
            }
        }

        private void combID_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string connectionString = ClassSQL.GetConnSQL();
            string query = "select S_SURNAME from Sotrudniki where S_ID  = '" + combID.SelectedItem + "'";
            string query1 = "select S_NAME from Sotrudniki where S_ID  = '" + combID.SelectedItem + "'";
            string query2 = "select S_PATR from Sotrudniki where S_ID  = '" + combID.SelectedItem + "'";
            string query3 = "select S_PHONE from Sotrudniki where S_ID  ='" + combID.SelectedItem + "'";
            string query4 = "select P_NAME from Sotrudniki inner join Post on Sotrudniki.S_POSTID = Post.P_ID where S_ID ='" + combID.SelectedItem + "'";
            string query5 = "select S_LOGIN from Sotrudniki where S_ID  ='" + combID.SelectedItem + "'";
            string query6 = "select S_PASSWORD from Sotrudniki where S_ID  ='" + combID.SelectedItem + "'";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                connection.Open();

                SqlCommand Sqlcmd = new SqlCommand(query, connection);
                Sur.Text = Convert.ToString(Sqlcmd.ExecuteScalar());

                SqlCommand Sqlcmd10 = new SqlCommand(query1, connection);
                Name.Text = Convert.ToString(Sqlcmd10.ExecuteScalar());

                SqlCommand Sqlcmd11 = new SqlCommand(query2, connection);
                FIO.Text = Convert.ToString(Sqlcmd11.ExecuteScalar());

                SqlCommand Sqlcmd2 = new SqlCommand(query3, connection);
                phone.Text = Convert.ToString(Sqlcmd2.ExecuteScalar());

                SqlCommand Sqlcmd3 = new SqlCommand(query4, connection);
                combPost.SelectedItem = Convert.ToString(Sqlcmd3.ExecuteScalar());

                SqlCommand Sqlcmd6 = new SqlCommand(query5, connection);
                Login.Text = Convert.ToString(Sqlcmd6.ExecuteScalar());

                SqlCommand Sqlcmd7 = new SqlCommand(query6, connection);
                Password.Text = Convert.ToString(Sqlcmd7.ExecuteScalar());

                connection.Close();
            }
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (combID.SelectedIndex == -1)
            {
                MessageBox.Show("Вы не выбрали индекс сотрудника!");
            }
            else
            {
                if (Sur.Text == "" || Name.Text == "" || FIO.Text == "" || phone.Text == "" || combPost.SelectedIndex == -1 || Login.Text == "" || Password.Text == "")
                {
                    MessageBox.Show("Вы не заполнили все данные");
                }
                else
                {
                    string connectionString = ClassSQL.GetConnSQL();
                    SqlConnection savezak = new SqlConnection(connectionString);
                    try
                    {
                        savezak.Open();
                        string save = "SELECT P_ID FROM Post WHERE P_NAME ='" + combPost.SelectedItem + "'";
                        SqlCommand save1 = new SqlCommand(save, savezak);
                        string savePost = Convert.ToString(save1.ExecuteScalar());

                        string savezakaz = "Update Sotrudniki set S_POSTID = '" + savePost + "', S_SURNAME = '" + Sur.Text + "', S_NAME = '" + Name.Text + "', S_PATR = '" + FIO.Text + "', S_PHONE = '" + phone.Text + "', S_LOGIN = '" + Login.Text + "', S_PASSWORD = '" + Password.Text + "' where S_ID =" + combID.SelectedItem;
                        SqlCommand sz = new SqlCommand(savezakaz, savezak); sz.ExecuteNonQuery();

                        savezak.Close();
                        combID.SelectedIndex = -1;
                        combPost.SelectedIndex = -1;
                        SotrudnikID();
                        MessageBox.Show("Данные обновлены");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        throw;
                    }
                    finally
                    {

                    }
                }
            }
        }
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (combID.SelectedIndex == -1)
            {
                MessageBox.Show("Вы не выбрали индекс сотрудника!");
            }
            else
            {
                string connectionString = ClassSQL.GetConnSQL();
                string query = "DELETE FROM Sotrudniki WHERE S_ID = @SotrudnikId";

                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SotrudnikId", Convert.ToString(combID.SelectedItem));

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                        combID.SelectedIndex = -1;
                        combPost.SelectedIndex = -1;
                        SotrudnikID();
                        MessageBox.Show("Сотрудник удален");
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show("SQL Error: " + ex.Message);
                    }
                }
            }
        }

        private void menuBack_Click(object sender, RoutedEventArgs e)
        {
            Admin admin = new Admin();
            admin.Show();
            this.Close();
        }
        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Здесь вы осуществляете добавление сотрудник, а также редактирование и удаление их данных.");
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (Sur.Text == "" || Name.Text == "" || FIO.Text == "" || phone.Text == "" || combPost.SelectedIndex == -1 || Login.Text == "" || Password.Text == "")
            {
                MessageBox.Show("Вы не заполнили все данные");
            }
            else
            {
                combID.SelectedIndex = -1;
                string connectionString = ClassSQL.GetConnSQL();
                SqlConnection connection = new SqlConnection(connectionString);
                try
                {
                    connection.Open();
                    string Post = "SELECT P_ID FROM Post WHERE P_NAME = '" + combPost.SelectedItem + "'";
                    SqlCommand cmd = new SqlCommand(Post, connection);
                    int postId = Convert.ToInt32(cmd.ExecuteScalar());

                    string addAll = "Insert into Sotrudniki (S_SURNAME, S_NAME, S_PATR, S_PHONE, S_POSTID, S_LOGIN, S_PASSWORD) values ('" + Sur.Text + "','" + Name.Text + "','" + FIO.Text + "','" + phone.Text + "','" + postId + "','" + Login.Text + "','" + Password.Text + "')";
                    SqlCommand saveAddAll = new SqlCommand(addAll, connection); saveAddAll.ExecuteNonQuery();

                    connection.Close();
                    btnBackAdd_Click(null, null);
                    MessageBox.Show("Сотрудник успешно добавлен");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    throw;
                }
                finally
                {

                }
            }
        }

        private void btnRed_Click(object sender, RoutedEventArgs e)
        {
            btnRed.Visibility = Visibility.Hidden;
            btnAdd.Visibility = Visibility.Hidden;
            combID.Visibility = Visibility.Visible;
            txtID.Visibility = Visibility.Visible;
            btnSave.Visibility = Visibility.Visible;
            btnDelete.Visibility = Visibility.Visible;
            btnBackAdd.Visibility = Visibility.Visible;

            Sur.Clear();
            Name.Clear();
            FIO.Clear();
            phone.Clear();
            Login.Clear();
            Password.Clear();
            combID.Items.Clear();
            combPost.Items.Clear();
            SotrudnikID();
            PostName();
        }
        private void btnBackAdd_Click(object sender, RoutedEventArgs e)
        {
            btnRed.Visibility = Visibility.Visible;
            btnAdd.Visibility = Visibility.Visible;
            combID.Visibility = Visibility.Hidden;
            txtID.Visibility = Visibility.Hidden;
            btnSave.Visibility = Visibility.Hidden;
            btnDelete.Visibility = Visibility.Hidden;
            btnBackAdd.Visibility = Visibility.Hidden;

            Sur.Clear();
            Name.Clear();
            FIO.Clear();
            phone.Clear();
            Login.Clear();
            Password.Clear();
            combID.Items.Clear();
            combPost.Items.Clear();
            SotrudnikID();
            PostName();
        }
    }
}
