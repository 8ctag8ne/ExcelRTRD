namespace test;

using System.Numerics;
using Antlr4.Runtime.Misc;
using DemoParser.Parsing;
using Windows.Networking.NetworkOperators;

public class Cell
{
	public double value{get; set;}
	private string expression{get; set;}
	private int coordinateX{get; set;}
	private int coordinateY{get; set;}
	public string name {get; set;}
	public int ID{get;}
	private static int count=0;

	public string GetExpression()
	{
		return expression;
	}

	public Pair<int, int> GetCoordinates()
	{
		Pair<int, int> p = new Pair<int, int>(this.coordinateX, this.coordinateY);
		return p;
	}

	public string CoordinatesToName()
	{
		string ans = string.Empty;
		int x = coordinateX;
		while(x>0)
		{
			ans=Convert.ToChar((x-1)%26+65)+ans;
			x/=26;
		}
		ans=ans+Convert.ToString(coordinateY);
		return ans;
	}
	public Cell(int x, int y)
	{
		coordinateX = x;
		coordinateY = y;
		value = 0;
		expression = string.Empty;
		name = CoordinatesToName();
		ReCalculate();
		ID = Cell.count;
		Cell.count++;
	}
	private void ReCalculate()
	{
		value = 0;
		if(expression!="")
		{
			value = Calculator.Evaluate(this.expression);
		}
		if(Calculator.GlobalScope.ContainsKey(this.name))
		{
			Calculator.GlobalScope.Remove(this.name);
		}
		Calculator.GlobalScope.Add(this.name, this.value);
	}
	public void ChangeExpression(string exp)
	{
		expression = exp;
		ReCalculate();
	}

	public Cell(int x, int y, string exp)
	{
		coordinateX = x;
		coordinateY = y;
		expression = exp;
		name = CoordinatesToName();
		ID = Cell.count;
		Cell.count++;
		ReCalculate();
	}

	/*public void Refresh()
	{
		Queue<Cell>Q = new Queue<Cell>();
		Q.Enqueue(this);
		foreach(Cell cell in Q)
		{
			cell.ReCalculate();
			foreach(Cell i in cell.dependentCells)
			{
				Q.Enqueue(i);
			}
		}
	}*/

	public void Delete()
	{
		expression = "0";
		Calculator.GlobalScope.Remove(this.name);
	}

	public void ChangePosition(int x, int y)
	{
		coordinateX = x;
		coordinateY = y;
		if(Calculator.GlobalScope.ContainsKey(this.name))
		{
			Calculator.GlobalScope.Remove(this.name);
		}
		name = CoordinatesToName();
		Calculator.GlobalScope.Add(this.name, this.value);
	}

}