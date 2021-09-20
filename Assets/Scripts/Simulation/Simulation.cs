using System.Collections.Generic;
using UnityEngine;

public class Simulation : MonoBehaviour
{
	public float pointRadius;
	public float stickThickness;
	bool drawingStick;
	int stickStartIndex;

	public Color pointCol;
	public Color fixedPointCol;
	public Color lineCol;
	public Color lineDrawCol;
	public bool autoStickMode;
	public bool constrainStickMinLength = true;
	public bool simulating;

	public float gravity = 10;
	public int solveIterations = 5;
	protected List<Point> points;
	protected List<Stick> sticks;
	int[] order;

	protected virtual void Start()
	{

		if (points == null)
		{
			points = new List<Point>();
		}
		if (sticks == null)
		{
			sticks = new List<Stick>();
		}

		CreateOrderArray();
	}

	void Simulate()
	{
		foreach (Point p in points)
		{
			if (!p.locked)
			{
				Vector2 positionBeforeUpdate = p.position;
				p.position += p.position - p.prevPosition;
				p.position += Vector2.down * gravity * Time.deltaTime * Time.deltaTime;
				p.prevPosition = positionBeforeUpdate;
			}
		}

		for (int i = 0; i < solveIterations; i++)
		{
			for (int s = 0; s < sticks.Count; s++)
			{
				Stick stick = sticks[order[s]];
				if (stick.dead)
				{
					continue;
				}

				Vector2 stickCentre = (stick.pointA.position + stick.pointB.position) / 2;
				Vector2 stickDir = (stick.pointA.position - stick.pointB.position).normalized;
				float length = (stick.pointA.position - stick.pointB.position).magnitude;

				if (length > stick.length || constrainStickMinLength)
				{
					if (!stick.pointA.locked)
					{
						stick.pointA.position = stickCentre + stickDir * stick.length / 2;
					}
					if (!stick.pointB.locked)
					{
						stick.pointB.position = stickCentre - stickDir * stick.length / 2;
					}
				}

			}
		}
	}
	[System.Serializable]
	public class Point
	{
		public Vector2 position, prevPosition;
		public bool locked;
	}

	[System.Serializable]
	public class Stick
	{
		public Point pointA, pointB;
		public float length;
		public bool dead;

		public Stick(Point pointA, Point pointB)
		{
			this.pointA = pointA;
			this.pointB = pointB;
			length = Vector2.Distance(pointA.position, pointB.position);
		}
	}


	protected virtual void HandleInput(Vector2 mousePos)
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			simulating = !simulating;
		}
		if (simulating)
		{
		}
		else
		{

			int i = MouseOverPointIndex(mousePos);
			bool mouseOverPoint = i != -1;

			if (Input.GetMouseButtonDown(1) && mouseOverPoint)
			{
				points[i].locked = !points[i].locked;
			}

			if (Input.GetMouseButtonDown(0))
			{
				if (mouseOverPoint)
				{
					drawingStick = true;
					stickStartIndex = i;
				}
				else
				{
					points.Add(new Point() { position = mousePos, prevPosition = mousePos });
				}
			}

			if (Input.GetMouseButtonUp(0))
			{
				if (mouseOverPoint && drawingStick)
				{
					if (i != stickStartIndex)
					{
						sticks.Add(new Stick(points[stickStartIndex], points[i]));
						CreateOrderArray();
					}
				}
				drawingStick = false;
			}
			if (autoStickMode)
			{
				sticks.Clear();
				for (int k = 0; k < points.Count - 1; k++)
				{
					sticks.Add(new Stick(points[k], points[k + 1]));
					CreateOrderArray();
				}
			}
		}
	}

	int MouseOverPointIndex(Vector2 mousePos)
	{
		for (int i = 0; i < points.Count; i++)
		{
			float dst = (points[i].position - mousePos).magnitude;

			if (dst < pointRadius)
			{
				return i;
			}
		}
		return -1;
	}

	void Draw()
	{

		foreach (Point point in points)
		{
			Visualizer.SetColour((point.locked) ? fixedPointCol : pointCol);
			Visualizer.DrawSphere(point.position, pointRadius);
		}

		Visualizer.SetColour(lineCol);
		foreach (Stick stick in sticks)
		{
			if (!stick.dead)
				Visualizer.DrawLine(stick.pointA.position, stick.pointB.position, stickThickness);
		}

		if (drawingStick)
		{
			Visualizer.SetColour(lineDrawCol);
			Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Visualizer.DrawLine(points[stickStartIndex].position, mousePos, stickThickness);
		}
	}


	void Update()
	{
		HandleInput(Camera.main.ScreenToWorldPoint(Input.mousePosition));
		if (simulating)
		{
			Simulate();
		}
	}

	void LateUpdate()
	{
		Draw();
	}

	public static T[] ShuffleArray<T>(T[] array, System.Random prng)
	{

		int elementsRemainingToShuffle = array.Length;
		int randomIndex = 0;

		while (elementsRemainingToShuffle > 1)
		{

			// Choose a random element from array
			randomIndex = prng.Next(0, elementsRemainingToShuffle);
			T chosenElement = array[randomIndex];

			// Swap the randomly chosen element with the last unshuffled element in the array
			elementsRemainingToShuffle--;
			array[randomIndex] = array[elementsRemainingToShuffle];
			array[elementsRemainingToShuffle] = chosenElement;
		}

		return array;
	}

	protected void CreateOrderArray()
	{
		order = new int[sticks.Count];
		for (int i = 0; i < order.Length; i++)
		{
			order[i] = i;
		}
		ShuffleArray(order, new System.Random());
	}

	void OnDrawGizmos()
	{
		//Gizmos.DrawWireCube(Vector3.zero, boundsSize);
	}
}
