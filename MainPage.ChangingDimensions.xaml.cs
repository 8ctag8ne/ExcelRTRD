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