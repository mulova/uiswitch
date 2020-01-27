#if STANDALONE
using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using static UnityEditorInternal.ReorderableList;
using UnityEditorInternal;

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
#endif