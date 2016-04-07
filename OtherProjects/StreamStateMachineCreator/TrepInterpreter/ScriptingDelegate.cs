using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrepInterpreter
{
    public class ScriptingDelegate
    {
        public ScriptingSpace space = null;

        public ScriptingDelegate()
        {
        }

        public ScriptingDelegate(ScriptingSpace sp)
        {
            space = sp;
        }

        public void InitWithSpace(ScriptingSpace sp)
        {
            space = sp;
            RegisterDataTypes(sp);
        }

        /// <summary>
        /// Here we add definitions of datatypes implemented in this delegate
        /// </summary>
        public virtual void RegisterDataTypes(ScriptingSpace aSpace)
        {
        }

        public virtual void RegisterGlobalVariables(ScriptingSpace aSpace)
        {
        }

        /// <summary>
        /// Here we can implement some custom operations or commands
        /// </summary>
        /// <param name="node">We are interested in children of this node. This node is LIST.</param>
        /// <returns></returns>
        public virtual SValue ExecuteTokenList(Scripting parent, ScriptingSpace space, SVList node)
        {
            return new SValue();
        }

        /// <summary>
        /// Creates instance of data type whose name is specified in argument
        /// </summary>
        /// <param name="dataTypeName">Name of data type</param>
        /// <param name="optArgs">Optional arguments for new instance</param>
        /// <returns></returns>
        public virtual SValue createInstance(Scripting parent, string dataTypeName, List<SValue> optArgs)
        {
            return null;
        }
    }
}
