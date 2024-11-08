using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;
using OfficeOpenXml;
namespace cargo
{
    public partial class otchet1 : Form
    {
        public otchet1()
        {
            InitializeComponent();
        }
        private void otchet1_Load(object sender, EventArgs e)
        {
            LoadData();
            LoadTotalStockValue(); // Вызов метода для загрузки общей стоимости
        }
        private void LoadData()
        {
            string connectionString = "Data Source=NEGGER;Initial Catalog=10241367;Integrated Security=True;Encrypt=False;";
            string query = @"SELECT 
                k.name AS CategoryName,
                dt.name_2 AS ProductName,
                ISNULL(SUM(CASE 
                    WHEN wo.OperationType = 'Приход' THEN wo.Quantity 
                    WHEN wo.OperationType = 'Отгрузка' THEN -wo.Quantity 
                    ELSE 0 
                END), 0) AS RemainingQuantity,
                ISNULL(SUM(CASE 
                    WHEN wo.OperationType = 'Приход' THEN wo.Quantity * dt.cost 
                    WHEN wo.OperationType = 'Отгрузка' THEN -wo.Quantity * dt.cost 
                    ELSE 0 
                END), 0) AS RemainingValue
            FROM 
                dbo.drop_table dt
            JOIN 
                dbo.kat k ON dt.catecoria = k.id
            LEFT JOIN 
                dbo.WarehouseOperations wo ON dt.id = wo.ProductID
            GROUP BY 
                k.name, dt.name_2
            ORDER BY 
                k.name, dt.name_2;";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                try
                {
                    connection.Open();
                    adapter.Fill(dataTable);
                    dataGridView1.DataSource = dataTable; // Привязываем DataTable к DataGridView
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message);
                }
            }
        }
        private void LoadTotalStockValue()
        {
            string connectionString = "Data Source=NEGGER;Initial Catalog=10241367;Integrated Security=True;Encrypt=False;";
            string query = @"SELECT 
                ISNULL(SUM(CASE 
                    WHEN wo.OperationType = 'Приход' THEN wo.Quantity * dt.cost 
                    WHEN wo.OperationType = 'Отгрузка' THEN -wo.Quantity * dt.cost 
                    ELSE 0 
                END), 0) AS TotalStockValue
            FROM 
                dbo.drop_table dt
            LEFT JOIN 
                dbo.WarehouseOperations wo ON dt.id = wo.ProductID;";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                try
                {
                    connection.Open();
                    var result = command.ExecuteScalar(); // Выполняем запрос и получаем одно значение
                    if (result != null)
                    {
                        label1.Text = "Общая стоимость товаров: " + result.ToString();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message);
                }
            }
        }
        private void btnExport_Click(object sender, EventArgs e)
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

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}

