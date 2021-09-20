using UnityEngine;
using System.Linq;

[ExecuteInEditMode]
public class IKVis : MonoBehaviour
{

	public Transform[] pointsT;
	public Transform target;
	public float radius;
	public float lineThickness;

	IKSolver solver;
	Vector3[] originalPoints;

	void Start()
	{
		solver = new IKSolver();
	}

	void Update()
	{
		Vector3[] points = pointsT.Select((p) => p.position).ToArray();

		if (Application.isPlaying)
		{
			solver.Solve(points, target.position);
		}

		for (int i = 0; i < points.Length; i++)
		{
			Visualizer.SetColour(Color.white);
			Visualizer.DrawSphere(points[i], radius);
			if (i < points.Length - 1)
			{
				Visualizer.SetColour(Color.white);
				Visualizer.DrawLine(points[i], points[i + 1], lineThickness);
			}
		}

		Visualizer.SetColour(Color.red);
		Visualizer.DrawSphere(target.position + Vector3.forward * 2, radius * 1.5f);

		for (int i = 0; i < pointsT.Length; i++)
		{
			pointsT[i].position = points[i];
		}
	}
}
