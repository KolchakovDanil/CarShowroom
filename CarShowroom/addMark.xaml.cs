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
    /// Логика взаимодействия для addMark.xaml
    /// </summary>
    public partial class addMark : Window
    {
        public addMark()
        {
            InitializeComponent();
            marksID();
        }
        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Здесь вы можете добавлять, редактировать и удалять марки автомобилей.");
        }
        public void marksID()
        {
            CSzak.Items.Clear();
            string connectionString = ClassSQL.GetConnSQL();

            string query = "SELECT M_ID FROM Models";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int orderId = reader.GetInt32(0);
                        CSzak.Items.Add(orderId);
                    }
                }
            }
        }
        private void CSzak_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CSzak.SelectedValue != null)
            {
                string connectionString = ClassSQL.GetConnSQL();
                SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();
                string sql = "select M_NAME from Models where M_ID  = '" + CSzak.SelectedItem + "'";
                SqlCommand Sqlcmd = new SqlCommand(sql, conn);
                txtName.Text = Convert.ToString(Sqlcmd.ExecuteScalar());
                string sql2 = "select M_IMG from Models where M_ID  = '" + CSzak.SelectedItem + "'";
                SqlCommand Sqlcmd2 = new SqlCommand(sql2, conn);
                txtImg.Text = Convert.ToString(Sqlcmd2.ExecuteScalar());

                int selectedCarId = Convert.ToInt32(CSzak.SelectedItem);
                LoadImage(selectedCarId);
                conn.Close();
            }

        }
        private void btnNowAdd_Click(object sender, RoutedEventArgs e)
        {
            CSzak.SelectedIndex = -1;
            txtName.Text = "";
            btnAdd.Visibility = Visibility.Visible;
            CSzak.Visibility = Visibility.Hidden;
            TIDzak.Visibility = Visibility.Hidden;
            btnSave.Visibility = Visibility.Hidden;
            btnDel.Visibility = Visibility.Hidden;
            btnNowAdd.Visibility = Visibility.Hidden;
            btnEdit.Visibility = Visibility.Visible;
            marksID();
            imgCar.Source = null;
            txtImg.Clear();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            imgCar.Source = null;
            txtImg.Clear();
            CSzak.SelectedIndex = -1;
            txtName.Text = "";
            btnAdd.Visibility = Visibility.Hidden;
            CSzak.Visibility = Visibility.Visible;
            TIDzak.Visibility = Visibility.Visible;
            btnSave.Visibility = Visibility.Visible;
            btnDel.Visibility = Visibility.Visible;
            btnNowAdd.Visibility = Visibility.Visible;
            btnEdit.Visibility = Visibility.Hidden;
            marksID();
            
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (txtName.Text == "" || txtImg.Text == "")
            {
                MessageBox.Show("Название марки автомобиля не должно быть пустым!");
            }
            else
            {
                CSzak.SelectedIndex = -1;
                string connectionString = ClassSQL.GetConnSQL();
                SqlConnection conn6 = new SqlConnection(connectionString);
                try
                {
                    conn6.Open();

                    string add = "Insert into Models (M_NAME, M_IMG) values ('" + txtName.Text + "','" + txtImg.Text + "')";
                    SqlCommand addTB = new SqlCommand(add, conn6); addTB.ExecuteNonQuery();

                    conn6.Close();

                    MessageBox.Show("Марка успешно добавлена");
                    RedAuto ra = new RedAuto();
                    ra.Update();
                    btnNowAdd_Click(null, null);
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
                    SqlCommand command = new SqlCommand("SELECT M_IMG FROM Models WHERE M_ID = @MarkId", connection);
                    command.Parameters.AddWithValue("@MarkId", carId);
                    string imageUrl = command.ExecuteScalar()?.ToString();

                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        string basePath = AppDomain.CurrentDomain.BaseDirectory;
                        string imagePath = System.IO.Path.Combine(basePath, imageUrl);
                        imgCar.Source = new BitmapImage(new Uri(imagePath));
                    }
                    else
                    {
                        MessageBox.Show("Изображение не найдено для выбранной марки.");
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

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (CSzak.SelectedIndex == -1)
            {
                MessageBox.Show("Вы не выбрали код марки!");
            }
            else
            {
                if (txtName.Text == "" || txtImg.Text == "")
                {
                    MessageBox.Show("Данные марки не должны быть пустыми!");
                }
                else
                {
                    string connectionString = ClassSQL.GetConnSQL();
                    SqlConnection saveType = new SqlConnection(connectionString);
                    try
                    {
                        saveType.Open();
                        string savezakaz = "Update Models set M_NAME = '" + txtName.Text + "', M_IMG = '" + txtImg.Text + "' where M_ID =" + CSzak.SelectedItem;

                        SqlCommand sz = new SqlCommand(savezakaz, saveType); sz.ExecuteNonQuery();

                        saveType.Close();

                        btnEdit_Click(null, null);
                        RedAuto ra = new RedAuto();
                        ra.Update();
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

        private void btnDel_Click(object sender, RoutedEventArgs e)
        {
            if (CSzak.SelectedIndex == -1)
            {
                MessageBox.Show("Вы не выбрали код марки!");
            }
            else
            {
                string connectionString = ClassSQL.GetConnSQL();

                string checkBooksQuery = "SELECT COUNT(*) FROM Cars WHERE C_MODELID = @ModelCarId";

                string deleteCategoryQuery = "DELETE FROM Models WHERE M_ID = @ModelId";

                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        using (SqlCommand command = new SqlCommand(checkBooksQuery, connection))
                        {
                            command.Parameters.AddWithValue("@ModelCarId", Convert.ToString(CSzak.SelectedItem));

                            int booksCount = (int)command.ExecuteScalar();
                            if (booksCount > 0)
                            {
                                MessageBox.Show("Нельзя удалить марку с заполненными данными!");
                            }
                            else
                            {
                                using (SqlCommand deleteCommand = new SqlCommand(deleteCategoryQuery, connection))
                                {
                                    deleteCommand.Parameters.AddWithValue("@ModelId", Convert.ToString(CSzak.SelectedItem));

                                    int rowsAffected = deleteCommand.ExecuteNonQuery();
                                    if (rowsAffected > 0)
                                    {
                                        btnEdit_Click(null, null);
                                        RedAuto ra = new RedAuto();
                                        ra.Update();
                                        MessageBox.Show("Маркаа успешно удалена");
                                    }
                                    else
                                    {
                                        MessageBox.Show("Марка не была удалена. Возможно, указанный идентификатор категории не существует.");
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("SQL Error: " + ex.Message);
                }
            }
        }
        
    }
}
