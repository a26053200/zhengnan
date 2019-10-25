using System;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine.Serialization;

namespace BM
{
    /// <summary>
    /// <para>Author: zhengnan</para>
    /// <para>Create: 2019/6/13 22:13:32</para>
    /// </summary> 
    public class BMSettings : ScriptableObject
    {
        //应用名字
        public string AppName = "AppName";

        public string BuildOutputDirName = "AssetBundle";

        public string resDir = "Assets/Res/";

        public bool useHashName = false;

        public bool clearManifestFile = true;
        //lua文件夹
        public List<string> luaFolderList = new List<string>();
        //打包类型
        public BuildType luaBuildType;
        //
        public CompressType luaCompressType;
        //
        public string luaPattern = "*.lua";

        //场景文件夹
        public List<string> scenesFolderList = new List<string>();
        //打包类型
        public BuildType scenesBuildType;             
        //
        public CompressType scenesCompressType;
        //后缀
        public string scenesPattern = "*.unity";

        //普通文件夹
        public List<string> singleFolderList = new List<string>();
        //打包类型
        public BuildType singleBuildType;
        //
        public CompressType singleCompressType;
        //后缀
        public string singlePattern = "*.*";

        //打包文件夹
        public List<string> packFolderList = new List<string>();
        //打包类型
        public BuildType packBuildType;
        //
        public CompressType packCompressType;
        //后缀
        public string packPattern = "*.*";

        //shader文件夹
        public List<string> shaderFolderList = new List<string>();
        //打包类型
        public BuildType shaderBuildType;
        //
        public CompressType shaderCompressType;
        //后缀
        public string shaderPattern = "*.*";

        //后缀
        public string Suffix_Bundle = "bundle";
        //场景文件后缀
        public string Scene_Suffix = "*.unity";
        //忽略后缀文件
        public string Ignore_Suffix = ".meta,.DS_Store";
        //忽略文件夹
        public string Ignore_Folder = ".svn";

        //拥有依赖性的文件后缀名
        public List<string> ownerDependenceSuffixs = new List<string>();

        //SpriteAtlas文件夹
        public List<string> atlasSpriteFolderList = new List<string>();

        public string atlasSpritePrefabDir;

        //场景版本号
        [HideInInspector] 
        public List<string> scenePaths = new List<string>();
        
        [HideInInspector]
        public List<int> sceneVersions = new List<int>();
    }
}
    

