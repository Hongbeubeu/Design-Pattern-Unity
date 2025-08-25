using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace hcore.SimpleParticleSystem
{
    public partial class SimpleParticleSystem
    {
        private void OnDrawGizmosSelected()
        {
            DrawGizmoShape();
        }

        private void DrawGizmoShape()
        {
            switch (shapeEmission.shapeEmissionType)
            {
                case ShapeEmissionType.Point:
                    Gizmos.color = Color.green;
                    Gizmos.DrawSphere(transform.position + shapeEmission.position, 0.1f); // Điểm nhỏ cho Point
                    break;

                case ShapeEmissionType.Circle:
                    Gizmos.color = Color.green;
                    DrawWireCircle(transform.position + shapeEmission.position, shapeEmission.radius); // Vẽ vòng tròn cho Circle
                    break;

                case ShapeEmissionType.Sphere:
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireSphere(transform.position + shapeEmission.position, shapeEmission.radius); // Vẽ Sphere
                    break;

                case ShapeEmissionType.Box:
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireCube(transform.position + shapeEmission.position, shapeEmission.size); // Vẽ Box
                    break;
            }
        }

        private static void DrawWireCircle(Vector3 center, float radius)
        {
            int segments = 64;
            float angleStep = 360f / segments;
            Vector3 prevPoint = center + new Vector3(radius, 0, 0);

            for (int i = 1; i <= segments; i++)
            {
                float angle = i * angleStep * Mathf.Deg2Rad;
                Vector3 newPoint = center + new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
                Gizmos.DrawLine(prevPoint, newPoint);
                prevPoint = newPoint;
            }
        }


        private void OnGUI()
        {
            if (Selection.activeGameObject != gameObject)
                return;
            if (GUILayout.Button("Play", new GUIStyle(GUI.skin.button)
                                         {
                                             fontSize = 20,
                                             fixedWidth = 150,
                                             fixedHeight = 50
                                         }))
            {
                Play();
            }

            if (GUILayout.Button("Stop", new GUIStyle(GUI.skin.button)
                                         {
                                             fontSize = 20,
                                             fixedWidth = 150,
                                             fixedHeight = 50
                                         }))
            {
                Stop();
            }

            if (GUILayout.Button("Force Stop", new GUIStyle(GUI.skin.button)
                                               {
                                                   fontSize = 20,
                                                   fixedWidth = 150,
                                                   fixedHeight = 50
                                               }))
            {
                Stop(true);
            }

            var title = isPaused ? "Resume" : "Pause";
            if (GUILayout.Button(title, new GUIStyle(GUI.skin.button)
                                        {
                                            fontSize = 20,
                                            fixedWidth = 150,
                                            fixedHeight = 50
                                        }))
            {
                isPaused = !isPaused;
            }

            GUILayout.Label($"Particles In pool: {SimpleParticlePooler.Count}");
            GUILayout.Label($"Particles In scene: {Particles.Count}");
        }
    }
}
#endif