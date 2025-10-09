using UnityEngine;

namespace Player.Movement
{
    public class CameraRelativeDirection
    {
        private readonly Camera camera;
        private bool initialized;
        private Vector3 camF; //forward on XZ
        private Vector3 camR; // right on XZ

        public CameraRelativeDirection()
        {
            camera = Camera.main;
        }
        public CameraRelativeDirection(Camera camera)
        {
            this.camera = camera != null ? camera : Camera.main;
        }

        private void InitAxes()
        {
            if (initialized) return;
            Debug.Assert(camera is not null, "_camera != null");
            
            camF = camera.transform.forward;
            camF.y = 0f;
            camF.Normalize();
            
            camR = new Vector3(camF.z, 0f, -camF.x);
            initialized = true;
        }

        public Vector3 Map(Vector2 input)
        {
            InitAxes();
            return camR * input.x + camF * input.y;
        }
    }
}