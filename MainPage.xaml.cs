using Antlr4.Runtime.Misc;
using ExtensionMethods;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Compatibility;
using System;
using System.Collections.Generic;
using Microsoft.Maui.Storage;
using System.Collections.Generic;
using Grid = Microsoft.Maui.Controls.Grid;
using CommunityToolkit.Maui.Storage;
using System.Text;
namespace test
{
	public partial class MainPage : ContentPage
	{
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private double originalWidth=10.0;
        private int countColumn = 20; // кількість стовпчиків (A to Z)
        private int countRow = 50; // кількість рядків
        private Table table;
        public int CountColumn { get => countColumn; set => countColumn = value; }
        public int CountRow { get => countRow; set => countRow = value; }
        public Table Table { get => table; set => table = value; }

        public MainPage()
		{
            Table = new Table();
            InitializeComponent();
            CreateGrid();
		}
		//створення таблиці
		private void CreateGrid()
		{
            AddColumnsAndColumnLabels();
            AddRowsAndCellEntries();
		}
		private void AddColumnsAndColumnLabels()
		{
            // Додати стовпці та підписи для стовпців
            for (int col = 0; col < CountColumn + 1; col++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                if (col > 0)
                {
                    var label = new Label
                    {
                        Text = GetColumnName(col),
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Center
                    };
                    Grid.SetRow(label, 0);
                    Grid.SetColumn(label, col);
                    grid.Children.Add(label);
                }
            }
		}
		private void AddRowsAndCellEntries()
		{
            // Додати рядки, підписи для рядків та комірки
            for (int row = 0; row < CountRow; row++)
            {
                grid.RowDefinitions.Add(new RowDefinition());
                // Додати підпис для номера рядка
                var label = new Label
                {
                    Text = (row + 1).ToString(),
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center
                };
                Grid.SetRow(label, row + 1);
                Grid.SetColumn(label, 0);
                grid.Children.Add(label);
                // Додати комірки (Entry) для вмісту
                for (int col = 0; col < CountColumn; col++)
                {
                    var entry = new Entry
                    {
                        Text = "",
                        VerticalOptions = LayoutOptions.Fill,
                        HorizontalOptions = LayoutOptions.Fill,
                        HorizontalTextAlignment = TextAlignment.Center
                    };
                    originalWidth = entry.Width;
                    entry.Focused += Entry_Focused;
                    entry.Unfocused += Entry_Unfocused; // обробник події Unfocused
                    Grid.SetRow(entry, row + 1);
                    Grid.SetColumn(entry, col + 1);
                    grid.Children.Add(entry);
                }
            }
		}
		private string GetColumnName(int colIndex)
		{
            int dividend = colIndex;
            string columnName = string.Empty;
            while (dividend > 0)
            {
                int modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo) + columnName;
                dividend = (dividend - modulo) / 26;
            }
            return columnName;
		}
		
        private void Entry_Focused(object sender, FocusEventArgs e)
		{
            var entry = (Entry)sender;
            entry.WidthRequest = 100;
            //entry.AnchorX += 50;
            var row = Grid.GetRow(entry) - 1;
            var col = Grid.GetColumn(entry) - 1;
            Tuple<int, int>coordinates = new Tuple<int, int>(col+1, row+1);
            if(Table.CellExists(coordinates))
            {
                entry.Text=Table.GetExpression(coordinates);
            } else
            {
                entry.Text = "";
            }
		}

        // викликається, коли користувач вийде зі зміненої клітинки (втратить фокус)
		private void Entry_Unfocused(object sender, FocusEventArgs e)
		{
            var entry = (Entry)sender;
            var row = Grid.GetRow(entry) - 1;
            entry.WidthRequest = originalWidth;
            //entry.AnchorX -=50;
            var col = Grid.GetColumn(entry) - 1;
            Tuple<int, int>coordinates = new Tuple<int, int>(col+1, row+1);
            var content = entry.Text;
            if(content == "")
            {
                content ="0";
            }
            try
            {
                Calculator.Evaluate(content);
            }
            catch(Exception E)
            {
                string s = E.Message;
                if(s[0]>='A' && s[0]<='Z')
                {
                     s = "Введено некоректний вираз.";
                }
                DisplayAlert("Помилка", s+"😵", "Добре");
                content = "0";
            }
            if(Table.CellExists(coordinates) && entry.Text!="")
            {
                if(content!="0")
                {
                    try
                    {
                        Table.EditCell(coordinates, content);
                    }
                    catch(Exception E)
                    {
                        string s = E.Message;
                        if(s[0]>='A' && s[0]<='Z')
                        {
                            s = "Введено некоректний вираз.";
                        }
                        DisplayAlert("Помилка", s+"😵", "Добре");
                    }
                }
            } else
            if(content!="0")
            {
                try
                {
                    Table.AddCell(coordinates, content);
                }
                catch(Exception E)
                {
                    string s = E.Message;
                    if(s[0]>='A' && s[0]<='Z')
                    {
                        s = "Введено некоректний вираз.";
                    }
                    DisplayAlert("Помилка", s+"😵", "Добре");
                }
            }
            if(Table.CellExists(coordinates))
            {
                content = Convert.ToString(Table.GetCellValue(coordinates));
            }
            entry.Text = content;
            Refresh();
            //entry.WidthRequest = Math.Min(originalWidth * 1.5, entry.Text.Length*10);
		// Додайте додаткову логіку, яка виконується при виході зі зміненої клітинки
		}
        private void Refresh()
        {
            foreach (View child in grid.Children)
            {
                if (child is Entry newEntry)
                {
                    newEntry.Text = "";
                    Tuple<int, int> coordinates = new(Grid.GetColumn(child), Grid.GetRow(child));
                    if (Table.CellExists(coordinates))
                    {
                        newEntry.Text = Convert.ToString(Table.GetCellValue(coordinates));
                    }
                    if (newEntry.Text == "0" && (Table.GetExpression(coordinates) == "0" || Table.GetExpression(coordinates) == ""))
                    {
                        newEntry.Text = "";
                    }
                }
            }
        }
		private void CalculateButton_Clicked(object sender, EventArgs e)
		{
		// Обробка кнопки "Порахувати"
		}
		private async void SaveButton_Clicked(object sender, EventArgs e) // Обробка кнопки "Зберегти"
		{
            try{
                using var stream = new MemoryStream(Encoding.Default.GetBytes("Text"));
                var path = await FileSaver.SaveAsync("table.json", stream, cancellationTokenSource.Token);
                //DisplayPromptAsync("Збереження файлу", "Вкажіть шлях до розташування файлу:", "Добре", "Закрити", initialValue: "");
                if(path.FilePath!=null)
                {
                    JSONManager.SaveFile(path.FilePath, new JsonSerializable_(Table, CountColumn, CountRow));
                }
            }
            catch (NullReferenceException)
            {
                return;
            }
            catch(Exception)
            {
                await DisplayAlert("Помилка", "Неможливо зберегти поточний файл.", "Добре");
                return;
            }
		}
		private async void ReadButton_Clicked(object sender, EventArgs e)// Обробка кнопки "Прочитати"
		{
            var customFileType = new FilePickerFileType(
                new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.WinUI, new[] { ".json"} }, // file extension
                });
            var MyFile = await FilePicker.PickAsync(new PickOptions{
                FileTypes = customFileType,
                PickerTitle = "Оберіть файл"
            });
            try
            { 
                string result = MyFile.FullPath;
                JsonSerializable_ obj = JSONManager.ReadFile(result);
                CountColumn = obj.CountColumn;
                CountRow = obj.CountRow;
                foreach(var cell in Table.CellByID.Values) //видаляємо старі значення клітин з GlobalScope
                {
                    cell.Delete();
                }
                Table = new Table();
                Cell.Count = 0;
                foreach(var cell in obj.A) //додаємо нові клітинки в таблицю
                {
                    Table.CellByID.Add(cell.ID, new Cell(cell.value, cell.expression, cell.coordinateX, cell.coordinateY, cell.name, cell.ID));
                    Table.IDByName.Add(cell.name, cell.ID);
                    Table.BasisCells.Add(cell.ID, cell.BasisCells);
                    Table.DependentCells.Add(cell.ID, cell.DependentCells);
                    Table.Color.Add(cell.ID, 0);
                    Table.IDByCoordinates.Add(new Tuple<int, int>(cell.coordinateX, cell.coordinateY), cell.ID);
                }
                Refresh(); //оновлюємо Grid для коректного відображення нових значень
            }
            catch(NullReferenceException) //якщо користувач натиснув "Закрити"
            {
                return;
            }
            catch (Exception) //якщо користувач - пітух
            {
                await DisplayAlert("Помилка", "Неможливо обрати даний файл.", "Добре");
            }
		}
		private async void ExitButton_Clicked(object sender, EventArgs e)
		{
            bool answer = await DisplayAlert("Підтвердження", "Ви дійсно хочете вийти?🤨🤨🤨",
            "Так", "Ні");
            if (answer)
            {
                System.Environment.Exit(0);
            }
		}
		private async void HelpButton_Clicked(object sender, EventArgs e)
		{
		    await DisplayAlert("Довідка", "Лабораторна робота №1 за варіантом 19.\nСтудента групи К-24 Яготіна Назарія Валентиновича.\nВиконана під науковим керівництвом Минька Вадима та ChatGPT😎🤙", "Крутяк");
		}
		private async void DeleteRowButton_Clicked(object sender, EventArgs e)
		{
            string result = await DisplayPromptAsync("Видалити рядок:", "Введіть номер рядка:", "Добре", "Закрити", initialValue: "");
            if(result==null)
            {
                return;
            }
            if (int.TryParse(result, out int number))
            {
                try 
                {
                    Table.DeleteRow(number);
                    Refresh();
                }
                catch (ArgumentException E)
                {
                    string s = E.Message;
                    if(s[0]>='A' && s[0]<='Z')
                    {
                        s = "Введено неправильний вираз";
                    }
                    await DisplayAlert("Помилка", s+"💀", "Добре");
                }
            }
            else
            if(result!=null)
            {
                await DisplayAlert("Помилка", "Введений текст не є числом.👽", "Добре");
            }
		}
		private async void DeleteColumnButton_Clicked(object sender, EventArgs e)
		{
            string result = await DisplayPromptAsync("Видалити стовпець:", "Введіть номер або значення стовпця:", "Добре", "Закрити", initialValue: "");
            if(result=="")
            {
                return;
            }
            if (int.TryParse(result, out int number))
            {
                try 
                {
                    Table.DeleteColumn(number);
                    Refresh();
                }
                catch (ArgumentException E)
                {
                    string s = E.Message;
                    if(s[0]>='A' && s[0]<='Z')
                    {
                        s = "Введено неправильний вираз";
                    }
                    await DisplayAlert("Помилка", s+"💀", "Добре");
                }
            }
            else
            {
                try
                {
                    int num = MyExtension.Convert26To10(result);
                    Table.DeleteColumn(num);
                    Refresh();
                }
                catch(ArgumentException E)
                {
                    string s = E.Message;
                    if(s[0]>='A' && s[0]<='Z')
                    {
                        s = "Введено неправильний вираз";
                    }
                    await DisplayAlert("Помилка", s+"💀", "Добре");
                }
            }
            
		}
		private void AddRowButton_Clicked(object sender, EventArgs e)
		{
            int newRow = grid.RowDefinitions.Count;
            CountRow++;
            // Add a new row definition
            grid.RowDefinitions.Add(new RowDefinition());
            // Add label for the row number
            var label = new Label
            {
                Text = newRow.ToString(),
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };
            Grid.SetRow(label, newRow);
            Grid.SetColumn(label, 0);
            grid.Children.Add(label);
            // Add entry cells for the new row
            for (int col = 0; col < CountColumn; col++)
            {
                var entry = new Entry
                {
                    Text = "",
                    VerticalOptions = LayoutOptions.Fill,
                    HorizontalOptions = LayoutOptions.Fill,
                    HorizontalTextAlignment = TextAlignment.Center
                };
                originalWidth = entry.Width;
                entry.Focused += Entry_Focused;
                entry.Unfocused += Entry_Unfocused;
                Grid.SetRow(entry, newRow);
                Grid.SetColumn(entry, col + 1);
                grid.Children.Add(entry);
            }
		}
		private void AddColumnButton_Clicked(object sender, EventArgs e)
		{
			int newColumn = grid.ColumnDefinitions.Count;
            CountColumn++;
			// Add a new column definition
			grid.ColumnDefinitions.Add(new ColumnDefinition());
			// Add label for the column name
			var label = new Label
			{
                Text = GetColumnName(newColumn),
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
			};
			Grid.SetRow(label, 0);
			Grid.SetColumn(label, newColumn);
			grid.Children.Add(label);
			// Add entry cells for the new column
			for (int row = 0; row < CountRow; row++)
			{
			    var entry = new Entry
                {
                    Text = "",
                    VerticalOptions = LayoutOptions.Fill,
                    HorizontalOptions = LayoutOptions.Fill,
                    HorizontalTextAlignment = TextAlignment.Center
                };
                originalWidth = entry.Width;
                entry.Focused += Entry_Focused;
                entry.Unfocused += Entry_Unfocused;
                Grid.SetRow(entry, row + 1);
                Grid.SetColumn(entry, newColumn);
                grid.Children.Add(entry);
			}
		}
	}
}