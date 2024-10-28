using UnityEngine;

namespace SimpleParticleSystem
{
    public partial class SimpleParticleSystem
    {
        private void OnGUI()
        {
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
            GUILayout.Label($"Particles In scene: {_particles.Count}");
        }
    }
}