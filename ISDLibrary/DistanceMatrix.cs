using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISDLibrary;
public abstract class DistanceMatrix
{
	public OccurenceMatrix OccurenceMatrix;
	public DistanceMatrix(OccurenceMatrix occurenceMatrix)
	{
		OccurenceMatrix = occurenceMatrix;
	}

	public abstract double[,] GetDistanceMatrix();
    //{
    //	return new double[OccurenceMatrix.Length];
    //}

    public abstract double GetDistance(int first, int second);
	//{
	//	return 1;
	//}
}
