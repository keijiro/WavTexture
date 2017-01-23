// WavTexture - Audio waveform to texture converter
// https://github.com/keijiro/WavTexture

using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace WavTexture
{
    [CustomEditor(typeof(WavTexture))]
    public class WavTextureEditor : Editor
    {
        #region Inspector functions

        public override void OnInspectorGUI()
        {
            var instance = (WavTexture)target;
            EditorGUILayout.LabelField("Channels", instance.channelCount.ToString());
            EditorGUILayout.LabelField("Sample Rate", instance.sampleRate + " Hz");
            EditorGUILayout.LabelField("Bit Rate", instance.bitRate.ToString());
            EditorGUILayout.LabelField("Length", instance.length + " seconds");
        }

        #endregion

        #region Context menu functions

        static AudioClip[] SelectedAudioClipAssets {
            get {
                var assets = Selection.GetFiltered(typeof(AudioClip), SelectionMode.Deep);
                return assets.Select(x => (AudioClip)x).ToArray();
            }
        }

        [MenuItem("Assets/WavTexture/Convert Clip (Low Bit Rate)", true)]
        static bool ValidateAssetsLowBitRate()
        {
            return SelectedAudioClipAssets.Length > 0;
        }

        [MenuItem("Assets/WavTexture/Convert Clip (Low Bit Rate)")]
        static void ConvertAssetsLowBitRate()
        {
            ConvertAssets(WavTexture.BitRate.Low);
        }

        [MenuItem("Assets/WavTexture/Convert Clip (High Bit Rate)", true)]
        static bool ValidateAssetsHighBitRate()
        {
            return SelectedAudioClipAssets.Length > 0;
        }

        [MenuItem("Assets/WavTexture/Convert Clip (High Bit Rate)")]
        static void ConvertAssetsHighBitRate()
        {
            ConvertAssets(WavTexture.BitRate.High);
        }

        static void ConvertAssets(WavTexture.BitRate bitRate)
        {
            var converted = new List<Object>();

            foreach (var source in SelectedAudioClipAssets)
            {
                // Destination file path.
                var dirPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(source));
                var filename = source.name + " WavTexture.asset";
                var assetPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(dirPath, filename));

                // Create a converted WavTexture asset.
                var temp = ScriptableObject.CreateInstance<WavTexture>();
                temp.Initialize(source, bitRate);
                AssetDatabase.CreateAsset(temp, assetPath);
                for (var i = 0; i < temp.channelCount; i++)
                    AssetDatabase.AddObjectToAsset(temp.GetTexture(i), temp);

                converted.Add(temp);
            }

            // Save the generated assets.
            AssetDatabase.SaveAssets();

            // Select the generated assets.
            EditorUtility.FocusProjectWindow();
            Selection.objects = converted.ToArray();
        }

        #endregion
    }
}
