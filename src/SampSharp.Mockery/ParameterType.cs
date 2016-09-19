namespace SampSharp.Mockery
{
    public enum ParameterType
    {
        [ParameterType(typeof(string))]
        String,
        [ParameterType(typeof(int))]
        Int,
        [ParameterType(typeof(float))]
        Float,
        [ParameterType(typeof(bool))]
        Bool,
        [ParameterType(typeof(int[]))]
        IntArray,
        [ParameterType(typeof(float[]))]
        FloatArray,
        [ParameterType(typeof(bool[]))]
        BoolArray,
    }
}