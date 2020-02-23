#if !CORE_LIB
namespace mulova.ui
{
    public static class FloatEx
    {
        public const float Epsilon = 0.000001f;
        public static bool ApproximatelyEquals(this float f1, float f2)
        {
            return f1.Equals(f2, Epsilon);
        }
        
        public static bool Equals(this float f1, float f2, float tolerance)
        {
            var delta = f1 - f2;
            return -tolerance <= delta && delta <= tolerance;
        }
        
        public static float Clamp(this float v, float min, float max)
        {
            if (v <= min)
            {
                return min;
            }
            if (v >= max)
            {
                return max;
            }
            return v;
        }
        
        public static bool IsInRange(this float val, float min, float max)
        {
            return val >= min && val <= max;
        }
        
        public static float Interpolate(this float val, float min, float max)
        {
            float diff = max - min;
            float inter = diff * val;
            return min + inter;
        }
    }
}
#endif
