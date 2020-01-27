#if STANDALONE
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class EditorGUILayoutUtil
{

    public static bool PopupNullable<T>(string label, ref T selection, IList<T> items, params GUILayoutOption[] options)
    {
        T sel = selection;
        bool changed = PopupNullable<T>(label, ref sel, items, ObjToString.ScenePathToString, options);
        selection = sel;
        return changed;
    }

    public static bool PopupNullable<T>(string label, ref T selection, IList<T> items, ToStr toString, params GUILayoutOption[] options)
    {
        if (items.Count == 0)
        {
            bool changed = false;
            Color old = GUI.backgroundColor;
            GUI.backgroundColor = Color.red;
            if (label == null)
            {
                int i = EditorGUILayout.Popup(1, new string[] { "-", selection.ToString() }, options);
                if (i == 0)
                {
                    selection = default(T);
                    changed = true;
                }
            }
            else
            {
                int i = EditorGUILayout.Popup(label, 1, new string[] { "-", selection.ToString() }, options);
                if (i == 0)
                {
                    selection = default(T);
                    changed = true;
                }
            }
            GUI.backgroundColor = old;
            return changed;
        }

        int index = 0;
        string[] str = new string[items.Count + 1];
        str[0] = "-";
        for (int i = 1; i <= items.Count; i++)
        {
            str[i] = toString(items[i - 1]);
            if (object.Equals(items[i - 1], selection))
            {
                index = i;
            }
        }
        int newIndex = 0;
        if (label == null)
        {
            newIndex = EditorGUILayout.Popup(index, str, options);
        }
        else
        {
            newIndex = EditorGUILayout.Popup(label, index, str, options);
        }
        if (newIndex == 0)
        {
            selection = default(T);
        }
        else
        {
            selection = items[newIndex - 1];
        }
        return newIndex != index;
    }

    /// <summary>
    /// Draws the selector.
    /// </summary>
    /// <returns><c>true</c>, if selection was changed, <c>false</c> otherwise.</returns>
    public static bool PopupFiltered<T>(string label, ref T selection, IList<T> items, ref string filter, params GUILayoutOption[] options)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(label, GUILayout.Width(50));
        EditorGUILayoutUtil.TextField(null, ref filter, EditorStyles.toolbarTextField);
        IList<T> filtered = null;
        if (!filter.IsEmpty())
        {
            filtered = new List<T>(items.Count);
            foreach (T i in items)
            {
                if (i.ToString().Contains(filter))
                {
                    filtered.Add(i);
                }
            }
        }
        else
        {
            filtered = items;
        }
        bool changed = EditorGUILayoutUtil.PopupNullable<T>(null, ref selection, filtered);
        EditorGUILayout.EndHorizontal();
        return changed;
    }

    public static bool SearchField(string label, ref string str)
    {
        string s = str;
        EditorGUILayout.BeginHorizontal();
        EditorGUILayoutUtil.TextField("", ref s, new GUIStyle("SearchTextField"));
        if (GUILayout.Button("", "SearchCancelButton", GUILayout.Width(18f)))
        {
            s = "";
            GUIUtility.keyboardControl = 0;
        }
        EditorGUILayout.EndHorizontal();
        bool changed = s != str;
        str = s;
        return changed;
    }

    public static bool TextField(string label, ref string str, params GUILayoutOption[] options)
    {
        return TextField(label, ref str, EditorStyles.textField, options);
    }

    public static bool TextField(string label, ref string str, GUIStyle style, params GUILayoutOption[] options)
    {
        if (str == null)
        {
            str = "";
        }
        string newStr = label == null ?
            EditorGUILayout.TextField(str, style, options) :
            EditorGUILayout.TextField(label, str, style, options);
        bool changed = newStr != str;
        str = newStr;
        return changed;
    }

    public static bool TextArea(string title, ref string str, params GUILayoutOption[] options)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(title);
        bool changed = TextArea(ref str, options);
        EditorGUILayout.EndHorizontal();
        return changed;
    }

    public static bool TextArea(ref string str, params GUILayoutOption[] options)
    {
        return TextArea(ref str, null, options);
    }

    public static bool TextArea(ref string str, GUIStyle style, params GUILayoutOption[] options)
    {
        if (str == null)
        {
            str = "";
        }
        string newStr = style != null ?
            EditorGUILayout.TextArea(str, style, options) :
            EditorGUILayout.TextArea(str, options);
        if (newStr != str)
        {
            str = newStr;
            return true;
        }
        return false;
    }

    public static bool PopupEnum(Type type, string label, ref Enum selection, params GUILayoutOption[] options)
    {
        Array arr = Enum.GetValues(type);
        Enum[] values = new Enum[arr.Length];
        for (int i = 0; i < values.Length; ++i)
        {
            values[i] = (Enum)arr.GetValue(i);
        }
        return Popup<Enum>(label, ref selection, values, options);
    }

    public static bool PopupEnum<T>(string label, ref T selection, IList<T> list, params GUILayoutOption[] options) where T : struct, IComparable, IConvertible, IFormattable
    {
        return Popup<T>(label, ref selection, list, ObjToString.DefaultToString, null, options);
    }

    public static bool PopupEnum<T>(string label, ref T selection, params GUILayoutOption[] options) where T : struct, IComparable, IConvertible, IFormattable
    {
        return Popup<T>(label, ref selection, EnumUtil.Values<T>(), ObjToString.DefaultToString, null, options);
    }

    public static bool PopupEnum<T>(string label, ref T selection, GUIStyle style, params GUILayoutOption[] options) where T : struct, IComparable, IConvertible, IFormattable
    {
        bool ret = Popup<T>(label, ref selection, EnumUtil.Values<T>(), ObjToString.DefaultToString, style, options);
        return ret;
    }

    public static bool Popup<T>(ref T selection, IList<T> items, params GUILayoutOption[] options)
    {
        bool ret = Popup<T>(null, ref selection, items, ObjToString.DefaultToString, options);
        return ret;
    }

    public static bool Popup<T>(string label, ref T selection, IList<T> items, params GUILayoutOption[] popupOptions)
    {
        return Popup<T>(label, ref selection, items, ObjToString.DefaultToString, popupOptions);
    }

    public static bool Popup<T>(string label, ref T selection, IList<T> items, ToStr toString, params GUILayoutOption[] popupOptions)
    {
        return Popup<T>(label, ref selection, items, toString, null, popupOptions);
    }
    /**
    * @param label null이면 출력하지 않는다.
    * @return 선택이 되었으면 true
    */
    public static bool Popup<T>(string label, ref T selection, IList<T> items, ToStr toString, GUIStyle style, params GUILayoutOption[] popupOptions)
    {
        if (items.Count == 0)
        {
            return false;
        }
        int index = -1;
        string[] str = new string[items.Count];
        for (int i = 0; i < items.Count; i++)
        {
            str[i] = toString(items[i]);
            if (object.ReferenceEquals(items[i], selection) || object.Equals(items[i], selection))
            {
                index = i;
            }
        }
        int newIndex = 0;
        // Show if current value is not the member of popup list
        Color c = GUI.contentColor;
        if (index < 0 && selection != null)
        {
            EditorGUILayout.BeginVertical();
            GUI.contentColor = Color.red;
            EditorGUILayout.LabelField("Invalid: " + selection.ToString(), EditorStyles.miniBoldLabel);
            index = 0;
        }
        GUI.contentColor = c;
        if (style != null)
        {
            if (label != null)
            {
                newIndex = EditorGUILayout.Popup(label, index, str, style, popupOptions);
            }
            else
            {
                newIndex = EditorGUILayout.Popup(index, str, style, popupOptions);
            }
        }
        else
        {
            if (label != null)
            {
                newIndex = EditorGUILayout.Popup(label, index, str, popupOptions);
            }
            else
            {
                newIndex = EditorGUILayout.Popup(index, str, popupOptions);
            }
        }
        if (index < 0 && selection != null)
        {
            EditorGUILayout.EndVertical();
        }
        if (index != newIndex)
        {
            selection = items[newIndex];
            return true;
        }
        return false;
    }

    public static bool Toggle(string label, ref bool b, params GUILayoutOption[] options)
    {
        return Toggle(label, ref b, null, options);
    }

    public static bool Toggle(string label, ref bool b, GUIStyle style, params GUILayoutOption[] options)
    {
        bool newb = false;
        if (label == null)
        {
            if (style != null)
            {
                newb = EditorGUILayout.Toggle(b, style, options);
            }
            else
            {
                newb = EditorGUILayout.Toggle(b, options);
            }
        }
        else
        {
            if (style != null)
            {
                newb = EditorGUILayout.Toggle(label, b, style, options);
            }
            else
            {
                newb = EditorGUILayout.Toggle(label, b, options);
            }
        }
        if (newb != b)
        {
            b = newb;
            return true;
        }
        return false;
    }

    public static bool IntField(string label, ref float f, params GUILayoutOption[] options)
    {
        int i = (int)f;
        bool ret = IntField(label, ref i, options);
        f = i;
        return ret;
    }

    public static bool IntField(string label, ref int i, params GUILayoutOption[] options)
    {
        return IntField(label, ref i, null, int.MinValue, int.MaxValue, options);
    }

    public static bool IntField(string label, ref int i, GUIStyle style, params GUILayoutOption[] options)
    {
        return IntField(label, ref i, style, int.MinValue, int.MaxValue, options);
    }

    public static bool IntField(string label, ref int i, int min, int max, params GUILayoutOption[] options)
    {
        return IntField(label, ref i, null, min, max, options);
    }

    public static bool IntField(string label, ref int i, GUIStyle style, int min, int max, params GUILayoutOption[] options)
    {
        int newi = i;
        if (label != null)
        {
            if (style != null)
            {
                newi = EditorGUILayout.IntField(label, i, style, options);
            }
            else
            {
                newi = EditorGUILayout.IntField(label, i, options);
            }
        }
        else
        {
            if (style != null)
            {
                newi = EditorGUILayout.IntField(i, style, options);
            }
            else
            {
                newi = EditorGUILayout.IntField(i, options);
            }
        }
        newi = Mathf.Clamp(newi, min, max);

        if (newi != i)
        {
            i = newi;
            return true;
        }
        return false;
    }

    public static bool FloatField(string label, ref float f, params GUILayoutOption[] options)
    {
        return FloatField(label, ref f, null, float.NegativeInfinity, float.PositiveInfinity, options);
    }

    public static bool FloatField(string label, ref float f, GUIStyle style, params GUILayoutOption[] options)
    {
        return FloatField(label, ref f, style, float.NegativeInfinity, float.PositiveInfinity, options);
    }

    public static bool FloatField(string label, ref float f, float min, float max, params GUILayoutOption[] options)
    {
        return FloatField(label, ref f, null, min, max, options);
    }

    public static bool FloatField(string label, ref float f, GUIStyle style, float min, float max, params GUILayoutOption[] options)
    {
        float newf = f;
        if (!string.IsNullOrEmpty(label))
        {
            if (style != null)
            {
                newf = EditorGUILayout.FloatField(label, f, style, options);
            }
            else
            {
                newf = EditorGUILayout.FloatField(label, f, options);
            }
        }
        else
        {
            if (style != null)
            {
                newf = EditorGUILayout.FloatField(f, style, options);
            }
            else
            {
                newf = EditorGUILayout.FloatField(f, options);
            }
        }
        newf = Mathf.Clamp(newf, min, max);

        if (newf != f)
        {
            f = newf;
            return true;
        }
        return false;
    }

    public static bool Slider(string label, ref float f, float min, float max, params GUILayoutOption[] options)
    {
        float newf = f;
        if (!string.IsNullOrEmpty(label))
        {
            newf = EditorGUILayout.Slider(label, f, min, max, options);
        }
        else
        {
            newf = EditorGUILayout.Slider(f, min, max, options);
        }
        if (newf != f)
        {
            f = newf;
            return true;
        }
        return false;
    }

    public static bool ColorField(string label, ref Color c, params GUILayoutOption[] options)
    {
        Color newc = c;
        if (label != null)
        {
            newc = EditorGUILayout.ColorField(label, c, options);
        }
        else
        {
            newc = EditorGUILayout.ColorField(c, options);
        }

        if (newc != c)
        {
            c = newc;
            return true;
        }
        return false;
    }

    public static bool Vector4Field(string label, ref Vector4 vec4, params GUILayoutOption[] options)
    {
        Vector4 newVec4 = EditorGUILayout.Vector4Field(label, vec4, options);
        if (newVec4 != vec4)
        {
            vec4 = newVec4;
            return true;
        }
        return false;
    }

    public static bool Vector3Field(string label, ref Vector3 vec3, params GUILayoutOption[] options)
    {
        Vector3 newVec3 = EditorGUILayout.Vector3Field(label, vec3, options);
        if (newVec3 != vec3)
        {
            vec3 = newVec3;
            return true;
        }
        return false;
    }

    public static bool Vector2Field(string label, ref Vector2 vec2, params GUILayoutOption[] options)
    {
        Vector2 newVec2 = EditorGUILayout.Vector2Field(label, vec2, options);
        if (newVec2 != vec2)
        {
            vec2 = newVec2;
            return true;
        }
        return false;
    }

    public static bool LayerField(string label, ref int layer, ref int sel, params GUILayoutOption[] options)
    {
        bool changed = false;
        EditorGUILayout.BeginHorizontal();
        sel = EditorGUILayout.LayerField(label, sel, options);
        if (GUILayout.Button("Everything"))
        {
            layer = -1;
        }
        if (GUILayout.Button("Set"))
        {
            layer |= 1 << sel;
            changed = true;
        }
        if (GUILayout.Button("Clear"))
        {
            layer &= ~(1 << sel);
            changed = true;
        }
        EditorGUILayout.EndHorizontal();
        return changed;
    }

    public static bool GUIDField<T>(string label, ref string guid, params GUILayoutOption[] options) where T : Object
    {
        T o = null;
        string assetPath = !guid.IsEmpty() ? AssetDatabase.GUIDToAssetPath(guid) : null;
        if (!assetPath.IsEmpty())
        {
            o = AssetDatabase.LoadAssetAtPath<T>(assetPath);
        }
        T newObj = EditorGUILayout.ObjectField(label, o, typeof(T), false, options) as T;
        if (newObj != o)
        {
            guid = newObj != null ? AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(newObj)) : null;
            return true;
        }
        return false;
    }

    public static bool ObjectField<T>(ref T obj, bool allowSceneObjects, params GUILayoutOption[] options) where T : Object
    {
        T newObj = (T)EditorGUILayout.ObjectField(obj, typeof(T), allowSceneObjects, options);
        if (newObj != obj)
        {
            obj = newObj;
            return true;
        }
        return false;
    }

    public static bool ObjectField<T>(string label, ref T obj, bool allowSceneObjects, params GUILayoutOption[] options) where T : Object
    {
        T newObj = (T)EditorGUILayout.ObjectField(label, obj, typeof(T), allowSceneObjects, options);
        if (newObj != obj)
        {
            obj = newObj;
            return true;
        }
        return false;
    }

    public static bool GameObjectField(string label, ref GameObject obj, bool allowSceneObjects, ref bool floating, params GUILayoutOption[] options)
    {
        EditorGUILayout.BeginHorizontal();
        bool changed = ObjectField<GameObject>(label, ref obj, allowSceneObjects);
        EditorGUILayoutUtil.Toggle(null, ref floating, GUILayout.Width(30));
        if (floating && Selection.activeGameObject != null)
        {
            if (obj != Selection.activeGameObject)
            {
                obj = Selection.activeGameObject;
                changed = true;
            }
        }
        EditorGUILayout.EndHorizontal();
        return changed;
    }

    public static bool ComponentField<T>(string label, ref T obj, bool allowSceneObjects, ref bool floating, params GUILayoutOption[] options) where T : Component
    {
        EditorGUILayout.BeginHorizontal();
        bool changed = ObjectField<T>(label, ref obj, allowSceneObjects);
        EditorGUILayoutUtil.Toggle(null, ref floating, GUILayout.Width(30));
        if (floating && Selection.activeGameObject != null)
        {
            T selObj = Selection.activeGameObject.GetComponent<T>();
            if (selObj != null && obj != selObj)
            {
                obj = selObj;
                changed = true;
            }
        }
        EditorGUILayout.EndHorizontal();
        return changed;
    }

    public static bool ObjectPropertyField(SerializedProperty prop, params GUILayoutOption[] options)
    {
        Object old = prop.objectReferenceValue;
        EditorGUILayout.PropertyField(prop);
        return old != prop.objectReferenceValue;
    }

    public static bool BoolPropertyField(SerializedProperty prop, params GUILayoutOption[] options)
    {
        bool old = prop.boolValue;
        EditorGUILayout.PropertyField(prop);
        return old != prop.boolValue;
    }

    public static bool IntPropertyField(SerializedProperty prop, params GUILayoutOption[] options)
    {
        int old = prop.intValue;
        EditorGUILayout.PropertyField(prop);
        return old != prop.intValue;
    }

    public static bool EnumPropertyField(SerializedProperty prop, params GUILayoutOption[] options)
    {
        int old = prop.enumValueIndex;
        EditorGUILayout.PropertyField(prop);
        return old != prop.enumValueIndex;
    }


    /**
    * Component를 가진 GameObject들을 선택한다.
    */
    public static void SelectComponents(Type type)
    {
        UnityEngine.Object[] found = GameObject.FindObjectsOfType(type);
        if (found.Length > 0)
        {
            GameObject[] objects = new GameObject[found.Length];
            for (int i = 0; i < objects.Length; i++)
            {
                objects[i] = ((Component)found[i]).gameObject;
            }
            Selection.objects = objects;
        }
    }

    public static void SelectPlaySpeed()
    {
        GUI.enabled = Application.isPlaying;
        Time.timeScale = EditorGUILayout.Slider("Play Speed", Time.timeScale, 0, 3);
    }

    public static bool Confirm(string message)
    {
        return EditorUtility.DisplayDialog("Confirm", message, "OK", "Cancel");
    }

    public static void TitleBar(string title)
    {
        EditorGUILayout.BeginHorizontal("Toolbar");
        EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();
    }

    public static void DrawSeparator()
    {
        GUILayout.Space(12f);

        if (Event.current.type == EventType.Repaint)
        {
            Texture2D tex = EditorGUIUtility.whiteTexture;
            Rect rect = GUILayoutUtility.GetLastRect();
            GUI.color = new Color(0f, 0f, 0f, 0.25f);
            GUI.DrawTexture(new Rect(0f, rect.yMin + 6f, Screen.width, 4f), tex);
            GUI.DrawTexture(new Rect(0f, rect.yMin + 6f, Screen.width, 1f), tex);
            GUI.DrawTexture(new Rect(0f, rect.yMin + 9f, Screen.width, 1f), tex);
            GUI.color = Color.white;
        }
    }


    private static object dragData;
    /// <summary>
    /// Checks the drag.
    /// </summary>
    /// <returns>The drag.</returns>
    /// <param name="dragObj">Drag object.</param>
    public static object CheckDrag(object dragObj)
    {

        Rect r = GUILayoutUtility.GetLastRect();
        Vector2 mousePosition = Event.current.mousePosition;
        if (r.Contains(mousePosition))
        {
            if (Event.current.type == EventType.DragUpdated)
            {
                //              if (dragData == null) { // dragged from outside
                //                  dragData = dragObj;
                //              }
                DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                Event.current.Use();
            }
            else if (Event.current.type == EventType.DragPerform || Event.current.type == EventType.DragExited)
            {
                object ret = dragData;
                dragData = null;
                DragAndDrop.AcceptDrag();
                Event.current.Use();
                return ret;
            }
            else if (Event.current.type == EventType.MouseDrag)
            {
                DragAndDrop.PrepareStartDrag();
                DragAndDrop.StartDrag("unilova drag");
                dragData = dragObj;
                Event.current.Use();
            }
        }
        return null;
    }

    public static void HelpBox(string message, MessageType type, int fontSize = 12)
    {
        Color bgColor = GUI.backgroundColor;
        if (type == MessageType.Error)
        {
            GUI.backgroundColor = Color.red;
        }
        else if (type == MessageType.Warning)
        {
            GUI.backgroundColor = Color.gray;
        }
        GUIStyle myStyle = GUI.skin.GetStyle("HelpBox");
        myStyle.wordWrap = true;
        myStyle.richText = true;
        myStyle.fontSize = fontSize;

        EditorGUILayout.TextArea(message, myStyle);
        GUI.backgroundColor = bgColor;
    }

    public static Object[] DnD()
    {
        return DnD(new Rect());
    }

    public static Object[] DnD(Rect rect)
    {
        if (Event.current.type == EventType.DragUpdated)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            Event.current.Use();
            return null;
        }
        else if (Event.current.type == EventType.DragPerform || Event.current.type == EventType.DragExited)
        {
            if (rect.width > 0 && !rect.Contains(Event.current.mousePosition))
            {
                return null;
            }
            DragAndDrop.AcceptDrag();
            Object[] ret = DragAndDrop.objectReferences;
            DragAndDrop.objectReferences = new Object[0];

            Event.current.Use();
            return ret;
        }
        else
        {
            return null;
        }
    }

    public static Rect[] SplitRectHorizontally(Rect src, float ratio)
    {
        Rect left = src;
        Rect right = src;
        left.width = src.width * ratio;
        right = src;
        right.x = left.x + left.width;
        right.width = src.width - left.width;
        return new Rect[] { left, right };
    }

    public static Rect[] SplitRectHorizontally(Rect src, int pixel)
    {
        Rect left = src;
        Rect right = src;
        if (pixel > 0)
        {
            left.width = pixel;
        }
        else
        {
            left.width = src.width + pixel;
        }
        right = src;
        right.x = left.x + left.width;
        right.width = src.width - left.width;
        return new Rect[] { left, right };
    }

    public static void DisplayProgressBar<T>(IList<T> list, string title, bool failOnError, Action<T> func)
    {
        if (list == null)
        {
            return;
        }
        for (int i = 0; i < list.Count; ++i)
        {
            if (SystemInfo.graphicsDeviceID != 0)
            {
                EditorUtility.DisplayProgressBar(title, list[i].ToString(), i / (float)list.Count);
            }
            try
            {
                func(list[i]);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                if (failOnError)
                {
                    EditorUtility.ClearProgressBar();
                    throw ex;
                }
            }
        }
        EditorUtility.ClearProgressBar();
    }
}
#endif