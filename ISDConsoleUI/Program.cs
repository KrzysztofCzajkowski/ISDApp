using ISDLibrary;
using System.Diagnostics;

// Get file names from directory and terms from terms.csv file
string basePath = @"C:\Studia\Inteligentne Systemy Decyzyjne\";
string[] filenames = Directory.GetFiles(basePath, "*.pdf");
string[] terms = File.ReadAllText($"{basePath}terms.csv").Split(',');

// Create dictionaries
Dictionary<int, Document> fileDictionary = new();
for (int i = 0; i < filenames.Length; i++)
{
    fileDictionary.Add(i, new Document(filenames[i], 0));
}

Dictionary<int, string> termDictionary = new();
for (int i = 0; i < terms.Length; i++)
{
    termDictionary.Add(i, terms[i]);
}

// List documents
Console.WriteLine("Document filenames:");
for (int i = 0; i < filenames.Length; i++)
    Console.WriteLine($"D{i+1} - {filenames[i]}");
Console.WriteLine();

// List terms
Console.WriteLine("Terms:");
for (int i = 0; i < terms.Length; i++)
    Console.WriteLine($"T{i + 1} - {terms[i]}");
Console.WriteLine();

// Calculate occurence matrix of given terms in the documents and list occurences for each document
Stopwatch sw = Stopwatch.StartNew();
OccurenceMatrix occurenceMatrix = PdfHandler.CountOccurenceMatrix(filenames, terms);

for (int i = 0; i < filenames.Length; i++)
{
    Console.WriteLine($"File: {filenames[i]}");
    for (int j = 0; j < terms.Length; j++)
    {
        Console.WriteLine($"{terms[j]}: {occurenceMatrix.Matrix[i,j]}");
    }
    Console.WriteLine();
}
sw.Stop();
Console.WriteLine($"Elapsed time: {sw.Elapsed.TotalSeconds}s");

occurenceMatrix.Print2("Occurence matrix:");
// Normalize
OccurenceMatrix normalizedOccurenceMatrix = occurenceMatrix.Normalize();

normalizedOccurenceMatrix.Print2("Normalized occurence matrix:");

// Calculate euclidean distance matrix, p=2, r=2
var euclidean = new EuclideanDistanceMatrix(normalizedOccurenceMatrix);
euclidean.Print2("Euclidean distance matrix (p = 2, r = 2):");

// Calculate euclidean distance matrix, p=2, r=3
var euclidean2 = new EuclideanDistanceMatrix(normalizedOccurenceMatrix, 2, 3);
euclidean2.Print2("Euclidean distance matrix (p = 2, r = 3):");

// Calculate euclidean distance matrix, p=3, r=3
var euclidean3 = new EuclideanDistanceMatrix(normalizedOccurenceMatrix, 3, 3);
euclidean3.Print2("Euclidean distance matrix (p = 3, r = 3):");

// Calculate clustering
int k = 3;  // number clusters
string initMethod = "plusplus";
int maxIter = 100;  // max (likely less)
int seed = 0;

KMeans km = new KMeans(k, normalizedOccurenceMatrix, initMethod, maxIter, seed);

int trials = 10;  // attempts to find best
Console.WriteLine("\nStarting clustering w/ trials = " +
  trials);
km.Cluster(trials);
Console.WriteLine("Done");

Console.WriteLine("\nBest clustering found: ");
ShowVector(km.clustering, 3);

Console.WriteLine("\nCluster counts: ");
ShowVector(km.counts, 4);

//Console.WriteLine("\nThe cluster means: ");
//ShowMatrix(km.means, new int[] { 4, 4 },
//  new int[] { 8, 8 }, true);

Console.WriteLine("\nTotal within-cluster SS = " +
  km.wcss.ToString("F4"));

Console.WriteLine("\nClustered data: ");
ShowClustered(occurenceMatrix.Matrix, k, km.clustering, new int[] { 2, 3 },
  new int[] { 8, 10 }, true);

// Write cluster assignments into file dictionary
for (int i = 0; i < fileDictionary.Count; i++)
{
    fileDictionary[i].Cluster = km.clustering[i];
}


Console.WriteLine("File list:");
foreach (var item in fileDictionary)
{
    Console.WriteLine($"D{item.Key + 1}\t-\t{item.Value.Name}");
}

int pick; // document choice by user
do
{
    Console.WriteLine("\nPick yer file, lad!");
    pick = int.Parse(Console.ReadLine());
    if (pick > 0 && pick <= fileDictionary.Count)
    {
        Random random = new Random();
        var matches = fileDictionary.Where(x => x.Value.Cluster == fileDictionary[pick - 1].Cluster && x.Key != pick-1);
        if (matches.Any())
        {
            var match = matches.ElementAt(random.Next(0, matches.Count()));
            Console.WriteLine("Aye, ye might want to read this next:");
            Console.WriteLine($"D{match.Key + 1}\t-\t{match.Value.Name}");
            Console.WriteLine($"'Tis in the same cluster: {fileDictionary[pick - 1].Cluster}");
        }
        else
        {
            Console.WriteLine("Sad times! No matching document found!");
        }
    }
    if (pick < 0 || pick > fileDictionary.Count)
    {
        Console.WriteLine("Arr! Document not found!");
    }
} while (pick != 0);

// Pick 2 random documents, mark one as "bad" and other as "good"
Random rnd = new();
int badIndex = rnd.Next(0, fileDictionary.Count);
int goodIndex;
do
{
    goodIndex = rnd.Next(0, fileDictionary.Count);
} while (badIndex == goodIndex);
var badFile = fileDictionary[badIndex];
var goodFile = fileDictionary[goodIndex];

// Add new document by user (specify frequency of each term
string specification = String.Empty;
string[] termFrequenciesString;
do
{
    Console.WriteLine($"Specify {terms.Length} term frequencies for new document, separated by spaces:");
    specification = Console.ReadLine();
    termFrequenciesString = specification.Split(" ");
} while (termFrequenciesString.Length != terms.Length);

// Create new occurence matrix with 3 documents.
OccurenceMatrix goodVsBadOccurenceMatrix = new(3, terms.Length);
for (int i = 0; i < terms.Length; i++)
{
    goodVsBadOccurenceMatrix.Matrix[0, i] = occurenceMatrix.Matrix[badIndex, i];
}
for (int i = 0; i < terms.Length; i++)
{
    goodVsBadOccurenceMatrix.Matrix[1, i] = occurenceMatrix.Matrix[goodIndex, i];
}
for (int i = 0; i < terms.Length; i++)
{
    goodVsBadOccurenceMatrix.Matrix[2, i] = double.Parse(termFrequenciesString[i]);
}

// Normalize
OccurenceMatrix normalizedGoodVsBad = goodVsBadOccurenceMatrix.Normalize();

// Get distances
var euclideanDistances = new EuclideanDistanceMatrix(normalizedGoodVsBad);

// Find which document is closer to the newly added
var closestIndex = euclideanDistances.FindClosest(2);
if (closestIndex == 0)
{
    Console.WriteLine($"Document closer to bad document {fileDictionary[badIndex].Name}");
}
else
{
    Console.WriteLine($"Document closer to good document {fileDictionary[goodIndex].Name}");
}



static void ShowVector(int[] vec, int wid)
{
    int n = vec.Length;
    for (int i = 0; i < n; ++i)
        Console.Write(vec[i].ToString().PadLeft(wid));
    Console.WriteLine("");
}

static void ShowMatrix(double[][] m, int[] dec,
  int[] wid, bool indices)
{
    int n = m.Length;
    int iPad = n.ToString().Length + 1;

    for (int i = 0; i < n; ++i)
    {
        if (indices == true)
            Console.Write("[" +
              i.ToString().PadLeft(iPad) + "] ");
        for (int j = 0; j < m[0].Length; ++j)
        {
            double x = m[i][j];
            if (Math.Abs(x) < 1.0e-5) x = 0.0;
            Console.Write(x.ToString("F" +
              dec[j]).PadLeft(wid[j]));
        }
        Console.WriteLine("");
    }
}

static void ShowClustered(double[,] data, int K,
  int[] clustering, int[] decs, int[] wids, bool indices)
{
    int n = data.GetLength(0);
    int iPad = n.ToString().Length + 1;  // indices
    int numDash = 0;
    for (int i = 0; i < wids.Length; ++i)
        numDash += wids[i];
    for (int i = 0; i < numDash + iPad + 5; ++i)
        Console.Write("-");
    Console.WriteLine("");
    for (int k = 0; k < K; ++k) // display by cluster
    {
        for (int i = 0; i < n; ++i) // each data item
        {
            if (clustering[i] == k) // curr data is in cluster k
            {
                if (indices == true)
                    Console.Write("[" +
                      (i+1).ToString().PadLeft(iPad) + "]   ");
                for (int j = 0; j < data.GetLength(1); ++j)
                {
                    double x = data[i,j];
                    if (x < 1.0e-5) x = 0.0;  // prevent "-0.00"
                    string s = x.ToString("F2");
                    Console.Write(s.PadLeft(8));
                }
                Console.WriteLine("");
            }
        }
        for (int i = 0; i < numDash + iPad + 5; ++i)
            Console.Write("-");
        Console.WriteLine("");
    }
} // ShowClustered()