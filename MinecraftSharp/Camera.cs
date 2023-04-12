using System;
using OpenTK.Mathematics;

namespace Cubicus
{

    public class Camera
    {
        public enum CameraMovement
        {
            Forward,
            Backward,
            Left,
            Right,
            Up,
            Down
        }

        private const float SPEED = 7.5f;
        private const float SENSITIVITY = 0.16f;

        public Vector3 position;
        public Vector3 forward;
        public Vector3 up;
        public Vector3 right;
        public Vector3 worldUp;
        public Vector3 lookAt;

        // рысканье
        public float yaw;
        // тангаж
        public float pitch;
        public float movementSpeed = SPEED;
        public float mouseSensitivity = SENSITIVITY;
        public float zoom = MathHelper.PiOver2;
        
        public Camera(Vector3 pos, Vector3 up, float yaw, float pitch)
        { 
            position = pos;
            worldUp = up;
            forward = new Vector3(0.0f, 0.0f, -1.0f);
            this.yaw = yaw;
            this.pitch = pitch;

            UpdateCameraVectors();
        }

        public Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(position, position + lookAt, up);
        }

        public void ProcessKeyboard(CameraMovement dir, float deltaTime)
        {
            var inputVector = GetInputVector(dir);

            //var direction = new Vector3(forward.X, 0.0f, forward.Z).Normalized() * inputVector.Z;

            var direction = forward * inputVector.Z + right * inputVector.X + new Vector3(0, up.Y, 0) * inputVector.Y;

            position += direction * movementSpeed * deltaTime;
        }

        private Vector3 GetInputVector(CameraMovement dir)
        {
            var inputVector = new Vector3();

            if (dir == CameraMovement.Forward)
                inputVector.Z = 1;

            if (dir == CameraMovement.Backward)
                inputVector.Z = -1;

            if (dir == CameraMovement.Right)
                inputVector.X = 1;

            if (dir == CameraMovement.Left)
                inputVector.X = -1;

            if (dir == CameraMovement.Up)
                inputVector.Y = 1;

            if (dir == CameraMovement.Down)
                inputVector.Y = -1;

            return inputVector;
        }

        public void ProcessMouseMovement(float xoffset, float yoffset)
        {
            xoffset *= mouseSensitivity;
            yoffset *= mouseSensitivity;

            yaw += xoffset;
            pitch += yoffset;

            if (pitch > 89.0f)
                pitch = 89.0f;
            if (pitch < -89.0f)
                pitch = -89.0f;

            UpdateCameraVectors();
        }

        private void UpdateCameraVectors()
        {
            float cosPitch = (float)MathHelper.Cos(MathHelper.DegreesToRadians(pitch));
            float sinPitch = (float)MathHelper.Sin(MathHelper.DegreesToRadians(pitch));
            float cosYaw = (float)MathHelper.Cos(MathHelper.DegreesToRadians(yaw));
            float sinYaw = (float)MathHelper.Sin(MathHelper.DegreesToRadians(yaw));

            lookAt = new Vector3(cosYaw * cosPitch, sinPitch, sinYaw * cosPitch);
            forward = new Vector3(cosYaw * cosPitch, 0, sinYaw * cosPitch).Normalized();
            right = Vector3.Normalize(Vector3.Cross(forward, worldUp));
            up = Vector3.Normalize(Vector3.Cross(right, forward));
        }
    }
}
