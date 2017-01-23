// WavTexture - Audio waveform to texture converter
// https://github.com/keijiro/WavTexture

using UnityEngine;

namespace WavTexture
{
    /// Basic waveform graph renderer.
    class WaveformRenderer : MonoBehaviour
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

        /// Color of the graph line.
        public Color color {
            get { return _color; }
            set { _color = value; }
        }

        [SerializeField, Tooltip("Color of the graph line.")]
        Color _color = new Color(1, 1, 1, 0.5f);

        /// Graph resolution (the number of vertices in the graph).
        public int resolution {
            get { return _resolution; }
            set { _resolution = value; OnValidate(); }
        }

        [SerializeField, Tooltip("Graph resolution (the number of vertices in the graph).")]
        int _resolution = 1024;

        #endregion

        #region Internal resources

        [SerializeField, HideInInspector] Shader _shader;

        #endregion

        #region Private members

        Mesh _mesh;
        Material _material;
        MaterialPropertyBlock _propertyBlock;

        void Setup()
        {
            if (_mesh == null)
            {
                // Simple straight line mesh.
                var vertices = new Vector3[_resolution];
                for (var i = 0; i < _resolution; i++)
                    vertices[i] = new Vector3((float)i / (_resolution - 1), 0, 0);

                var indices = new int[(_resolution - 1) * 2];
                for (var i = 0; i < _resolution - 1; i++)
                {
                    indices[i * 2] = i;
                    indices[i * 2 + 1] = i + 1;
                }

                _mesh = new Mesh();
                _mesh.hideFlags = HideFlags.DontSave;
                _mesh.vertices = vertices;
                _mesh.SetIndices(indices, MeshTopology.Lines, 0);
                _mesh.bounds = new Bounds(Vector3.zero, Vector3.one);
                _mesh.UploadMeshData(true);
            }

            if (_material == null)
            {
                _material = new Material(_shader);
                _propertyBlock = new MaterialPropertyBlock();
            }
        }

        #endregion

        #region MonoBehaviour functions

        void OnValidate()
        {
            _resolution = Mathf.Clamp(_resolution, 2, 65535);

            if (_mesh != null)
            {
                Destroy(_mesh);
                _mesh = null;
            }
        }

        void OnDestroy()
        {
            if (_mesh != null)
                Destroy(_mesh);

            if (_material != null)
                Destroy(_material);
        }

        void Update()
        {
            Setup();

            var time = _positionSource != null ? _positionSource.time : Time.time;
            _propertyBlock.SetTexture("_WavTex", _wavTexture.GetTexture(_channel));
            _propertyBlock.SetFloat("_StartTime", time * _wavTexture.sampleRate);
            _propertyBlock.SetFloat("_Duration", (float)_wavTexture.sampleRate / 60);
            _propertyBlock.SetColor("_Color", _color);

            Graphics.DrawMesh(
                _mesh, transform.localToWorldMatrix, _material,
                gameObject.layer, null, 0, _propertyBlock
            );
        }

        #endregion
    }
}
