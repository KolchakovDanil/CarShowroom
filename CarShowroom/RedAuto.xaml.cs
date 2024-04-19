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
    /// Логика взаимодействия для RedAuto.xaml
    /// </summary>
    public partial class RedAuto : Window
    {
        public RedAuto()
        {
            InitializeComponent();
            CarsID();
            MarkName();
        }
        public void Update()
        {
            CarsID();
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
            MessageBox.Show("В данном окне вы можете добавлять, изменять и удалять автомобили. Также вы можете добавить новую марку автомобиля, " +
                "нажав на кнопку \"Добавить марку\".");
        }
        public void CarsID()
        {
            combID.Items.Clear();
            string connectionString = ClassSQL.GetConnSQL();
            string query = "SELECT C_ID FROM Cars";

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
                MessageBox.Show("Вы не выбрали код автомобиля!");
            }
            else
            {
                if (combMark.SelectedIndex == -1 || combModel.SelectedIndex == -1 || txtPrice.Text == "" || txtInfo.Text == "" || txtImg.Text == "")
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

                        string saveQuery2 = "SELECT C_NAME FROM Cars WHERE C_ID ='" + combID.SelectedItem + "'";
                        SqlCommand save2 = new SqlCommand(saveQuery2, saveZak);
                        string saveCarName = Convert.ToString(save2.ExecuteScalar());

                        string saveQuery3 = "SELECT M_ID FROM Models WHERE M_NAME ='" + combMark.SelectedItem + "'";
                        SqlCommand save3 = new SqlCommand(saveQuery3, saveZak);
                        string saveMarkID = Convert.ToString(save3.ExecuteScalar());

                        string saveCar = "Update Cars set C_MODELID = '" + saveMarkID + "', C_NAME = '" + saveCarName + "', C_PRICE = '" + txtPrice.Text + "', C_IMG = '" + txtImg.Text + "', C_INFO = '" + txtInfo.Text + "' where C_ID =" + combID.SelectedItem;
                        SqlCommand sz = new SqlCommand(saveCar, saveZak); sz.ExecuteNonQuery();

                        saveZak.Close();

                        btnRed_Click(null, null);
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
                MessageBox.Show("Вы не выбрали код автомобиля!");
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("Вы уверены что хотите удалить данный автомобиль?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    string connectionString = ClassSQL.GetConnSQL();
                    string query = "DELETE FROM Cars WHERE C_ID = @CarsId";

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CarsId", Convert.ToString(combID.SelectedItem));

                        try
                        {
                            connection.Open();
                            command.ExecuteNonQuery();

                            btnRed_Click(null, null);
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

        private void btnBackAdd_Click(object sender, RoutedEventArgs e)
        {
            btnRed.Visibility = Visibility.Visible;
            btnAdd.Visibility = Visibility.Visible;
            combID.Visibility = Visibility.Hidden;
            txtID.Visibility = Visibility.Hidden;
            btnSave.Visibility = Visibility.Hidden;
            btnDelete.Visibility = Visibility.Hidden;
            btnBackAdd.Visibility = Visibility.Hidden;
            txtModel.Visibility = Visibility.Visible;

            txtPrice.IsEnabled = true;

            txtModel.Clear();
            txtPrice.Clear();
            txtInfo.Clear();
            txtImg.Clear();
            combID.Items.Clear();
            combMark.Items.Clear();
            combModel.Items.Clear();
            CarsID();
            MarkName();
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
            txtModel.Visibility = Visibility.Hidden;

            txtPrice.IsEnabled = false;

            txtModel.Clear();
            txtPrice.Clear();
            txtInfo.Clear();
            txtImg.Clear();
            combID.Items.Clear();
            combMark.Items.Clear();
            combModel.Items.Clear();
            CarsID();
            MarkName();
        }

        private void combID_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int carsId = Convert.ToInt32(combID.SelectedItem);

            string connectionString = ClassSQL.GetConnSQL();

            string query4 = "select M_NAME from Cars inner join Models on Cars.C_MODELID = Models.M_ID where C_ID  ='" + carsId + "'";
            string query5 = "select C_NAME from Cars where C_ID ='" + carsId + "'";
            string query6 = "select C_PRICE from Cars where C_ID  ='" + carsId + "'";
            string query7 = "select C_INFO from Cars where C_ID  ='" + carsId + "'";
            string query8 = "select C_IMG from Cars where C_ID  ='" + carsId + "'";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query4, connection))
            using (SqlCommand command2 = new SqlCommand(query5, connection))
            using (SqlCommand command3 = new SqlCommand(query6, connection))
            using (SqlCommand command4 = new SqlCommand(query7, connection))
            using (SqlCommand command5 = new SqlCommand(query8, connection))
            {
                connection.Open();

                combMark.SelectedItem = Convert.ToString(command.ExecuteScalar());

                combModel.SelectedItem = Convert.ToString(command2.ExecuteScalar());

                txtPrice.Text = Convert.ToString(command3.ExecuteScalar());

                txtInfo.Text = Convert.ToString(command4.ExecuteScalar());

                txtImg.Text = Convert.ToString(command5.ExecuteScalar());

                connection.Close();
            }
        }

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
                        MessageBox.Show("Изображение не найдено для выбранного автомобиля.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка загрузки изображения: " + ex.Message);
                }
            }
        }

        private void btnRedImg_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.InitialDirectory = System.IO.Path.Combine(Environment.CurrentDirectory, "PhotoCars");

            dlg.Filter = "Изображения (*.jpg, *.png)|*.jpg;*.png|Все файлы (*.*)|*.*";

            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                string selectedFile = dlg.FileName;

                if (!selectedFile.Contains("PhotoCars"))
                {
                    MessageBox.Show("Пожалуйста, выберите файл из папки PhotoCars", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string fileName = System.IO.Path.GetFileName(selectedFile);

                string relativePath = System.IO.Path.Combine("PhotoCars", fileName);

                txtImg.Text = relativePath;

                string basePath = AppDomain.CurrentDomain.BaseDirectory;
                string imagePath = System.IO.Path.Combine(basePath, relativePath);
                imgCar.Source = new BitmapImage(new Uri(imagePath));
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (combMark.SelectedIndex == -1 || txtModel.Text == "" || txtInfo.Text == "" || txtImg.Text == "" || txtPrice.Text == "")
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
                    string Mark = "SELECT M_ID FROM Models WHERE M_NAME = '" + combMark.SelectedItem + "'";
                    SqlCommand cmd = new SqlCommand(Mark, connection);
                    int markId = Convert.ToInt32(cmd.ExecuteScalar());

                    string addAll = "Insert into Cars (C_NAME, C_PRICE, C_IMG, C_INFO, C_MODELID) values ('" + txtModel.Text + "','" + txtPrice.Text + "','" + txtImg.Text + "','" + txtInfo.Text + "','" + markId + "')";
                    SqlCommand saveAddAll = new SqlCommand(addAll, connection); saveAddAll.ExecuteNonQuery();

                    connection.Close();
                    btnBackAdd_Click(null, null);
                    MessageBox.Show("Автомобиль успешно добавлен");
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

        private void btnAddMark_Click(object sender, RoutedEventArgs e)
        {
            addMark am = new addMark();
            am.Show();
            //this.Close();
        }

    }
}
