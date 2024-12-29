using UnityEngine;

namespace EzySlice
{
    /**
     * TextureRegion defines a region of a specific texture which can be used
     * for custom UV Mapping Routines.
     *
     * TextureRegions are always stored in normalized UV Coordinate space between
     * 0.0f and 1.0f
     */
    public struct TextureRegion
    {
        private readonly float _posStartX;
        private readonly float _posStartY;
        private readonly float _posEndX;
        private readonly float _posEndY;

        public TextureRegion(float startX, float startY, float endX, float endY)
        {
            _posStartX = startX;
            _posStartY = startY;
            _posEndX = endX;
            _posEndY = endY;
        }

        public float StartX => _posStartX;
        public float StartY => _posStartY;
        public float EndX => _posEndX;
        public float EndY => _posEndY;

        public Vector2 Start => new(StartX, StartY);
        public Vector2 End => new(EndX, EndY);

        /**
         * Perform a mapping of a UV coordinate (computed in 0,1 space)
         * into the new coordinates defined by the provided TextureRegion
         */
        public Vector2 Map(Vector2 uv)
        {
            return Map(uv.x, uv.y);
        }

        /**
         * Perform a mapping of a UV coordinate (computed in 0,1 space)
         * into the new coordinates defined by the provided TextureRegion
         */
        public Vector2 Map(float x, float y)
        {
            var mappedX = Map(x, 0.0f, 1.0f, _posStartX, _posEndX);
            var mappedY = Map(y, 0.0f, 1.0f, _posStartY, _posEndY);

            return new Vector2(mappedX, mappedY);
        }

        /**
         * Our mapping function to map arbitrary values into our required texture region
         */
        private static float Map(float x, float inMin, float inMax, float outMin, float outMax)
        {
            return (x - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
        }
    }

    /**
     * Define our TextureRegion extension to easily calculate
     * from a Texture2D Object.
     */
    public static class TextureRegionExtension
    {
        /**
         * Helper function to quickly calculate the Texture Region from a material.
         * This extension function will use the mainTexture component to perform the
         * calculation.
         *
         * Will throw a null exception if the texture does not exist. See
         * Texture.getTextureRegion() for function details.
         */
        public static TextureRegion GetTextureRegion(this Material mat,
            int pixX,
            int pixY,
            int pixWidth,
            int pixHeight)
        {
            return mat.mainTexture.GetTextureRegion(pixX, pixY, pixWidth, pixHeight);
        }

        /**
         * Using a Texture2D, calculate and return a specific TextureRegion
         * Coordinates are provided in pixel coordinates where 0,0 is the
         * bottom left corner of the texture.
         *
         * The texture region will automatically be calculated to ensure that it
         * will fit inside the provided texture.
         */
        public static TextureRegion GetTextureRegion(this Texture tex,
            int pixX,
            int pixY,
            int pixWidth,
            int pixHeight)
        {
            var textureWidth = tex.width;
            var textureHeight = tex.height;

            // ensure we are not referencing out of bounds coordinates
            // relative to our texture
            var calcWidth = Mathf.Min(textureWidth, pixWidth);
            var calcHeight = Mathf.Min(textureHeight, pixHeight);
            var calcX = Mathf.Min(Mathf.Abs(pixX), textureWidth);
            var calcY = Mathf.Min(Mathf.Abs(pixY), textureHeight);

            var startX = calcX / (float)textureWidth;
            var startY = calcY / (float)textureHeight;
            var endX = (calcX + calcWidth) / (float)textureWidth;
            var endY = (calcY + calcHeight) / (float)textureHeight;

            // texture region is a struct which is allocated on the stack
            return new TextureRegion(startX, startY, endX, endY);
        }
    }
}