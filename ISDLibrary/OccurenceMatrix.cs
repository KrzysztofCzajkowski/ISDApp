using Spectre.Console;

namespace ISDLibrary;
public class OccurenceMatrix
{
    public double[,] Matrix { get; set; }

    public int Length
    {
        get { return Matrix.GetLength(0); }
    }

    public int Width
    {
        get { return Matrix.GetLength(1); }
    }

    public OccurenceMatrix(int length, int width)
    {
        Matrix = new double[length, width];
    }

    public OccurenceMatrix(double[,] matrix)
    {
        Matrix = matrix;
    }

    public OccurenceMatrix Normalize()
    {
        OccurenceMatrix normalized = new OccurenceMatrix(Length, Length);
        // Iterate by term
        for (int i = 0; i < Length; i++)
        {
            double max = 0;
            // Iterate by document to get max number of occurences
            for (int j = 0; j < Length; j++)
            {
                if (Matrix[j,i] > max)
                    max = Matrix[j,i];
            }
            // Iterate by document again and normalize by max value
            if (max > 0)
            {
                for (int j = 0; j < Length; j++)
                {
                    normalized.Matrix[j,i] = (Matrix[j,i]/max) * 100;
                }
            }
        }
        return normalized;
    }

    public string[] GetRowString(int rowNumber)
    {
        string[] row = new string[Width+1];
        row[0] = $"D {rowNumber+1}";
        for (int i = 1; i < Width+1; i++)
        {
            row[i] = Matrix[rowNumber, i-1].ToString("F2");
        }

        return row;
    }

    public void Print(string header)
    {
        Console.WriteLine();
        // Table header
        Console.WriteLine(header);
        Console.Write("|\t\t|");
        for (int i = 0; i < Length; i++)
        {
            Console.Write($"\tT{i + 1}\t");
            Console.Write("|");
        }
        Console.Write(Environment.NewLine);
        Console.WriteLine();
        // Table data
        for (int i = 0; i < Length; i++)
        {
            Console.Write($"|\tD{i + 1}\t|");
            for (int j = 0; j < Width; j++)
            {
                Console.Write($"\t{Matrix[i, j]:F2} \t|");
            }
            Console.Write(Environment.NewLine);
            Console.WriteLine();
        }
    }

    public void Print2(string header)
    {
        var table = new Table();
        table.Title = new TableTitle(header);
        table.AddColumn("");
        for (int i = 0; i < Width; i++)
        {
            table.AddColumn($"T {i+1}");
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
