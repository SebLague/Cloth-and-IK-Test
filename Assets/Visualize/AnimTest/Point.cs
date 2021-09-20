using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Visualization
{
	public class Point : MonoBehaviour
	{

		public Anim moveAnim;
		public Anim colAnim;

		Material material;
		Color targetCol;
		public bool debug;


		void Start()
		{
			//moveAnim = new Anim(null, 0);
			//moveAnim.done = true;

			material = GetComponent<MeshRenderer>().material;
		}

		void Update()
		{

			moveAnim?.Update(Time.deltaTime);
			colAnim?.Update(Time.deltaTime);



			//material.color = Color.Lerp(material.color, targetCol, Time.deltaTime * 5);
		}

		public Anim MoveTo(Vector2 targetPos, float duration)
		{
			Vector3 initialPos = transform.position;
			System.Action<float> action = (t) => transform.position = Vector3.Lerp(initialPos, targetPos, t);
			moveAnim = new Anim(action, duration);
			moveAnim.ease = Ease.Cubic.InOut;
			return moveAnim;
		}

		public Anim ChangeColour(Color col, float duration)
		{

			Color initialC = material.color;
			var r = ((Vector4)col - (Vector4)targetCol).sqrMagnitude;
			if (r > 0.001f)
			{
				targetCol = col;
				if (debug)
				{
					//Debug.Log("Set col : " +)
				}
				System.Action<float> action = (t) => Test(initialC, col, t);
				colAnim = new Anim(action, duration);
				colAnim.ease = Ease.Cubic.InOut;
			}
			return colAnim;

		}

		void Test(Color initialC, Color col, float t)
		{
			if (debug)
			{
				Debug.Log(t);
			}

			material.color = Color.Lerp(initialC, col, t);
		}

		public Vector3 position
		{
			get
			{
				return transform.position;
			}
			set
			{
				transform.position = value;
			}
		}



		public static Point CreateShape(Vector2 position, float radius, Color color)
		{
			GameObject gameObject = new GameObject("Point");
			gameObject.transform.position = position;
			gameObject.transform.localScale = Vector3.one * radius;
			gameObject.transform.eulerAngles = new Vector3(-90, 0, 0);
			Point p = gameObject.AddComponent<Point>();
			MeshRenderer renderer = gameObject.AddComponent<MeshRenderer>();
			MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();

			renderer.material = new Material(Shader.Find("Unlit/Color"));
			renderer.material.color = color;
			Mesh mesh = new Mesh();
			meshFilter.mesh = mesh;
			Visualization.MeshGeneration.ArcMesh.GenerateMesh(mesh, 360);

			return p;
		}
	}

	[System.Serializable]
	public class Anim
	{
		public float t;
		float duration;
		System.Action<float> action;
		public System.Func<float, float> ease;
		public bool done;

		public Anim(System.Action<float> action, float duration)
		{
			ease = Ease.Linear.Ease;
			this.duration = duration;
			this.action = action;
		}

		public void Update(float deltaTime)
		{
			if (!done)
			{
				t += deltaTime / duration;
				float easedT = ease.Invoke(t);
				action.Invoke(easedT);
				done = t >= 1;
			}
		}
	}
}
