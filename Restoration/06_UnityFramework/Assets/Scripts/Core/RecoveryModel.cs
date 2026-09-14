using System;
using UnityEngine;
namespace CoinMerge.Recovery
{
    [Serializable] public class RecoveryModel { public string variant, uuid, name, type; public int originalObjectCount; public NodeModel[] nodes; }
    [Serializable] public class NodeModel
    {
        public int id, parent; public string name, rawJson; public int[] children;
        public bool active; public float[] position, rotation, scale, size, pivot, color;
        public ComponentModel[] components;
    }
    [Serializable] public class ComponentModel
    {
        public string type, className, rawJson, sprite, text, font;
        public int sourceId, fontSize, lineHeight, horizontalAlign, verticalAlign, spriteType, fillType, target, content, viewport, bodyType;
        public float fillRange, fillStart, radius, gravityScale, linearDamping, angularDamping, friction, restitution, density;
        public float[] offset, size;
        public bool enabled, sensor, isCustom;
    }
    [Serializable] public class SpriteImportModel { public SpriteImportEntry[] sprites; }
    [Serializable] public class SpriteImportEntry { public string uuid, variant, path; public float[] border; }
}
