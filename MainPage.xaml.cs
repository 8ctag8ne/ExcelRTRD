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
		
		private void CalculateButton_Clicked(object sender, EventArgs e)
		{
		// Обробка кнопки "Порахувати"
		}
	}
}