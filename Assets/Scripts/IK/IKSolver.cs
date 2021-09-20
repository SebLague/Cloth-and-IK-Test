using UnityEngine;

public class IKSolver {

	const int maxIterations = 100;
	const float minAcceptableDst = 0.01f;

	public void Solve(Vector3[] points, Vector3 target) {
		Vector3 origin = points[0];
		float[] segmentLengths = new float[points.Length - 1];
		for (int i = 0; i < points.Length - 1; i++) {
			segmentLengths[i] = (points[i + 1] - points[i]).magnitude;
		}

		for (int iteration = 0; iteration < maxIterations; iteration ++) {
			bool startingFromTarget = iteration % 2 == 0;
			// Reverse arrays to alternate between forward and backward passes
			System.Array.Reverse(points);
			System.Array.Reverse(segmentLengths);
			points[0] = (startingFromTarget) ? target : origin;

			// Constrain lengths
			for (int i = 1; i < points.Length; i++) {
				Vector3 dir = (points[i] - points[i-1]).normalized;
				points[i] = points[i-1] + dir * segmentLengths[i-1];
			}

			// Terminate if close enough to target
			float dstToTarget = (points[points.Length - 1] - target).magnitude;
			if (!startingFromTarget && dstToTarget <= minAcceptableDst) {
				return;
			}
		}
	}
}