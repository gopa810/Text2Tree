using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrepInterpreter
{
    public class ScriptingSpace
    {
        public SValue nullValue = new SVNull();
        public List<ScriptingSpaceLevel> levels = new List<ScriptingSpaceLevel>();
        public Dictionary<string, SVDataType> dataTypes = new Dictionary<string, SVDataType>();

        public SValue GetValue(string objName)
        {
            for (int i = levels.Count - 1; i >= 0; i--)
            {
                if (levels[i].ContainsObject(objName))
                {
                    return levels[i].GetValue(objName);
                }
            }

            return null;
        }

        public bool ContainsObject(string on)
        {
            foreach (ScriptingSpaceLevel lv in levels)
            {
                if (lv.ContainsObject(on))
                    return true;
            }
            return false;
        }

        public void addFunctionDefinition(ScriptingDefun sd)
        {
            LastLevel.fmap.Add(sd.Name, sd);
        }

        public ScriptingDefun FindFunction(string funcName)
        {
            for (int i = levels.Count - 1; i >= 0; i--)
            {
                if (levels[i].fmap.ContainsKey(funcName))
                    return levels[i].fmap[funcName];
            }

            return null;
        }

        /// <summary>
        /// Duplicates SpaceLevels up to the level, where given function is defined
        /// </summary>
        /// <param name="funcName">name of the function</param>
        /// <returns></returns>
        public ScriptingSpace DuplicateUpToFunction(string funcName)
        {
            int i, j;
            for (i = levels.Count - 1; i >= 0; i--)
            {
                if (levels[i].fmap.ContainsKey(funcName))
                    break;
            }

            ScriptingSpace ds = new ScriptingSpace();
            ds.dataTypes = this.dataTypes;
            for (j = 0; j <= i; j++)
            {
                ds.levels.Add(this.levels[j]);
            }

            return ds;
        }

        public void pushLevel()
        {
            levels.Add(new ScriptingSpaceLevel());
        }

        public void popLevel()
        {
            if (levels.Count > 0)
                levels.RemoveAt(levels.Count - 1);
        }

        public ScriptingSpaceLevel LastLevel
        {
            get
            {
                if (levels.Count > 0)
                    return levels[levels.Count - 1];
                return new ScriptingSpaceLevel();
            }
        }

        public void SetValue(string s, SValue sv)
        {
            LastLevel.SetObjectValue(s, sv);
        }

        public void AddDatatype(string name, SValue typeRep)
        {
            if (dataTypes.ContainsKey(name))
            {
                dataTypes.Remove(name);
            }
            SVDataType sdt = new SVDataType();
            sdt.dataTypeName = name;
            sdt.dataType = typeRep;
            dataTypes.Add(name, sdt);
        }
    }
}
