using System.Collections.ObjectModel;
using Antlr4.Runtime.Misc;
using ExtensionMethods;
using Microsoft.UI.Xaml.Controls;
using Windows.Perception.Spatial;

namespace test;
public class Table
{
	private Dictionary<int, Cell> cellByID;
	private Dictionary<string, int>IDByName;
	private Dictionary<Pair<int, int>, int>IDByCoordinates;
	private Dictionary<int, int>color;
	private Dictionary<int, List<int>>DependentCells;
	private Dictionary<int, List<int>>BasisCells;

	public List<int> UnprocessedCells;


	public Table()
	{
		cellByID = new Dictionary<int, Cell>();
		IDByName = new Dictionary<string, int>();
		IDByCoordinates = new Dictionary<Pair<int, int>, int>();
		color = new Dictionary<int, int>();
		DependentCells = new Dictionary<int, List<int>>();
		BasisCells = new Dictionary<int, List<int>>();
		UnprocessedCells = new List<int>();
	}

	public double GetCellValue(Pair<int, int>coordinates)
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
	public bool CellExists(Pair<int, int>coordinates)
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

	public string GetExpression(Pair<int, int>coordinates)
	{
		if(IDByCoordinates.ContainsKey(coordinates) && cellByID.ContainsKey(IDByCoordinates[coordinates]))
		{
			return cellByID[IDByCoordinates[coordinates]].GetExpression();
		}
		return "";
	}


	public void EditCell(Pair<int, int> coordinates, string expression)
	{
		List<string>str = MyExtension.ParseName(expression);
		List<int>OldBasis = new List<int>();
		OldBasis = BasisCells[IDByCoordinates[coordinates]];
		DeleteCell(coordinates);

		foreach(string s in str)
		{
			Pair<int, int> p = MyExtension.NameToCoordinates(s);
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
			throw new ArgumentException("Cycle has occured");
		} else
		{
			cellByID[IDByCoordinates[coordinates]].ChangeExpression(expression);
			Refresh(IDByCoordinates[coordinates]);
		}
	}

	public void AddCell(Pair<int, int> coordinates)
	{
		Cell cell = new Cell(coordinates.a, coordinates.b);
		cellByID.Add(cell.ID, cell);
		IDByName.Add(cell.name, cell.ID);
		IDByCoordinates.Add(coordinates, cell.ID);

		color.Add(cell.ID, 0);
		DependentCells.Add(cell.ID, new List<int>());
		BasisCells.Add(cell.ID, new List<int>());
	}

	public void AddCell(Pair<int, int> coordinates, string expression)
	{
		Cell cell = new Cell(coordinates.a, coordinates.b, expression);
		cellByID.Add(cell.ID, cell);
		IDByName.Add(cell.name, cell.ID);
		IDByCoordinates.Add(coordinates, cell.ID);

		color.Add(cell.ID, 0);
		DependentCells.Add(cell.ID, new List<int>());
		BasisCells.Add(cell.ID, new List<int>());

		List<string>str = MyExtension.ParseName(expression);
		foreach(string s in str)
		{
			Pair<int, int> p = MyExtension.NameToCoordinates(s);
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
			throw new ArgumentException("Cycle has occured");
		}
	}
	
	public void DeleteCell(Pair<int, int> coordinates)
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
		UnprocessedCells.Add(ID);
		foreach(var i in DependentCells[ID])
		{
			Refresh(i);
		}
	}

	private void DeletePermanently(Pair<int, int>coordinates)
	{
		int ID = IDByCoordinates[coordinates];
		if(DependentCells[ID].Count>0)
		{
			//Cell cell = cellByID[ID];
			string s = "There is a cell "+cellByID[ID].name+" which is used to calcute values of such cells:\n";
			foreach(var newID in DependentCells[ID])
			{
				s+=cellByID[newID].name+"; ";
			}
			s+="\nChange the expression of the cells mentioned above before deleting current";
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
			UnprocessedCells.Remove(ID);
		}

	}

	public void DeleteRow(int number)
	{
		try{
			List<Pair<int, int>> DeleteQuery = new List<Pair<int, int>>();
			foreach(var cell in cellByID.Values)
			{
				if(cell.GetCoordinates().b == number)
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
			throw new ArgumentException(e.Message+" row.");
		}
	}

	public void DeleteColumn(int number)
	{
		try{
			List<Pair<int, int>> DeleteQuery = new List<Pair<int, int>>();
			foreach(var cell in cellByID.Values)
			{
				if(cell.GetCoordinates().a == number)
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
			throw new ArgumentException(e.Message+" column.");
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
		//bool flag = false;
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

	public bool FindCycles(Pair<int, int> coordinates)
	{
		foreach(var key in color.Keys)
		{
			color[key] = 0;
		}
		return DFS(IDByCoordinates[coordinates]);
	}
}