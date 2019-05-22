using System;
using UnityEngine;
using System.Collections.Generic;
/// <summary>
/// <para>Class Introduce</para>
/// <para>Author: zhengnan</para>
/// <para>Create: 2019/5/22 22:11:09</para>
/// </summary> 

namespace Framework
{
    [RequireComponent(typeof(MeshFilter))]
    public class CubeMesh : MonoBehaviour
    {
        private MeshFilter _meshFilter;
        private Mesh _mesh;

        private List<Vector3> _verticesList = new List<Vector3>();
        private List<Vector2> _uvList = new List<Vector2>();
        private List<int> _triList = new List<int>();

        public Mesh mesh
        {
            get
            {
                return _mesh;
            }
        }
        private void Start()
        {
            _meshFilter = this.GetComponent<MeshFilter>();
            _mesh = _meshFilter.mesh;

            RebuildMesh();

        }
        private void RebuildMesh()
        {
            _mesh = GetCubeMesh();
            _meshFilter.mesh = _mesh;
        }

        private Mesh GetCubeMesh()
        {
            Mesh m = new Mesh();

            _verticesList.Add(new Vector3(0.5f, -0.5f, -0.5f));
            _verticesList.Add(new Vector3(-0.5f, -0.5f, -0.5f));
            _verticesList.Add(new Vector3(-0.5f, 0.5f, -0.5f));
            _verticesList.Add(new Vector3(0.5f, 0.5f, -0.5f));

            _verticesList.Add(new Vector3(0.5f, -0.5f, 0.5f));
            _verticesList.Add(new Vector3(-0.5f, -0.5f, 0.5f));
            _verticesList.Add(new Vector3(-0.5f, 0.5f, 0.5f));
            _verticesList.Add(new Vector3(0.5f, 0.5f, 0.5f));

            m.SetVertices(_verticesList);

            _uvList.Add(new Vector2(1, 0));
            _uvList.Add(new Vector2(0, 0));
            _uvList.Add(new Vector2(0, 1));
            _uvList.Add(new Vector2(1, 1));

            _uvList.Add(new Vector2(1, 0));
            _uvList.Add(new Vector2(0, 0));
            _uvList.Add(new Vector2(0, 1));
            _uvList.Add(new Vector2(1, 1));

            m.SetUVs(0, _uvList);


            // 前面
            _triList.Add(0);
            _triList.Add(1);
            _triList.Add(2);


            _triList.Add(0);
            _triList.Add(2);
            _triList.Add(3);


            // 后面
            _triList.Add(4);
            _triList.Add(7);
            _triList.Add(5);

            _triList.Add(5);
            _triList.Add(7);
            _triList.Add(6);


            // 左面
            _triList.Add(1);
            _triList.Add(5);
            _triList.Add(6);

            _triList.Add(1);
            _triList.Add(6);
            _triList.Add(2);


            // 右面
            _triList.Add(0);
            _triList.Add(3);
            _triList.Add(4);

            _triList.Add(3);
            _triList.Add(7);
            _triList.Add(4);

            // 上面
            _triList.Add(2);
            _triList.Add(6);
            _triList.Add(3);

            _triList.Add(3);
            _triList.Add(6);
            _triList.Add(7);

            // 下面
            _triList.Add(1);
            _triList.Add(4);
            _triList.Add(5);

            _triList.Add(0);
            _triList.Add(4);
            _triList.Add(1);

            m.SetTriangles(_triList, 0);

            return m;

        }
    }
}
    

