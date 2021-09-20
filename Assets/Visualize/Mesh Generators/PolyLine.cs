using UnityEngine;

namespace Visualization.MeshGeneration
{
	public static class PolyLine
	{

		// TODO: breaks down when line thickness causes a line segment to overlap with another segment (other than prev or next segment)

		public static void CreateLine(Vector2[] points, float thickness, ref Mesh mesh, float depth = 0)
		{
			float halfThickness = thickness / 2;

			LineRay[] rays = new LineRay[points.Length - 1];
			for (int i = 0; i < points.Length - 1; i++)
			{
				rays[i] = new LineRay(points[i], points[i + 1], halfThickness);
			}

			// Calculate points
			for (int i = 0; i < rays.Length - 1; i++)
			{
				LineRay ray = rays[i];
				LineRay nextRay = rays[i + 1];

				float dirDot = Vector2.Dot(ray.dir, nextRay.dir);

				if (dirDot != 1)
				{
					rays[i + 1].a1 = MathUtility.PointOfLineLineIntersection(ray.a1, ray.a1 + ray.dir, nextRay.a1, nextRay.a1 + nextRay.dir);
					rays[i + 1].a2 = MathUtility.PointOfLineLineIntersection(ray.a2, ray.a2 + ray.dir, nextRay.a2, nextRay.a2 + nextRay.dir);
				}
			}

			// Create mesh
			int numVerts = points.Length * 2;
			int numTris = (points.Length - 1) * 2;

			Vector3[] vertices = new Vector3[numVerts];
			int[] triangles = new int[numTris * 3];

			int triIndex = 0;


			for (int i = 0; i < rays.Length; i++)
			{
				LineRay ray = rays[i];

				Vector2 pointA = ray.a1;
				Vector2 pointB = ray.a2;

				vertices[i * 2] = (Vector3)(pointA) + Vector3.forward * depth;
				vertices[i * 2 + 1] = (Vector3)(pointB) + Vector3.forward * depth;

				if (i == rays.Length - 1)
				{
					vertices[i * 2 + 2] = (Vector3)(ray.target + ray.normal * halfThickness) + Vector3.forward * depth;
					vertices[i * 2 + 3] = (Vector3)(ray.target - ray.normal * halfThickness) + Vector3.forward * depth;
				}

				int currA = i * 2;
				int currB = i * 2 + 1;
				int nextA = (i + 1) * 2;
				int nextB = (i + 1) * 2 + 1;

				triangles[triIndex] = currA;
				triangles[triIndex + 1] = nextB;
				triangles[triIndex + 2] = currB;

				triangles[triIndex + 3] = currA;
				triangles[triIndex + 4] = nextA;
				triangles[triIndex + 5] = nextB;
				triIndex += 6;
			}


			mesh.vertices = vertices;
			mesh.triangles = triangles;
		}

		public struct LineRay
		{
			public Vector2 dir;
			public Vector2 normal;

			public Vector2 origin;
			public Vector2 target;
			public Vector2 a1;
			public Vector2 a2;

			public LineRay(Vector2 origin, Vector2 target, float halfThickness)
			{
				this.origin = origin;
				this.target = target;
				dir = (target - origin).normalized;
				normal = new Vector2(-dir.y, dir.x);
				a1 = origin + normal * halfThickness;
				a2 = origin - normal * halfThickness;
			}

		}
	}
}