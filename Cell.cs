namespace test;

using System.Numerics;
using Antlr4.Runtime.Misc;
using ExtensionMethods;
using DemoParser.Parsing;
using Windows.Networking.NetworkOperators;
using System.Text.Json;
using System.Text.Json.Serialization;

public class Cell
{
	public double value{get; set;}
	public string expression{get; set;}
	public int coordinateX{get; set;}
	public int coordinateY{get; set;}
	public string name {get; set;}
	public int ID{get;}
	public static int count=0;

	public Cell(double val, string exp, int coordX, int coordY, string jname, int id)
	{
		value = val;
		expression = exp;
		coordinateX = coordX;
		coordinateY = coordY;
		name = jname;
		ID = id;
		Cell.count = Math.Max(Cell.count, ID+1);
	}

	public string GetExpression()
	{
		return expression;
	}

	public Tuple<int, int> GetCoordinates()
	{
		Tuple<int, int> p = new Tuple<int, int>(this.coordinateX, this.coordinateY);
		return p;
	}

	public string GetName()
	{
		return this.name;
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

	public void Delete()
	{
		expression = "0";
		Calculator.GlobalScope.Remove(this.name);
	}

}