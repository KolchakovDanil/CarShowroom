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
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;

namespace CarShowroom
{
    /// <summary>
    /// Логика взаимодействия для Raspisanie.xaml
    /// </summary>
    public partial class Raspisanie : Window
    {
        private string ConnectionString = ClassSQL.GetConnSQL();

        private ObservableCollection<ScheduleItem> ScheduleItems { get; set; }
        public void getDostup()
        {
            dostup = 1;
        }
        public class ScheduleItem
        {
            public DateTime Date { get; set; }
            public string EmployeeName { get; set; }
            public string WorkType { get; set; }
        }
        public Raspisanie()
        {
            InitializeComponent();
            ScheduleItems = new ObservableCollection<ScheduleItem>();
            LoadScheduleFromDatabase();
            dgSchedule.ItemsSource = ScheduleItems;
        }
        int dostup = 0;
        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Здесь можно мониторить расписание работы персонала и редактировать его за Администратора.");
        }

        private void menuBack_Click(object sender, RoutedEventArgs e)
        {
            if (dostup == 1)
            {
                Admin ad = new Admin();
                ad.Show();
                this.Close();
            }
            else if (dostup == 0)
            {
                Prodavec prod = new Prodavec();
                prod.Show();
                this.Close();
            }
        }

        private void LoadScheduleFromDatabase()
        {
            ScheduleItems.Clear();
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = "SELECT Days.Date, Sotrudniki.S_NAME, Raspisanie.R_TYPE " +
                               "FROM Raspisanie " +
                               "JOIN Days ON Raspisanie.R_DayID = Days.DayID " +
                               "JOIN Sotrudniki ON Raspisanie.R_SID = Sotrudniki.S_ID";
                SqlCommand command = new SqlCommand(query, connection);
                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        ScheduleItems.Add(new ScheduleItem
                        {
                            Date = reader.GetDateTime(0),
                            EmployeeName = reader.GetString(1),
                            WorkType = reader.GetString(2)
                        });
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при загрузке данных: " + ex.Message);
                }
            }
        }
        private void AddScheduleItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void EditScheduleItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteScheduleItem_Click(object sender, RoutedEventArgs e)
        {

        }


        //INTERFACE
        
    }
}
