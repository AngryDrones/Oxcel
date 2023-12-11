using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Oxcel.Class;

namespace Oxcel
{
    public partial class MainPage : ContentPage
    {
        private ObservableCollection<ObservableCollection<CellItem>> cells;
        private Grid grid = new Grid();
        private Parser parser = new Parser();
        private FileManager fileManager = new FileManager();

        // Стартова кількість рядків та стовпців
        private const int InitRowCount = 5;
        private const int InitColumnCount = 5;

        // Шлях до json файлу
        private const string JsonFilePath = @"C:\Users\amaza\Documents\table.json";

        public MainPage()
        {
            InitializeComponent();

            parser = new Parser();

            // Ініціалізація змінних та об'єктів
            cells = new ObservableCollection<ObservableCollection<CellItem>>();

            grid = this.FindByName<Grid>("cellGrid"); // ініціалізація
            InitializeTable();
        }

        private void InitializeTable()
        {
            for (int i = 0; i < InitRowCount; i++)
            {
                // Новий рядок до таблиці
                cellGrid.RowDefinitions.Add(new RowDefinition());

                var newRow = new ObservableCollection<CellItem>();

                for (int j = 0; j < InitColumnCount; j++)
                {
                    var entry = new Entry
                    {
                        Text = "",
                        //HorizontalOptions = LayoutOptions.FillAndExpand,
                        //VerticalOptions = LayoutOptions.FillAndExpand,
                    };

                    cellGrid.Children.Add(entry);
                    Grid.SetRow(entry, i);
                    Grid.SetColumn(entry, j);

                    // Атрибут Name для клітинки
                    var cellPosition = $"{(char)('A' + j)}{i + 1}";
                    newRow.Add(new CellItem(string.Empty, cellPosition));
                    //newRow.Add(new CellItem(string.Empty));

                    entry.Focused += Entry_Focused;
                    entry.Unfocused += Entry_Unfocused;
                }

                cells.Add(newRow);
            }

            // Новий стовпець до таблиці
            for (int j = 0; j < InitColumnCount; j++)
            {
                cellGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }
        }

        private void AddRow_Clicked(object sender, EventArgs e)
        {
            int rowCount = cellGrid.RowDefinitions.Count;

            cellGrid.RowDefinitions.Add(new RowDefinition());

            var newRow = new ObservableCollection<CellItem>();

            for (int i = 0; i < cellGrid.ColumnDefinitions.Count; i++)
            {
                var entry = new Entry
                {
                    Text = ""
                };

                cellGrid.Children.Add(entry);
                Grid.SetRow(entry, rowCount);
                Grid.SetColumn(entry, i);

                var cellPosition = $"{(char)('A' + i)}{rowCount + 1}";
                newRow.Add(new CellItem(string.Empty, cellPosition));

                entry.Focused += Entry_Focused;
                entry.Unfocused += Entry_Unfocused;
            }

            cells.Add(newRow);
        }

        private void AddColumn_Clicked(object sender, EventArgs e)
        {
            int columnCount = cellGrid.ColumnDefinitions.Count;

            cellGrid.ColumnDefinitions.Add(new ColumnDefinition());

            for (int i = 0; i < cellGrid.RowDefinitions.Count; i++)
            {
                var entry = new Entry
                {
                    Text = ""
                };

                cellGrid.Children.Add(entry);
                Grid.SetRow(entry, i);
                Grid.SetColumn(entry, columnCount);

                var cellPosition = $"{(char)('A' + columnCount)}{i + 1}";
                cells[i].Add(new CellItem(string.Empty, cellPosition));

                entry.Focused += Entry_Focused;
                entry.Unfocused += Entry_Unfocused;
            }
        }

        private void DeleteRow_Clicked(object sender, EventArgs e)
        {
            if (cellGrid.RowDefinitions.Count > 0)
            {
                int lastRowIndex = cellGrid.RowDefinitions.Count - 1;

                // Видалення даних
                cells.RemoveAt(lastRowIndex);

                // Видалення самих елементів з таблиці
                for (int i = cellGrid.Children.Count - 1; i >= 0; i--)
                {
                    if (cellGrid.Children[i] is View view)
                    {
                        int rowIndex = Grid.GetRow(view);
                        if (rowIndex == lastRowIndex)
                        {
                            cellGrid.Children.RemoveAt(i);
                        }
                    }
                }

                cellGrid.RowDefinitions.RemoveAt(lastRowIndex);
            }
        }

        private void DeleteColumn_Clicked(object sender, EventArgs e)
        {
            if (cellGrid.ColumnDefinitions.Count > 0)
            {
                int lastColumnIndex = cellGrid.ColumnDefinitions.Count - 1;

                // Видалення  даних
                foreach (var row in cells)
                {
                    row.RemoveAt(lastColumnIndex);
                }

                // Видалення самих елементів щ таблиці
                for (int i = cellGrid.Children.Count - 1; i >= 0; i--)
                {
                    if (cellGrid.Children[i] is View view)
                    {
                        int columnIndex = Grid.GetColumn(view);
                        if (columnIndex == lastColumnIndex)
                        {
                            cellGrid.Children.RemoveAt(i);
                        }
                    }
                }

                cellGrid.ColumnDefinitions.RemoveAt(lastColumnIndex);
            }
        }

        private void SaveTable_Clicked(object sender, EventArgs e)
        {
            try
            {
                FileManager.SaveTableToJson(cells, JsonFilePath);
                DisplayAlert("Увага", "Таблицю збережено успішно", "ОК");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
                DisplayAlert("Помилка", "Виникла помилка під час зберігання таблиці.", "ОК");
            }
        }

        private void LoadTable_Clicked(object sender, EventArgs e)
        {
            var loadedCells = FileManager.LoadTableFromJson(JsonFilePath);
            if (loadedCells != null)
            {
                cells = loadedCells;

                SimulateFocusAndUnfocusForAllCells(); // імітація для показу вмісту клітин
                DisplayAlert("Alert", "Table was loaded successfully", "OK");
            }
        }

        private void ClearTable_Clicked(object sender, EventArgs e)
        {
            cellGrid.RowDefinitions.Clear();
            cells.Clear();

            cellGrid.ColumnDefinitions.Clear();

            cellGrid.Children.Clear();

            // Заново ініціалізація таблиці 3x3
            InitializeTable();
        }

        private async void Info_Clicked(object sender, EventArgs e)
        {
            await DisplayAlert("Довідка", "Лабораторна робота 1, К-24 Джиджора Данило", "ОК");
        }

        private async void Exit_Clicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Вихід", "Ви впевнені?", "ТАК", "НІ");
            if (answer)
            {
                System.Environment.Exit(0);
            }
        }

        private void Entry_Focused(object sender, FocusEventArgs e)
        {
            var entry = (Entry)sender;
            var rowIndex = Grid.GetRow(entry);
            var columnIndex = Grid.GetColumn(entry);

            // При фокусу показувати вираз
            entry.Text = cells[rowIndex][columnIndex].CellExpression;
        }

        private void Entry_Unfocused(object sender, FocusEventArgs e)
        {
            var entry = (Entry)sender;
            var rowIndex = Grid.GetRow(entry);
            var columnIndex = Grid.GetColumn(entry);

            var expression = entry.Text?.Trim() ?? string.Empty;
            var result = string.IsNullOrWhiteSpace(expression) ? int.MinValue : CalculateExpression(expression, cells);
            cells[rowIndex][columnIndex].CellExpression = expression;
            cells[rowIndex][columnIndex].CellValue = result;

            // При виході показувати результат або нічого
            entry.Text = result != int.MinValue ? result.ToString() : string.Empty;
        }

        private void SimulateFocusAndUnfocusForAllCells()
        {
            for (int i = 0; i < cellGrid.RowDefinitions.Count; i++)
            {
                for (int j = 0; j < cellGrid.ColumnDefinitions.Count; j++)
                {
                    var entry = (Entry)cellGrid.Children[i * cellGrid.ColumnDefinitions.Count + j];

                    // Імітація фокусу і вивходу з фокусу
                    entry.Focus();
                    entry.Unfocus();
                }
            }
        }

        private int CalculateExpression(string expression, ObservableCollection<ObservableCollection<CellItem>> cells)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(expression))
                {
                    return 0;
                }

                int result = Parser.ParseExpression(expression, cells);
                return result;
            }
            catch (ArgumentException ex)
            {
                string errorMessage = "Неправильний ввід. ";

                if (ex.Message.Contains("Invalid token", StringComparison.OrdinalIgnoreCase))
                {
                    errorMessage += "Деякі символи або оператори не підтримуються.";
                }
                else if (ex.Message.Contains("Invalid input", StringComparison.OrdinalIgnoreCase))
                {
                    errorMessage += "Неправильний формат.";
                }
                else if (ex.Message.Contains("1 or 0 allowed only", StringComparison.OrdinalIgnoreCase))
                {
                    errorMessage += "Для логічних операцій дозволено лише 1 та 0.";
                }
                DisplayAlert("Помилка", errorMessage, "OK");
                return 0;
            }/*
            catch (ArgumentException ex)
            {
                DisplayAlert("Помилка", ex.Message, "OK");
                return 0;
            }*/
        }

        private void UpdateGridUI()
        {
            cellGrid.Children.Clear();
            cellGrid.RowDefinitions.Clear();
            cellGrid.ColumnDefinitions.Clear();

            InitializeTable();
        }
    }
}
