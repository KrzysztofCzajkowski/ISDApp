using IronPdf;
using System.Text.RegularExpressions;

namespace ISDLibrary;

public static class PdfHandler
{
    public static string ReadWholePdf(string filename)
    {
        using PdfDocument document = PdfDocument.FromFile(filename);
        string text = document.ExtractAllText();
        return text;
    }

    public static int CountOccurences(string filename, string term)
    {
        using PdfDocument document = PdfDocument.FromFile(filename);
        string text = document.ExtractAllText();
        int count = Regex.Matches(text, term, RegexOptions.IgnoreCase, TimeSpan.FromSeconds(30)).Count;
        return count;
    }

    public static double[] CountOccurencesVector(string filename, string[] terms)
    {
        using PdfDocument document = PdfDocument.FromFile(filename);
        string text = document.ExtractAllText();
        double[] count = new double[terms.Length];

        for (int i = 0; i < terms.Length; i++)
        {
            count[i] = Regex.Matches(text, terms[i], RegexOptions.IgnoreCase, TimeSpan.FromSeconds(30)).Count;
        }
        return count;
    }

    public static OccurenceMatrix CountOccurenceMatrix(string[] filenames, string[] terms)
    {
        string text = String.Empty;
        double[,] count = new double[filenames.Length,terms.Length];
        for (int i = 0; i < filenames.Length; i++)
        {
            using PdfDocument document = PdfDocument.FromFile(filenames[i]);
            text = document.ExtractAllText();

            for (int j = 0; j < terms.Length; j++)
            {
                count[i,j] = Regex.Matches(text, terms[j], RegexOptions.IgnoreCase, TimeSpan.FromSeconds(30)).Count;
            }
        }
        return new OccurenceMatrix(count);
    }
}
