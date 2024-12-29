using UnityEngine;

namespace EzySlice
{
    public struct Line
    {
        private readonly Vector3 _posA;
        private readonly Vector3 _posB;

        public Line(Vector3 pta, Vector3 ptb)
        {
            _posA = pta;
            _posB = ptb;
        }

        public float Dist => Vector3.Distance(_posA, _posB);

        public float DistSq => (_posA - _posB).sqrMagnitude;

        public Vector3 PositionA => _posA;

        public Vector3 PositionB => _posB;
    }
}