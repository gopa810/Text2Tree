using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace StreamStateMachineCreator
{
    public class SMachine
    {
        public string Name;

        public string Input = string.Empty;

        private int nextId = 1;

        public string InitialState = string.Empty;

        public List<SMState> States = new List<SMState>();

        public List<SMVariable> Variables = new List<SMVariable>();

        public StringBuilder ErrorLog = new StringBuilder();

        public bool HasState(string s)
        {
            foreach (SMState ss in States)
            {
                if (ss.Name == s)
                    return true;
            }
            return false;
        }

        public SMState CreateState(string s)
        {
            SMState ss = new SMState();
            ss.Name = s;
            States.Add(ss);
            return ss;
        }

        public SMState GetState(int sid)
        {
            foreach (SMState ss in States)
            {
                if (ss.StateId == sid)
                    return ss;
            }
            return null;
        }

        public void Open(string fileName)
        {
            XmlDocument doc = new XmlDocument();

            doc.Load(fileName);

            ReadDocument(doc);
        }

        public void Save(string fileName)
        {
            XmlDocument doc = new XmlDocument();

            PrepareDocument(doc);

            doc.Save(fileName);
        }

        public void Save(Stream stream)
        {
            XmlDocument doc = new XmlDocument();

            PrepareDocument(doc);

            doc.Save(stream);
        }

        public void Save(XmlWriter xw)
        {
            XmlDocument doc = new XmlDocument();

            PrepareDocument(doc);

            doc.Save(xw);
        }

        public int GetNextID()
        {
            int nid = nextId;
            nextId++;
            return nid;
        }

        public int GetInitialStateID()
        {
            return GetStateID(InitialState);
        }

        public void PrepareDocument(XmlDocument doc)
        {
            XmlElement root = doc.CreateElement("StateMachine");
            doc.AppendChild(root);

            XmlElement ele3, ele4;
            XmlElement elem = doc.CreateElement("name");
            elem.InnerText = Name;
            root.AppendChild(elem);

            elem = doc.CreateElement("properties");
            elem.SetAttribute("nextid", nextId.ToString());
            root.AppendChild(elem);

            XmlElement ele2 = doc.CreateElement("input");
            ele2.SetAttribute("one", Input);
            root.AppendChild(ele2);

            elem = doc.CreateElement("variables");
            root.AppendChild(elem);
            foreach (SMVariable sv in Variables)
            {
                ele2 = doc.CreateElement("var");
                ele2.SetAttribute("datatype", sv.DataType);
                ele2.SetAttribute("name", sv.Variable);
                ele2.SetAttribute("initial", sv.InitialValue);
                elem.AppendChild(ele2);
            }

            elem = doc.CreateElement("states");
            root.AppendChild(elem);
            foreach (SMState ss in States)
            {
                ele2 = doc.CreateElement("state");
                ele2.SetAttribute("name", ss.Name);
                elem.AppendChild(ele2);

                foreach (SMStateProcessing sp in ss.Process)
                {
                    ele3 = doc.CreateElement("proc");
                    ele3.SetAttribute("co", sp.Comparer);
                    ele3.SetAttribute("nextstate", sp.NextState);
                    ele3.SetAttribute("reproc", sp.ReprocessInput.ToString());
                    ele2.AppendChild(ele3);

                    ele4 = doc.CreateElement("vals");
                    ele4.InnerText = sp.Values;
                    ele3.AppendChild(ele4);

                    ele4 = doc.CreateElement("acts");
                    ele4.InnerText = sp.Actions;
                    ele3.AppendChild(ele4);

                }
            }

        }

        public void ReadDocument(XmlDocument doc)
        {
            XmlElement root = null;

            foreach (XmlElement e in doc.ChildNodes)
            {
                if (e.Name.Equals("StateMachine"))
                {
                    root = e;
                    break;
                }
            }

            if (root == null)
                return;

            foreach (XmlElement e in root.ChildNodes)
            {
                if (e.Name == "name")
                {
                    Name = e.InnerText;
                }
                else if (e.Name == "properties")
                {
                    if (e.HasAttribute("nextid"))
                    {
                        int.TryParse(e.GetAttribute("nextid"), out nextId);
                    }
                }
                else if (e.Name == "input")
                {
                    Input = e.GetAttribute("one");
                }
                else if (e.Name == "variables")
                {
                    Variables = new List<SMVariable>();
                    foreach (XmlElement e2 in e.ChildNodes)
                    {
                        if (e2.Name == "var")
                        {
                            SMVariable sv = new SMVariable();
                            sv.DataType = e2.GetAttribute("datatype");
                            sv.Variable = e2.GetAttribute("name");
                            sv.InitialValue = e2.GetAttribute("initial");
                            Variables.Add(sv);
                        }
                    }
                }
                else if (e.Name == "states")
                {
                    States = new List<SMState>();
                    foreach (XmlElement e2 in e.ChildNodes)
                    {
                        if (e2.Name == "state")
                        {
                            SMState ss = new SMState();
                            ss.Name = e2.GetAttribute("name");

                            foreach (XmlElement e3 in e2.ChildNodes)
                            {
                                if (e3.Name == "proc")
                                {
                                    SMStateProcessing sp = new SMStateProcessing();
                                    sp.Comparer = e3.GetAttribute("co");
                                    if (e3.HasAttribute("nextstate"))
                                        sp.NextState = e3.GetAttribute("nextstate");
                                    if (e3.HasAttribute("reproc"))
                                        sp.ReprocessInput = bool.Parse(e3.GetAttribute("reproc"));
                                    ss.Process.Add(sp);

                                    foreach (XmlElement e4 in e3.ChildNodes)
                                    {
                                        if (e4.Name == "vals")
                                            sp.Values = e4.InnerText;
                                        else if (e4.Name == "acts")
                                            sp.Actions = e4.InnerText;
                                    }
                                }
                            }

                            States.Add(ss);
                        }
                    }
                }
            }
        }

        public int GetStateID(string s)
        {
            foreach (SMState ss in States)
            {
                if (ss.Name.Equals(s))
                {
                    if (ss.StateId < 0)
                        ss.StateId = GetNextID();
                    return ss.StateId;
                }
            }

            return -1;
        }

        /// <summary>
        /// Prepares state machine for test running.
        /// - Initialization of state IDs
        /// - Compilation of state's actions
        /// </summary>
        /// <returns>Number of errors during preparation. Count 0 means preparation went without errors.</returns>
        public int PrepareForRun()
        {
            ErrorLog.Clear();
            int countErrors = 0;

            int initStateId = GetStateID(InitialState);
            if (initStateId < 0)
                throw new Exception("Initial state is not defined.");

            foreach (SMState st in States)
            {
                if (st.StateId < 0)
                {
                    st.StateId = GetNextID();
                    ErrorLog.AppendFormat("Assigned ID {0} to state {1}\n", st.StateId, st.Name);
                }
                foreach (SMStateProcessing sp in st.Process)
                {
                    try
                    {
                        sp.Compile();
                    }
                    catch (Exception ex)
                    {
                        ErrorLog.AppendFormat("Error during compiling of state \"{0}\" actions:\n{1}\n", st.Name, ex.Message);
                        countErrors++;
                    }

                    sp.CompiledNextState = GetStateID(sp.NextState);
                    if (sp.CompiledNextState < 0)
                    {
                        ErrorLog.AppendFormat("Unknown state \"{0}\" defined for next state property for state \"{1}\", comp {2}, values {3}\n", sp.NextState, st.Name, sp.Comparer, sp.Values);
                        countErrors++;
                    }
                }
            }

            return countErrors;

        }
    }
}
