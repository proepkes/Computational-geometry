using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Habrador_Computational_Geometry
{
    //Help enum, to make it easier to deal with three cases: intersecting inside, intersecting on edge, not intersecting 
    //If we have two cases we can just return a bool
    public enum IntersectionCases
    {
        IsInside,
        IsOnEdge,
        NoIntersection
    }

    public static class Intersections
    {
        //
        // Are two lines intersecting?
        //
        //http://thirdpartyninjas.com/blog/2008/10/07/line-segment-intersection/
        public static bool LineLine(MyVector2 l1_p1, MyVector2 l1_p2, MyVector2 l2_p1, MyVector2 l2_p2, bool shouldIncludeEndPoints)
        {
            //To avoid floating point precision issues we can add a small value
            float epsilon = MathUtility.EPSILON;

            bool isIntersecting = false;

            float denominator = (l2_p2.y - l2_p1.y) * (l1_p2.x - l1_p1.x) - (l2_p2.x - l2_p1.x) * (l1_p2.y - l1_p1.y);

            //Make sure the denominator is > 0, if so the lines are parallel
            if (denominator != 0f)
            {
                float u_a = ((l2_p2.x - l2_p1.x) * (l1_p1.y - l2_p1.y) - (l2_p2.y - l2_p1.y) * (l1_p1.x - l2_p1.x)) / denominator;
                float u_b = ((l1_p2.x - l1_p1.x) * (l1_p1.y - l2_p1.y) - (l1_p2.y - l1_p1.y) * (l1_p1.x - l2_p1.x)) / denominator;

                //Are the line segments intersecting if the end points are the same
                if (shouldIncludeEndPoints)
                {
                    //Is intersecting if u_a and u_b are between 0 and 1 or exactly 0 or 1
                    if (u_a >= 0f + epsilon && u_a <= 1f - epsilon && u_b >= 0f + epsilon && u_b <= 1f - epsilon)
                    {
                        isIntersecting = true;
                    }
                }
                else
                {
                    //Is intersecting if u_a and u_b are between 0 and 1
                    if (u_a > 0f + epsilon && u_a < 1f - epsilon && u_b > 0f + epsilon && u_b < 1f - epsilon)
                    {
                        isIntersecting = true;
                    }
                }

            }

            return isIntersecting;
        }



        //Whats the coordinate of an intersection point between two lines in 2d space if we know they are intersecting
        //http://thirdpartyninjas.com/blog/2008/10/07/line-segment-intersection/
        public static MyVector2 GetLineLineIntersectionPoint(MyVector2 l1_p1, MyVector2 l1_p2, MyVector2 l2_p1, MyVector2 l2_p2)
        {
            float denominator = (l2_p2.y - l2_p1.y) * (l1_p2.x - l1_p1.x) - (l2_p2.x - l2_p1.x) * (l1_p2.y - l1_p1.y);

            float u_a = ((l2_p2.x - l2_p1.x) * (l1_p1.y - l2_p1.y) - (l2_p2.y - l2_p1.y) * (l1_p1.x - l2_p1.x)) / denominator;

            MyVector2 intersectionPoint = l1_p1 + u_a * (l1_p2 - l1_p1);

            return intersectionPoint;
        }



        //
        // Ray-plane intersection in 2d space (Same can be used in 3d space as well if you change it to vector3)
        //
        //The plane normal and the ray dir have to point against each other (not exactly against each other but generally)
        //http://www.scratchapixel.com/lessons/3d-basic-rendering/minimal-ray-tracer-rendering-simple-shapes/ray-plane-and-ray-disk-intersection
        public static bool RayPlane(MyVector2 planePos, MyVector2 planeNormal, MyVector2 rayStart, MyVector2 rayDir)
        {
            //To avoid floating point precision issues we can add a small value
            float epsilon = MathUtility.EPSILON;

            bool areIntersecting = false;

            float denominator = MyVector2.Dot(planeNormal * -1f, rayDir);

            //Debug.Log(denominator);

            if (denominator > epsilon)
            {
                MyVector2 vecBetween = planePos - rayStart;

                float t = MyVector2.Dot(vecBetween, planeNormal * -1f) / denominator;

                //Debug.Log(t);

                if (t >= 0f)
                {
                    areIntersecting = true;
                }
            }

            return areIntersecting;
        }

        //Get the coordinate if we know a ray-plane is intersecting
        public static MyVector2 GetRayPlaneIntersectionCoordinate(MyVector2 planePos, MyVector2 planeNormal, MyVector2 rayStart, MyVector2 rayDir)
        {
            float denominator = MyVector2.Dot(-planeNormal, rayDir);

            MyVector2 vecBetween = planePos - rayStart;

            float t = MyVector2.Dot(vecBetween, -planeNormal) / denominator;

            MyVector2 intersectionPoint = rayStart + rayDir * t;

            return intersectionPoint;
        }



        //
        // Line-plane intersection
        //
        public static bool LinePlane(MyVector2 planePos, MyVector2 planeNormal, MyVector2 line_p1, MyVector2 line_p2)
        {
            //To avoid floating point precision issues we can add a small value
            float epsilon = MathUtility.EPSILON;

            bool areIntersecting = false;

            MyVector2 lineDir = MyVector2.Normalize(line_p1 - line_p2);

            float denominator = MyVector2.Dot(-planeNormal, lineDir);

            //Debug.Log(denominator);

            //No intersection if the line and plane are parallell
            if (denominator > epsilon || denominator < -epsilon)
            {
                MyVector2 vecBetween = planePos - line_p1;

                float t = MyVector2.Dot(vecBetween, -planeNormal) / denominator;

                MyVector2 intersectionPoint = line_p1 + lineDir * t;

                //Gizmos.DrawWireSphere(intersectionPoint, 0.5f);

                if (Geometry.IsPointBetweenPoints(line_p1, line_p2, intersectionPoint))
                {
                    areIntersecting = true;
                }
            }

            return areIntersecting;
        }

        //We know a line plane is intersecting and now we want the coordinate of intersection
        public static MyVector2 GetLinePlaneIntersectionCoordinate(MyVector2 planePos, MyVector2 planeNormal, MyVector2 line_p1, MyVector2 line_p2)
        {
            MyVector2 vecBetween = planePos - line_p1;

            MyVector2 lineDir = MyVector2.Normalize(line_p1 - line_p2);

            float denominator = MyVector2.Dot(-planeNormal, lineDir);

            float t = MyVector2.Dot(vecBetween, -planeNormal) / denominator;

            MyVector2 intersectionPoint = line_p1 + lineDir * t;

            return intersectionPoint;
        }



        //
        // Is a point inside a triangle?
        //
        //From http://totologic.blogspot.se/2014/01/accurate-point-in-triangle-test.html
        public static bool PointTriangle(Triangle2 t, MyVector2 p, bool includeBorder)
        {
            //To avoid floating point precision issues we can add a small value
            float epsilon = MathUtility.EPSILON;

            float zero = 0f - epsilon;
            float one = 1f + epsilon;

            //Based on Barycentric coordinates
            float denominator = ((t.p2.y - t.p3.y) * (t.p1.x - t.p3.x) + (t.p3.x - t.p2.x) * (t.p1.y - t.p3.y));

            float a = ((t.p2.y - t.p3.y) * (p.x - t.p3.x) + (t.p3.x - t.p2.x) * (p.y - t.p3.y)) / denominator;
            float b = ((t.p3.y - t.p1.y) * (p.x - t.p3.x) + (t.p1.x - t.p3.x) * (p.y - t.p3.y)) / denominator;
            float c = 1 - a - b;

            bool isWithinTriangle = false;

            if (includeBorder)
            {
                //The point is within the triangle or on the border
                if (a >= zero && a <= one && b >= zero && b <= one && c >= zero && c <= one)
                {
                    isWithinTriangle = true;
                }
            }
            else
            {
                //The point is within the triangle
                if (a > zero && a < one && b > zero && b < one && c > zero && c < one)
                {
                    isWithinTriangle = true;
                }
            }

            return isWithinTriangle;
        }


        //Is a point inside, outside, or on the border of a triangle
        //-1 if outside, 0 if on the border, 1 if inside the triangle
        //BROKEN use if the point is to the left or right of all edges in the triangle
        //public static int IsPointInOutsideOnTriangle(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p)
        //{
        //    //To avoid floating point precision issues we can add a small value
        //    float epsilon = MathUtility.EPSILON;

        //    float zero = 0f + epsilon;
        //    float one = 1f - epsilon;

        //    //Based on Barycentric coordinates
        //    float denominator = ((p2.y - p3.y) * (p1.x - p3.x) + (p3.x - p2.x) * (p1.y - p3.y));

        //    float a = ((p2.y - p3.y) * (p.x - p3.x) + (p3.x - p2.x) * (p.y - p3.y)) / denominator;
        //    float b = ((p3.y - p1.y) * (p.x - p3.x) + (p1.x - p3.x) * (p.y - p3.y)) / denominator;
        //    float c = 1 - a - b;

        //    int returnValue = -1;

        //    //The point is on the border, meaning exactly 0 or 1 in a world with no floating precision issues

        //    //The point is on or within the triangle
        //    if (a >= zero && a <= one && b >= zero && b <= one && c >= zero && c <= one)
        //    {
        //        returnValue = 1;
        //    }

        //    return returnValue;
        //}



        //
        // Is a triangle inside a triangle
        //
        //Is triangle 1 inside triangle 2?
        public static bool IsTriangleInsideTriangle(Triangle2 t1, Triangle2 t2)
        {
            bool isWithin = false;

            if (
                PointTriangle(t2, t1.p1, false) &&
                PointTriangle(t2, t1.p2, false) &&
                PointTriangle(t2, t1.p3, false))
            {
                isWithin = true;
            }

            return isWithin;
        }



        //
        // Are two Axis-aligned-bounding-box (boxes are here rectangles) intersecting?
        //
        //r1_minX - the smallest x-coordinate of all corners belonging to rectangle 1
        public static bool AABB_AABB_2D(
            float r1_minX, float r1_maxX, float r1_minY, float r1_maxY,
            float r2_minX, float r2_maxX, float r2_minY, float r2_maxY)
        {
            //If the min of one box in one dimension is greater than the max of another box then the boxes are not intersecting
            //They have to intersect in 2 dimensions. We have to test if box 1 is to the left or box 2 and vice versa
            bool isIntersecting = true;

            //X axis
            if (r1_minX > r2_maxX)
            {
                isIntersecting = false;
            }
            else if (r2_minX > r1_maxX)
            {
                isIntersecting = false;
            }
            //Y axis
            else if (r1_minY > r2_maxY)
            {
                isIntersecting = false;
            }
            else if (r2_minY > r1_maxY)
            {
                isIntersecting = false;
            }

            return isIntersecting;
        }



        //
        // Is a point intersecting with a circle?
        //
        //Is a point d inside, outside or on the same circle where a, b, c are all on the circle's edge
        public static IntersectionCases PointCircle(MyVector2 a, MyVector2 b, MyVector2 c, MyVector2 testPoint)
        {
            //Center of circle
            MyVector2 circleCenter = Geometry.CalculateCircleCenter(a, b, c);

            //The radius of the circle
            float radiusSqr = MyVector2.SqrDistance(a, circleCenter);

            //The distance from the point to the circle center
            float distPointCenterSqr = MyVector2.SqrDistance(testPoint, circleCenter);
            
            //Add/remove a small value becuse we will never be exactly on the edge because of floating point precision issues
            //Mutiply epsilon by two because we are using sqr root???
            if (distPointCenterSqr < radiusSqr - MathUtility.EPSILON * 2f)
            {
                return IntersectionCases.IsInside;
            }
            else if (distPointCenterSqr > radiusSqr + MathUtility.EPSILON * 2f)
            {
                return IntersectionCases.NoIntersection;
            }
            else
            {
                return IntersectionCases.IsOnEdge;
            }
        }



        //
        // Is a point intersecting with a polygon?
        //
        //The list describing the polygon has to be sorted either clockwise or counter-clockwise because we have to identify its edges
        //TODO: May sometimes fail because of floating point precision issues
        public static bool PointPolygon(List<MyVector2> polygonPoints, MyVector2 point)
        {
            //Step 1. Find a point outside of the polygon
            //Pick a point with a x position larger than the polygons max x position, which is always outside
            MyVector2 maxXPosVertex = polygonPoints[0];

            for (int i = 1; i < polygonPoints.Count; i++)
            {
                if (polygonPoints[i].x > maxXPosVertex.x)
                {
                    maxXPosVertex = polygonPoints[i];
                }
            }

            //The point should be outside so just pick a number to move it outside
            //Should also move it up a little to minimize floating point precision issues
            //This is where it fails if this line is exactly on a vertex
            MyVector2 pointOutside = maxXPosVertex + new MyVector2(1f, 0.01f);


            //Step 2. Create an edge between the point we want to test with the point thats outside
            MyVector2 l1_p1 = point;
            MyVector2 l1_p2 = pointOutside;

            //Debug.DrawLine(l1_p1.XYZ(), l1_p2.XYZ());


            //Step 3. Find out how many edges of the polygon this edge is intersecting with
            int numberOfIntersections = 0;

            for (int i = 0; i < polygonPoints.Count; i++)
            {
                //Line 2
                MyVector2 l2_p1 = polygonPoints[i];

                int iPlusOne = MathUtility.ClampListIndex(i + 1, polygonPoints.Count);

                MyVector2 l2_p2 = polygonPoints[iPlusOne];

                //Are the lines intersecting?
                if (Intersections.LineLine(l1_p1, l1_p2, l2_p1, l2_p2, shouldIncludeEndPoints: true))
                {
                    numberOfIntersections += 1;
                }
            }


            //Step 4. Is the point inside or outside?
            bool isInside = true;

            //The point is outside the polygon if number of intersections is even or 0
            if (numberOfIntersections == 0 || numberOfIntersections % 2 == 0)
            {
                isInside = false;
            }

            return isInside;
        }



        //
        // Are two triangles intersecting in 2d space
        //
        public static bool TriangleTriangle2D(Triangle2 t1, Triangle2 t2, bool do_AABB_test)
        {
            bool isIntersecting = false;

            //Step 0. AABB intersection which may speed up the algorithm if the triangles are far apart
            if (do_AABB_test)
            {
                //Rectangle that covers t1 
                float r1_minX = t1.MinX();
                float r1_maxX = t1.MaxX();

                float r1_minY = t1.MinY();
                float r1_maxY = t1.MaxY();

                //Rectangle that covers t2
                float r2_minX = t2.MinX();
                float r2_maxX = t2.MaxX();

                float r2_minY = t2.MinY();
                float r2_maxY = t2.MaxY();

                if (!AABB_AABB_2D(r1_minX, r1_maxX, r1_minY, r1_maxY, r2_minX, r2_maxX, r2_minY, r2_maxY))
                {
                    return false;
                }
            }


            //Step 1. Line-line instersection

            //Line 1 of t1 against all lines of t2
            if (
                LineLine(t1.p1, t1.p2, t2.p1, t2.p2, true) ||
                LineLine(t1.p1, t1.p2, t2.p2, t2.p3, true) ||
                LineLine(t1.p1, t1.p2, t2.p3, t2.p1, true)
            )
            {
                isIntersecting = true;
            }
            //Line 2 of t1 against all lines of t2
            else if (
                LineLine(t1.p2, t1.p3, t2.p1, t2.p2, true) ||
                LineLine(t1.p2, t1.p3, t2.p2, t2.p3, true) ||
                LineLine(t1.p2, t1.p3, t2.p3, t2.p1, true)
            )
            {
                isIntersecting = true;
            }
            //Line 3 of t1 against all lines of t2
            else if (
                LineLine(t1.p3, t1.p1, t2.p1, t2.p2, true) ||
                LineLine(t1.p3, t1.p1, t2.p2, t2.p3, true) ||
                LineLine(t1.p3, t1.p1, t2.p3, t2.p1, true)
            )
            {
                isIntersecting = true;
            }

            //Now we can return if we are intersecting so we dont need to spend time testing something else
            if (isIntersecting)
            {
                return isIntersecting;
            }


            //Step 2. Point-in-triangle intersection
            //We only need to test one corner from each triangle
            //If this point is not in the triangle, then the other points can't be in the triangle, because if this point is outside
            //and another point is inside, then the line between them would have been covered by step 1: line-line intersections test
            if (PointTriangle(t2, t1.p1, true) || PointTriangle(t1, t2.p1, true))
            {
                isIntersecting = true;
            }


            return isIntersecting;
        }
    }
}