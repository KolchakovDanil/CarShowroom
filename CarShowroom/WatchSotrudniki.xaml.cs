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
using System.Data.SqlClient;
using System.Windows.Navigation;
using System.Diagnostics;
using Microsoft.Office.Interop.Excel;
using System.Data;
using DataTable = System.Data.DataTable;

namespace CarShowroom
{
    public partial class WatchSotrudniki
    {
        private DataTable ordersTable;
        private DataView ordersView;

        public WatchSotrudniki()
        {
            InitializeComponent();
            tableZak();
        }
        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Данное окно служит для просмотра всех сотрудников, их поиска и при необходимости вывести на печать сохранненый документ .xls \n" +
                "Поиск осуществляется по фамилии сотрудника!");
        }
        static DataTable ExecuteSql(string sql)
        {
            DataTable DT = new DataTable();
            string connectionString = ClassSQL.GetConnSQL();
            SqlConnection sqlcon = new SqlConnection(connectionString);
            using (sqlcon)
            {
                sqlcon.Open();
                SqlCommand sqlcd = new SqlCommand(sql, sqlcon);
                SqlDataReader sqlread = sqlcd.ExecuteReader();
                using (sqlread)
                {
                    DT.Load(sqlread);
                }
            }
            return DT;
        }
        public void tableZak()
        {
            ordersTable = ExecuteSql("SELECT * FROM Sotrudniki inner join Post on Sotrudniki.S_POSTID = Post.P_ID");
            ordersView = new DataView(ordersTable);
            listSotrudniki.ItemsSource = ordersView;
        }
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ordersView != null)
            {
                string searchText = txtSearch.Text.ToLower();
                if (!string.IsNullOrEmpty(searchText))
                {
                    StringBuilder filterBuilder = new StringBuilder();

                    string[] searchWords = searchText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string word in searchWords)
                    {
                        if (filterBuilder.Length > 0)
                        {
                            filterBuilder.Append(" AND ");
                        }
                        filterBuilder.Append($"S_SURNAME LIKE '%{word}%'");
                    }
                    ordersView.RowFilter = filterBuilder.ToString();
                }
                else
                {
                    ordersView.RowFilter = string.Empty;
                }
            }
        }

        private void saveReport_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
            excelApp.Visible = true;

            Workbook workbook = excelApp.Workbooks.Add();
            Worksheet worksheet = workbook.ActiveSheet;

            try
            {
                int row = 1;

                worksheet.Cells[row, 1] = "Id";
                worksheet.Cells[row, 2] = "Фамилия";
                worksheet.Cells[row, 3] = "Имя";
                worksheet.Cells[row, 4] = "Отчество";
                worksheet.Cells[row, 5] = "Телефон";
                worksheet.Cells[row, 6] = "Логин";
                worksheet.Cells[row, 7] = "Пароль";
                worksheet.Cells[row, 8] = "Должность";
                row++;

                worksheet.Columns[1].ColumnWidth = 10;
                worksheet.Columns[2].ColumnWidth = 20;
                worksheet.Columns[3].ColumnWidth = 20;
                worksheet.Columns[4].ColumnWidth = 20;
                worksheet.Columns[5].ColumnWidth = 20;
                worksheet.Columns[6].ColumnWidth = 10;
                worksheet.Columns[7].ColumnWidth = 10;
                worksheet.Columns[8].ColumnWidth = 20;

                foreach (DataRowView rowView in ordersView)
                {
                    DataRow rowItem = rowView.Row;
                    worksheet.Cells[row, 1] = rowItem["S_ID"].ToString();
                    worksheet.Cells[row, 2] = rowItem["S_SURNAME"].ToString();
                    worksheet.Cells[row, 3] = rowItem["S_NAME"].ToString();
                    worksheet.Cells[row, 4] = rowItem["S_PATR"].ToString();
                    worksheet.Cells[row, 5] = rowItem["S_PHONE"].ToString();
                    worksheet.Cells[row, 6] = rowItem["S_LOGIN"].ToString();
                    worksheet.Cells[row, 7] = rowItem["S_PASSWORD"].ToString();
                    worksheet.Cells[row, 8] = rowItem["P_NAME"].ToString();
                    row++;
                }

                workbook.SaveAs("sotrudnikReport.xls");

                MessageBox.Show("Отчет успешно создан и сохранен как sotrudnikReport.xls", "Отчет создан", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при создании отчета: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                excelApp.Quit();
            }
        }
    }
}
