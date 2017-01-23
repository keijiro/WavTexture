// WavTexture - Audio waveform to texture converter
// https://github.com/keijiro/WavTexture

using UnityEngine;

namespace WavTexture
{
    /// Texture-encoded audio waveform asset class.
    public class WavTexture : ScriptableObject
    {
        #region Serialized properties

        /// Number of channels.
        public int channelCount { get { return _channelCount; } }
        [SerializeField] int _channelCount;

        /// Sample rate.
        public int sampleRate { get { return _sampleRate; } }
        [SerializeField] int _sampleRate;

        /// Bit rate.
        public enum BitRate { Low, High }
        public BitRate bitRate { get { return _bitRate; } }
        [SerializeField] BitRate _bitRate;

        /// Audio length (in seconds).
        public float length { get { return _length; } }
        [SerializeField] float _length;

        /// Returns the texture object of the specified channel.
        public Texture2D GetTexture(int channel) { return _textures[channel]; }
        [SerializeField] Texture2D[] _textures;

        #endregion

        #region Initializer methods

        #if UNITY_EDITOR

        /// Asset initialization (editor only)
        public void Initialize(AudioClip source, BitRate bitRate)
        {
            _channelCount = source.channels;
            _sampleRate = source.frequency;
            _bitRate = bitRate;
            _length = source.length;
            _textures = new Texture2D[_channelCount];

            var format = bitRate == BitRate.High ?
                TextureFormat.RGBAHalf : TextureFormat.RGBA32;

            for (var i = 0; i < _channelCount; i++)
                _textures[i] = BakeClip(source, i, format);
        }

        // Bake an audio clip into a 2D texture.
        Texture2D BakeClip(AudioClip source, int channel, TextureFormat format)
        {
            var samples = new float[source.channels * source.samples];
            source.GetData(samples, 0);

            var width = 4096;
            var height = (source.samples + width - 1) / (4 * width);

            var tex = new Texture2D(width, height, format, false, true);
            tex.name = "Channel " + channel;
            tex.filterMode = FilterMode.Point;
            tex.wrapMode = TextureWrapMode.Clamp;

            var offs = channel;
            var stride = source.channels;
            for (var ty = 0; ty < height; ty++)
            {
                for (var tx = 0; tx < width; tx++)
                {
                    var s1 = offs < samples.Length ? samples[offs] : 0; offs += stride;
                    var s2 = offs < samples.Length ? samples[offs] : 0; offs += stride;
                    var s3 = offs < samples.Length ? samples[offs] : 0; offs += stride;
                    var s4 = offs < samples.Length ? samples[offs] : 0; offs += stride;

                    s1 = (s1 + 1) * 0.5f;
                    s2 = (s2 + 1) * 0.5f;
                    s3 = (s3 + 1) * 0.5f;
                    s4 = (s4 + 1) * 0.5f;

                    tex.SetPixel(tx, ty, new Color(s1, s2, s3, s4));
                }
            }

            tex.Apply(false, true);

            return tex;
        }

        #endif

        #endregion

        #region ScriptableObject functions

        void OnEnable()
        {
        }

        #endregion
    }
}
