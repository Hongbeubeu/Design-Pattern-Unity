using UnityEngine;

namespace EzySlice
{
    /**
     * Define Extension methods for easy access to slicer functionality
     */
    public static class SlicerExtensions
    {
        /**
         * SlicedHull Return functions and appropriate overrides!
         */
        public static SlicedHull Slice(this GameObject obj, Plane pl, Material crossSectionMaterial = null)
        {
            return Slice(obj, pl, new TextureRegion(0.0f, 0.0f, 1.0f, 1.0f), crossSectionMaterial);
        }

        public static SlicedHull Slice(this GameObject obj, Vector3 position, Vector3 direction, Material crossSectionMaterial = null)
        {
            return Slice(obj, position, direction, new TextureRegion(0.0f, 0.0f, 1.0f, 1.0f), crossSectionMaterial);
        }

        public static SlicedHull Slice(this GameObject obj, Vector3 position, Vector3 direction, TextureRegion textureRegion, Material crossSectionMaterial = null)
        {
            var cuttingPlane = new Plane();

            var mat = obj.transform.worldToLocalMatrix;
            var transpose = mat.transpose;
            var inv = transpose.inverse;

            var refUp = inv.MultiplyVector(direction).normalized;
            var refPt = obj.transform.InverseTransformPoint(position);

            cuttingPlane.Compute(refPt, refUp);

            return Slice(obj, cuttingPlane, textureRegion, crossSectionMaterial);
        }

        public static SlicedHull Slice(this GameObject obj, Plane pl, TextureRegion textureRegion, Material crossSectionMaterial = null)
        {
            return Slicer.Slice(obj, pl, textureRegion, crossSectionMaterial);
        }

        /**
         * These functions (and overrides) will return the final indtaniated GameObjects types
         */
        public static GameObject[] SliceInstantiate(this GameObject obj, Plane pl)
        {
            return SliceInstantiate(obj, pl, new TextureRegion(0.0f, 0.0f, 1.0f, 1.0f));
        }

        public static GameObject[] SliceInstantiate(this GameObject obj, Vector3 position, Vector3 direction)
        {
            return SliceInstantiate(obj, position, direction, null);
        }

        public static GameObject[] SliceInstantiate(this GameObject obj, Vector3 position, Vector3 direction, Material crossSectionMat)
        {
            return SliceInstantiate(obj, position, direction, new TextureRegion(0.0f, 0.0f, 1.0f, 1.0f), crossSectionMat);
        }

        public static GameObject[] SliceInstantiate(this GameObject obj, Vector3 position, Vector3 direction, TextureRegion cuttingRegion, Material crossSectionMaterial = null)
        {
            var cuttingPlane = new Plane();

            var mat = obj.transform.worldToLocalMatrix;
            var transpose = mat.transpose;
            var inv = transpose.inverse;

            var refUp = inv.MultiplyVector(direction).normalized;
            var refPt = obj.transform.InverseTransformPoint(position);

            cuttingPlane.Compute(refPt, refUp);

            return SliceInstantiate(obj, cuttingPlane, cuttingRegion, crossSectionMaterial);
        }

        public static GameObject[] SliceInstantiate(this GameObject obj, Plane pl, TextureRegion cuttingRegion, Material crossSectionMaterial = null)
        {
            var slice = Slicer.Slice(obj, pl, cuttingRegion, crossSectionMaterial);

            if (slice == null)
            {
                return null;
            }

            var upperHull = slice.CreateUpperHull(obj, crossSectionMaterial);
            var lowerHull = slice.CreateLowerHull(obj, crossSectionMaterial);

            if (upperHull != null && lowerHull != null)
            {
                return new[] { upperHull, lowerHull };
            }

            // otherwise return only the upper hull
            if (upperHull != null)
            {
                return new[] { upperHull };
            }

            // otherwise return only the lower hull
            if (lowerHull != null)
            {
                return new[] { lowerHull };
            }

            // nothing to return, so return nothing!
            return null;
        }
    }
}