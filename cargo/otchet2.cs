using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace cargo
{
    public partial class otchet2 : Form
    {
        public otchet2()
        {
            InitializeComponent();
            LoadData();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
           
        }
        private void LoadData()
        {
            string connectionString = "Data Source=NEGGER;Initial Catalog=10241367;Integrated Security=True;Encrypt=False;";

            string query = @"
    WITH SalesData AS (
        SELECT 
            z.date AS SaleDate,
            zc.name AS CustomerName,
            dt.name_2 AS ProductName,
            SUM(z.cost) AS TotalCost,
            COUNT(z.id_zakaza) AS TotalOrders
        FROM 
            zakaz z
        JOIN 
            zakazchik zc ON z.id_zak = zc.id
        JOIN 
            drop_table dt ON z.drop_id = dt.id
        GROUP BY 
            z.date, zc.name, dt.name_2
    )
    SELECT 
        SaleDate,
        CustomerName,
        SUM(TotalCost) AS [Общая_стоимость],
        SUM(TotalOrders) AS [Общее_количество_заказов]
    FROM 
        SalesData
    GROUP BY 
        SaleDate, CustomerName
    UNION ALL
    SELECT 
        SaleDate,
        'Итого' AS CustomerName,
        SUM(TotalCost) AS [Общая_стоимость],
        SUM(TotalOrders) AS [Общее_количество_заказов]
    FROM 
        SalesData
    GROUP BY 
        SaleDate
    ORDER BY 
        SaleDate, CustomerName;";


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            DataTable dt = new DataTable();
                            dt.Load(reader);

                            dataGridView1.DataSource = dt;

                            // Устанавливаем заголовки колонок на русском языке
                            dataGridView1.Columns["SaleDate"].HeaderText = "Дата продажи";
                            dataGridView1.Columns["CustomerName"].HeaderText = "Имя клиента";
                            dataGridView1.Columns["Общая_стоимость"].HeaderText = "Общая стоимость";
                            dataGridView1.Columns["Общее_количество_заказов"].HeaderText = "Общее количество заказов";


                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Ошибка SQL: {ex.Message}", "Ошибка");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Непредвиденная ошибка: {ex.Message}", "Ошибка");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
            saveFileDialog.Title = "Сохранить как";
            saveFileDialog.FileName = "Отчет.xlsx";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (ExcelPackage excelPackage = new ExcelPackage())
                    {
                        ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Отчет");
                        // Записываем заголовки столбцов
                        for (int i = 0; i < dataGridView1.Columns.Count; i++)
                        {
                            if (dataGridView1.Columns[i].Visible)
                            {
                                worksheet.Cells[1, i + 1].Value = dataGridView1.Columns[i].HeaderText;
                            }
                        }
                        int rowCount = 2; // Начинаем со второй строки после заголовков
                        // Записываем данные строк
                        foreach (DataGridViewRow row in dataGridView1.Rows)
                        {
                            if (!row.IsNewRow) // Пропускаем новую строку, если она есть
                            {
                                bool hasData = false; // Флаг для проверки наличия данных в строке
                                // Проверяем, есть ли ненулевое значение
                                foreach (DataGridViewCell cell in row.Cells)
                                {
                                    if (cell.OwningColumn.Visible) // Проверяем, видима ли колонка
                                    {
                                        string cellValue = cell.Value?.ToString() ?? string.Empty;
                                        if (!string.IsNullOrEmpty(cellValue) && cellValue != "0")
                                        {
                                            hasData = true; // Устанавливаем флаг, если есть ненулевое значение
                                            break; // Выходим из цикла, если нашли ненулевое значение
                                        }
                                    }
                                }
                                // Если в строке есть данные, добавляем ее в Excel
                                if (hasData)
                                {
                                    for (int i = 0; i < row.Cells.Count; i++)
                                    {
                                        if (row.Cells[i].OwningColumn.Visible) // Проверяем, видима ли колонка
                                        {
                                            worksheet.Cells[rowCount, i + 1].Value = row.Cells[i].Value?.ToString();
                                        }
                                    }
                                    rowCount++; // Переходим к следующей строке
                                }
                            }
                        }
                        // Сохраняем файл
                        FileInfo excelFile = new FileInfo(saveFileDialog.FileName);
                        excelPackage.SaveAs(excelFile);
                    }
                    MessageBox.Show("Данные успешно экспортированы в Excel.", "Успех");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при экспорте данных: " + ex.Message, "Ошибка");
                }
            }
        }
    }
}
