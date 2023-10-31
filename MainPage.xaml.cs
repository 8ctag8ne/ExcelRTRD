using Antlr4.Runtime.Misc;
using ExtensionMethods;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Compatibility;
using System;
using System.Collections.Generic;
using Grid = Microsoft.Maui.Controls.Grid;
namespace test
{
	public partial class MainPage : ContentPage
	{
        double originalWidth=10.0;
		public int CountColumn = 20; // кількість стовпчиків (A to Z)
		public int CountRow = 50; // кількість рядків

        Table table;
		public MainPage()
		{
            table = new Table();
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
            if(table.CellExists(coordinates))
            {
                entry.Text=table.GetExpression(coordinates);
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
                DisplayAlert("Помилка", E.Message, "Добре");
                content = "0";
            }
            if(table.CellExists(coordinates) && entry.Text!="")
            {
                if(content!="0")
                {
                    try
                    {
                        table.EditCell(coordinates, content);
                    }
                    catch(Exception E)
                    {
                        DisplayAlert("Помилка", E.Message, "Добре");
                    }
                }
            } else
            if(content!="0")
            {
                try
                {
                    table.AddCell(coordinates, content);
                }
                catch(Exception E)
                {
                    DisplayAlert("Помилка", E.Message, "Добре");
                }
            }
            if(table.CellExists(coordinates))
            {
                content = Convert.ToString(table.GetCellValue(coordinates));
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
                if (child is Entry)
                {
                    Entry newEntry = (Entry)child;
                    newEntry.Text = "";
                    Tuple<int, int>coordinates = new Tuple<int, int>(Grid.GetColumn(child), Grid.GetRow(child));
                    if(table.CellExists(coordinates))
                    {
                        newEntry.Text = Convert.ToString(table.GetCellValue(coordinates));
                    } 
                    if(newEntry.Text == "0" && (table.GetExpression(coordinates)=="0" || table.GetExpression(coordinates)==""))
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
            string result = await DisplayPromptAsync("Збереження файлу", "Вкажіть шлях до розташування файлу:", "Добре", "Закрити", initialValue: "");
            JSONManager.SaveFile(result, new JsonSerializable_(table, CountColumn, CountRow));
		}
		private async void ReadButton_Clicked(object sender, EventArgs e)// Обробка кнопки "Прочитати"
		{

            string result = await DisplayPromptAsync("Прочитати файл", "Вкажіть шлях до розташування файлу:", "Добре", "Закрити", initialValue: "");
            if(result!="") try
            { 
                JsonSerializable_ obj = JSONManager.ReadFile(result);
                CountColumn = obj.CountColumn;
                CountRow = obj.CountRow;
                table = new Table();
                foreach(var cell in obj.A)
                {
                    table.cellByID.Add(cell.ID, new Cell(cell.value, cell.expression, cell.coordinateX, cell.coordinateY, cell.name, cell.ID));
                    table.IDByName.Add(cell.name, cell.ID);
                    table.BasisCells.Add(cell.ID, cell.BasisCells);
                    table.DependentCells.Add(cell.ID, cell.DependentCells);
                    table.color.Add(cell.ID, 0);
                    table.IDByCoordinates.Add(new Tuple<int, int>(cell.coordinateX, cell.coordinateY), cell.ID);
                }
                Refresh();
            }
            catch (Exception E)
            {
                DisplayAlert("Помилка", E.Message, "Добре");
            }
		}
		private async void ExitButton_Clicked(object sender, EventArgs e)
		{
            bool answer = await DisplayAlert("Підтвердження", "Ви дійсно хочете вийти?",
            "Так", "Ні");
            if (answer)
            {
                System.Environment.Exit(0);
            }
		}
		private async void HelpButton_Clicked(object sender, EventArgs e)
		{
		    await DisplayAlert("Довідка", "Лабораторна робота №1 за варіантом 19.\nСтудента групи К-24 Яготіна Назарія Валентиновича.\nВиконана під науковим керівництвом Минька Вадима)", "OK");
		}
		private async void DeleteRowButton_Clicked(object sender, EventArgs e)
		{
            string result = await DisplayPromptAsync("Delete row:", "Please enter a value:", "OK", "Cancel", initialValue: "");
            if (int.TryParse(result, out int number))
            {
                try 
                {
                    table.DeleteRow(number);
                }
                catch (ArgumentException E)
                {
                    DisplayAlert("Помилка", E.Message, "Добре");
                }
                Refresh();
            }
            else
            {
                DisplayAlert("Помилка", "Введений текст не є числом.", "Добре");
            }
		}
		private async void DeleteColumnButton_Clicked(object sender, EventArgs e)
		{
            string result = await DisplayPromptAsync("Delete column:", "Please enter a value:", "OK", "Cancel", initialValue: "");
            if (int.TryParse(result, out int number))
            {
                try 
                {
                    table.DeleteColumn(number);
                }
                catch (ArgumentException E)
                {
                    DisplayAlert("Помилка", E.Message, "Добре");
                }
                Refresh();
            }
            else
            {
                try
                {
                    int num = MyExtension.Convert26To10(result);
                    table.DeleteColumn(num);
                }
                catch(ArgumentException E)
                {
                    DisplayAlert("Помилка", E.Message, "Добре");
                }
                Refresh();
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