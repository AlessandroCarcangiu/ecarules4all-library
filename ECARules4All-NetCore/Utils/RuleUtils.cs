﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Antlr4.Runtime.Misc;
using ECARules4AllPack;
using ECARules4AllPack.UI;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using ECARules4AllPack.Parsers;
using Action = ECARules4AllPack.Action;
using Behaviour = ECARules4AllPack.Behaviour;
using Object = UnityEngine.Object;
using SystemIOPath = System.IO.Path;


namespace ECARules4AllPack.Utils
{
    public struct StringAction
    {
        private string subj;

        public string Subj
        {
            get => subj;
            set => subj = value;
        }

        public string Verb
        {
            get => verb;
            set => verb = value;
        }

        public string Obj
        {
            get => obj;
            set => obj = value;
        }

        public string Value
        {
            get => value;
            set => this.value = value;
        }

        private string verb;
        private string obj;
        private string value;
        private string prep;
        public string Prep
        {
            get => prep;
            set => this.prep = value;
        }
        private string objThe;
        public string ObjThe
        {
            get => objThe;
            set => this.objThe = value;
        }
    }
    
    public struct StringCondition
    {
        private string toCheck;
        private string andOr;

        public string AndOr
        {
            get => andOr;
            set => andOr = value;
        }

        public string ToCheck
        {
            get => toCheck;
            set => toCheck = value;
        }

        public string Property
        {
            get => property;
            set => property = value;
        }

        public string CheckSymbol
        {
            get => checkSymbol;
            set => checkSymbol = value;
        }

        public string CompareWith
        {
            get => compareWith;
            set => compareWith = value;
        }

        private string property;
        private string checkSymbol;
        private string compareWith;
    }
    
    public struct RuleString
    {
        private StringAction eventString;
        private List<StringCondition> conditions;
        private List<StringAction> actionsString;
        
        public RuleString(StringAction eventString, List<StringCondition> conditions, List<StringAction> actionsString)
        {
            this.eventString = eventString;
            this.conditions = conditions;
            this.actionsString = actionsString;
        }

        public StringAction EventString
        {
            get => eventString;
            set => eventString = value;
        }

        public List<StringCondition> Conditions
        {
            get => conditions;
            set => conditions = value;
        }

        public List<StringAction> ActionsString
        {
            get => actionsString;
            set => actionsString = value;
        }
    }
    
    public class RuleUtils : MonoBehaviour
    {
        // to be removed
        public static Dictionary<Color, string> reversedColorDict = new Dictionary<Color, string>()
        {
            // { UIColors.blue, "blue" }, // 0xff1f77b4,
            {UIColors.orange, "orange"}, // 0xffff7f0e
            {UIColors.green, "green"}, // 0xffd62728
            {UIColors.red, "red"}, // 0xff9467bd
            {UIColors.purple, "purple"}, // 0xff9467bd
            {UIColors.brown, "brown"}, // 0xff8c564b
            {UIColors.pink, "pink"}, // 0xffe377c2
            {UIColors.gray, "gray"}, // 0xff7f7f7f
            {UIColors.grey, "grey"}, // 0xff7f7f7f
            {UIColors.yellow, "yellow"}, // 0xffbcbd22
            {UIColors.cyan, "cyan"}, // 0xff17becf
            {UIColors.white, "white"}, // 0xffffffff
        };
        
        public struct RulesStruct
        {
            public GameObject prefab;
            public Rule rule;
            public RuleString ruleString;

            public RulesStruct(GameObject prefab, Rule rule, RuleString ruleString)
            {
                this.prefab = prefab;
                this.rule = rule;
                this.ruleString = ruleString;
            }
        }

        // Dictionary mapping GameObjects and colors for the interface
        public static Dictionary<GameObject, Color> interfaceObjectColors = new Dictionary<GameObject, Color>();

        //Dictionary of Rules
        public static Dictionary<string, RulesStruct> rulesDictionary =
            new Dictionary<string, RulesStruct>();
        
        public static RuleString ConvertRuleObjectToRuleString(Rule rule, string ruleText)
        {
            RuleString ruleString = new RuleString() { };
            List<StringAction> actionString = new List<StringAction>();
            List<StringCondition> conditionString = new List<StringCondition>();
            StringAction eventString = new StringAction();
        
            //When action
            Action eventRule = rule.GetEvent();
            //find when row in the text
            string whenString = FindElementInText(ruleText, "when");
            //convert to StringAction
            eventString = ConvertActionToString(eventRule, whenString);
        
            //First Then action
            List<Action> listOfActions = rule.GetActions();
            //using a regex I find all the actions in the file searching for anything that starts with "the" and ends with ";"
            Regex rgx = new Regex("(?<=the\\s)(.*?)(?=;)");
            int i = 0;
            foreach (Match match in rgx.Matches(ruleText))
            {
                actionString.Add(ConvertActionToString(listOfActions[i], match.Value));
                i++;
            }
            
            //Conditions
            Condition condition = rule.GetCondition();
            if (condition!=null)
            {
                if (condition.GetType() == typeof(SimpleCondition)) 
                    conditionString.Add(ConvertConditionToString((SimpleCondition)condition, ruleText, null));
                else
                {
                    CompositeCondition ccondition = condition as CompositeCondition;
                    conditionString = ConvertCompositeCondition(ccondition, ruleText);
                }
            }

            ruleString.EventString = eventString;
            ruleString.ActionsString = actionString;
            ruleString.Conditions = conditionString;
            return ruleString;
        }
        
        
        private static StringCondition ConvertConditionToString(SimpleCondition condition, string stringText, string andOr)
        {
            StringCondition stringCondition = new StringCondition();
            //toCheck
            string toCheck = condition.GetSubject().name;
            string toCheckType = Regex.Match(stringText, "\\w+(?=\\s+"+toCheck+")").Groups[0].Value;
            stringCondition.ToCheck = FirstCharToUpper(toCheckType) + " " + toCheck;
        
            //property
            stringCondition.Property = condition.GetProperty();
        
            //checksymbol
            stringCondition.CheckSymbol = condition.GetSymbol();
        
            //comparewith
            string compareWith = condition.GetValueToCompare().ToString();
            //the word before comparewith, it can be a type (e.g. color blue -> color) or 
            //it can only the comparewith (e.g. is on)
            // The regex extracts the word BEFORE compareWith
            string objType = Regex.Match(stringText, "\\w+(?=\\s+"+compareWith+")").Groups[0].Value;
            if (objType != stringCondition.CheckSymbol) 
            {
                stringCondition.CompareWith = objType + " " + compareWith;
            }
            else stringCondition.CompareWith = compareWith;
            if (andOr != null) stringCondition.AndOr = andOr;
        
            return stringCondition;
        }

        private static List<StringCondition> ConvertCompositeCondition(CompositeCondition condition,
            string stringText)
        {
            // From the text, get all conditions
            // Regex: (?<=^if|^and\s|^or\s)(.*) -> picks all lines staring with either if, and or or
            // Use Matches to do this
            
            // for each match -> string => call ConvertSimpleCondition

            List<StringCondition> stringConditions = new List<StringCondition>();
            
            Regex rgx = new Regex("(?<=\\^if\\s|\\^and\\s|\\^or\\s)(.*)");
            int i = 0;
            var matches = rgx.Matches(stringText);
            // foreach (var c in condition.Children())
            // {
            //     // conditions = ;
            // }
            //
            // foreach (Match match in )
            // {
            //     actionString.Add(ConvertActionToString(listOfActions[i], match.Value));
            //     i++;
            // }

            return stringConditions;
        }
        
        static string FindElementInText(string rule, string elem)
        {
            return Regex.Match(rule, elem + " .*?\n").Groups[0].Value;
        }
        
        public static string FirstCharToUpper(string input) =>
            input switch
            {
                null => throw new ArgumentNullException(nameof(input)),
                "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
                _ => input.First().ToString().ToUpper() + input.Substring(1)
            };
        
        
        public static StringAction ConvertActionToString(Action action, string stringText)
        {
            StringAction stringAction = new StringAction();
        
            //subject
            string subj = action.GetSubject().name;
            string subjType = Regex.Match(stringText, "\\w+(?=\\s+"+subj+")").Groups[0].Value;
            stringAction.Subj = FirstCharToUpper(subjType) + " " + subj;
        
            //verb
            stringAction.Verb = action.GetActionMethod();
        
            //object
            if (action.GetObject() != null && action.GetModifierValue()==null)
            {
                //object name
                string obj = action.GetObject().ToString();
                
                // Remove (UnityEngine.GameObject) from "obj" (e.g.: obj = "bigButton1 (UnityEngine.GameObject)")
                obj = obj.Split('(')[0];
                obj = obj.Remove(obj.Length-1, 1);
                //the word before object, it can be a type (e.g. interacts with the character x -> character) or 
                //it can be the verb (e.g. turns on)
                string objType = Regex.Match(stringText, "\\w+(?=\\s+"+obj+")").Groups[0].Value;

                if (objType != stringAction.Verb) //maybe not needed
                {
                    stringAction.Obj = FirstCharToUpper(objType) + " " + obj;
                }
                else stringAction.Obj = obj;
                
                //check if there is a "the" before the object
                int theOccurrences = Regex.Matches(stringText, " the ").Count;
                if (theOccurrences>0) stringAction.ObjThe = "The";
                
            }

            //object + value
            if (action.GetModifierValue() != null)
            {
                //object name
                string obj = action.GetObject().ToString();
                stringAction.Obj = obj;
                stringAction.Prep = action.GetModifier();
                // stringAction.Value = Regex.Match(stringText, "\\w+(?=\\s+"+stringAction.Value+")").Groups[0].Value;
                // (?<=to\s).*  /// Picks everything after "to"

                stringAction.Value = Regex.Match(stringText, "(?<=" + stringAction.Prep + "\\s).*").Groups[0].Value.Split(' ')[0];

            }
            return stringAction;
        }

        //Some verbs have more action attributes but inherit from two different ecascript, but they are the same, so we check if the verb
        //is equal
        public static bool SameAttributesList(List<ActionAttribute> list)
        {
            string previousVerb = list[0].Verb;
            bool flag = true;
            foreach (var act in list)
            {
                flag = flag && (act.Verb == previousVerb);
            }

            if (flag && !string.IsNullOrEmpty(list[0].variableName))
            {
                string previousVariableName = list[0].variableName;
                foreach (var vAttribute in list)
                {
                    flag = flag && (vAttribute.variableName == previousVariableName);
                }
            }
            return flag;
        }

        public static Dictionary<int, Dictionary<GameObject, string>> FindSubjects()
        {
            //ref to gameObject and inner type of ecaComponent
            var oldResult = new Dictionary<GameObject, string>();
            var result = new Dictionary<int, Dictionary<GameObject, string>>();

            //the subjects are eca components inside the scene
            var foundSubjects = FindObjectsOfType<ECAObject>();
            
            int i = 0;
            //foreach gameobject found with the ecaobject script
            foreach (var ecaObject in foundSubjects)
            {
                string type = FindInnerTypeNotBehaviour(ecaObject.gameObject);
                Dictionary<GameObject, string> dictionary = new Dictionary<GameObject, string>()
                    {{ecaObject.gameObject, type}};
                result.Add(i, dictionary);
                i++;
            }

            return result;
        }

        /// <summary>
        /// Returns behaviour children
        /// </summary>
        /// <param name="listOfEcaAttributes"></param>
        /// <returns></returns>
        static ArrayList<Type> FindBehaviourChildrenAmongEcaAttributes(List<Type> listOfEcaAttributes)
        {
            ArrayList<Type> behaviours = new ArrayList<Type>();
            foreach (var type in listOfEcaAttributes)
            {
                RequireComponent[] requiredComponentsAtts = Attribute.GetCustomAttributes(type,
                    typeof(RequireComponent), true) as RequireComponent[];

                if (requiredComponentsAtts.Length > 0) //e.g. interactable has two requires
                {
                    if (requiredComponentsAtts[0] != null &&
                        requiredComponentsAtts[0].m_Type0 == typeof(Behaviour)) //behaviour children
                    {
                        behaviours.Add(type);
                    }
                }
            }

            return behaviours;
        }

        public static string FindInnerTypeNotBehaviour(GameObject gameObject)
        {
            //retrieve list of EcaAttributes of the gameobject
            List<Type> listOfEcaAttributes = RetrieveECAAttributes(gameObject);

            //from here we search among the attributes and we create another list without 
            //the behaviour and the children of the behaviour

            //first, we search for the behaviour
            Type behaviour = FindBehaviourAmongEcaAttributes(listOfEcaAttributes);
            if (behaviour != null)
            {
                listOfEcaAttributes.Remove(behaviour);
                //if there is a behaviour, probably there will be the children
                ArrayList<Type> behaviourChildren = FindBehaviourChildrenAmongEcaAttributes(listOfEcaAttributes);
                if (behaviourChildren != null)
                {
                    foreach (var beh in behaviourChildren)
                    {
                        listOfEcaAttributes.Remove(beh);
                    }
                }
            }
            return FindTheInnerOne(listOfEcaAttributes);
            ;
        }

        static List<Type> RetrieveECAAttributes(GameObject gameObject)
        {
            List<Type> listOfEcaAttributes = new List<Type>();
            Component[] listOfComponents = gameObject.GetComponents<Component>();
            foreach (Component c in listOfComponents)
            {
                Type cType = c.GetType();

                //searching for the components of type ecarules
                if (Attribute.IsDefined(cType, typeof(ECARules4AllAttribute)))
                {
                    //take all the feasible components
                    listOfEcaAttributes.Add(cType);
                }
            }

            return listOfEcaAttributes;
        }

        static Type FindBehaviourAmongEcaAttributes(List<Type> listOfEcaAttributes)
        {
            foreach (var type in listOfEcaAttributes)
            {
                if (type.Name.Equals("Behaviour"))
                {
                    return type;
                }
            }

            return null;
        }

        //checks if the gameobject has an entry for behaviour or not
        static bool checkIfBehaviour(GameObject g, Dictionary<int, Dictionary<GameObject, string>> subjects)
        {
            //if the behaviour is present it means that there are two entries of the dictionary with the same gameobject
            int i = 0;
            foreach (var s in subjects)
            {
                foreach (var ss in subjects[i])
                {
                    if (ss.Key == g)
                    {
                        i++;
                    }
                }
            }

            return i > 1;
        }

        //we pass bool passive when we have to retrieve passive verbs
        public static Dictionary<int, VerbComposition> FindActiveVerbs(GameObject subjSelected,
            Dictionary<int, Dictionary<GameObject, string>> subjects, [CanBeNull] string selectedType,
            bool passive)
        {
            Dictionary<int, VerbComposition> result = new Dictionary<int, VerbComposition>();
            int i = 0;
            bool behaviourExist = checkIfBehaviour(subjSelected, subjects);

            foreach (Component c in subjSelected.GetComponents<Component>())
            {
                Type cType = c.GetType();

                //searching for the components of type ecarules
                if (Attribute.IsDefined(cType, typeof(ECARules4AllAttribute)))
                {
                    if (behaviourExist)
                    {
                        //foreach component we find the verbs
                        var componentVerbs = ListActionsItem(cType);
                        foreach (var el in componentVerbs)
                        {
                            result.Add(i, el);
                            i++;
                        }
                    }
                    else
                    {
                        //foreach component we find the verbs
                        var componentVerbs = ListActionsItem(cType);
                        foreach (var el in componentVerbs)
                        {
                            //for example, food has the verb eats, that has as subject Character, we don't
                            //want to add it to the verbs of food
                            if (passive)
                            {
                                if (el.ActionAttribute.SubjectType.Name == cType.Name)
                                {
                                    result.Add(i, el);
                                    i++;
                                }
                            }
                            else
                            {
                                result.Add(i, el);
                                i++;
                            }
                        }
                    }

                    //result = result.Concat(componentVerbs).ToDictionary(s => s.Key, s => s.Value);
                }
            }

            /*Debug.Log("Verbs: ");
            foreach (KeyValuePair<int, VerbComposition> kvp in result)
            {
                Debug.Log( string.Format("Key = {0}, Value = {1}", kvp.Key, kvp.Value.Verb + kvp.Value.ActionAttribute) );
            }*/
            return result;
        }

        public static void FindPassiveVerbs(GameObject subjSelected,
            Dictionary<int, Dictionary<GameObject, string>> subjects, string selectedType,
            ref Dictionary<int, VerbComposition> activeVerbs)
        {
            List<string> ecaScriptOfTheGameobject = FindECAScripts(subjSelected);
            foreach (var subj in subjects)
            {
                foreach (var var in subj.Value)
                {
                    if (var.Key != subjSelected && var.Value != selectedType)
                    {
                        //we pass "passive" as false in order to include in the verbs of each subject even those who don't
                        //have itself as subject. (e.g. among Food verbs there will be Eats)
                        Dictionary<int, VerbComposition> verbs = FindActiveVerbs(var.Key, subjects, null, false);
                        //foreach verb of each subject
                        foreach (var v in verbs)
                        {
                            //selected type is the inner ecascript selected, but an animal is also a character,
                            //so we need to find also the verbs of the superior hierarchy, so we check with
                            //all the ecaScript
                            foreach (var script in ecaScriptOfTheGameobject)
                            {
                                if (v.Value.ActionAttribute.SubjectType.Name == script)
                                {
                                    //It doesn't already exist in my list
                                    if(!DictionaryContainsValue(activeVerbs, v.Value)) 
                                    {
                                        int lastIndex = activeVerbs.Count - 1;
                                        activeVerbs.Add(lastIndex + 1, v.Value);
                                    }
                                }
                                   
                            }
                            
                        }
                    }
                }
            }
        }

        private static List<string> FindECAScripts(GameObject gameObject)
        {
            List<string> result = new List<string>();
            foreach (Component c in gameObject.GetComponents<Component>())
            {
                Type cType = c.GetType();

                //searching for the components of type ecarules
                if (Attribute.IsDefined(cType, typeof(ECARules4AllAttribute)))
                {
                    result.Add(cType.Name);
                }
            }

            return result;
        }

        private static bool DictionaryContainsValue(Dictionary<int, VerbComposition> verbs, VerbComposition value)
        {
            foreach (var var in verbs)
            {
                if (var.Value.ActionEquals(value))
                {
                    return true;
                }
            }

            return false;
        }

        public static string FindTheInnerOne(List<Type> listEcaComponents)
        {
            Dictionary<int, string> depts = new Dictionary<int, string>();
            foreach (var comp in listEcaComponents)
            {
                int dept = GetDepth(comp, 0);
                depts.Add(dept, comp.Name);
            }

            var maxKey = depts.Keys.Max();
            return depts[maxKey];
        }

        /// <summary>
        /// Returns the dept of a ecarules component
        /// </summary>
        /// <param name="c"></param> type of a component
        /// <param name="depth"></param> the starting depth (0 if we want to search from ecaobject)
        /// <returns>the dept of a ecacomponent</returns>
        private static int GetDepth(MemberInfo c, int depth = 0)
        {
            MemberInfo info = c;
            object[] attributes = info.GetCustomAttributes(true);
            for (int i = 0; i < attributes.Length; i++)
            {
                if (attributes[i] is RequireComponent)
                {
                    MemberInfo new_info = ((RequireComponent) attributes[i]).m_Type0;
                    return GetDepth(new_info, depth + 1);
                }
            }

            return depth;
        }

        ///<summary>
        ///<c>ListActions</c> returns all the ActionAttribute tagged variables. 
        ///<para/>
        ///<strong>Parameters:</strong> 
        ///<list type="bullet">
        ///<item><description><paramref name="c"/>: The Type to check</description></item>
        ///</list>
        ///<para/>
        ///<strong>Returns:</strong> A dictionary of the string of the action and the object type (Position, Rotation...)
        ///</summary>
        public static List<VerbComposition> ListActionsItem(Type c)
        {
            List<VerbComposition> actions = new List<VerbComposition>();
            foreach (MethodInfo m in c.GetMethods())
            {
                var a = m.GetCustomAttributes(typeof(ActionAttribute), true);
                if (a.Length > 0)
                {
                    foreach (var item in a)
                    {
                        ActionAttribute ac = (ActionAttribute) item;
                        if (ac.ObjectType != null)
                        {
                            VerbComposition verbComposition = new VerbComposition(ac.ObjectType.ToString(), ac);
                            actions.Add(verbComposition);
                        }
                        else //siamo nel caso in cui abbiamo verbi tipo cambia visibilità a 
                        {
                            VerbComposition verbComposition = new VerbComposition(ac.variableName, ac);
                            actions.Add(verbComposition);
                        }
                    }
                }
            }

            return actions;
        }

        /**
         * Returns for each state variable the name and the ECARules4AllType
         */
        public static Dictionary<string, ECARules4AllType> FindStateVariables(GameObject gameObject)
        {
            Dictionary<string, ECARules4AllType> variables = new Dictionary<string, ECARules4AllType>();

            foreach (Component c in gameObject.GetComponents<Component>())
            {
                Type cType = c.GetType();

                //searching for the components of type ecarules
                if (Attribute.IsDefined(cType, typeof(ECARules4AllAttribute)))
                {
                    //foreach component we find the verbs
                    var componentVariables = ListStateVariables(cType);
                    foreach (var var in componentVariables)
                    {
                        if(!variables.ContainsKey(var.Key)) variables.Add(var.Key, var.Value);
                    }
                    //variables = variables.Concat(componentVariables).ToDictionary(s => s.Key, s => s.Value);
                 
                }
            }

            return variables;
        }


        public static Dictionary<string, ECARules4AllType> ListStateVariables(Type cType)
        {
            Dictionary<string, ECARules4AllType> variables = new Dictionary<string, ECARules4AllType>();
            foreach (FieldInfo m in cType.GetFields())
            {
                object[] a = m.GetCustomAttributes(typeof(StateVariableAttribute), true);
                if (a.Length > 0)
                {
                    foreach (var item in a)
                    {
                        StateVariableAttribute var = (StateVariableAttribute) item;
                        variables.Add(var.Name, var.type);
                    }
                }
            }

            return variables;
        }

        
        // to be removed - unity
        public static void ChangeColorGameObjectDropdown(GameObject gameObject, string type, Transform rowTransform)
        {
            // UIColors colore = new UIColors ();

            // between a given GameObject and a color to be used in the interface
            // Then, for each item assign the correct color to the dropdown via the function
            if (!interfaceObjectColors.Keys.Contains(gameObject))
            {
                // If the gameObject isn't in the dictionary we need to add it and assign it a color
                int numOfColors = reversedColorDict.Keys.Count;
                // The colors will repeat after a given number of item is used
                int idx = interfaceObjectColors.Keys.Count % numOfColors;
                // Get the color and add the mapping to the dictionary
                Color color = reversedColorDict.Keys.ElementAt(idx);
                interfaceObjectColors.Add(gameObject, color);
            }

            // Assign the color to the UI
            Color oColor = interfaceObjectColors[gameObject];
            outlineColor(gameObject, oColor);
            rowTransform.GetComponent<Image>().color = oColor; // todo get Image from UnityEngine.UI!!


            // Prima con due soggetti diversi
            // Poi con lo stesso due volte
            // Cambia soggetto e vedi se cambia colore
        }

        public static void outlineColor(GameObject gameObject, Color color)
        {
            ECAOutline ecaOutline = gameObject.GetComponent<ECAOutline>();
            if (ecaOutline == null)
            {
                ecaOutline = gameObject.AddComponent<ECAOutline>();
                ecaOutline.OutlineColor = color;
                ecaOutline.OutlineWidth = 5f;
            }
            else
            {
                ecaOutline.OutlineColor = color;
                ecaOutline.OutlineWidth = 5f;
            }
        }
        
        public static void printList(List<string> list)
        {
            foreach (var e in list)
            {
                Debug.Log(e);
            }
        }

        public static void printList(ECAObject[] list)
        {
            foreach (var e in list)
            {
                Debug.Log(e.ToString());
            }
        }

        public static void printList(Component[] list)
        {
            foreach (var e in list)
            {
                Debug.Log(e.ToString());
            }
        }
        
        // to be removed - unity ui
        public static void clearInputField(InputField inputfield)
        {
            inputfield.Select();
            inputfield.text = "";
        }

        public static string RemoveECAFromString(string ecaType)
        {
            return ecaType.Replace("ECA", "");
        }
        
        
        // to be removed - unity ui
        //When the object of the rule is not a component in the subject, we need to retrieve it from other gameobjects
        //e.g. character eats typeof(Food), the character is not also a food, so we need to find between all the
        //gameobjects the ones with Food component
        public static void AddObjectPassiveVerbs( Dictionary<int,Dictionary<GameObject,string>> subjects, string comp,
            Dropdown objDrop)
        {
            bool found = false;
            ArrayList<string> resArrayList = new ArrayList<string>();
                                
            foreach (var subj in subjects)
            {
                foreach (var var in subj.Value)
                {
                    if (var.Key.GetComponent(comp) != null)
                    {
                        found = true;
                        string type = FindInnerTypeNotBehaviour(var.Key);
                        type = RemoveECAFromString(type);
                        resArrayList.Add(type + " " + var.Key.name);
                    }
                }
            }

            if (found)
            {
                // Used to sort each dropdown's options
                List<string> entries = new List<string>();
                
                // objDrop.options.Add(new Dropdown.OptionData(""));
                foreach (var option in resArrayList)
                {
                    // objDrop.options.Add(new Dropdown.OptionData(option));
                    entries.Add(option);
                }
                AddToDropdownInAlphabeticalOrder(objDrop, entries);
            }
        }
        
        // to be removed - unity ui
        public static void AddObjectActiveVerbs(Dictionary<int, Dictionary<GameObject, string>> subjects, string comp,
            Dropdown objDrop, GameObject subjectSelected)
        {
            objDrop.options.Add(new Dropdown.OptionData(""));
            // Used to sort each dropdown's options
            List<string> entries = new List<string>();
            
            for (int i = 0; i < subjects.Count; i++)
            {
                foreach (KeyValuePair<GameObject, string> entry in subjects[i])
                {
                    //TODO handle alias
                    if (entry.Key != subjectSelected && entry.Key.GetComponent(comp))
                    {
                        string type = FindInnerTypeNotBehaviour(entry.Key);
                        type = RemoveECAFromString(type);
                        // objDrop.options.Add(new Dropdown.OptionData(type + " " + entry.Key.name));
                        entries.Add(type + " " + entry.Key.name);
                    }
                }
            }
            AddToDropdownInAlphabeticalOrder(objDrop, entries);
        }
        
        // to be removed - unity ui
        public static void LoadRulesAndAddToUI(string path)
        {
            // Read the txt file containing the rules
            TextRuleParser ruleParser = new TextRuleParser();
            ruleParser.ReadRuleFile(path);
            
            // For each rule in the RuleEngine, add it to the RuleList (UI)
            foreach (Rule rule in RuleEngine.GetInstance().Rules())
            {
                GameObject prefab = ButtonsHandle.CreateRuleRow(null, rule);
                string newRuleUuid = Guid.NewGuid().ToString();
                prefab.name = newRuleUuid;
                
                // TODO: Populate RuleString
                if (!rulesDictionary.ContainsKey(newRuleUuid))
                {
                    // Add to rulesDictionary
                    // Need the UUID, the RuleStruct (prefab, rule, ruleString)
                    GameObject ruleString2 = prefab.transform.GetChild(0).gameObject;
                    string textRule = ruleString2.GetComponent<Text>().text;
                    RuleString ruleString = ConvertRuleObjectToRuleString(rule, textRule);
                    rulesDictionary.Add(newRuleUuid, new RulesStruct(prefab, rule, ruleString));
                }
                
            }
            
            // Add to rulesDictionary
            // Need the UUID, the RuleStruct (prefab, rule, ruleString) [The ruleString will be temporarily null]
            
        }
        
        // to be removed - unity ui
        public static void AddToDropdownInAlphabeticalOrder(Dropdown dropdown, List<string> entries)
        {
            entries.Sort();
            foreach (var s in entries)
            {
                dropdown.options.Add(new Dropdown.OptionData( s));
            }
        }


        public static void SaveRulesToFile()
        {
            // Save the rules on the file
            TextRuleSerializer serializer = new TextRuleSerializer();
            // string path = Path.Combine("Assets", Path.Combine("Resources", "storedRules.txt"));
            //string path = Path.Combine(Directory.GetCurrentDirectory(), "storedRules.txt");
            string path = SystemIOPath.Combine(Application.streamingAssetsPath, "storedRules.txt");
            serializer.SaveRules(path);
        }
        

        // +--------------------------------------+
        // | Logic methods from DropdownHandle.cs |
        // +--------------------------------------+
         // Method to get the truncated string from dropdown text
        public string GetSelectedCutString(string selectedSubjectString)
        {
            return Regex.Match(selectedSubjectString, "[^ ]* (.*)").Groups[1].Value;
        }

        // Method to get the type of the selected subject from a dictionary
        public string GetSubjectType(GameObject subjectSelected, Dictionary<string, Dictionary<GameObject, string>> subjects)
        {
            foreach (var item in subjects)
            {
                foreach (var keyValuePair in item.Value)
                {
                    if (keyValuePair.Key == subjectSelected)
                    {
                        return keyValuePair.Value;
                    }
                }
            }
            return null;
        }

        // Method to get action attributes associated with the selected verb
        public List<ActionAttribute> GetActionAttributes(Dictionary<string, List<ActionAttribute>> verbsString, string verbSelectedString)
        {
            return verbsString[verbSelectedString];
        }

        // Method to handle action attributes for the object dropdown
        public List<string> GetObjectDropdownEntries(GameObject subjectSelected, Dictionary<int, Dictionary<GameObject, string>> subjects, ActionAttribute ac)
        {
            List<string> entries = new List<string>();
            if (ac.ObjectType != null)
            {
                switch (ac.ObjectType.Name)
                {
                    case "Object":
                    case "ECAObject":
                    case "GameObject":
                        for (int i = 0; i < subjects.Count; i++)
                        {
                            foreach (KeyValuePair<GameObject, string> entry in subjects[i])
                            {
                                if (entry.Key != subjectSelected)
                                {
                                    // string type = RuleUtils.FindInnerTypeNotBehaviour(entry.Key);
                                    // type = RuleUtils.RemoveECAFromString(type);
                                    // entries.add(type + " " + entry.Key.name);

                                    string type = FindInnerTypeNotBehaviour(entry.Key);
                                    type = RemoveECAFromString(type);
                                    entries.add(type + " " + entry.Key.name);
                                }
                            }
                        }
                        break;
                    case "YesNo":
                        entries.Add("yes");
                        entries.Add("no");
                        break;
                    case "TrueFalse":
                        entries.Add("true");
                        entries.Add("false");
                        break;
                    case "OnOff":
                        entries.Add("on");
                        entries.Add("off");
                        break;
                    case "Position":
                        Vector3 selectedPos = raycastPointer.pos;
                        if (selectedPos != Vector3.zero)
                        {
                            entries.Add("Last selected position");
                        }
                        break;
                    // Handle other cases...
                }
            }
            return entries;
        }

        // Method to handle value dropdown entries
        public List<string> GetValueDropdownEntries(List<ActionAttribute> actionAttributes)
        {
            List<string> entries = new List<string>();
            foreach (var ac in actionAttributes)
            {
                if (ac.ValueType != null)
                {
                    entries.Add(ac.variableName);
                }
            }
            return entries;
        }
        
        // Method to find the index of a dropdown option by the GameObject name
        public int FindDropdownOptionIndexByName(Dropdown dropdown, string objectName)
        {
            for (int i = 0; i < dropdown.options.Count; i++)
            {
                if (objectName == dropdown.options[i].text.Split(' ').Last())
                {
                    return i;
                }
            }
            return -1; // Object not found
        }


        // +-----------------------------------------------------------------+
        // | Methods from DropdownHandle.cs to check - probably to eliminate |
        // +-----------------------------------------------------------------+
        
        // Method to get active and passive verbs associated with the selected subject
        public List<VerbItem> GetVerbs(GameObject subjectSelected, Dictionary<string, Dictionary<GameObject, string>> subjects, string subjectSelectedType)
        {
            // var verbsItem = RuleUtils.FindActiveVerbs(subjectSelected, subjects, subjectSelectedType, true);
            // RuleUtils.FindPassiveVerbs(subjectSelected, subjects, subjectSelectedType, ref verbsItem);
            // return verbsItem;

            var verbsItem = FindActiveVerbs(subjectSelected, subjects, subjectSelectedType, true);
            FindPassiveVerbs(subjectSelected, subjects, subjectSelectedType, ref verbsItem);
            return verbsItem;
        }

        // Method to find a GameObject by its truncated name
        public GameObject FindGameObject(string selectedCutString)
        {
            return GameObject.Find(selectedCutString);
        }

        // Method to get the last selected object by raycast
        public GameObject GetLastSelectedObjectByRaycast()
        {
            return raycastPointer.LastSelectedObject;
        }

    }
}