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
    /// Логика взаимодействия для Zakaz.xaml
    /// </summary>
    public partial class Zakaz : Window
    {
        public Zakaz()
        {
            InitializeComponent();
            MarkName();
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
        private void menuBack_Click(object sender, RoutedEventArgs e)
        {
            Prodavec prod = new Prodavec();
            prod.Show();
            this.Close();
        }
        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Программа разработана для упрощения работы персонала на предприяти \"Автосалон\".");
        }
        public void MarkName()
        {
            combMark.Items.Clear();
            string connectionString = ClassSQL.GetConnSQL();
            string query = "SELECT M_NAME FROM Models";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string postdName = reader.GetString(0);
                        combMark.Items.Add(postdName);
                    }
                }
            }
        }
        public void CarsName()
        {
            combModel.Items.Clear();
            string connectionString = ClassSQL.GetConnSQL();
            string nameMark = Convert.ToString(combMark.SelectedItem);
            string query = "SELECT C_NAME FROM Cars inner join Models on Cars.C_MODELID = Models.M_ID where M_NAME = '" + nameMark + "'";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string postdName = reader.GetString(0);
                        combModel.Items.Add(postdName);
                    }
                }
            }
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (Sur.Text == "" || Name.Text == "" || FIO.Text == "" || phone.Text == "" || combMark.SelectedIndex == -1 || combModel.SelectedIndex == -1 || txtPrice.Text == "" || txtInfo.Text == "")
            {
                MessageBox.Show("Вы не заполнили все данные");
            }
            else
            {
                string connectionString = ClassSQL.GetConnSQL();
                SqlConnection saveZak = new SqlConnection(connectionString);

                try
                {
                    saveZak.Open();
                    DateTime dateNow = DateTime.Now;

                    string saveQuery2 = "SELECT C_ID FROM Cars WHERE C_NAME ='" + combModel.SelectedItem + "'";
                    SqlCommand save2 = new SqlCommand(saveQuery2, saveZak);
                    string saveCar = Convert.ToString(save2.ExecuteScalar());

                    string saveQuery3 = "Insert into Klienti (K_SURNAME,K_NAME,K_PATR,K_PHONE) values ('" + Sur.Text + "','" + Name.Text + "','" + FIO.Text + "','" + phone.Text + "')";
                    SqlCommand save3 = new SqlCommand(saveQuery3, saveZak); save3.ExecuteNonQuery();

                    string saveQuery31 = "select Top 1 K_ID from Klienti order by K_ID desc";
                    SqlCommand save31 = new SqlCommand(saveQuery31, saveZak);
                    int klientID = Convert.ToInt32(save31.ExecuteScalar());

                    string savezakaz = "Insert into Zakaz (Z_CID,Z_KID,Z_SID,Z_CINA,Z_DATE) values ('" + saveCar + "','" + klientID + "','" + 3 + "','" + txtPrice.Text + "','" + dateNow + "')";
                    SqlCommand sz = new SqlCommand(savezakaz, saveZak); sz.ExecuteNonQuery();
                    saveZak.Close();

                    combMark.SelectedIndex = -1;
                    combModel.SelectedIndex = -1;
                    imgCar.Source = null;

                    MessageBox.Show("Данные занесены в БД");
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

        //ВЫВОД ИЗОБРАЖЕНИЯ
        private void LoadImage(int carId)
        {
            string connectionString = ClassSQL.GetConnSQL();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT C_IMG FROM Cars WHERE C_ID = @CarId", connection);
                    command.Parameters.AddWithValue("@CarId", carId);
                    string imageUrl = command.ExecuteScalar()?.ToString();

                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        string basePath = AppDomain.CurrentDomain.BaseDirectory;
                        string imagePath = System.IO.Path.Combine(basePath, imageUrl);
                        imgCar.Source = new BitmapImage(new Uri(imagePath));
                    }
                    else
                    {
                        MessageBox.Show("Изображение не найдено для выбранной книги.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка загрузки изображения: " + ex.Message);
                }
            }
        }

        //СОРТИРОВКА КОМБОБОКСОВ
        private void combMark_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CarsName();
            imgCar.Source = null;
            txtPrice.Clear();
            txtInfo.Clear();
        }

        private void combModel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (combModel.SelectedValue != null)
            {
                string connectionString = ClassSQL.GetConnSQL();
                string nameCar = Convert.ToString(combModel.SelectedItem);
                string query = "SELECT C_ID FROM Cars where C_NAME = '" + nameCar + "'";

                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();

                    int selectedCarId = Convert.ToInt32(command.ExecuteScalar());
                    LoadImage(selectedCarId);
                    CarsPriceInfo();
                }
            }
        }
        public void CarsPriceInfo()
        {
            if (combModel.SelectedIndex != -1)
            {
                txtPrice.Clear();
                txtInfo.Clear();
                string connectionString = ClassSQL.GetConnSQL();
                string nameCar = Convert.ToString(combModel.SelectedItem);
                string query = "SELECT C_PRICE FROM Cars where C_NAME = '" + nameCar + "'";
                string query2 = "SELECT C_INFO FROM Cars where C_NAME = '" + nameCar + "'";

                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlCommand command2 = new SqlCommand(query2, connection))
                {
                    connection.Open();

                    string carPrice = Convert.ToString(command.ExecuteScalar());
                    txtPrice.Text = carPrice;

                    string carInfo = Convert.ToString(command2.ExecuteScalar());
                    txtInfo.Text = carInfo;

                    connection.Close();
                }
            }
        }
    }
}
