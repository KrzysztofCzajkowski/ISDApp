namespace ISDLibrary;
public abstract class DistanceMatrix
{
	public OccurenceMatrix OccurenceMatrix;
	public DistanceMatrix(OccurenceMatrix occurenceMatrix)
	{
		OccurenceMatrix = occurenceMatrix;
	}

	/// <summary>
	/// Method for finding closest document index to a given index.
	/// </summary>
	/// <param name="index"></param>
	/// <returns></returns>
	public abstract int FindClosest(int index);
	/// <summary>
	/// Method for returning a document distance 2D matrix, based on the OccurenceMatrix.
	/// </summary>
	/// <returns></returns>
	public abstract double[,] GetDistanceMatrix();
	/// <summary>
	/// Method for returning distance between 2 documents by index.
	/// </summary>
	/// <param name="first"></param>
	/// <param name="second"></param>
	/// <returns></returns>
    public abstract double GetDistance(int first, int second);
}
