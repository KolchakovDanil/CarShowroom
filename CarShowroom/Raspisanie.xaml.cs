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
using System.ComponentModel;

namespace CarShowroom
{
    /// <summary>
    /// Логика взаимодействия для Raspisanie.xaml
    /// </summary>
    public partial class Raspisanie : Window
    {
        private string ConnectionString = ClassSQL.GetConnSQL();
        private ICollectionView collectionView;
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
            public int ID { get; set; }
        }
        public Raspisanie()
        {
            Loaded += Window_Loaded;
            InitializeComponent();
            ScheduleItems = new ObservableCollection<ScheduleItem>();
            LoadScheduleFromDatabase();
            dgSchedule.ItemsSource = ScheduleItems;
        }
        int dostup = 0;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            getDostup_Buttons();
        }
        public void getDostup_Buttons()
        {
            if (dostup == 1)
            {
                btnAdd.Visibility = Visibility.Visible;
                btnEdit.Visibility = Visibility.Visible;
                btnDel.Visibility = Visibility.Visible;
            }
            else if (dostup == 0)
            {
                btnAdd.Visibility = Visibility.Hidden;
                btnEdit.Visibility = Visibility.Hidden;
                btnDel.Visibility = Visibility.Hidden;

                var column = dgSchedule.Columns.FirstOrDefault(c => c.Header.ToString() == "Тип работы");
                column.IsReadOnly = true;
            }
        }

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
                string query = "SELECT Raspisanie.R_ID, Days.Date, Sotrudniki.S_SURNAME, Raspisanie.R_TYPE " +
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
                            ID = reader.GetInt32(0),
                            Date = reader.GetDateTime(1),
                            EmployeeName = reader.GetString(2),
                            WorkType = reader.GetString(3)
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
        //МЕТОДЫ
        private void DeleteScheduleFromDatabase(ScheduleItem selectedItem)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = "DELETE FROM Raspisanie WHERE R_ID = @ID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ID", txtID.Text);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при удалении записи из базы данных: " + ex.Message);
                }
            }
        }

        private void dgSchedule_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            
            if (dgSchedule.CurrentColumn != null && dgSchedule.CurrentColumn.Header.ToString() == "Тип работы")
            {
                if (!(e.Text == "Р" || e.Text == "Н"))
                {
                    e.Handled = true;
                }
            }
        }
        private void dgSchedule_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ScheduleItem selectedItem = dgSchedule.SelectedItem as ScheduleItem;

            if (selectedItem != null)
            {
                txtID.Text = selectedItem.ID.ToString();
            }
            else
            {
                txtID.Text = "";
            }
        }
        //КНОПКИ
        private void AddScheduleItem_Click(object sender, RoutedEventArgs e)
        {
            addNewRaspisanie addNR = new addNewRaspisanie();
            addNR.Show();
        }
        private void UpdateScheduleInDatabase(ScheduleItem updatedItem)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = "UPDATE Raspisanie SET R_Type = @Type WHERE R_ID = @ID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Type", updatedItem.WorkType);
                command.Parameters.AddWithValue("@ID", updatedItem.ID); // Предполагается, что у вас есть свойство ID в объекте ScheduleItem

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при обновлении записи в базе данных: " + ex.Message);
                }
            }
        }
        private void EditScheduleItem_Click(object sender, RoutedEventArgs e)
        {
            DataGridCellInfo cell = dgSchedule.CurrentCell;

            ScheduleItem selectedItem = dgSchedule.SelectedItem as ScheduleItem;

            if (selectedItem != null)
            {
                dgSchedule.BeginEdit();

                

                dgSchedule.CurrentCell = new DataGridCellInfo(dgSchedule.SelectedItem, dgSchedule.Columns[2]); // Предполагается, что столбец с типом работы находится на позиции 2

                dgSchedule.CommitEdit();

            }
            else
            {
                MessageBox.Show("Выберите запись для редактирования.");
            }
        }
        private void dgSchedule_CurrentCellChanged(object sender, EventArgs e)
        {
            DataGridCellInfo cell = dgSchedule.CurrentCell;
            if (cell != null && cell.Column != null && cell.Column.DisplayIndex == 2)
            {
                ScheduleItem selectedItem = dgSchedule.SelectedItem as ScheduleItem;

                if (selectedItem != null && !string.IsNullOrEmpty(selectedItem.WorkType))
                {
                    UpdateScheduleInDatabase(selectedItem);
                }
            }
        }
        private void DeleteScheduleItem_Click(object sender, RoutedEventArgs e)
        {
            ScheduleItem selectedItem = dgSchedule.SelectedItem as ScheduleItem;

            if (selectedItem != null)
            {
                MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить эту запись?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    DeleteScheduleFromDatabase(selectedItem);
                    LoadScheduleFromDatabase();
                }
            }
            else
            {
                MessageBox.Show("Выберите запись для удаления.");
            }
        }

        
        private void dateRaspisanie_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime selectedDate = dateRaspisanie.SelectedDate.HasValue ? dateRaspisanie.SelectedDate.Value.Date : DateTime.MinValue;
            if (selectedDate != DateTime.MinValue)
            {
                // Создаем ICollectionView на основе вашей коллекции ScheduleItems
                collectionView = CollectionViewSource.GetDefaultView(ScheduleItems);

                // Применяем фильтр, чтобы отобразить только записи с выбранной датой
                collectionView.Filter = item =>
                {
                    ScheduleItem scheduleItem = item as ScheduleItem;
                    return scheduleItem != null && scheduleItem.Date.Date == selectedDate;
                };

                // Устанавливаем ItemsSource вашего DataGrid на ICollectionView
                dgSchedule.ItemsSource = collectionView;
            }
        }

        private void btnAllDate_Click(object sender, RoutedEventArgs e)
        {
            ScheduleItems = new ObservableCollection<ScheduleItem>();
            LoadScheduleFromDatabase();
            dgSchedule.ItemsSource = ScheduleItems;
        }



        //INTERFACE
        
    }
}
