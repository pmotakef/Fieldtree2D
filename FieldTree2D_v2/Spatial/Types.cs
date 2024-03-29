﻿
namespace FieldTree2D_v2.Spatial
{
    public enum CreationMode
    {
        ALL_AT_ONCE = 1,
        LAZY = 2,
    }

    public enum NodeType
    {
        COVER_NODE = 1,
        PARTITION_NODE = 2,
    }

    public static class Statics
    {
        public const float EPSILON = 0.00001f;
    }
}
