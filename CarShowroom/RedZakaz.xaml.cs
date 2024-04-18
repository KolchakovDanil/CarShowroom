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
    /// Логика взаимодействия для RedZakaz.xaml
    /// </summary>
    public partial class RedZakaz : Window
    {
        public RedZakaz()
        {
            InitializeComponent();
            ZakazID();
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
            Admin admin = new Admin();
            admin.Show();
            this.Close();
        }
        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Программа разработана для упрощения работы персонала на предприяти \"Автосалон\".");
        }
        public void ZakazID()
        {
            combID.Items.Clear();
            string connectionString = ClassSQL.GetConnSQL();
            string query = "SELECT Z_ID FROM Zakaz";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int zakazId = reader.GetInt32(0);
                        combID.Items.Add(zakazId);
                    }
                }
            }
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
            if (combID.SelectedIndex == -1)
            {
                MessageBox.Show("Вы не выбрали код заказа!");
            }
            else
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

                        string saveQuery2 = "SELECT C_ID FROM Cars WHERE C_NAME ='" + combModel.SelectedItem + "'";
                        SqlCommand save2 = new SqlCommand(saveQuery2, saveZak);
                        string saveCar = Convert.ToString(save2.ExecuteScalar());

                        string saveQuery3 = "SELECT K_ID FROM Klienti inner join Zakaz on Z_KID = K_ID where Z_ID ='" + combID.SelectedItem + "'";
                        SqlCommand save3 = new SqlCommand(saveQuery3, saveZak);
                        string saveKlient = Convert.ToString(save3.ExecuteScalar());

                        string savezakaz = "Update Zakaz set Z_CID = '" + saveCar + "', Z_KID = '" + saveKlient + "', Z_CINA = '" + txtPrice.Text + "' where Z_ID =" + combID.SelectedItem;
                        SqlCommand sz = new SqlCommand(savezakaz, saveZak); sz.ExecuteNonQuery();

                        string saveklient2 = "Update Klienti set K_SURNAME = '" + Sur.Text + "', K_NAME = '" + Name.Text + "', K_PATR = '" + FIO.Text + "', K_PHONE = '" + phone.Text + "' where K_ID =" + saveKlient;
                        SqlCommand sk = new SqlCommand(saveklient2, saveZak); sk.ExecuteNonQuery();

                        saveZak.Close();

                        combID.SelectedIndex = -1;
                        combMark.SelectedIndex = -1;
                        combModel.SelectedIndex = -1;
                        imgCar.Source = null;
                        ZakazID();
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
                MessageBox.Show("Вы не выбрали код заказа!");
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("Вы уверены что хотите удалить данный заказ?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    string connectionString = ClassSQL.GetConnSQL();
                    string query = "DELETE FROM Zakaz WHERE Z_ID = @ZakazId";

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ZakazId", Convert.ToString(combID.SelectedItem));

                        try
                        {
                            connection.Open();
                            command.ExecuteNonQuery();
                            combID.SelectedIndex = -1;
                            combMark.SelectedIndex = -1;
                            combModel.SelectedIndex = -1;
                            imgCar.Source = null;
                            ZakazID();
                            MessageBox.Show("Заказ удален");
                        }
                        catch (SqlException ex)
                        {
                            MessageBox.Show("SQL Error: " + ex.Message);
                        }
                    }
                }
                else if (result == MessageBoxResult.No)
                {

                }
            }
        }

        private void combID_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int carsId = Convert.ToInt32(combID.SelectedItem);

            string connectionString = ClassSQL.GetConnSQL();
            string query = "select K_SURNAME from Zakaz inner join Klienti on Zakaz.Z_KID = Klienti.K_ID where Z_ID  = '" + carsId + "'";
            string query1 = "select K_NAME from Zakaz inner join Klienti on Zakaz.Z_KID = Klienti.K_ID where Z_ID  = '" + carsId + "'";
            string query2 = "select K_PATR from Zakaz inner join Klienti on Zakaz.Z_KID = Klienti.K_ID where Z_ID  = '" + carsId + "'";
            string query3 = "select K_PHONE from Zakaz inner join Klienti on Zakaz.Z_KID = Klienti.K_ID where Z_ID  ='" + carsId + "'";
            string query4 = "select M_NAME from Zakaz inner join Cars on Zakaz.Z_CID = Cars.C_ID " +
                            "inner join Models on Cars.C_MODELID = Models.M_ID where Z_ID  ='" + carsId + "'";
            string query5 = "select C_NAME from Zakaz inner join Cars on Zakaz.Z_CID = Cars.C_ID where Z_ID ='" + carsId + "'";
            string query6 = "select C_PRICE from Zakaz inner join Cars on Zakaz.Z_CID = Cars.C_ID where Z_ID  ='" + carsId + "'";
            string query7 = "select C_INFO from Zakaz inner join Cars on Zakaz.Z_CID = Cars.C_ID where Z_ID  ='" + carsId + "'";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                connection.Open();

                SqlCommand Sqlcmd = new SqlCommand(query, connection);
                Sur.Text = Convert.ToString(Sqlcmd.ExecuteScalar());

                SqlCommand Sqlcmd1 = new SqlCommand(query1, connection);
                Name.Text = Convert.ToString(Sqlcmd1.ExecuteScalar());

                SqlCommand Sqlcmd2 = new SqlCommand(query2, connection);
                FIO.Text = Convert.ToString(Sqlcmd2.ExecuteScalar());

                SqlCommand Sqlcmd3 = new SqlCommand(query3, connection);
                phone.Text = Convert.ToString(Sqlcmd3.ExecuteScalar());

                SqlCommand Sqlcmd4 = new SqlCommand(query4, connection);
                combMark.SelectedItem = Convert.ToString(Sqlcmd4.ExecuteScalar());

                SqlCommand Sqlcmd5 = new SqlCommand(query5, connection);
                combModel.SelectedItem = Convert.ToString(Sqlcmd5.ExecuteScalar());

                SqlCommand Sqlcmd6 = new SqlCommand(query6, connection);
                txtPrice.Text = Convert.ToString(Sqlcmd6.ExecuteScalar());

                SqlCommand Sqlcmd7 = new SqlCommand(query7, connection);
                txtInfo.Text = Convert.ToString(Sqlcmd7.ExecuteScalar());

                connection.Close();
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
            if(combModel.SelectedIndex != -1)
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
