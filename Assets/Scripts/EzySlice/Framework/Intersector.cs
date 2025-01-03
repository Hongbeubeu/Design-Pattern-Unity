using UnityEngine;

namespace EzySlice
{
    /**
     * Contains static functionality to perform geometric intersection tests.
     */
    public static class Intersector
    {
        public const float EPSILON = 0.0001f;

        /**
         * Perform an intersection between Plane and Line, storing intersection point
         * in reference q. Function returns true if intersection has been found or
         * false otherwise.
         */
        public static bool Intersect(Plane pl, Line ln, out Vector3 q)
        {
            return Intersect(pl, ln.PositionA, ln.PositionB, out q);
        }


        /**
         * Perform an intersection between Plane and Line made up of points a and b. Intersection
         * point will be stored in reference q. Function returns true if intersection has been
         * found or false otherwise.
         */
        private static bool Intersect(Plane pl, Vector3 a, Vector3 b, out Vector3 q)
        {
            var normal = pl.Normal;
            var ab = b - a;

            var t = (pl.Dist - Vector3.Dot(normal, a)) / Vector3.Dot(normal, ab);

            // need to be careful and compensate for floating errors
            if (t is >= -EPSILON and <= 1 + EPSILON)
            {
                q = a + t * ab;

                return true;
            }

            q = Vector3.zero;

            return false;
        }

        /**
         * Support functionality
         */
        public static float TriArea2D(float x1, float y1, float x2, float y2, float x3, float y3)
        {
            return (x1 - x2) * (y2 - y3) - (x2 - x3) * (y1 - y2);
        }

        /**
         * Perform an intersection between Plane and Triangle. This is a comprehensive function
         * which alwo builds a HULL Hirearchy useful for decimation projects. This obviously
         * comes at the cost of more complex code and runtime checks, but the returned results
         * are much more flexible.
         * Results will be filled into the IntersectionResult reference. Check result.isValid()
         * for the final results.
         */
        public static void Intersect(Plane pl, Triangle tri, IntersectionResult result)
        {
            // clear the previous results from the IntersectionResult
            result.Clear();

            // grab local variables for easier access
            var a = tri.PositionA;
            var b = tri.PositionB;
            var c = tri.PositionC;

            // check to see which side of the plane the points all
            // lay in. SideOf operation is a simple dot product and some comparison
            // operations, so these are a very quick checks
            var sa = pl.SideOf(a);
            var sb = pl.SideOf(b);
            var sc = pl.SideOf(c);

            // we cannot intersect if the triangle points all fall on the same side
            // of the plane. This is an easy early out test as no intersections are possible.
            if (sa == sb && sb == sc)
            {
                return;
            }

            // detect cases where two points lay straight on the plane, meaning
            // that the plane is actually parralel with one of the edges of the triangle
            else if ((sa == SideOfPlane.On && sa == sb) ||
                     (sa == SideOfPlane.On && sa == sc) ||
                     (sb == SideOfPlane.On && sb == sc))
            {
                return;
            }

            // detect cases where one point is on the plane and the other two are on the same side
            else if ((sa == SideOfPlane.On && sb != SideOfPlane.On && sb == sc) ||
                     (sb == SideOfPlane.On && sa != SideOfPlane.On && sa == sc) ||
                     (sc == SideOfPlane.On && sa != SideOfPlane.On && sa == sb))
            {
                return;
            }

            // keep in mind that intersection points are shared by both
            // the upper HULL and lower HULL hence they lie perfectly
            // on the plane that cut them
            Vector3 qa;
            Vector3 qb;

            // check the cases where the points of the triangle actually lie on the plane itself
            // in these cases, there is only going to be 2 triangles, one for the upper HULL and
            // the other on the lower HULL
            // we just need to figure out which points to accept into the upper or lower hulls.
            if (sa == SideOfPlane.On)
            {
                // if the point a is on the plane, test line b-c
                if (Intersect(pl, b, c, out qa))
                {
                    // line b-c intersected, construct out triangles and return approprietly
                    result.AddIntersectionPoint(qa);
                    result.AddIntersectionPoint(a);

                    // our two generated triangles, we need to figure out which
                    // triangle goes into the UPPER hull and which goes into the LOWER hull
                    var ta = new Triangle(a, b, qa);
                    var tb = new Triangle(a, qa, c);

                    // generate UV coordinates if there is any
                    if (tri.HasUV)
                    {
                        // the computed UV coordinate if the intersection point
                        var pq = tri.GenerateUV(qa);
                        var pa = tri.UvA;
                        var pb = tri.UvB;
                        var pc = tri.UvC;

                        ta.SetUV(pa, pb, pq);
                        tb.SetUV(pa, pq, pc);
                    }

                    // generate Normal coordinates if there is any
                    if (tri.HasNormal)
                    {
                        // the computed Normal coordinate if the intersection point
                        var pq = tri.GenerateNormal(qa);
                        var pa = tri.NormalA;
                        var pb = tri.NormalB;
                        var pc = tri.NormalC;

                        ta.SetNormal(pa, pb, pq);
                        tb.SetNormal(pa, pq, pc);
                    }

                    // generate Tangent coordinates if there is any
                    if (tri.HasTangent)
                    {
                        // the computed Tangent coordinate if the intersection point
                        var pq = tri.GenerateTangent(qa);
                        var pa = tri.TangentA;
                        var pb = tri.TangentB;
                        var pc = tri.TangentC;

                        ta.SetTangent(pa, pb, pq);
                        tb.SetTangent(pa, pq, pc);
                    }

                    // b point lies on the upside of the plane
                    if (sb == SideOfPlane.Up)
                    {
                        result.AddUpperHull(ta).AddLowerHull(tb);
                    }

                    // b point lies on the downside of the plane
                    else if (sb == SideOfPlane.Down)
                    {
                        result.AddUpperHull(tb).AddLowerHull(ta);
                    }
                }
            }

            // test the case where the b point lies on the plane itself
            else if (sb == SideOfPlane.On)
            {
                // if the point b is on the plane, test line a-c
                if (Intersector.Intersect(pl, a, c, out qa))
                {
                    // line a-c intersected, construct out triangles and return approprietly
                    result.AddIntersectionPoint(qa);
                    result.AddIntersectionPoint(b);

                    // our two generated triangles, we need to figure out which
                    // triangle goes into the UPPER hull and which goes into the LOWER hull
                    var ta = new Triangle(a, b, qa);
                    var tb = new Triangle(qa, b, c);

                    // generate UV coordinates if there is any
                    if (tri.HasUV)
                    {
                        // the computed UV coordinate if the intersection point
                        var pq = tri.GenerateUV(qa);
                        var pa = tri.UvA;
                        var pb = tri.UvB;
                        var pc = tri.UvC;

                        ta.SetUV(pa, pb, pq);
                        tb.SetUV(pq, pb, pc);
                    }

                    // generate Normal coordinates if there is any
                    if (tri.HasNormal)
                    {
                        // the computed Normal coordinate if the intersection point
                        var pq = tri.GenerateNormal(qa);
                        var pa = tri.NormalA;
                        var pb = tri.NormalB;
                        var pc = tri.NormalC;

                        ta.SetNormal(pa, pb, pq);
                        tb.SetNormal(pq, pb, pc);
                    }

                    // generate Tangent coordinates if there is any
                    if (tri.HasTangent)
                    {
                        // the computed Tangent coordinate if the intersection point
                        var pq = tri.GenerateTangent(qa);
                        var pa = tri.TangentA;
                        var pb = tri.TangentB;
                        var pc = tri.TangentC;

                        ta.SetTangent(pa, pb, pq);
                        tb.SetTangent(pq, pb, pc);
                    }

                    // a point lies on the upside of the plane
                    if (sa == SideOfPlane.Up)
                    {
                        result.AddUpperHull(ta).AddLowerHull(tb);
                    }

                    // a point lies on the downside of the plane
                    else if (sa == SideOfPlane.Down)
                    {
                        result.AddUpperHull(tb).AddLowerHull(ta);
                    }
                }
            }

            // test the case where the c point lies on the plane itself
            else if (sc == SideOfPlane.On)
            {
                // if the point c is on the plane, test line a-b
                if (Intersect(pl, a, b, out qa))
                {
                    // line a-c intersected, construct out triangles and return approprietly
                    result.AddIntersectionPoint(qa);
                    result.AddIntersectionPoint(c);

                    // our two generated triangles, we need to figure out which
                    // triangle goes into the UPPER hull and which goes into the LOWER hull
                    var ta = new Triangle(a, qa, c);
                    var tb = new Triangle(qa, b, c);

                    // generate UV coordinates if there is any
                    if (tri.HasUV)
                    {
                        // the computed UV coordinate if the intersection point
                        var pq = tri.GenerateUV(qa);
                        var pa = tri.UvA;
                        var pb = tri.UvB;
                        var pc = tri.UvC;

                        ta.SetUV(pa, pq, pc);
                        tb.SetUV(pq, pb, pc);
                    }

                    // generate Normal coordinates if there is any
                    if (tri.HasNormal)
                    {
                        // the computed Normal coordinate if the intersection point
                        var pq = tri.GenerateNormal(qa);
                        var pa = tri.NormalA;
                        var pb = tri.NormalB;
                        var pc = tri.NormalC;

                        ta.SetNormal(pa, pq, pc);
                        tb.SetNormal(pq, pb, pc);
                    }

                    // generate Tangent coordinates if there is any
                    if (tri.HasTangent)
                    {
                        // the computed Tangent coordinate if the intersection point
                        var pq = tri.GenerateTangent(qa);
                        var pa = tri.TangentA;
                        var pb = tri.TangentB;
                        var pc = tri.TangentC;

                        ta.SetTangent(pa, pq, pc);
                        tb.SetTangent(pq, pb, pc);
                    }

                    // a point lies on the upside of the plane
                    if (sa == SideOfPlane.Up)
                    {
                        result.AddUpperHull(ta).AddLowerHull(tb);
                    }

                    // a point lies on the downside of the plane
                    else if (sa == SideOfPlane.Down)
                    {
                        result.AddUpperHull(tb).AddLowerHull(ta);
                    }
                }
            }

            // at this point, all edge cases have been tested and failed, we need to perform
            // full intersection tests against the lines. From this point onwards we will generate
            // 3 triangles
            else if (sa != sb && Intersect(pl, a, b, out qa))
            {
                // intersection found against a - b
                result.AddIntersectionPoint(qa);

                // since intersection was found against a - b, we need to check which other
                // lines to check (we only need to check one more line) for intersection.
                // the line we check against will be the line against the point which lies on
                // the other side of the plane.
                if (sa == sc)
                {
                    // we likely have an intersection against line b-c which will complete this loop
                    if (Intersect(pl, b, c, out qb))
                    {
                        result.AddIntersectionPoint(qb);

                        // our three generated triangles. Two of these triangles will end
                        // up on either the UPPER or LOWER hulls.
                        var ta = new Triangle(qa, b, qb);
                        var tb = new Triangle(a, qa, qb);
                        var tc = new Triangle(a, qb, c);

                        // generate UV coordinates if there is any
                        if (tri.HasUV)
                        {
                            // the computed UV coordinate if the intersection point
                            var pqa = tri.GenerateUV(qa);
                            var pqb = tri.GenerateUV(qb);
                            var pa = tri.UvA;
                            var pb = tri.UvB;
                            var pc = tri.UvC;

                            ta.SetUV(pqa, pb, pqb);
                            tb.SetUV(pa, pqa, pqb);
                            tc.SetUV(pa, pqb, pc);
                        }

                        // generate Normal coordinates if there is any
                        if (tri.HasNormal)
                        {
                            // the computed Normal coordinate if the intersection point
                            var pqa = tri.GenerateNormal(qa);
                            var pqb = tri.GenerateNormal(qb);
                            var pa = tri.NormalA;
                            var pb = tri.NormalB;
                            var pc = tri.NormalC;

                            ta.SetNormal(pqa, pb, pqb);
                            tb.SetNormal(pa, pqa, pqb);
                            tc.SetNormal(pa, pqb, pc);
                        }

                        // generate Tangent coordinates if there is any
                        if (tri.HasTangent)
                        {
                            // the computed Tangent coordinate if the intersection point
                            var pqa = tri.GenerateTangent(qa);
                            var pqb = tri.GenerateTangent(qb);
                            var pa = tri.TangentA;
                            var pb = tri.TangentB;
                            var pc = tri.TangentC;

                            ta.SetTangent(pqa, pb, pqb);
                            tb.SetTangent(pa, pqa, pqb);
                            tc.SetTangent(pa, pqb, pc);
                        }

                        if (sa == SideOfPlane.Up)
                        {
                            result.AddUpperHull(tb).AddUpperHull(tc).AddLowerHull(ta);
                        }
                        else
                        {
                            result.AddLowerHull(tb).AddLowerHull(tc).AddUpperHull(ta);
                        }
                    }
                }
                else
                {
                    // in this scenario, the point a is a "lone" point which lies in either upper
                    // or lower HULL. We need to perform another intersection to find the last point
                    if (Intersect(pl, a, c, out qb))
                    {
                        result.AddIntersectionPoint(qb);

                        // our three generated triangles. Two of these triangles will end
                        // up on either the UPPER or LOWER hulls.
                        var ta = new Triangle(a, qa, qb);
                        var tb = new Triangle(qa, b, c);
                        var tc = new Triangle(qb, qa, c);

                        // generate UV coordinates if there is any
                        if (tri.HasUV)
                        {
                            // the computed UV coordinate if the intersection point
                            var pqa = tri.GenerateUV(qa);
                            var pqb = tri.GenerateUV(qb);
                            var pa = tri.UvA;
                            var pb = tri.UvB;
                            var pc = tri.UvC;

                            ta.SetUV(pa, pqa, pqb);
                            tb.SetUV(pqa, pb, pc);
                            tc.SetUV(pqb, pqa, pc);
                        }

                        // generate Normal coordinates if there is any
                        if (tri.HasNormal)
                        {
                            // the computed Normal coordinate if the intersection point
                            var pqa = tri.GenerateNormal(qa);
                            var pqb = tri.GenerateNormal(qb);
                            var pa = tri.NormalA;
                            var pb = tri.NormalB;
                            var pc = tri.NormalC;

                            ta.SetNormal(pa, pqa, pqb);
                            tb.SetNormal(pqa, pb, pc);
                            tc.SetNormal(pqb, pqa, pc);
                        }

                        // generate Tangent coordinates if there is any
                        if (tri.HasTangent)
                        {
                            // the computed Tangent coordinate if the intersection point
                            var pqa = tri.GenerateTangent(qa);
                            var pqb = tri.GenerateTangent(qb);
                            var pa = tri.TangentA;
                            var pb = tri.TangentB;
                            var pc = tri.TangentC;

                            ta.SetTangent(pa, pqa, pqb);
                            tb.SetTangent(pqa, pb, pc);
                            tc.SetTangent(pqb, pqa, pc);
                        }

                        if (sa == SideOfPlane.Up)
                        {
                            result.AddUpperHull(ta).AddLowerHull(tb).AddLowerHull(tc);
                        }
                        else
                        {
                            result.AddLowerHull(ta).AddUpperHull(tb).AddUpperHull(tc);
                        }
                    }
                }
            }

            // if line a-b did not intersect (or the lie on the same side of the plane)
            // this simplifies the problem a fair bit. This means we have an intersection 
            // in line a-c and b-c, which we can use to build a new UPPER and LOWER hulls
            // we are expecting both of these intersection tests to pass, otherwise something
            // went wrong (float errors? missed a checked case?)
            else if (Intersect(pl, c, a, out qa) && Intersect(pl, c, b, out qb))
            {
                // in here we know that line a-b actually lie on the same side of the plane, this will
                // simplify the rest of the logic. We also have our intersection points
                // the computed UV coordinate of the intersection point

                result.AddIntersectionPoint(qa);
                result.AddIntersectionPoint(qb);

                // our three generated triangles. Two of these triangles will end
                // up on either the UPPER or LOWER hulls.
                var ta = new Triangle(qa, qb, c);
                var tb = new Triangle(a, qb, qa);
                var tc = new Triangle(a, b, qb);

                // generate UV coordinates if there is any
                if (tri.HasUV)
                {
                    // the computed UV coordinate if the intersection point
                    var pqa = tri.GenerateUV(qa);
                    var pqb = tri.GenerateUV(qb);
                    var pa = tri.UvA;
                    var pb = tri.UvB;
                    var pc = tri.UvC;

                    ta.SetUV(pqa, pqb, pc);
                    tb.SetUV(pa, pqb, pqa);
                    tc.SetUV(pa, pb, pqb);
                }

                // generate Normal coordinates if there is any
                if (tri.HasNormal)
                {
                    // the computed Normal coordinate if the intersection point
                    var pqa = tri.GenerateNormal(qa);
                    var pqb = tri.GenerateNormal(qb);
                    var pa = tri.NormalA;
                    var pb = tri.NormalB;
                    var pc = tri.NormalC;

                    ta.SetNormal(pqa, pqb, pc);
                    tb.SetNormal(pa, pqb, pqa);
                    tc.SetNormal(pa, pb, pqb);
                }

                // generate Tangent coordinates if there is any
                if (tri.HasTangent)
                {
                    // the computed Tangent coordinate if the intersection point
                    var pqa = tri.GenerateTangent(qa);
                    var pqb = tri.GenerateTangent(qb);
                    var pa = tri.TangentA;
                    var pb = tri.TangentB;
                    var pc = tri.TangentC;

                    ta.SetTangent(pqa, pqb, pc);
                    tb.SetTangent(pa, pqb, pqa);
                    tc.SetTangent(pa, pb, pqb);
                }

                if (sa == SideOfPlane.Up)
                {
                    result.AddUpperHull(tb).AddUpperHull(tc).AddLowerHull(ta);
                }
                else
                {
                    result.AddLowerHull(tb).AddLowerHull(tc).AddUpperHull(ta);
                }
            }
        }
    }
}