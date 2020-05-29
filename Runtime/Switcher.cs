//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using Assert = UnityEngine.Assertions.Assert;
#if CORE_LIB
using System.Collections.Generic.Ex;
using System.Text.Ex;
using mulova.commons;
using mulova.unicore;
using UnityEngine.Ex;
#endif

namespace mulova.switcher
{
    [ExecuteAlways]
    public class Switcher : MonoBehaviour
    {
#pragma warning disable 0414
        [SerializeField, EnumType] private string enumType = "";
#pragma warning restore 0414
        [SerializeField, HideInInspector] public List<GameObject> objs = new List<GameObject>();
        public List<SwitchSet> switches = new List<SwitchSet>();
        [SerializeField, HideInInspector] public List<SwitchPreset> preset = new List<SwitchPreset>();
        public bool caseSensitive = true;
        public bool showTrans { get; set; } = false; // editor only
        public bool showAction { get; set; } = false; // editor only
        public bool showPreset { get; set; } = false; // editor only

        private SwitchSet DUMMY = new SwitchSet();
        private HashSet<string> keySet = new HashSet<string>();

        private ILogger log
        {
            get
            {
                return Debug.unityLogger;
            }
        }

        public void ResetSwitch()
        {
            keySet.Clear();
        }

        public bool ContainsKey(object o)
        {
            string s = NormalizeKey(o);
            return keySet.Contains(s);
        }

        public bool IsKeys(params object[] list)
        {
            if (list.GetCount() == keySet.Count)
            {
                foreach (object o in list)
                {
                    if (!ContainsKey(o))
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        public bool IsPreset(string s)
        {
            var p = preset.Find(i => i.presetName == s);
            if (p == null)
            {
                return false;
            }
            return IsKeys(p.keys);
        }

        public void SetPreset(object id)
        {
            var k = NormalizeKey(id);
            foreach (var p in preset)
            {
                if (p.presetName == k)
                {
                    SetKey(p.keys);
                    break;
                }
            }
        }

        public void SetKey(params object[] param)
        {
            ResetSwitch();
            AddKey(param);
            Apply();
        }

        public void AddKey(params object[] list)
        {
            foreach (object o in list)
            {
                keySet.Add(NormalizeKey(o));
            }
        }

        public void RemoveKey(params object[] list)
        {
            foreach (object o in list)
            {
                keySet.Remove(NormalizeKey(o));
            }
        }

        public void ToggleKey(object key)
        {
            string k = NormalizeKey(key);
            if (keySet.Contains(k))
            {
                keySet.Remove(k);
            }
            else
            {
                keySet.Add(k);
            }
        }

        public void SetAction(object key, UnityAction action)
        {
            string k = NormalizeKey(key);
            SwitchSet s = switches.Find(e => e.name.EqualsIgnoreCase(k));
            if (s.isValid)
            {
                s.action.RemoveAllListeners();
                s.action.AddListener(action);
            }
            else
            {
                Assert.IsTrue(false, $"Key {k} not found");
            }
        }

        public void AddAction(object key, UnityAction action)
        {
            string k = NormalizeKey(key);
            SwitchSet s = switches.Find(e => e.name.EqualsIgnoreCase(k));
            if (s.isValid)
            {
                s.action.AddListener(action);
            }
            else
            {
                Assert.IsTrue(false, $"Key {k} not found");
            }
        }

        private string NormalizeKey(object o)
        {
            if (caseSensitive)
        {
            return o.ToString();
            } else
            {
                return o.ToString().ToLower();
            }
        }

        public List<string> GetAllKeys()
        {
            return switches.ConvertAll(s => s.name);
        }

        public void Apply()
        {
            var visible = new bool[objs.Count];
            int match = 0;
            foreach (SwitchSet e in switches)
            {
                if (keySet.Contains(NormalizeKey(e.name)))
                {
                    // groups objects to switch on and off
                    for (int i = 0; i < e.visibility.Count; ++i)
                    {
                        visible[i] |= e.visibility[i];
                    }
                    match++;
                    // set positions
                    for (int i = 0; i < e.trans.Count; ++i)
                    {
                        e.trans[i].localPosition = e.pos[i];
                    }
                    try
                    {
#if UNITY_2019_1_OR_NEWER
                        foreach (var d in e.data)
                        {
#if UNITY_EDITOR
                            if (!Application.isPlaying)
                            {
                                UnityEditor.Undo.RecordObject(d.target, d.target.name);
                            }
#endif
                            d.ApplyTo(d.target);
#if UNITY_EDITOR
                            if (!Application.isPlaying)
                            {
                                UnityEditor.EditorUtility.SetDirty(d.target);
                            }
#endif
                        }
#endif
                        e.action.Invoke();
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                    }
                }
            }

            for (int i = 0; i < objs.Count; ++i)
            {
                objs[i].SetActive(visible[i]);
            }

            if (log.IsLoggable(LogType.Log))
            {
                log.Debug("Switcher {0}", keySet.Join(","));
            }
            if (match != keySet.Count)
            {
                Assert.IsTrue(false, $"Invalid param {keySet.Join(",")}");
            }
        }

        public bool Remove(GameObject o)
        {
            int i = objs.IndexOf(o);
            if (i >= 0)
            {
                foreach (var s in switches)
                {
                    s.RemoveAt(i);
                }

                objs.RemoveAt(i);
                return true;
            }
            else
            {
                return false;
            }
        }

        public GameObject GetObject(object key, string name)
        {
            // Get Switch slot
            SwitchSet slot = GetSlot(key);
            return slot.FindObject(objs, o => o.name.EqualsIgnoreCase(name));
        }

        public T GetComponent<T>(object key) where T : Component
        {
            // Get Switch slot
            SwitchSet slot = GetSlot(key);
            for (int i = 0; i < slot.visibility.Count; ++i)
            {
                if (slot.visibility[i])
                {
                    T c = objs[i].GetComponent<T>();
                    if (c != null)
                    {
                        return c;
                    }
                }
            }
            return null;
        }

        private SwitchSet GetSlot(object key)
        {
            string id = NormalizeKey(key);
            foreach (SwitchSet e in switches)
            {
                if (e.name.EqualsIgnoreCase(id))
                {
                    return e;
                }
            }
            return DUMMY;
        }
    }
}