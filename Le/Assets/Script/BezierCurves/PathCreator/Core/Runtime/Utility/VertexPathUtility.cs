using System.Collections.Generic;
using UnityEngine;

namespace PathCreation.Utility
{
    public static class VertexPathUtility
    {
        public static PathSplitData SplitBezierPathByAngleError(BezierPath bezierPath, float maxAngleError,
            float minVertexDst, float accuracy)
        {
            // Debug.Log("---------"+ (180 - MathUtility.MinAngle(a, b, c)));
            // Debug.LogError("splitBezierPathByAngleError" + bezierPath.NumPoints + "/" + maxAngleError + "/" +
            // minVertexDst + "/" + accuracy+"/"+bezierPath.NumSegments);

            // var a1 = new Vector3(2.877f, -1.852f, 0f);
            // var c1 = new Vector3(1.575f, -1.852f, 0.049f);
            // var c2 = new Vector3(-3.421f, 0f, -0.063f);
            // var a2 = new Vector3(-4.263f, 0, 0.037f);
            // var tpos = CubicBezierUtility.EvaluateCurveDerivative(a1, c1, c2, a2, 0);
            // Debug.LogError("-00000000000000000000000===========" + tpos);
            PathSplitData splitData = new PathSplitData();

            splitData.vertices.Add(bezierPath[0]);
            splitData.tangents.Add(CubicBezierUtility.EvaluateCurveDerivative(bezierPath.GetPointsInSegment(0), 0)
                .normalized);
            splitData.cumulativeLength.Add(0);
            splitData.anchorVertexMap.Add(0);
            splitData.minMax.AddValue(bezierPath[0]);

            Vector3 prevPointOnPath = bezierPath[0];
            Vector3 lastAddedPoint = bezierPath[0];

            float currentPathLength = 0;
            float dstSinceLastVertex = 0;

            // Go through all segments and split up into vertices
            for (int segmentIndex = 0; segmentIndex < bezierPath.NumSegments; segmentIndex++)
            {
                Vector3[] segmentPoints = bezierPath.GetPointsInSegment(segmentIndex);

                float estimatedSegmentLength = CubicBezierUtility.EstimateCurveLength(segmentPoints[0],
                    segmentPoints[1], segmentPoints[2], segmentPoints[3]);

                int divisions = Mathf.CeilToInt(estimatedSegmentLength * accuracy);
                float increment = 1f / divisions;
                var num = 0;
                // Debug.LogError("segmentIndex:" + segmentIndex + "/" + estimatedSegmentLength+"/"+divisions+"/"+increment);
                for (float t = increment; t <= 1; t += increment)
                {
                    bool isLastPointOnPath = (t + increment > 1 && segmentIndex == bezierPath.NumSegments - 1);
                    if (isLastPointOnPath)
                    {
                        t = 1;
                    }

                    Vector3 pointOnPath = CubicBezierUtility.EvaluateCurve(segmentPoints, t);
                    Vector3 nextPointOnPath = CubicBezierUtility.EvaluateCurve(segmentPoints, t + increment);
                    // Debug.LogError("num:" + num + "/p:=" + pointOnPath);
                    num++;
                    // angle at current point on path
                    float localAngle = 180 - MathUtility.MinAngle(prevPointOnPath, pointOnPath, nextPointOnPath);
                    // angle between the last added vertex, the current point on the path, and the next point on the path
                    float angleFromPrevVertex =
                        180 - MathUtility.MinAngle(lastAddedPoint, pointOnPath, nextPointOnPath);
                    float angleError = Mathf.Max(localAngle, angleFromPrevVertex);
                    // Debug.LogWarning("add:"+num+"/prevPointOnPath="+prevPointOnPath
                    //                  +"/pointOnPath="+pointOnPath
                    //                  +"/nextPointOnPath="+nextPointOnPath);

                    // Debug.LogWarning("add:"+num+"/angleError="+angleError
                    //                  +"/localAngle="+localAngle
                    //                  +"/angleFromPrevVertex="+angleFromPrevVertex);
                    // Debug.LogWarning("add:"+num+"/angleError="+angleError+"/maxAngleError="
                    //                  +maxAngleError+"/dstSinceLastVertex="+dstSinceLastVertex
                    //                  +"/minVertexDst="+minVertexDst
                    //                  +"/isLastPointOnPath="+isLastPointOnPath);
                    if ((angleError > maxAngleError && dstSinceLastVertex >= minVertexDst) || isLastPointOnPath)
                    {
                        currentPathLength += (lastAddedPoint - pointOnPath).magnitude;
                        splitData.cumulativeLength.Add(currentPathLength);
                        splitData.vertices.Add(pointOnPath);
                        // Debug.LogWarning("add:"+num);
                        splitData.tangents.Add(CubicBezierUtility.EvaluateCurveDerivative(segmentPoints, t).normalized);
                        splitData.minMax.AddValue(pointOnPath);
                        dstSinceLastVertex = 0;
                        lastAddedPoint = pointOnPath;
                    }
                    else
                    {
                        dstSinceLastVertex += (pointOnPath - prevPointOnPath).magnitude;
                    }

                    prevPointOnPath = pointOnPath;
                }

                splitData.anchorVertexMap.Add(splitData.vertices.Count - 1);
            }

            // Debug.LogError("---------:"+splitData.vertices.Count);
            //
            // for (int i = 0; i < splitData.tangents.Count; i++)
            // {
            //     Debug.LogError("---------i:" + i + "/" + JsonUtility.ToJson(splitData.tangents[i]));
            // }

            return splitData;
        }

        public static PathSplitData SplitBezierPathEvenly(BezierPath bezierPath, float spacing, float accuracy)
        {
            PathSplitData splitData = new PathSplitData();

            splitData.vertices.Add(bezierPath[0]);
            splitData.tangents.Add(CubicBezierUtility.EvaluateCurveDerivative(bezierPath.GetPointsInSegment(0), 0)
                .normalized);
            splitData.cumulativeLength.Add(0);
            splitData.anchorVertexMap.Add(0);
            splitData.minMax.AddValue(bezierPath[0]);

            Vector3 prevPointOnPath = bezierPath[0];
            Vector3 lastAddedPoint = bezierPath[0];

            float currentPathLength = 0;
            float dstSinceLastVertex = 0;

            // Go through all segments and split up into vertices
            for (int segmentIndex = 0; segmentIndex < bezierPath.NumSegments; segmentIndex++)
            {
                Vector3[] segmentPoints = bezierPath.GetPointsInSegment(segmentIndex);
                float estimatedSegmentLength = CubicBezierUtility.EstimateCurveLength(segmentPoints[0],
                    segmentPoints[1], segmentPoints[2], segmentPoints[3]);
                int divisions = Mathf.CeilToInt(estimatedSegmentLength * accuracy);
                float increment = 1f / divisions;

                for (float t = increment; t <= 1; t += increment)
                {
                    bool isLastPointOnPath = (t + increment > 1 && segmentIndex == bezierPath.NumSegments - 1);
                    if (isLastPointOnPath)
                    {
                        t = 1;
                    }

                    Vector3 pointOnPath = CubicBezierUtility.EvaluateCurve(segmentPoints, t);
                    dstSinceLastVertex += (pointOnPath - prevPointOnPath).magnitude;

                    // If vertices are now too far apart, go back by amount we overshot by
                    if (dstSinceLastVertex > spacing)
                    {
                        float overshootDst = dstSinceLastVertex - spacing;
                        pointOnPath += (prevPointOnPath - pointOnPath).normalized * overshootDst;
                        t -= increment;
                    }

                    if (dstSinceLastVertex >= spacing || isLastPointOnPath)
                    {
                        currentPathLength += (lastAddedPoint - pointOnPath).magnitude;
                        splitData.cumulativeLength.Add(currentPathLength);
                        splitData.vertices.Add(pointOnPath);
                        splitData.tangents.Add(CubicBezierUtility.EvaluateCurveDerivative(segmentPoints, t).normalized);
                        splitData.minMax.AddValue(pointOnPath);
                        dstSinceLastVertex = 0;
                        lastAddedPoint = pointOnPath;
                    }

                    prevPointOnPath = pointOnPath;
                }

                splitData.anchorVertexMap.Add(splitData.vertices.Count - 1);
            }

            return splitData;
        }


        public class PathSplitData
        {
            public List<Vector3> vertices = new List<Vector3>();
            public List<Vector3> tangents = new List<Vector3>();
            public List<float> cumulativeLength = new List<float>();
            public List<int> anchorVertexMap = new List<int>();
            public MinMax3D minMax = new MinMax3D();
        }
    }
}