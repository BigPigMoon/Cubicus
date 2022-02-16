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

        private const float SPEED = 3.5f;
        private const float SENSITIVITY = 0.16f;

        public Vector3 position;
        public Vector3 front;
        public Vector3 up;
        public Vector3 right;
        public Vector3 worldUp;

        public float yaw;
        public float pitch;
        public float movementSpeed = SPEED;
        public float mouseSensitivity = SENSITIVITY;
        public float zoom = 45.0f;

        public Camera(float posX, float posY, float posZ, float upX, float upY, float upZ, float yaw, float pitch)
        { 
            position = new Vector3(posX, posY, posZ);
            worldUp = new Vector3(upX, upY, upZ);
            front = new Vector3(0.0f, 0.0f, -1.0f);
            this.yaw = yaw;
            this.pitch = pitch;

            UpdateCameraVectors();
        }
        
        public Camera(Vector3 pos, Vector3 up, float yaw, float pitch)
        { 
            position = pos;
            worldUp = up;
            front = new Vector3(0.0f, 0.0f, -1.0f);
            this.yaw = yaw;
            this.pitch = pitch;

            UpdateCameraVectors();
        }

        public Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(position, position + front, up);
        }

        public void ProcessKeyboard(CameraMovement dir, float deltaTime)
        {
            float velocity = movementSpeed * deltaTime;

            if (dir == CameraMovement.Forward)
            {
                position += front * velocity;
            }
            if (dir == CameraMovement.Backward)
            {
                position.X -= front.X * velocity;
                position.Z -= front.Z * velocity;
            }
            if (dir == CameraMovement.Left)
                position -= right * velocity;
            if (dir == CameraMovement.Right)
                position += right * velocity;
            if (dir == CameraMovement.Up)
                position += new Vector3(0.0f, 1.0f, 0.0f) * velocity;
            if (dir == CameraMovement.Down)
                position -= new Vector3(0.0f, 1.0f, 0.0f) * velocity;
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
            front.X = (float)(MathHelper.Cos(MathHelper.DegreesToRadians(yaw)) * MathHelper.Cos(MathHelper.DegreesToRadians(pitch)));
            front.Y = (float)(MathHelper.Sin(MathHelper.DegreesToRadians(pitch)));
            front.Z = (float)(MathHelper.Sin(MathHelper.DegreesToRadians(yaw)) * MathHelper.Cos(MathHelper.DegreesToRadians(pitch)));

            front.Normalize();
            right = Vector3.Normalize(Vector3.Cross(front, worldUp));
            up = Vector3.Normalize(Vector3.Cross(right, front));
        }
    }
}
