using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement
{
    public abstract class Movement
    {
        public abstract string Name { get; }
        public virtual Color DebugColor { get; set; }

        public Color OutputDebugColor { get; set; }
        public bool OutputDebug { get; set; }

        public abstract MovementOutput GetMovement();
    }
}
