using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Framework
{
    public class DeleteMissingScripts
    {
        //[MenuItem("Tools/Delete Missing Scripts")]
        static void CleanupMissingScript()
        {
            GameObject[] pAllObjects = (GameObject[]) Resources.FindObjectsOfTypeAll(typeof(GameObject));

            int r;
            int j;
            for (int i = 0; i < pAllObjects.Length; i++)
            {
                if (pAllObjects[i].hideFlags == HideFlags.None) //HideFlags.None 获取Hierarchy面板所有Object
                {
                    var components = pAllObjects[i].GetComponents<Component>();
                    var serializedObject = new SerializedObject(pAllObjects[i]);
                    var prop = serializedObject.FindProperty("m_Component");
                    r = 0;

                    for (j = 0; j < components.Length; j++)
                    {
                        if (components[j] == null)
                        {
                            prop.DeleteArrayElementAtIndex(j - r);
                            r++;
                        }
                    }

                    serializedObject.ApplyModifiedProperties();
                }
            }
        }

        [MenuItem("Tools/Find Missing Refrence In Current Scene",false,49)]
        public static void FindMissingRefrencesInCurrentScene()
        {
            var objects = GetSceneObjects();

            FindMissingReferences(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, objects);
        }
        [MenuItem("Assets/Find Missing Refrence Select Object",false,49)]
        public static void FindMissingRefrencesInCurrentObject()
        {
            var objects = Selection.gameObjects;

            FindMissingReferences(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, objects);
        }
        private static GameObject[] GetSceneObjects() => Resources.FindObjectsOfTypeAll<GameObject>().Where(go =>
            string.IsNullOrEmpty(AssetDatabase.GetAssetPath(go)) && go.hideFlags == HideFlags.None).ToArray();

        private static void FindMissingReferences(string context, GameObject[] objects)
        {
            foreach (var go in objects)
            {
                var components = go.GetComponents<Component>();
                foreach (var c in components)
                {
                    if (!c)
                    {
                        Debug.LogError("Missing Component in GO: " + FullPath(go), go);
                        GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
                        continue;
                    }
                    SerializedObject so = new SerializedObject(c);
                    var sp = so.GetIterator();
                    while (sp.NextVisible(true))
                    {
                        if (sp.propertyType == SerializedPropertyType.ObjectReference)
                        {
                            if (sp.objectReferenceValue == null && sp.objectReferenceInstanceIDValue != 0)
                            {
                                ShowError(context, go, c.GetType().Name, ObjectNames.NicifyVariableName(sp.name));
                            }
                        }
                    }
                }
            }
        }

        private static void ShowError(string context, GameObject go, string c, string property) =>
            Debug.LogError($"Missing Refrence in : [{context}]{FullPath(go)}. Component: {c}, Property: {property}");

        private static string FullPath(GameObject go) => go.transform.parent == null
            ? go.name
            : FullPath(go.transform.parent.gameObject) + "/" + go.name;
    }
}