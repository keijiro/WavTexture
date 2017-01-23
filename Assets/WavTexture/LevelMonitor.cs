// WavTexture - Audio waveform to texture converter
// https://github.com/keijiro/WavTexture

using UnityEngine;

namespace WavTexture
{
    /// Calculate RMS level of an audio waveform.
    class LevelMonitor : MonoBehaviour
    {
        #region Editable properties

        /// Source waveform.
        public WavTexture wavTexture {
            get { return _wavTexture; }
            set { _wavTexture = value; }
        }

        [SerializeField, Tooltip("Source waveform.")]
        WavTexture _wavTexture;

        /// Specifies which channel in the wavTexture to be rendered.
        public int channel {
            get { return _channel; }
            set { _channel = value; }
        }

        [SerializeField, Tooltip("Specifies which channel in the wavTexture to be rendered.")]
        int _channel;

        /// Synchronizes the position to this audio source.
        public AudioSource positionSource {
            get { return _positionSource; }
            set { _positionSource = value; }
        }

        [SerializeField, Tooltip("Synchronized the position to this audio source.")]
        AudioSource _positionSource;

        [SerializeField, Tooltip("Renderers to be controlled by this monitor.")]
        Renderer[] _renderers;

        #endregion

        #region Runtime public properties

        /// Returns a texture that contains the current level.
        public Texture levelTexture { get { return _resultBuffer; } }

        #endregion

        #region Internal resources

        [SerializeField, HideInInspector] Shader _shader;

        #endregion

        #region Private members

        Material _material;
        MaterialPropertyBlock _propertyBlock;
        RenderTexture _cropBuffer;
        RenderTexture _resultBuffer;

        void Setup()
        {
            _material = new Material(_shader);
            _propertyBlock = new MaterialPropertyBlock();

            var format = RenderTextureFormat.RHalf;

            _cropBuffer = new RenderTexture(32, 32, 0, format); // 1024 points
            _cropBuffer.filterMode = FilterMode.Point;

            _resultBuffer = new RenderTexture(1, 1, 0, format);
            _resultBuffer.filterMode = FilterMode.Point;
        }

        void OnDestroy()
        {
            Destroy(_material);
            Destroy(_cropBuffer);
            Destroy(_resultBuffer);
        }

        void Update()
        {
            if (_material == null) Setup();

            var time = _positionSource != null ? _positionSource.time : Time.time;
            _material.SetFloat("_StartTime", time * _wavTexture.sampleRate);
            _material.SetFloat("_Duration", (float)_wavTexture.sampleRate / 60);
            _material.SetFloat("_CropRes", 1.0f / _cropBuffer.width);

            // Crop the current frame from the waveform.
            Graphics.Blit(_wavTexture.GetTexture(_channel), _cropBuffer, _material, 0);

            // Calculate the RMS level.
            Graphics.Blit(_cropBuffer, _resultBuffer, _material, 1);

            // Update the external renderers.
            _propertyBlock.SetTexture("_LevelTex", _resultBuffer);
            foreach (var r in _renderers) r.SetPropertyBlock(_propertyBlock);
        }

        #endregion
    }
}
