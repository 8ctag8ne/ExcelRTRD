using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Antlr4.Runtime.Misc;
using ExtensionMethods;
using Microsoft.UI.Xaml.Controls;
using Windows.Perception.Spatial;

namespace test;
public class Table
{
	public Dictionary<int, Cell> cellByID;

	public Dictionary<string, int>IDByName;

	//[JsonPropertyName("IDByCoordinates")]
	public Dictionary<Tuple<int, int>, int>IDByCoordinates;

	public Dictionary<int, int>color;

	public Dictionary<int, List<int>>DependentCells;

	public Dictionary<int, List<int>>BasisCells;

	/*public Table(Dictionary<int, Cell> JcellByID, Dictionary<string, int> JIDByName, Dictionary<int, int>Jcolor, Dictionary<int, List<int>>JDependentCells, Dictionary<int, List<int>>JBasisCells)
	{
		cellByID = JcellByID;
		IDByName = JIDByName;
		color = Jcolor;
		DependentCells = JDependentCells;
		BasisCells = JBasisCells;
		IDByCoordinates = new Dictionary<Tuple<int, int>, int>();
		foreach(var cell in cellByID.Values)
		{
			IDByCoordinates.Add(cell.GetCoordinates(), cell.ID);
		}
	}*/


	public Table()
	{
		cellByID = new Dictionary<int, Cell>();
		IDByName = new Dictionary<string, int>();
		IDByCoordinates = new Dictionary<Tuple<int, int>, int>();
		color = new Dictionary<int, int>();
		DependentCells = new Dictionary<int, List<int>>();
		BasisCells = new Dictionary<int, List<int>>();
	}

	public double GetCellValue(Tuple<int, int>coordinates)
	{
		if(IDByCoordinates.ContainsKey(coordinates) && cellByID.ContainsKey(IDByCoordinates[coordinates]))
		{
			return cellByID[IDByCoordinates[coordinates]].value;
		} else
		{
			return 0.0;
		}
	}

	public double GetCellValue(string name)
	{
		if(IDByName.ContainsKey(name) && cellByID.ContainsKey(IDByName[name]))
		{
			return cellByID[IDByName[name]].value;
		} else
		{
			return 0.0;
		}
	}
	public bool CellExists(Tuple<int, int>coordinates)
	{
		if(IDByCoordinates.ContainsKey(coordinates) && cellByID.ContainsKey(IDByCoordinates[coordinates]))
		{
			return true;
		}
		return false;
	}

	public bool CellExists(string name)
	{
		if(IDByName.ContainsKey(name) && cellByID.ContainsKey(IDByName[name]))
		{
			return true;
		}
		return false;
	}

	public string GetExpression(Tuple<int, int>coordinates)
	{
		if(IDByCoordinates.ContainsKey(coordinates) && cellByID.ContainsKey(IDByCoordinates[coordinates]))
		{
			return cellByID[IDByCoordinates[coordinates]].GetExpression();
		}
		return "";
	}


	public void EditCell(Tuple<int, int> coordinates, string expression)
	{
		List<string>str = MyExtension.ParseName(expression);
		List<int>OldBasis = new List<int>();
		OldBasis = BasisCells[IDByCoordinates[coordinates]];
		foreach(var oldID in BasisCells[IDByCoordinates[coordinates]])
		{
			DependentCells[oldID].Remove(IDByCoordinates[coordinates]);
		}
		BasisCells[IDByCoordinates[coordinates]].Clear();

		foreach(string s in str)
		{
			Tuple<int, int> p = MyExtension.NameToCoordinates(s);
			if(!CellExists(p))
			{
				AddCell(p);
			} 
			DependentCells[IDByCoordinates[p]].Add(IDByCoordinates[coordinates]);
			BasisCells[IDByCoordinates[coordinates]].Add(IDByCoordinates[p]);
		}

		if(FindCycles(coordinates)==true)
		{
			foreach(var ID in BasisCells[IDByCoordinates[coordinates]])
			{
				DependentCells[ID].Remove(IDByCoordinates[coordinates]);
			}
			
			BasisCells[IDByCoordinates[coordinates]] = OldBasis;
			foreach(var ID in BasisCells[IDByCoordinates[coordinates]])
			{
				DependentCells[ID].Add(IDByCoordinates[coordinates]);
			}
			
			throw new ArgumentException("Введений вираз призвів до утворення циклу.");
		} else
		{
			cellByID[IDByCoordinates[coordinates]].ChangeExpression(expression);
			Refresh(IDByCoordinates[coordinates]);
		}
	}

	public void AddCell(Tuple<int, int> coordinates)
	{
		Cell cell = new Cell(coordinates.Item1, coordinates.Item2);
		cellByID.Add(cell.ID, cell);
		IDByName.Add(cell.name, cell.ID);
		IDByCoordinates.Add(coordinates, cell.ID);

		color.Add(cell.ID, 0);
		DependentCells.Add(cell.ID, new List<int>());
		BasisCells.Add(cell.ID, new List<int>());
	}

	public void AddCell(Tuple<int, int> coordinates, string expression)
	{
		Cell cell = new Cell(coordinates.Item1, coordinates.Item2, expression);
		cellByID.Add(cell.ID, cell);
		IDByName.Add(cell.name, cell.ID);
		IDByCoordinates.Add(coordinates, cell.ID);

		color.Add(cell.ID, 0);
		DependentCells.Add(cell.ID, new List<int>());
		BasisCells.Add(cell.ID, new List<int>());

		List<string>str = MyExtension.ParseName(expression);
		foreach(string s in str)
		{
			Tuple<int, int> p = MyExtension.NameToCoordinates(s);
			if(!CellExists(p))
			{
				AddCell(p);
			} 
			DependentCells[IDByCoordinates[p]].Add(IDByCoordinates[coordinates]);
			BasisCells[IDByCoordinates[coordinates]].Add(IDByCoordinates[p]);
		}

		if(FindCycles(coordinates)==true)
		{
			DeleteCell(coordinates);
			throw new ArgumentException("Введений вираз призвів до утворення циклу.");
		}
	}
	
	public void DeleteCell(Tuple<int, int> coordinates)
	{
		Cell cell = cellByID[IDByCoordinates[coordinates]];
		//cellByID.Remove(cell.ID);
		//IDByName.Remove(cell.name);
		//IDByCoordinates.Remove(cell.GetCoordinates());

		//color.Remove(cell.ID);
		//DependentCells.Remove(cell.ID);
		foreach(var ID in BasisCells[cell.ID])
		{
			DependentCells[ID].Remove(cell.ID);
		}
		BasisCells[cell.ID].Clear();
		cellByID[cell.ID].ChangeExpression("0");
		Refresh(IDByCoordinates[coordinates]);
	}

	/*public void Refresh(Pair<int, int>coordinates)
	{
		Queue<int>Q = new Queue<int>();
		Q.Enqueue(IDByCoordinates[coordinates]);
		Collection<int> used = new Collection<int>
        {
            IDByCoordinates[coordinates]
        };
		for(int j=0; j<Q.Count; j++)
		{
			int ID = Q.ElementAt(j);
			string exp = cellByID[ID].GetExpression();
			cellByID[ID].ChangeExpression(exp);
			foreach(var i in DependentCells[ID])
			{
				if(!used.Contains(i))
				{
					Q.Enqueue(i);
					used.Add(i);
				}
			}
		}
	}*/

	public void Refresh(int ID)
	{
		string exp = cellByID[ID].GetExpression();
		cellByID[ID].ChangeExpression(exp);
		foreach(var i in DependentCells[ID])
		{
			Refresh(i);
		}
	}

	private void DeletePermanently(Tuple<int, int>coordinates)
	{
		int ID = IDByCoordinates[coordinates];
		if(DependentCells[ID].Count>0)
		{
			//Cell cell = cellByID[ID];
			string s = "Від значення клітинки "+cellByID[ID].name+" залежить значення таких клітинок:\n";
			foreach(var newID in DependentCells[ID])
			{
				s+=cellByID[newID].name+"; ";
			}
			s+="\nЗмініть вираз, записаний у даних клітинках перед видаленням цього";
			throw new ArgumentException(s);
		} else
		{
			Cell cell = cellByID[IDByCoordinates[coordinates]];
			foreach(var newID in BasisCells[cell.ID])
			{
				DependentCells[newID].Remove(cell.ID);
			}
			cellByID.Remove(cell.ID);
			IDByName.Remove(cell.name);
			IDByCoordinates.Remove(cell.GetCoordinates());

			color.Remove(cell.ID);
			DependentCells.Remove(cell.ID);
			BasisCells.Remove(cell.ID);
			cell.Delete();
		}

	}

	public void DeleteRow(int number)
	{
		try{
			List<Tuple<int, int>> DeleteQuery = new List<Tuple<int, int>>();
			foreach(var cell in cellByID.Values)
			{
				if(cell.GetCoordinates().Item2 == number)
				{
					DeleteQuery.Add(cell.GetCoordinates());
					if(DependentCells[cell.ID].Count>0)
					{
						DeletePermanently(cell.GetCoordinates());
					}
				}
			}
			foreach(var coordinates in DeleteQuery)
			{
				DeletePermanently(coordinates);
			}
		}
		catch (ArgumentException e)
		{
			throw new ArgumentException(e.Message+" рядка.");
		}
	}

	public void DeleteColumn(int number)
	{
		try{
			List<Tuple<int, int>> DeleteQuery = new List<Tuple<int, int>>();
			foreach(var cell in cellByID.Values)
			{
				if(cell.GetCoordinates().Item1 == number)
				{
					DeleteQuery.Add(cell.GetCoordinates());
					if(DependentCells[cell.ID].Count>0)
					{
						DeletePermanently(cell.GetCoordinates());
					}
				}
			}
			foreach(var coordinates in DeleteQuery)
			{
				DeletePermanently(coordinates);
			}
		}
		catch (ArgumentException e)
		{
			throw new ArgumentException(e.Message+" стовпця.");
		}

	}
	
	public void AddRow(int number)
	{

	}

	public void AddColumn(int number)
	{

	}

	private bool DFS(int ID)
	{
		color[ID] = 1;
		foreach(var newCellID in DependentCells[ID])
		{
			if(color[newCellID]==0)
			{
				if(DFS(newCellID) == true)
				{
					return true;
				}
			}
			if(color[newCellID]==1)
			{
				return true;
			}
		}
		color[ID] = 2;
		return false;
	}

	public bool FindCycles(Tuple<int, int> coordinates)
	{
		foreach(var key in color.Keys)
		{
			color[key] = 0;
		}
		return DFS(IDByCoordinates[coordinates]);
	}
}