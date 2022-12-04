using Spectre.Console;

namespace ISDLibrary;
public class EuclideanDistanceMatrix : DistanceMatrix
{
	int p;
	int r;

	public int Length { get;}

	private double[,] distanceMatrix;

	public double[,] DistanceMatrix
	{
		get { return distanceMatrix; }
	}

	public EuclideanDistanceMatrix(OccurenceMatrix occurenceMatrix, int p = 2, int r = 2) : base(occurenceMatrix)
	{
		this.p = p;
		this.r = r;
        Length = occurenceMatrix.Length;
		distanceMatrix = GetDistanceMatrix();
	}

    public override int FindClosest(int index)
    {
		var minDistance = GetDistance(0, index);
		int minIndex = 0;
        for (int i = 1; i < Length; i++)
		{
			// Don't return the same document as closest.
			if (i != index)
			{
				var distance = GetDistance(i, index);
				if (distance < minDistance)
				{
					minDistance = distance;
					minIndex = i;
				}
			}
		}
		return minIndex;
    }
    public override double[,] GetDistanceMatrix()
	{
		var result = new double[Length, Length];
		for (int i = 0; i < Length; i++)
		{
			for (int j = 0; j < Length; j++)
			{
				if (i == j)
					result[i, i] = 0;
				else
					result[i, j] += GetDistance(i, j);
            }
		}
		return result;
	}

	public override double GetDistance(int first, int second)
	{
		double distance = 0;
		for (int i = 0; i < Length; i++)
		{
			distance += Math.Pow(Math.Abs(OccurenceMatrix.Matrix[first, i] - OccurenceMatrix.Matrix[second, i]), p);
		}
		return Math.Pow(distance, 1.0/r);
	}

    public string[] GetRowString(int rowNumber)
    {
        string[] row = new string[Length + 1];
        row[0] = $"D {rowNumber + 1}";
        for (int i = 1; i < Length + 1; i++)
        {
            row[i] = DistanceMatrix[rowNumber, i - 1].ToString("F2");
        }

        return row;
    }

    public void Print2(string header)
    {
        var table = new Table();
        table.Title = new TableTitle(header);
        table.AddColumn("");
        for (int i = 0; i < Length; i++)
        {
            table.AddColumn($"D {i + 1}");
        }

        foreach (var column in table.Columns)
        {
            column.RightAligned();
            column.Width = 7;
        }

        for (int i = 0; i < Length; i++)
        {
            table.AddRow(GetRowString(i));
        }
        AnsiConsole.Write(table);
    }

}
