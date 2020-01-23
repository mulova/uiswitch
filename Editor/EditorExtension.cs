#if STANDALONE
using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Color = UnityEngine.Color;
using Object = UnityEngine.Object;
using static UnityEditorInternal.ReorderableList;
using UnityEditorInternal;
using System.Collections.Generic;

public class ColorScope : IDisposable
{
    private Color old;
    public ColorScope(Color c, bool apply = true)
    {
        old = GUI.color;
        if (apply)
        {
            GUI.color = c;
        }
    }

    public void Dispose()
    {
        GUI.color = old;
    }
}

public class EnableScope : IDisposable
{
    private bool enabled;
    public EnableScope(bool e)
    {
        enabled = GUI.enabled;
        GUI.enabled = e;
    }

    public void Dispose()
    {
        GUI.enabled = enabled;
    }
}

public class ContentColorScope : IDisposable
{
    private Color old;
    public ContentColorScope(Color c, bool apply = true)
    {
        old = GUI.contentColor;
        if (apply)
        {
            GUI.contentColor = c;
        }
    }

    public void Dispose()
    {
        GUI.contentColor = old;
    }
}

public static class EditorUtil
{
    public static void SetDirty(Object o)
    {
        if (Application.isPlaying || o == null)
        {
            return;
        }
        GameObject go = null;
        if (o is GameObject)
        {
            go = o as GameObject;
        }
        else if (o is Component)
        {
            go = (o as Component).gameObject;
        }
        if (go != null && go.scene.IsValid())
        {
            EditorSceneManager.MarkSceneDirty(go.scene);
        }
        else
        {
            EditorUtility.SetDirty(o);
        }
    }
}

public class PopupReorder : PropertyReorder<string>
{
    public string[] options;
    public PopupReorder(SerializedProperty prop, string[] options) : base(prop)
    {
        this.options = options;
    }

    public PopupReorder(SerializedObject ser, string propPath, string[] options) : base(ser, propPath)
    {
        this.options = options;
    }

    protected override void DrawItem(SerializedProperty item, Rect rect, int index, bool isActive, bool isFocused)
    {
        string sel = item.stringValue;
        int i1 = Array.FindIndex(options, o => o == sel);
        var i2 = EditorGUI.Popup(rect, Math.Max(0, i1), options);
        if (i1 != i2)
        {
            item.stringValue = options[i2];
        }
    }
}

public class PropertyReorder<T>
{
    public delegate T CreateItemDelegate();
    public delegate void DrawItemDelegate(SerializedProperty item, Rect rect, int index, bool isActive, bool isFocused);
    public delegate T GettemDelegate(SerializedProperty p);
    public delegate void SetItemDelegate(SerializedProperty p, T value);
    public delegate void AddDelegate(int index);
    public delegate void RemoveDelegate(int index);
    public delegate void ReorderDelegate(int i1, int i2);
    public delegate void ChangeDelegate();
    public delegate bool CanAddDelegate();

    public ReorderableList drawer { get; private set; }

    public CreateItemDelegate createItem { private get; set; }
    public DrawItemDelegate drawItem { private get; set; }
    public GettemDelegate getItem { private get; set; }
    public SetItemDelegate setItem { private get; set; }
    public AddDelegate onAdd = i => { };
    public RemoveDelegate onRemove = i => { };
    public ChangeDelegate onChange = () => { };
    public ReorderDelegate onReorder = (i1, i2) => { };
    public ElementHeightCallbackDelegate getElementHeight;
    public CanAddDelegate canAdd = () => true;

    // backup
    private float elementHeight;
    private float headerHeight;
    private float footerHeight;
    private bool dirty;

    private Predicate<T> match;

    private string _title;
    public string title
    {
        set
        {
            _title = value;
            drawer.headerHeight = _title.IsEmpty() ? 0 : headerHeight;
        }
    }

    public bool displayIndex;

    public bool displayAdd
    {
        get
        {
            return drawer.displayAdd;
        }
        set
        {
            drawer.displayAdd = value;
        }
    }

    public bool displayRemove
    {
        get
        {
            return drawer.displayRemove;
        }
        set
        {
            drawer.displayRemove = value;
        }
    }

    public bool draggable
    {
        get
        {
            return drawer.draggable;
        }
        set
        {
            drawer.draggable = value;
        }
    }

    public SerializedProperty property
    {
        get
        {
            return drawer.serializedProperty;
        }
    }

    public T this[int i]
    {
        get
        {
            var e = drawer.serializedProperty.GetArrayElementAtIndex(i);
            return getItem(e);
        }
        set
        {
            if (drawer.serializedProperty.arraySize < i)
            {
                drawer.serializedProperty.InsertArrayElementAtIndex(i);
            }
            var e = drawer.serializedProperty.GetArrayElementAtIndex(i);
            setItem(e, value);
        }
    }

    public int count
    {
        get
        {
            return drawer.serializedProperty.arraySize;
        }
    }

    protected Object obj
    {
        get
        {
            return drawer.serializedProperty.serializedObject.targetObject;
        }
    }

    public PropertyReorder(SerializedObject ser, string propPath)
    {
        var prop = ser.FindProperty(propPath);
        Init(prop);
    }

    public PropertyReorder(SerializedProperty prop)
    {
        Init(prop);
    }

    private void Init(SerializedProperty prop)
    {
        this.drawer = new ReorderableList(prop.serializedObject, prop, true, false, true, true);
        elementHeight = this.drawer.elementHeight;
        headerHeight = this.drawer.headerHeight;
        footerHeight = this.drawer.footerHeight;

        this.drawItem = DrawItem;
        this.drawer.onAddCallback = _OnAdd;
        this.drawer.onRemoveCallback = _OnRemove;
        this.drawer.drawHeaderCallback = _OnDrawHeader;
        this.drawer.drawElementCallback = _OnDrawItem;
        this.drawer.onReorderCallbackWithDetails = _OnReorder;
        this.drawer.elementHeightCallback = GetElementHeight;
        this.drawer.onCanAddCallback = _CanAdd;
        this.createItem = () => default(T);

        this.title = prop.displayName;
        // backup
    }

    private bool _CanAdd(ReorderableList list)
    {
        return canAdd();
    }

    private float GetElementHeight(int index)
    {
        if (match == null || match(this[index]))
        {
            if (getElementHeight != null)
            {
                return getElementHeight(index);
            }
            else
            {
                return EditorGUI.GetPropertyHeight(drawer.serializedProperty.GetArrayElementAtIndex(index)) + 5;
            }
        }
        else
        {
            return 0;
        }
    }

    private void SetDirty()
    {
        dirty = true;
        onChange();
    }

    private void _OnDrawHeader(Rect rect)
    {
        if (!_title.IsEmpty())
        {
            EditorGUI.LabelField(rect, new GUIContent(_title));
        }
    }

    protected virtual void DrawItem(SerializedProperty item, Rect rect, int index, bool isActive, bool isFocused)
    {
        EditorGUI.PropertyField(rect, item, new GUIContent(""));
    }

    private void _OnDrawItem(Rect rect, int index, bool isActive, bool isFocused)
    {
        if (match == null || match(this[index]))
        {
            Rect r = rect;
            var item = drawer.serializedProperty.GetArrayElementAtIndex(index);
            if (displayIndex)
            {
                var rects = rect.SplitByWidths(20);
                EditorGUI.LabelField(rects[0], index.ToString(), EditorStyles.boldLabel);
                r = rects[1];
            }
            r.y += 1;
            r.height -= 5;
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                drawItem(item, r, index, isActive, isFocused);
                if (dirty || check.changed)
                {
                    dirty = true;
                    drawer.index = index;
                }
                else
                {
                    int controlID = GUIUtility.GetControlID(FocusType.Keyboard, r);
                    if (controlID == GUIUtility.hotControl)
                    {
                        drawer.index = index;
                    }
                }
            }
        }
    }

    protected virtual void _OnReorder(ReorderableList list, int oldIndex, int newIndex)
    {
        onReorder(oldIndex, newIndex);
        SetDirty();
        EditorUtil.SetDirty(obj);
    }

    private void _OnAdd(ReorderableList list)
    {
        var index = list.index >= 0 ? list.index + 1 : list.count;
        index = Math.Min(index, list.count);
        list.serializedProperty.InsertArrayElementAtIndex(index);
        list.index = index;
        setItem?.Invoke(list.serializedProperty.GetArrayElementAtIndex(index), createItem());
        onAdd(index);
        SetDirty();
        EditorUtil.SetDirty(obj);
    }

    private void _OnRemove(ReorderableList list)
    {
        int index = list.index;
        defaultBehaviours.DoRemoveButton(list);
        onRemove(index);
        SetDirty();
        EditorUtil.SetDirty(obj);
    }

    public void Draw()
    {
        Record();
        drawer.DoLayoutList();
    }

    public void Draw(Rect rect)
    {
        Record();
        drawer.DoList(rect);
    }

    private void Record()
    {
        if (dirty)
        {
            Undo.RecordObject(obj, obj.name);
            property.serializedObject.ApplyModifiedProperties();
            dirty = false;
        }
    }

    public void Filter(Predicate<T> match)
    {
        this.match = match;
    }

    public float GetHeight()
    {
        return drawer.GetHeight();
    }
}

public abstract class PropertyDrawerBase : PropertyDrawer
{
    public int lineHeight
    {
        get
        {
            return (int)EditorGUIUtility.singleLineHeight;
        }
    }
    private SerializedProperty prop;
    protected Rect bound;

    protected virtual int GetLineCount(SerializedProperty p)
    {
        return -1;
    }

    protected SerializedProperty GetProperty(string name)
    {
        return prop.FindPropertyRelative(name);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        int lineCount = GetLineCount(property);
        if (lineCount > 0)
        {
            return lineCount * base.GetPropertyHeight(property, label);
        }
        else
        {
            return base.GetPropertyHeight(property, label);
        }
    }

    protected abstract void OnGUI(SerializedProperty p, Rect bound);

    protected Rect GetLineRect(int lineNo, int lineCount = 1)
    {
        Rect lineRect = bound;
        lineRect.y += lineHeight * lineNo;
        lineRect.height = lineHeight * lineCount;
        return lineRect;
    }

    protected Rect[] SplitRect(Rect src, float leftWidth)
    {
        Rect left = src;
        Rect right = src;
        left.width = (src.width + EditorGUIUtility.currentViewWidth) * leftWidth;
        right.x = left.x + left.width;
        right.width = src.width + EditorGUIUtility.currentViewWidth - left.width;
        return new Rect[] { left, right };
    }

    protected bool DrawObjectField<T>(Rect r, GUIContent label, ref T o, bool allowSceneObj = true) where T : Object
    {
        Rect controlRect = EditorGUI.PrefixLabel(r, label);
        Rect refRect = controlRect;
        refRect.width = controlRect.width - 15;

        T old = o as T;
        o = EditorGUI.ObjectField(refRect, o, typeof(T), allowSceneObj) as T;
        return o != old;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        this.prop = property;
        this.bound = position;
        Undo.RecordObject(property.serializedObject.targetObject, property.propertyPath);
        EditorGUI.indentLevel = property.depth;
        if (property.isArray)
        {
            ReorderableList list = new ReorderableList(property.serializedObject, property);
            list.DoList(position);
            list.drawElementCallback = DrawArrayElement;
        }
        else
        {
            OnGUI(property, position);
        }
        this.prop = null;
    }

    private void DrawArrayElement(Rect rect, int idx, bool active, bool focused)
    {
        var element = prop.GetArrayElementAtIndex(idx);
        this.bound = rect;
        OnGUI(element, rect);
    }

    protected void DrawTitles(Rect pos, params string[] propNames)
    {
        for (int i = 0; i < propNames.Length; i++)
        {
            float width = 1F / propNames.Length;
            DrawLabel(propNames[i], GetRect(pos, i * width, 0F, width, 1F));
        }
    }

    protected void DrawProperties(Rect pos, SerializedProperty p, params string[] propNames)
    {
        for (int i = 0; i < propNames.Length; i++)
        {
            SerializedProperty prop = p.FindPropertyRelative(propNames[i]);
            float height = 1F / propNames.Length;
            Rect rect = GetRect(pos, 0F, i * height, 1F, height);
            DrawProperty(rect, prop, true);
        }
    }

    protected void DrawProperty(Rect pos, SerializedProperty prop, bool title)
    {
        GUIContent label = GUIContent.none;
        if (title)
        {
            label = new GUIContent(prop.displayName);
        }
        if (prop.propertyType == SerializedPropertyType.AnimationCurve)
        {
            prop.animationCurveValue = EditorGUI.CurveField(pos, label, prop.animationCurveValue);
        }
        else if (prop.propertyType == SerializedPropertyType.ArraySize)
        {
        }
        else if (prop.propertyType == SerializedPropertyType.Boolean)
        {
            prop.boolValue = EditorGUI.Toggle(pos, label, prop.boolValue);
        }
        else if (prop.propertyType == SerializedPropertyType.Bounds)
        {
            prop.boundsValue = EditorGUI.BoundsField(pos, label, prop.boundsValue);
        }
        else if (prop.propertyType == SerializedPropertyType.Character)
        {
            prop.intValue = EditorGUI.IntField(pos, label, prop.intValue);
        }
        else if (prop.propertyType == SerializedPropertyType.Color)
        {
            prop.colorValue = EditorGUI.ColorField(pos, label, prop.colorValue);
        }
        else if (prop.propertyType == SerializedPropertyType.Enum)
        {
            string[] optionNames = prop.enumNames;
            GUIContent[] options = new GUIContent[optionNames.Length];
            for (int i = 0; i < optionNames.Length; i++)
            {
                options[i] = new GUIContent(optionNames[i]);
            }
            prop.enumValueIndex = EditorGUI.Popup(pos, label, prop.intValue, options);
        }
        else if (prop.propertyType == SerializedPropertyType.Float)
        {
            prop.floatValue = EditorGUI.FloatField(pos, label, prop.floatValue);
            //      } else if (prop.propertyType == SerializedPropertyType.Generic) {
            //          DrawGeneric(prop, pos);
        }
        else if (prop.propertyType == SerializedPropertyType.Gradient)
        {
        }
        else if (prop.propertyType == SerializedPropertyType.Integer)
        {
            prop.intValue = EditorGUI.IntField(pos, label, prop.intValue);
        }
        else if (prop.propertyType == SerializedPropertyType.LayerMask)
        {
            prop.intValue = EditorGUI.LayerField(pos, label, prop.intValue);
        }
        else if (prop.propertyType == SerializedPropertyType.ObjectReference)
        {
            prop.objectReferenceValue = EditorGUI.ObjectField(pos, label, prop.objectReferenceValue, Type.GetType(prop.type), true);
        }
        else if (prop.propertyType == SerializedPropertyType.Rect)
        {
            prop.rectValue = EditorGUI.RectField(pos, label, prop.rectValue);
        }
        else if (prop.propertyType == SerializedPropertyType.String)
        {
            prop.stringValue = EditorGUI.TextField(pos, label, prop.stringValue);
        }
        else if (prop.propertyType == SerializedPropertyType.Vector2)
        {
            prop.vector2Value = EditorGUI.Vector2Field(pos, label.text, prop.vector2Value);
        }
        else if (prop.propertyType == SerializedPropertyType.Vector3)
        {
            prop.vector3Value = EditorGUI.Vector2Field(pos, label.text, prop.vector3Value);
        }
        else if (prop.propertyType == SerializedPropertyType.Generic)
        {
            if (prop.type == typeof(Quaternion).ToString())
            {
                DrawVec4(prop, label, pos);
            }
        }
    }

    protected void DrawLabel(string title, Rect pos)
    {
        EditorGUI.SelectableLabel(pos, title);
    }

    protected void DrawVec4(SerializedProperty prop, GUIContent label, Rect pos)
    {
        Quaternion q = prop.quaternionValue;
        Vector4 vec4 = new Vector4(q.x, q.y, q.z, q.w);
        vec4 = EditorGUI.Vector4Field(pos, label.text, vec4);
        prop.quaternionValue = new Quaternion(vec4.x, vec4.y, vec4.z, vec4.w);
    }

    private Rect GetRect(Rect src, float x0, float y0, float width, float height)
    {
        return new Rect(x0.Interpolate(src.x, src.x + src.width),
                        y0.Interpolate(src.y, src.y + src.height),
                        src.width * width,
                        src.height * height);
    }

    public static bool Popup<T>(Rect rect, ref T selection, IList<T> items)
    {
        bool ret = Popup<T>(rect, null, ref selection, items);
        return ret;
    }

    public static bool Popup<T>(Rect rect, string label, ref T selection, IList<T> items)
    {
        return Popup<T>(rect, label, ref selection, items);
    }

    public static bool Popup<T>(Rect rect, string label, ref T selection, IList<T> items, GUIStyle style)
    {
        if (items.Count == 0)
        {
            return false;
        }
        int index = -1;
        string[] str = new string[items.Count];
        for (int i = 0; i < items.Count; i++)
        {
            str[i] = items[i]?.ToString();
            if (object.ReferenceEquals(items[i], selection) || object.Equals(items[i], selection))
            {
                index = i;
            }
        }
        int newIndex = 0;

        Rect controlRect = label != null ? EditorGUI.PrefixLabel(rect, new GUIContent(label)) : rect;

        // Show if current value is not the member of popup list
        Color c = GUI.contentColor;
        if (index < 0 && selection != null)
        {
            GUI.contentColor = Color.red;
            index = 0;
        }
        GUI.contentColor = c;
        if (style != null)
        {
            newIndex = EditorGUI.Popup(controlRect, index, str, style);
        }
        else
        {
            newIndex = EditorGUI.Popup(controlRect, index, str);
        }
        if (index != newIndex)
        {
            selection = items[newIndex];
            return true;
        }
        return false;
    }

    public static bool PopupNullable<T>(Rect rect, string label, ref T selection, IList<T> items)
    {
        Rect controlRect = label != null ? EditorGUI.PrefixLabel(rect, new GUIContent(label)) : rect;
        if (items.Count == 0)
        {
            bool changed = false;
            if (selection != null && !string.Empty.Equals(selection) && !selection.Equals(default(T)))
            {
                selection = default(T);
                changed = true;
            }
            EditorGUI.Popup(controlRect, 0, new string[] { "-" });
            return changed;
        }


        int index = 0;
        string[] str = new string[items.Count + 1];
        str[0] = "-";
        for (int i = 1; i <= items.Count; i++)
        {
            str[i] = items[i - 1]?.ToString();
            if (object.Equals(items[i - 1], selection))
            {
                index = i;
            }
        }
        int newIndex = EditorGUI.Popup(controlRect, index, str);
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

    public static bool TextField(Rect r, string label, SerializedProperty p)
    {
        return TextField(r, label, p, EditorStyles.textField);
    }

    public static bool TextField(Rect r, string label, SerializedProperty p, GUIStyle style)
    {
        string str = p.stringValue;
        string newStr = label == null ?
            EditorGUI.TextField(r, str, style) :
            EditorGUI.TextField(r, label, str, style);
        if (newStr != str)
        {
            p.stringValue = newStr;
            return true;
        }
        return false;
    }

    public static void LabelField(Rect r, SerializedProperty p)
    {
        LabelField(r, p, EditorStyles.label);
    }

    public static void LabelField(Rect r, SerializedProperty p, GUIStyle style)
    {
        string str = p.stringValue;
        EditorGUI.LabelField(r, str, style);
    }

    protected void SetDirty()
    {
        prop.serializedObject.ApplyModifiedProperties();
    }
}
#endif