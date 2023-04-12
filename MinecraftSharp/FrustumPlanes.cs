using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubicus
{
    internal class FrustumPlanes
    {
        public float Left;
        public float Right;
        public float Bottom;
        public float Top;
        public float Near;
        public float Far;

        void CalculateFrustumPlanes(Matrix4 projectionMatrix, Matrix4 viewMatrix)
        {
            Matrix4 viewProjection = viewMatrix * projectionMatrix;
            Left = (viewProjection.M14 + viewProjection.M11) / viewProjection.M44;
            Right = (viewProjection.M14 - viewProjection.M11) / viewProjection.M44;
            Bottom = (viewProjection.M14 + viewProjection.M12) / viewProjection.M44;
            Top = (viewProjection.M14 - viewProjection.M12) / viewProjection.M44;
            Near = (viewProjection.M14 + viewProjection.M13) / viewProjection.M44;
            Far = (viewProjection.M14 - viewProjection.M13) / viewProjection.M44;
        }

        // Функция для проверки видимости объекта
        bool IsObjectVisible(Vector3 objectPosition, float objectRadius)
        {
            for (int i = 0; i < 6; i++)
            {
                float distance = Left;
                switch (i)
                {
                    case 0: distance = Left; break;
                    case 1: distance = Right; break;
                    case 2: distance = Bottom; break;
                    case 3: distance = Top; break;
                    case 4: distance = Near; break;
                    case 5: distance = Far; break;
                }
                if (distance < -objectRadius)
                {
                    return false;
                }
            }
            return true;
        }

    }
}
