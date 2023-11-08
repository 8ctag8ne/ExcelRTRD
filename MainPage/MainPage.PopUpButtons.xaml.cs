using CommunityToolkit.Maui.Storage;
using System.Text;
namespace test
{
    public partial class MainPage : ContentPage
	{
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
    }
}