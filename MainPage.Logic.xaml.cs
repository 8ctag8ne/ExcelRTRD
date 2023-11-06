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
    }
}