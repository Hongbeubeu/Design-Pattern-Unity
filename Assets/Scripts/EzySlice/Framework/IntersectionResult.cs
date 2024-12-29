using UnityEngine;

namespace EzySlice
{
    /**
     * A Basic Structure which contains intersection information
     * for Plane->Triangle Intersection Tests
     * TO-DO -> This structure can be optimized to hold less data
     * via an optional indices array. Could lead for a faster
     * intersection test aswell.
     */
    public sealed class IntersectionResult
    {
        // general tag to check if this structure is valid
        private bool _isSuccess;

        // our intersection points/triangles
        private readonly Triangle[] _upperHull = new Triangle[2];
        private readonly Triangle[] _lowerHull = new Triangle[2];
        private readonly Vector3[] _intersectionPt = new Vector3[2];

        // our counters. We use raw arrays for performance reasons
        private int _upperHullCount;
        private int _lowerHullCount;
        private int _intersectionPtCount;

        public Triangle[] UpperHull => _upperHull;

        public Triangle[] LowerHull => _lowerHull;

        public Vector3[] IntersectionPoints => _intersectionPt;

        public int UpperHullCount => _upperHullCount;

        public int LowerHullCount => _lowerHullCount;

        public int IntersectionPointCount => _intersectionPtCount;

        public bool IsValid => _isSuccess;

        /**
         * Used by the intersector, adds a new triangle to the
         * upper hull section
         */
        public IntersectionResult AddUpperHull(Triangle tri)
        {
            _upperHull[_upperHullCount++] = tri;

            _isSuccess = true;

            return this;
        }

        /**
         * Used by the intersector, adds a new triangle to the
         * lower gull section
         */
        public IntersectionResult AddLowerHull(Triangle tri)
        {
            _lowerHull[_lowerHullCount++] = tri;

            _isSuccess = true;

            return this;
        }

        /**
         * Used by the intersector, adds a new intersection point
         * which is shared by both upper->lower hulls
         */
        public void AddIntersectionPoint(Vector3 pt)
        {
            _intersectionPt[_intersectionPtCount++] = pt;
        }

        /**
         * Clear the current state of this object
         */
        public void Clear()
        {
            _isSuccess = false;
            _upperHullCount = 0;
            _lowerHullCount = 0;
            _intersectionPtCount = 0;
        }

        /**
         * Editor only DEBUG functionality. This should not be compiled in the final
         * Version.
         */
        public void OnDebugDraw()
        {
            OnDebugDraw(Color.white);
        }

        private void OnDebugDraw(Color drawColor)
        {
#if UNITY_EDITOR

            if (!IsValid)
            {
                return;
            }

            var prevColor = Gizmos.color;

            Gizmos.color = drawColor;

            // draw the intersection points
            for (var i = 0; i < IntersectionPointCount; i++)
            {
                Gizmos.DrawSphere(IntersectionPoints[i], 0.1f);
            }

            // draw the upper hull in RED
            for (var i = 0; i < UpperHullCount; i++)
            {
                UpperHull[i].OnDebugDraw(Color.red);
            }

            // draw the lower hull in BLUE
            for (var i = 0; i < LowerHullCount; i++)
            {
                LowerHull[i].OnDebugDraw(Color.blue);
            }

            Gizmos.color = prevColor;

#endif
        }
    }
}