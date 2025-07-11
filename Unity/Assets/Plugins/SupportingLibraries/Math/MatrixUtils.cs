using UnityEngine;

namespace MathUtils {

    public static class MatrixExtensions {
        public static Quaternion ExtractRotation(this Matrix4x4 m) {
            Quaternion q = new Quaternion();
            q.w =Mathf.Sqrt(Mathf.Max(0,1+ m[0,0]+ m[1,1]+ m[2,2]))/2;
            q.x =Mathf.Sqrt(Mathf.Max(0,1+ m[0,0]- m[1,1]- m[2,2]))/2;
            q.y =Mathf.Sqrt(Mathf.Max(0,1- m[0,0]+ m[1,1]- m[2,2]))/2;
            q.z =Mathf.Sqrt(Mathf.Max(0,1- m[0,0]- m[1,1]+ m[2,2]))/2;
            q.x *=Mathf.Sign(q.x *(m[2,1]- m[1,2]));
            q.y *=Mathf.Sign(q.y *(m[0,2]- m[2,0]));
            q.z *=Mathf.Sign(q.z *(m[1,0]- m[0,1]));

            return q;
        }

        public static Vector3 ExtractPosition(this Matrix4x4 matrix) {
            Vector3 position;
            position.x = matrix.m03;
            position.y = matrix.m13;
            position.z = matrix.m23;
            return position;
        }

        public static Vector3 ExtractScale(this Matrix4x4 matrix) {
            Vector3 scale;
            scale.x = new Vector4(matrix.m00, matrix.m10, matrix.m20, matrix.m30).magnitude;
            scale.y = new Vector4(matrix.m01, matrix.m11, matrix.m21, matrix.m31).magnitude;
            scale.z = new Vector4(matrix.m02, matrix.m12, matrix.m22, matrix.m32).magnitude;
            return scale;
        }
    }
}
