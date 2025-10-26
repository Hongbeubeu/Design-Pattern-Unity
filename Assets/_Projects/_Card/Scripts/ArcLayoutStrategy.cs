using UnityEngine;

public enum ArcOrientation
{
    Up,
    Down,
    Right,
    Left
}

public class ArcLayoutStrategy : CardLayoutStrategy
{
    [Header("Arc Shape")] 
    [SerializeField, Range(0.1f, 20f)] private float _arcWidth = 10f;
    [SerializeField, Range(0.01f, 5f)] private float _arcHeight = 2f;
    [SerializeField, Range(0.01f, 5f)] private float _maxSpacing = 1f;
    [SerializeField] private ArcOrientation _arcOrientation = ArcOrientation.Up;
    
    private float _calculatedClosedSpacing;
    private Vector3 _arcCenter;
    private float _arcRadius;
    private Vector3 _arcStartVector;
    private float _arcTotalAngle;
    private float _tangentRotationAngle;
    private float _virtualArcWidth;
    private float _prevArcWidth;
    private float _prevArcHeight;
    private float _prevMaxSpacing;
    private ArcOrientation _prevArcOrientation;

    private void OnEnable()
    {
        _prevArcWidth = _arcWidth;
        _prevArcHeight = _arcHeight;
        _prevMaxSpacing = _maxSpacing;
        _prevArcOrientation = _arcOrientation;
    }

    public override LayoutData CalculateLayouts(int cardCount)
    {
        switch (cardCount)
        {
            case 0:
                return new LayoutData(0);
            case > 1:
            {
                var idealSpacing = _arcWidth / (cardCount - 1);
                _calculatedClosedSpacing = Mathf.Min(idealSpacing, _maxSpacing);

                break;
            }
            default:
                _calculatedClosedSpacing = 0;

                break;
        }

        CalculateVirtualArcShape();

        var (openPositions, openRotations, zOffsets) = CalculateOpenPositions(cardCount);

        var (closedPositions, closedRotations) = CalculateClosedPositions(cardCount, openPositions, zOffsets);

        return new LayoutData
               {
                   openPositions = openPositions,
                   openRotations = openRotations,
                   closedPositions = closedPositions,
                   closedRotations = closedRotations
               };
    }

    public override bool CheckForRuntimeChanges()
    {
        var changed = false;

        if (!Mathf.Approximately(_arcWidth, _prevArcWidth))
        {
            changed = true;
            _prevArcWidth = _arcWidth;
        }

        if (!Mathf.Approximately(_arcHeight, _prevArcHeight))
        {
            changed = true;
            _prevArcHeight = _arcHeight;
        }

        if (!Mathf.Approximately(_maxSpacing, _prevMaxSpacing))
        {
            changed = true;
            _prevMaxSpacing = _maxSpacing;
        } 

        if (_arcOrientation != _prevArcOrientation)
        {
            changed = true;
            _prevArcOrientation = _arcOrientation;
        }

        return changed;
    }


    #region Unchanged GetArcAxes

    private void GetArcAxes(out Vector3 baseAxis, out Vector3 heightAxis, out Quaternion baseRotation, out float h)
    {
        h = _arcHeight;

        switch (_arcOrientation)
        {
            case ArcOrientation.Up:
                baseAxis = Vector3.right;
                heightAxis = Vector3.up;
                baseRotation = Quaternion.identity;

                break;
            case ArcOrientation.Down:
                baseAxis = Vector3.right;
                heightAxis = Vector3.up;
                h = -_arcHeight;
                baseRotation = Quaternion.identity;

                break;
            case ArcOrientation.Right:
                baseAxis = Vector3.up;
                heightAxis = Vector3.right;
                baseRotation = Quaternion.Euler(0, 0, 90);

                break;
            case ArcOrientation.Left:
                baseAxis = Vector3.up;
                heightAxis = Vector3.right;
                h = -_arcHeight;
                baseRotation = Quaternion.Euler(0, 0, 90);

                break;
            default:
                baseAxis = Vector3.right;
                heightAxis = Vector3.up;
                baseRotation = Quaternion.identity;

                break;
        }
    }

    #endregion

    private void CalculateVirtualArcShape()
    {
        GetArcAxes(out var baseAxis, out var heightAxis, out _, out var h);
        var rotationAxis = Vector3.forward;

        _virtualArcWidth = _arcWidth;

        var virtualStartPos = baseAxis * (-_virtualArcWidth / 2f);
        var virtualEndPos = baseAxis * (_virtualArcWidth / 2f);
        var chordVector = virtualEndPos - virtualStartPos;
        var chordMidpoint = (virtualStartPos + virtualEndPos) / 2f;
        var halfChordLength = chordVector.magnitude / 2f;

        if (halfChordLength < 0.001f || Mathf.Abs(h) < 0.001f)
        {
            _arcCenter = Vector3.zero;
            _arcRadius = 0;
            _arcStartVector = Vector3.up;
            _arcTotalAngle = 0;
            _tangentRotationAngle = 90f;

            return;
        }

        _arcRadius = (h * h + halfChordLength * halfChordLength) / (2f * h);
        _arcCenter = chordMidpoint + heightAxis * (h - _arcRadius);
        _arcStartVector = virtualStartPos - _arcCenter;
        var radiusVectorEnd = virtualEndPos - _arcCenter;

        if (baseAxis == Vector3.right)
        {
            _tangentRotationAngle = -90f * Mathf.Sign(h);
        }
        else
        {
            _tangentRotationAngle = 90f * Mathf.Sign(h);
        }

        if (Mathf.Approximately(h, 0)) _tangentRotationAngle = 90f;

        _arcTotalAngle = Vector3.SignedAngle(_arcStartVector, radiusVectorEnd, rotationAxis);
        var isMajorArc = halfChordLength > 0 && (h * h > halfChordLength * halfChordLength);

        if (isMajorArc)
        {
            _arcTotalAngle -= 360f * Mathf.Sign(_arcTotalAngle);
        }
    }

    private (Vector3[] positions, Quaternion[] rotations, float[] zOffsets) CalculateOpenPositions(int cardCount)
    {
        var positions = new Vector3[cardCount];
        var rotations = new Quaternion[cardCount];
        var zOffsets = new float[cardCount];

        if (cardCount == 0 || Mathf.Approximately(_virtualArcWidth, 0)) return (positions, rotations, zOffsets);

        GetArcAxes(out _, out _, out _, out _);
        var rotationAxis = Vector3.forward;

        var totalClosedWidth = _calculatedClosedSpacing * (cardCount - 1);
        var closedOffset = -totalClosedWidth / 2f;

        for (var i = 0; i < cardCount; i++)
        {
            var linearPos = closedOffset + (i * _calculatedClosedSpacing);

            float t;
            if (Mathf.Approximately(_virtualArcWidth, 0))
                t = 0.5f;
            else
                t = linearPos / _virtualArcWidth + 0.5f;

            t = Mathf.Clamp01(t);

            var arcRotation = Quaternion.AngleAxis(_arcTotalAngle * t, rotationAxis);
            var currentRadiusVector = arcRotation * _arcStartVector;
            positions[i] = _arcCenter + currentRadiusVector;

            var currentTangentDir = Quaternion.AngleAxis(_tangentRotationAngle, rotationAxis) * currentRadiusVector.normalized;
            var currentTangentAngle = Mathf.Atan2(currentTangentDir.y, currentTangentDir.x) * Mathf.Rad2Deg;
            rotations[i] = Quaternion.AngleAxis(currentTangentAngle, rotationAxis);
        }

        return (positions, rotations, zOffsets);
    }
    
    #region Unchanged CalculateClosedPositions

    private (Vector3[] positions, Quaternion[] rotations) CalculateClosedPositions(int cardCount, Vector3[] openPositions, float[] zOffsets)
    {
        var positions = new Vector3[cardCount];
        var rotations = new Quaternion[cardCount];

        if (cardCount == 0) return (positions, rotations);

        GetArcAxes(out _, out _, out var baseRotation, out _);

        Vector3 centerPos;

        if (cardCount % 2 == 1)
        {
            centerPos = openPositions[cardCount / 2];
        }
        else
        {
            var midIndex1 = cardCount / 2 - 1;
            var midIndex2 = cardCount / 2;
            centerPos = (openPositions[midIndex1] + openPositions[midIndex2]) / 2f;
        }

        var targetXY = new Vector3(centerPos.x, centerPos.y, 0);

        for (var i = 0; i < cardCount; i++)
        {
            positions[i] = targetXY;
            positions[i].z = zOffsets[i];
            rotations[i] = baseRotation;
        }

        return (positions, rotations);
    }

    #endregion
    
    #region Unchanged OnDrawGizmosSelected

    private void OnDrawGizmosSelected()
    {
#if UNITY_EDITOR
        CalculateVirtualArcShape();
#endif

        if (Mathf.Approximately(_arcRadius, 0)) return;

        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_arcCenter, 0.1f);
        Gizmos.color = Color.yellow;

        var virtualStartPos = _arcCenter + _arcStartVector;
        var virtualEndPos = _arcCenter + (Quaternion.AngleAxis(_arcTotalAngle, Vector3.forward) * _arcStartVector);
        Gizmos.DrawLine(_arcCenter, virtualStartPos);
        Gizmos.DrawLine(_arcCenter, virtualEndPos);

        Gizmos.color = Color.cyan;
        const int segments = 40;
        var angleStep = _arcTotalAngle / segments;
        var prevPoint = virtualStartPos;

        for (var i = 1; i <= segments; i++)
        {
            var rotation = Quaternion.AngleAxis(angleStep * i, Vector3.forward);
            var nextPoint = _arcCenter + (rotation * _arcStartVector);
            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }
    }

    #endregion
}