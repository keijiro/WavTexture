using UnityEngine;

namespace WavTexture
{
    class LevelMonitor : MonoBehaviour
    {
        [SerializeField] WavTexture _wavTexture;
        [SerializeField] int _channel;
        [SerializeField] AudioSource _positionSource;
        [SerializeField] Renderer[] _renderers;

        [SerializeField, HideInInspector] Shader _shader;

        Material _material;
        MaterialPropertyBlock _propertyBlock;

        RenderTexture _reductionBuffer;
        RenderTexture _resultBuffer;

        public Texture levelTexture { get { return _resultBuffer; } }

        void Setup()
        {
            var format = RenderTextureFormat.RHalf;
            _reductionBuffer = new RenderTexture(32, 32, 0, format);
            _resultBuffer = new RenderTexture(1, 1, 0, format);
            _reductionBuffer.filterMode = FilterMode.Point;
            _resultBuffer.filterMode = FilterMode.Point;

            _material = new Material(_shader);

            _propertyBlock = new MaterialPropertyBlock();
            _propertyBlock.SetTexture("_LevelTex", _resultBuffer);
        }

        void OnDestroy()
        {
            Destroy(_material);
            Destroy(_reductionBuffer);
            Destroy(_resultBuffer);
        }

        void Update()
        {
            if (_material == null) Setup();

            var time = _positionSource != null ? _positionSource.time : Time.time;
            _material.SetFloat("_StartTime", time * _wavTexture.sampleRate);
            _material.SetFloat("_Duration", (float)_wavTexture.sampleRate / 60);

            Graphics.Blit(_wavTexture.GetTexture(_channel), _reductionBuffer, _material, 0);
            Graphics.Blit(_reductionBuffer, _resultBuffer, _material, 1);

            foreach (var r in _renderers) r.SetPropertyBlock(_propertyBlock);
        }
    }
}
