namespace Cuku.Utilities
{
    using UnityEngine;
#if DREAMTECK_SPLINES
    using Dreamteck.Splines;
#endif
    using System.Threading;
	using System.Diagnostics;
	using System.IO;
	using System.Collections.Generic;
	using System.Linq;

	public static class Utilities
    {
#region Point
        public static Vector2[] ProjectToXZPlane(this Vector3[] points3D)
        {
            var points = new Vector2[points3D.Length];
            for (int i = 0; i < points.Length; i++)
            {
                points[i] = new Vector2(points3D[i].x, points3D[i].z);
            }
            return points;
        }

        public static Vector3[] ProjectToTerrain(this Vector3[] points)
        {
            for (int p = 0; p < points.Length; p++)
            {
                points[p] = points[p].ProjectToTerrain();
            }

            return points;
        }

        public static Vector3 ProjectToTerrain(this Vector3 point)
        {
            point.y = point.GetHitTerrainHeight();

            return point;
        }

#if DREAMTECK_SPLINES
        public static SplineComputer CreateCurve(this Vector3[] points, int curveSampleRate = 4, bool XZPlane = false)
        {
            SplinePoint[] splinePoints = new SplinePoint[points.Length];
            for (int i = 0; i < splinePoints.Length; i++)
            {
                var point = points[i];
                if (XZPlane) point.y = 0;
                splinePoints[i] = new SplinePoint(point);
            }

            var curve = new GameObject("SP").AddComponent<SplineComputer>();
            curve.type = Spline.Type.Bezier;
            curve.sampleMode = SplineComputer.SampleMode.Uniform;
            curve.sampleRate = curveSampleRate;
            curve.SetPoints(splinePoints);

            return curve;
        }
#endif

        /// <summary>
        /// Gets the coordinates of the intersection point of two lines.
        /// </summary>
        /// <param name="A1">A point on the first line.</param>
        /// <param name="A2">Another point on the first line.</param>
        /// <param name="B1">A point on the second line.</param>
        /// <param name="B2">Another point on the second line.</param>
        /// <param name="found">Is set to false of there are no solution. true otherwise.</param>
        /// <returns>The intersection point coordinates. Returns Vector2.zero if there is no solution.</returns>
        public static Vector2 GetLinesIntersectionPoint(Vector2 A1, Vector2 A2, Vector2 B1, Vector2 B2, out bool found)
        {
            float tmp = (B2.x - B1.x) * (A2.y - A1.y) - (B2.y - B1.y) * (A2.x - A1.x);

            if (tmp == 0)
            {
                // No solution!
                found = false;
                return Vector2.zero;
            }

            float mu = ((A1.x - B1.x) * (A2.y - A1.y) - (A1.y - B1.y) * (A2.x - A1.x)) / tmp;

            found = true;

            var intersectionPoint = new Vector2(
                B1.x + (B2.x - B1.x) * mu,
                B1.y + (B2.y - B1.y) * mu
            );

            return intersectionPoint;
        }

        public static bool IsInside(this Vector2 point, Vector2[] points)
        {
            var j = points.Length - 1;
            var inside = false;
            for (int i = 0; i < points.Length; j = i++)
            {
                var pi = points[i];
                var pj = points[j];
                if (((pi.y <= point.y && point.y < pj.y) || (pj.y <= point.y && point.y < pi.y)) &&
                    (point.x < (pj.x - pi.x) * (point.y - pi.y) / (pj.y - pi.y) + pi.x))
                    inside = !inside;
            }
            return inside;
        }
 
        public static Vector3[] AddTileIntersectionPoints(this Vector3[] points)
        {
            var allPoints = new List<Vector3>();
            var intersectionPointIds = new List<int>();
            for (int i = 1; i < points.Length; i++)
            {
                var point1 = points[i - 1];
                var point2 = points[i];

                if (point1.GetHitTerrainName() != point2.GetHitTerrainName())
                {
                    var intersectionPoint = point1.GetTerrainTileIntersectionPoint(point2);
                    allPoints.Add(intersectionPoint);
                    intersectionPointIds.Add(allPoints.Count - 1);
                }

                allPoints.Add(point2);
            }

            // Shift points to match the start of a tile
            var shiftedPoints = new Vector3[allPoints.Count + 1];
            for (int i = 0; i < intersectionPointIds.Count; i++)
            {
                if (allPoints[intersectionPointIds[i]].GetHitTerrainName() == allPoints[intersectionPointIds[i + 1]].GetHitTerrainName()) continue;

                var startId = intersectionPointIds[i + 1];

                for (int j = startId; j < allPoints.Count; j++)
                {
                    shiftedPoints[j - startId] = allPoints[j];
                }
                for (int j = 0; j < startId; j++)
                {
                    shiftedPoints[(allPoints.Count - startId) + j] = allPoints[j];
                }
                shiftedPoints[shiftedPoints.Length - 1] = shiftedPoints[0];

                break;
            }

            return shiftedPoints;
        }

        private static Vector3 GetTerrainTileIntersectionPoint(this Vector3 point1, Vector3 point2)
        {
            var terrainAnglePoints = point1.GetHitTerrain().GetTerrainAnglePoints();

            var A1 = new Vector2(point1.x, point1.z);
            var A2 = new Vector2(point2.x, point2.z);

            bool found;
            Vector2[] intersections = new Vector2[4];
            intersections[0] = GetLinesIntersectionPoint(A1, A2, terrainAnglePoints[0], terrainAnglePoints[1], out found);
            intersections[1] = GetLinesIntersectionPoint(A1, A2, terrainAnglePoints[0], terrainAnglePoints[2], out found);
            intersections[2] = GetLinesIntersectionPoint(A1, A2, terrainAnglePoints[1], terrainAnglePoints[3], out found);
            intersections[3] = GetLinesIntersectionPoint(A1, A2, terrainAnglePoints[2], terrainAnglePoints[3], out found);

            var closest = 0;
            for (int i = 1; i < intersections.Length; i++)
            {
                if (Vector3.Distance(A1, intersections[i]) < Vector3.Distance(A1, intersections[closest]))
                {
                    closest = i;
                }
            }

            return intersections[closest].GetHitTerrainPosition();
        }
#endregion

#region Terrain
        public static Terrain GetHitTerrain(this Vector3 position)
        {
            return position.GetTerrainRaycastHit().transform.GetComponent<Terrain>();
        }

        public static string GetHitTerrainName(this Vector3 position)
        {
            return position.GetTerrainRaycastHit().transform.name;
        }

        public static Vector3 GetHitTerrainPosition(this Vector2 position)
        {
            Vector3 hitPosition = new Vector3(position.x, 0, position.y);
            hitPosition.y = hitPosition.GetTerrainRaycastHit().point.y;

            return hitPosition;
        }

        public static float GetHitTerrainHeight(this Vector3 position)
        {
            return position.GetTerrainRaycastHit().point.y;
        }

        public static Vector2[] GetTerrainAnglePoints(this Terrain terrain)
        {
            var tileResolution = terrain.terrainData.heightmapResolution;

            var tp1 = new Vector2(terrain.GetPosition().x, terrain.GetPosition().z);
            var tp2 = new Vector2(tp1.x + tileResolution, tp1.y);
            var tp3 = new Vector2(tp1.x, tp1.y + tileResolution);
            var tp4 = new Vector2(tp1.x + tileResolution, tp1.y + tileResolution);

            return new Vector2[] { tp1, tp2, tp3, tp4 };
        }

        static RaycastHit GetTerrainRaycastHit(this Vector3 origin)
        {
            origin.y = 10000;

            Ray ray = new Ray(origin, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.GetComponent<Terrain>())
                {
                    return hit;
                }
            }

            return hit;
        }

        public static Dictionary<Terrain, Vector3[]> GetHitTerrainsAndBoundaryPoints(this Vector3[] boundaryPoints, int curveOffset)
        {
            Dictionary<Terrain, int[]> terrainLimitsIds = new Dictionary<Terrain, int[]>();
            for (int i = 0; i < boundaryPoints.Length - 1; i++)
            {
                var terrain = boundaryPoints[i].GetHitTerrain();
                if (!terrainLimitsIds.ContainsKey(terrain))
                {
                    terrainLimitsIds.Add(terrain, new int[] { i, i });
                    continue;
                };

                var limits = terrainLimitsIds[terrain];
                limits[0] = Mathf.Min(limits[0], i);
                limits[1] = Mathf.Max(limits[1], i);

                terrainLimitsIds[terrain] = limits;
            }

            Dictionary<Terrain, Vector3[]> terrains = new Dictionary<Terrain, Vector3[]>();
            foreach (var terrain in terrainLimitsIds)
            {
                var limits = terrain.Value;
                var startId = limits[0] - curveOffset;
                var endId = limits[1] + curveOffset;

                var points = new List<Vector3>();
                if (limits[0] == 0) points.AddRange(boundaryPoints.Skip(boundaryPoints.Count() - curveOffset));
                points.AddRange(boundaryPoints.Skip(startId).Take(endId - startId));

                terrains.Add(terrain.Key, points.ToArray());
            }

            return terrains;
        }
#endregion

#region Process
        public static void ExecutePowerShellCommand(this string arguments, bool wait = false, bool admin = false)
        {
            UnityEngine.Debug.Log(arguments);

            var psArguments = "-Command " + arguments;

            if (wait)
            {
                ExecuteCommand("powershell.exe", psArguments, wait, admin);
            }
            else
            {
                "powershell.exe".ExecuteCommandThread(psArguments, wait, admin);
            }
        }

        private static void ExecuteCommandThread(this string fileName, string arguments, bool wait = false, bool admin = false)
        {
            var thread = new Thread(delegate () { ExecuteCommand(fileName, arguments, wait, admin); });
            thread.Start();
        }

        private static void ExecuteCommand(string fileName, string arguments, bool wait = false, bool admin = false)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(fileName, arguments);
            if (admin) startInfo.Verb = "runas";

            var process = Process.Start(startInfo);

            if (wait)
            {
                process.WaitForExit();
            }
            process.Close();
        }
#endregion

#region Path
        public static string GetPathInStreamingAssets(this string path)
		{
            return Path.Combine(Application.streamingAssetsPath, path);
		}
#endregion
	}
}
