using UnityEngine;

namespace EzySlice
{
    /**
     * Represents a simple 3D Triangle structure with position
     * and UV map. The UV is required if the slicer needs
     * to recalculate the new UV position for texture mapping.
     */
    public struct Triangle
    {
        // the points which represent this triangle
        // these have to be set and are immutable. Cannot be
        // changed once set
        private readonly Vector3 _mPosA;
        private readonly Vector3 _mPosB;
        private readonly Vector3 _mPosC;

        // the UV coordinates of this triangle
        // these are optional and may not be set
        private bool _mUVSet;
        private Vector2 _mUvA;
        private Vector2 _mUvB;
        private Vector2 _mUvC;

        // the Normals of the Vertices
        // these are optional and may not be set
        private bool _mNorSet;
        private Vector3 _mNorA;
        private Vector3 _mNorB;
        private Vector3 _mNorC;

        // the Tangents of the Vertices
        // these are optional and may not be set
        private bool _mTanSet;
        private Vector4 _mTanA;
        private Vector4 _mTanB;
        private Vector4 _mTanC;

        public Triangle(Vector3 posa,
            Vector3 posb,
            Vector3 posc)
        {
            _mPosA = posa;
            _mPosB = posb;
            _mPosC = posc;

            _mUVSet = false;
            _mUvA = Vector2.zero;
            _mUvB = Vector2.zero;
            _mUvC = Vector2.zero;

            _mNorSet = false;
            _mNorA = Vector3.zero;
            _mNorB = Vector3.zero;
            _mNorC = Vector3.zero;

            _mTanSet = false;
            _mTanA = Vector4.zero;
            _mTanB = Vector4.zero;
            _mTanC = Vector4.zero;
        }

        public Vector3 PositionA => _mPosA;

        public Vector3 PositionB => _mPosB;

        public Vector3 PositionC => _mPosC;

        public bool HasUV => _mUVSet;

        public void SetUV(Vector2 uvA, Vector2 uvB, Vector2 uvC)
        {
            _mUvA = uvA;
            _mUvB = uvB;
            _mUvC = uvC;

            _mUVSet = true;
        }

        public Vector2 UvA => _mUvA;

        public Vector2 UvB => _mUvB;

        public Vector2 UvC => _mUvC;

        public bool HasNormal => _mNorSet;

        public void SetNormal(Vector3 norA, Vector3 norB, Vector3 norC)
        {
            _mNorA = norA;
            _mNorB = norB;
            _mNorC = norC;

            _mNorSet = true;
        }

        public Vector3 NormalA => _mNorA;

        public Vector3 NormalB => _mNorB;

        public Vector3 NormalC => _mNorC;

        public bool HasTangent => _mTanSet;

        public void SetTangent(Vector4 tanA, Vector4 tanB, Vector4 tanC)
        {
            _mTanA = tanA;
            _mTanB = tanB;
            _mTanC = tanC;

            _mTanSet = true;
        }

        public Vector4 TangentA => _mTanA;

        public Vector4 TangentB => _mTanB;

        public Vector4 TangentC => _mTanC;

        /**
         * Compute and set the tangents of this triangle
         * Derived From https://answers.unity.com/questions/7789/calculating-tangents-vector4.html
         */
        public void ComputeTangents()
        {
            // computing tangents requires both UV and normals set
            if (!_mNorSet || !_mUVSet)
            {
                return;
            }

            var v1 = _mPosA;
            var v2 = _mPosB;
            var v3 = _mPosC;

            var w1 = _mUvA;
            var w2 = _mUvB;
            var w3 = _mUvC;

            var x1 = v2.x - v1.x;
            var x2 = v3.x - v1.x;
            var y1 = v2.y - v1.y;
            var y2 = v3.y - v1.y;
            var z1 = v2.z - v1.z;
            var z2 = v3.z - v1.z;

            var s1 = w2.x - w1.x;
            var s2 = w3.x - w1.x;
            var t1 = w2.y - w1.y;
            var t2 = w3.y - w1.y;

            var r = 1.0f / (s1 * t2 - s2 * t1);

            var sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
            var tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);

            var n1 = _mNorA;
            var nt1 = sdir;

            Vector3.OrthoNormalize(ref n1, ref nt1);
            var tanA = new Vector4(nt1.x, nt1.y, nt1.z, (Vector3.Dot(Vector3.Cross(n1, nt1), tdir) < 0.0f) ? -1.0f : 1.0f);

            var n2 = _mNorB;
            var nt2 = sdir;

            Vector3.OrthoNormalize(ref n2, ref nt2);
            var tanB = new Vector4(nt2.x, nt2.y, nt2.z, (Vector3.Dot(Vector3.Cross(n2, nt2), tdir) < 0.0f) ? -1.0f : 1.0f);

            var n3 = _mNorC;
            var nt3 = sdir;

            Vector3.OrthoNormalize(ref n3, ref nt3);
            var tanC = new Vector4(nt3.x, nt3.y, nt3.z, (Vector3.Dot(Vector3.Cross(n3, nt3), tdir) < 0.0f) ? -1.0f : 1.0f);

            // finally set the tangents of this object
            SetTangent(tanA, tanB, tanC);
        }

        /**
         * Calculate the Barycentric coordinate weight values u-v-w for Point p in respect to the provided
         * triangle. This is useful for computing new UV coordinates for arbitrary points.
         */
        public Vector3 Barycentric(Vector3 p)
        {
            var a = _mPosA;
            var b = _mPosB;
            var c = _mPosC;

            var m = Vector3.Cross(b - a, c - a);

            float nu;
            float nv;
            float ood;

            var x = Mathf.Abs(m.x);
            var y = Mathf.Abs(m.y);
            var z = Mathf.Abs(m.z);

            // compute areas of plane with largest projections
            if (x >= y && x >= z)
            {
                // area of PBC in yz plane
                nu = Intersector.TriArea2D(p.y, p.z, b.y, b.z, c.y, c.z);
                // area of PCA in yz plane
                nv = Intersector.TriArea2D(p.y, p.z, c.y, c.z, a.y, a.z);
                // 1/2*area of ABC in yz plane
                ood = 1.0f / m.x;
            }
            else if (y >= x && y >= z)
            {
                // project in xz plane
                nu = Intersector.TriArea2D(p.x, p.z, b.x, b.z, c.x, c.z);
                nv = Intersector.TriArea2D(p.x, p.z, c.x, c.z, a.x, a.z);
                ood = 1.0f / -m.y;
            }
            else
            {
                // project in xy plane
                nu = Intersector.TriArea2D(p.x, p.y, b.x, b.y, c.x, c.y);
                nv = Intersector.TriArea2D(p.x, p.y, c.x, c.y, a.x, a.y);
                ood = 1.0f / m.z;
            }

            var u = nu * ood;
            var v = nv * ood;
            var w = 1.0f - u - v;

            return new Vector3(u, v, w);
        }

        /**
         * Generate a set of new UV coordinates for the provided point pt in respect to Triangle.
         *
         * Uses weight values for the computation, so this triangle must have UV's set to return
         * the correct results. Otherwise Vector2.zero will be returned. check via hasUV().
         */
        public Vector2 GenerateUV(Vector3 pt)
        {
            // if not set, result will be zero, quick exit
            if (!_mUVSet)
            {
                return Vector2.zero;
            }

            var weights = Barycentric(pt);

            return (weights.x * _mUvA) + (weights.y * _mUvB) + (weights.z * _mUvC);
        }

        /**
         * Generates a set of new Normal coordinates for the provided point pt in respect to Triangle.
         *
         * Uses weight values for the computation, so this triangle must have Normal's set to return
         * the correct results. Otherwise Vector3.zero will be returned. check via hasNormal().
         */
        public Vector3 GenerateNormal(Vector3 pt)
        {
            // if not set, result will be zero, quick exit
            if (!_mNorSet)
            {
                return Vector3.zero;
            }

            var weights = Barycentric(pt);

            return weights.x * _mNorA + weights.y * _mNorB + weights.z * _mNorC;
        }

        /**
         * Generates a set of new Tangent coordinates for the provided point pt in respect to Triangle.
         *
         * Uses weight values for the computation, so this triangle must have Tangent's set to return
         * the correct results. Otherwise Vector4.zero will be returned. check via hasTangent().
         */
        public Vector4 GenerateTangent(Vector3 pt)
        {
            // if not set, result will be zero, quick exit
            if (!_mNorSet)
            {
                return Vector4.zero;
            }

            var weights = Barycentric(pt);

            return weights.x * _mTanA + weights.y * _mTanB + weights.z * _mTanC;
        }

        /**
         * Helper function to split this triangle by the provided plane and store
         * the results inside the IntersectionResult structure.
         * Returns true on success or false otherwise
         */
        public bool Split(Plane pl, IntersectionResult result)
        {
            Intersector.Intersect(pl, this, result);

            return result.IsValid;
        }

        /**
         * Check the triangle winding order, if it's Clock Wise or Counter Clock Wise
         */
        public bool IsCW()
        {
            return SignedSquare(_mPosA, _mPosB, _mPosC) >= float.Epsilon;
        }

        /**
         * Returns the Signed square of a given triangle, useful for checking the
         * winding order
         */
        public static float SignedSquare(Vector3 a, Vector3 b, Vector3 c)
        {
            return (a.x * (b.y * c.z - b.z * c.y) -
                    a.y * (b.x * c.z - b.z * c.x) +
                    a.z * (b.x * c.y - b.y * c.x));
        }

        /**
         * Editor only DEBUG functionality. This should not be compiled in the final
         * Version.
         */
        public void OnDebugDraw()
        {
            OnDebugDraw(Color.white);
        }

        public void OnDebugDraw(Color drawColor)
        {
#if UNITY_EDITOR
            var prevColor = Gizmos.color;

            Gizmos.color = drawColor;

            Gizmos.DrawLine(PositionA, PositionB);
            Gizmos.DrawLine(PositionB, PositionC);
            Gizmos.DrawLine(PositionC, PositionA);

            Gizmos.color = prevColor;
#endif
        }
    }
}