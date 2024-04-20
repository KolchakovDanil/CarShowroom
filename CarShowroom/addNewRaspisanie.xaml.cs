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
using System.Globalization;

namespace CarShowroom
{
    /// <summary>
    /// Логика взаимодействия для addNewRaspisanie.xaml
    /// </summary>
    public partial class addNewRaspisanie : Window
    {
        public addNewRaspisanie()
        {
            InitializeComponent();
            sotrudnikName();
        }
        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Здесь вы можете добавлять новые строчки для расписания персонала.\n " +
                           "В строчку 'Тип' вы можете выписывать лишь 'Р' - работает или 'Н' - не работает (на русской вкладке).");
        }
        public void sotrudnikName()
        {
            combSotName.Items.Clear();
            string connectionString = ClassSQL.GetConnSQL();

            string query = "SELECT S_SURNAME FROM Sotrudniki where S_POSTID = 2";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string orderId = reader.GetString(0);
                        combSotName.Items.Add(orderId);
                    }
                }
            }
        }

        private void dateDay_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime selectedDate = dateDay.SelectedDate.HasValue ? dateDay.SelectedDate.Value.Date : DateTime.MinValue;
            if (selectedDate != DateTime.MinValue)
            {
                txtDate.Text = selectedDate.ToString("yyyy-MM-dd");
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (txtDate.Text == "" || txtType.Text == "" || combSotName.SelectedIndex == -1)
            {
                MessageBox.Show("Заполните все данные!");
            }
            else
            {
                if (txtType.Text == "Р" || txtType.Text == "Н")
                {
                    string connectionString = ClassSQL.GetConnSQL();
                    SqlConnection conn6 = new SqlConnection(connectionString);
                    try
                    {
                        conn6.Open();
                        string query = "select DayID from Days where Date = '" + dateDay.SelectedDate + "'";
                        SqlCommand selectDayID = new SqlCommand(query, conn6);
                        int dayID = Convert.ToInt32(selectDayID.ExecuteScalar());

                        string query2 = "select S_ID from Sotrudniki where S_SURNAME ='" + combSotName.SelectedItem + "'";
                        SqlCommand selectSID = new SqlCommand(query2, conn6);
                        int sID = Convert.ToInt32(selectSID.ExecuteScalar());


                        string add = "Insert into Raspisanie (R_DayID, R_SID, R_Type) values ('" + dayID + "','" + sID + "','" + txtType.Text + "')";
                        SqlCommand addTB = new SqlCommand(add, conn6); addTB.ExecuteNonQuery();

                        conn6.Close();
                        combSotName.SelectedIndex = -1;
                        txtDate.Clear();
                        txtType.Clear();
                        MessageBox.Show("Строчка успешно добавлена!");
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
                else
                {
                    MessageBox.Show("В строку 'ТИП' нужно писать лишь символ 'Р' - работает или 'Н' - не работает.\n" +
                        "Помните, что ввод осуществляется на русской вкладке!");
                }
            }
        }
    }
}
