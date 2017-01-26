// WavTexture - Audio waveform to texture converter
// https://github.com/keijiro/WavTexture

using UnityEngine;

namespace WavTexture
{
    /// Crops the current frame of the waveform and fade it to renderers.
    class WaveformProvider : MonoBehaviour
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

        /// Returns a texture that contains the current frame of the waveform.
        public Texture waveFrameTexture { get { return _cropBuffer; } }

        #endregion

        #region Internal resources

        [SerializeField, HideInInspector] Shader _shader;

        #endregion

        #region Private members

        Material _material;
        MaterialPropertyBlock _propertyBlock;
        RenderTexture _cropBuffer;

        void Setup()
        {
            _material = new Material(_shader);
            _propertyBlock = new MaterialPropertyBlock();
            _cropBuffer = new RenderTexture(1024, 1, 0, RenderTextureFormat.RHalf);
        }

        void OnDestroy()
        {
            Destroy(_material);
            Destroy(_cropBuffer);
        }

        void Update()
        {
            if (_material == null) Setup();

            var time = _positionSource != null ? _positionSource.time : Time.time;
            _material.SetFloat("_StartTime", time * _wavTexture.sampleRate);
            _material.SetFloat("_Duration", (float)_wavTexture.sampleRate / 60);

            // Crop the current frame from the waveform.
            Graphics.Blit(_wavTexture.GetTexture(_channel), _cropBuffer, _material, 0);

            // Update the external renderers.
            _propertyBlock.SetTexture("_WaveformTex", _cropBuffer);
            foreach (var r in _renderers) r.SetPropertyBlock(_propertyBlock);
        }

        #endregion
    }
}
