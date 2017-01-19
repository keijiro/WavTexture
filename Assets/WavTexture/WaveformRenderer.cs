using UnityEngine;

namespace WavTexture
{
    class WaveformRenderer : MonoBehaviour
    {
        [SerializeField] WavTexture _wavTexture;
        [SerializeField] int _channel;
        [SerializeField] AudioSource _positionSource;
        [SerializeField] Color _lineColor = new Color(1, 1, 1, 0.5f);

        [SerializeField, HideInInspector] Shader _shader;

        const int kVertexCount = 2048;

        Mesh _mesh;
        Material _material;
        MaterialPropertyBlock _propertyBlock;

        void Setup()
        {
            var vertices = new Vector3[kVertexCount];
            for (var i = 0; i < kVertexCount; i++)
                vertices[i] = new Vector3((float)i / (kVertexCount - 1), 0, 0);

            var indices = new int[(kVertexCount - 1) * 2];
            for (var i = 0; i < kVertexCount - 1; i++)
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

            _material = new Material(_shader);

            _propertyBlock = new MaterialPropertyBlock();
        }

        void OnDestroy()
        {
            Destroy(_mesh);
            Destroy(_material);
        }

        void Update()
        {
            if (_mesh == null) Setup();

            var time = _positionSource != null ? _positionSource.time : Time.time;
            _propertyBlock.SetTexture("_WavTex", _wavTexture.GetTexture(_channel));
            _propertyBlock.SetFloat("_StartTime", time * _wavTexture.sampleRate);
            _propertyBlock.SetFloat("_Duration", (float)_wavTexture.sampleRate / 60);
            _propertyBlock.SetColor("_Color", _lineColor);

            Graphics.DrawMesh(_mesh, transform.localToWorldMatrix, _material, gameObject.layer, null, 0, _propertyBlock);
        }
    }
}
