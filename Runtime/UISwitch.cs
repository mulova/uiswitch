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

namespace mulova.ui
{
    [ExecuteAlways]
    public class UISwitch : MonoBehaviour
    {
#pragma warning disable 0414
        [SerializeField, EnumType] private string enumType = "";
#pragma warning restore 0414
        [SerializeField, HideInInspector] public List<GameObject> objs = new List<GameObject>();
        [SerializeField] public List<UISwitchSet> switches = new List<UISwitchSet>();
        [SerializeField] public List<UISwitchPreset> preset = new List<UISwitchPreset>();
        public bool showTrans { get; set; } = false; // editor only
        public bool showAction { get; set; } = false; // editor only

        private UISwitchSet DUMMY = new UISwitchSet();
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

        public bool Contains(object o)
        {
            string s = Normalize(o);
            return keySet.Find(k => k.Equals(Normalize(s))) != null;
        }

        public bool Is(params object[] list)
        {
            if (list.GetCount() == keySet.Count)
            {
                foreach (object o in list)
                {
                    if (!Contains(o))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public bool IsPreset(string s)
        {
            var p = preset.Find(i => i.presetName == s);
            if (p == null)
            {
                return false;
            }
            return Is(p.keys);
        }

        public void AddSwitch(params object[] list)
        {
            foreach (object o in list)
            {
                keySet.Add(Normalize(o));
            }
        }

        public void RemoveSwitch(params object[] list)
        {
            foreach (object o in list)
            {
                keySet.Remove(Normalize(o));
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
            } else
            {
                return false;
            }
        }

        public void ToggleSwitch(object key)
        {
            string k = Normalize(key);
            if (keySet.Contains(k))
            {
                keySet.Remove(k);
            } else
            {
                keySet.Add(k);
            }
        }

        public void SetAction(object key, UnityAction action)
        {
            string k = Normalize(key);
            UISwitchSet s = switches.Find(e => e.name.EqualsIgnoreCase(k));
            if (s.isValid)
            {
                s.action.RemoveAllListeners();
                s.action.AddListener(action);
            } else
            {
                Assert.IsTrue(false, $"Key {k} not found");
            }
        }

        public void AddAction(object key, UnityAction action)
        {
            string k = Normalize(key);
            UISwitchSet s = switches.Find(e => e.name.EqualsIgnoreCase(k));
            if (s.isValid)
            {
                s.action.AddListener(action);
            }
            else
            {
                Assert.IsTrue(false, $"Key {k} not found");
            }
        }

        public void Set(params object[] param)
        {
            ResetSwitch();
            AddSwitch(param);
            Apply();
        }

        public void SetPreset(object id)
        {
            var idStr = Normalize(id);
            foreach (var p in preset)
            {
                if (p.presetName == idStr)
                {
                    Set(p.keys);
                }
            }
        }

        private string Normalize(object o)
        {
            return o.ToString();
        }

        public List<string> GetAllKeys()
        {
            return switches.ConvertAll(s => s.name);
        }

        public void Apply()
        {
            var visible = new bool[objs.Count];
            int match = 0;
            foreach (UISwitchSet e in switches)
            {
                if (keySet.Contains(Normalize(e.name)))
                {
                    // groups objects to switch on and off
                    for (int i=0; i<e.visibility.Count; ++i)
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
                        e.data.ForEach(d => d.ApplyTo(d.target));
#endif
                        e.action.Invoke();
                    } catch (Exception ex)
                    {
                        log.Error(ex);
                    }
                }
            }

            for (int i=0; i<objs.Count; ++i)
            {
                objs[i].SetActive(visible[i]);
            }

            if (log.IsLoggable(LogType.Log))
            {
                log.Debug("{0}: switch {1}", name, keySet.Join(","));
            }
            if (match != keySet.Count)
            {
                Assert.IsTrue(false, $"Invalid param {keySet.Join(",")}");
            }
        }

        public GameObject GetObject(object key, string name)
        {
            // Get Switch slot
            UISwitchSet slot = GetSwitchSlot(key);
            return slot.FindObject(objs, o => o.name.EqualsIgnoreCase(name));
        }

        public T GetComponent<T>(object key) where T: Component
        {
            // Get Switch slot
            UISwitchSet slot = GetSwitchSlot(key);
            for (int i=0; i<slot.visibility.Count; ++i)
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

        private UISwitchSet GetSwitchSlot(object key)
        {
            string id = Normalize(key);
            foreach (UISwitchSet e in switches)
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