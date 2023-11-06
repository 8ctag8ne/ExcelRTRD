using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Antlr4.Runtime.Misc;
using ExtensionMethods;
using Microsoft.UI.Xaml.Controls;
using Windows.Perception.Spatial;

namespace test;
public partial class Table
{
	public Dictionary<int, Cell> CellByID{get; set;}
	public Dictionary<string, int>IDByName{get; set;}
	public Dictionary<Tuple<int, int>, int>IDByCoordinates{get; set;}
	public Dictionary<int, int>Color{get; set;}
	public Dictionary<int, List<int>>DependentCells{get; set;}
	public Dictionary<int, List<int>>BasisCells{get; set;}

	public Table()
	{
		CellByID = new Dictionary<int, Cell>();
		IDByName = new Dictionary<string, int>();
		IDByCoordinates = new Dictionary<Tuple<int, int>, int>();
		Color = new Dictionary<int, int>();
		DependentCells = new Dictionary<int, List<int>>();
		BasisCells = new Dictionary<int, List<int>>();
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
		string exp = CellByID[ID].GetExpression();
		CellByID[ID].ChangeExpression(exp);
		foreach(var i in DependentCells[ID])
		{
			Refresh(i);
		}
	}

	private bool DFS(int ID)
	{
		Color[ID] = 1;
		foreach(var newCellID in DependentCells[ID])
		{
			if(Color[newCellID]==0)
			{
				if(DFS(newCellID) == true)
				{
					return true;
				}
			}
			if(Color[newCellID]==1)
			{
				return true;
			}
		}
		Color[ID] = 2;
		return false;
	}

	public bool FindCycles(Tuple<int, int> coordinates)
	{
		foreach(var key in Color.Keys)
		{
			Color[key] = 0;
		}
		return DFS(IDByCoordinates[coordinates]);
	}
}