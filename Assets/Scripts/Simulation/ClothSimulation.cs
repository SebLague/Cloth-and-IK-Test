using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothSimulation : Simulation
{
	[Header("Cloth Settings")]
	public Vector2Int numPoints;
	public Vector2 boundsSize;
	Vector2 cutPosOld;

	protected override void Start()
	{
		base.Start();

		for (int y = 0; y < numPoints.y; y++)
		{
			float ty = y / (numPoints.y - 1f);
			for (int x = 0; x < numPoints.x; x++)
			{
				bool locked = y == 0 && x % 5 == 0;
				float tx = x / (numPoints.x - 1f);
				int i = IndexFrom2DCoord(x, y);
				Vector2 position = new Vector2((tx - 0.5f) * boundsSize.x * 0.7f, (0.5f - ty) * boundsSize.y * 0.7f);
				points.Add(new Point() { position = position, prevPosition = position, locked = locked });
			}
		}

		float length = (points[0].position - points[1].position).magnitude;

		for (int y = 0; y < numPoints.y; y++)
		{
			for (int x = 0; x < numPoints.x; x++)
			{
				if (x < numPoints.x - 1)
				{
					sticks.Add(new Stick(points[IndexFrom2DCoord(x, y)], points[IndexFrom2DCoord(x + 1, y)]));
				}
				if (y < numPoints.y - 1)
				{
					sticks.Add(new Stick(points[IndexFrom2DCoord(x, y)], points[IndexFrom2DCoord(x, y + 1)]));
				}
			}
		}

		CreateOrderArray();
	}

	protected override void HandleInput(Vector2 mousePos)
	{
		base.HandleInput(mousePos);

		if (simulating)
		{
			if (Input.GetMouseButtonDown(1))
			{
				cutPosOld = mousePos;
			}
			if (Input.GetMouseButton(1))
			{
				Cut(cutPosOld, mousePos);
				cutPosOld = mousePos;
			}
		}
	}


	int IndexFrom2DCoord(int x, int y)
	{
		return y * numPoints.x + x;
	}

	void Cut(Vector2 start, Vector2 end)
	{
		for (int i = sticks.Count - 1; i >= 0; i--)
		{
			if (LineSegmentsIntersect(start, end, sticks[i].pointA.position, sticks[i].pointB.position))
			{
				sticks[i].dead = true;
			}
		}
	}

	public static bool LineSegmentsIntersect(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
	{
		float d = (b2.x - b1.x) * (a1.y - a2.y) - (a1.x - a2.x) * (b2.y - b1.y);
		if (d == 0)
			return false;
		float t = ((b1.y - b2.y) * (a1.x - b1.x) + (b2.x - b1.x) * (a1.y - b1.y)) / d;
		float u = ((a1.y - a2.y) * (a1.x - b1.x) + (a2.x - a1.x) * (a1.y - b1.y)) / d;

		return t >= 0 && t <= 1 && u >= 0 && u <= 1;
	}
}
