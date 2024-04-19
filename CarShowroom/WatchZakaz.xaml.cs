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
    public partial class WatchZakaz
    {
        private DataTable ordersTable;
        private DataView ordersView;
        public WatchZakaz()
        {
            InitializeComponent();
            tableZak();
        }
        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Данное окно служит для просмотра всех заказов, их поиска и при необходимости вывести на печать сохранненый документ .xls \n" +
                "Поиск осуществляется по фамилии клиента!");
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
            ordersTable = ExecuteSql("SELECT * FROM Zakaz inner join Klienti on Zakaz.Z_KID = Klienti.K_ID " +
                                     "inner join Cars on Zakaz.Z_CID  = Cars.C_ID " +
                                     "inner join Models on Cars.C_MODELID  = Models.M_ID " +
                                     "inner join Sotrudniki on Zakaz.Z_SID = Sotrudniki.S_ID");
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
                        filterBuilder.Append($"K_SURNAME LIKE '%{word}%'");
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
                worksheet.Cells[row, 6] = "Марка автомобиля";
                worksheet.Cells[row, 7] = "Название автомобиля";
                worksheet.Cells[row, 8] = "Сотрудник";
                worksheet.Cells[row, 9] = "Итоговая цена";
                worksheet.Cells[row, 10] = "Дата";

                row++;

                worksheet.Columns[1].ColumnWidth = 10;
                worksheet.Columns[2].ColumnWidth = 15;
                worksheet.Columns[3].ColumnWidth = 15;
                worksheet.Columns[4].ColumnWidth = 15;
                worksheet.Columns[5].ColumnWidth = 10;
                worksheet.Columns[6].ColumnWidth = 20;
                worksheet.Columns[7].ColumnWidth = 20;
                worksheet.Columns[8].ColumnWidth = 15;
                worksheet.Columns[9].ColumnWidth = 15;
                worksheet.Columns[10].ColumnWidth = 20;

                foreach (DataRowView rowView in ordersView)
                {
                    DataRow rowItem = rowView.Row;
                    worksheet.Cells[row, 1] = rowItem["Z_ID"].ToString();
                    worksheet.Cells[row, 2] = rowItem["K_SURNAME"].ToString();
                    worksheet.Cells[row, 3] = rowItem["K_NAME"].ToString();
                    worksheet.Cells[row, 4] = rowItem["K_PATR"].ToString();
                    worksheet.Cells[row, 5] = rowItem["K_PHONE"].ToString();
                    worksheet.Cells[row, 6] = rowItem["M_NAME"].ToString();
                    worksheet.Cells[row, 7] = rowItem["C_NAME"].ToString();
                    worksheet.Cells[row, 8] = rowItem["S_SURNAME"].ToString();
                    worksheet.Cells[row, 9] = rowItem["Z_CINA"].ToString();
                    worksheet.Cells[row, 10] = rowItem["Z_DATE"].ToString();

                    row++;
                }

                workbook.SaveAs("zakazReport.xls");

                MessageBox.Show("Отчет успешно создан и сохранен как zakazReport.xls", "Отчет создан", MessageBoxButton.OK, MessageBoxImage.Information);
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
