using System;
using System.Collections.Generic;


namespace ECARules4AllPack.Utils
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class ActionAttribute : System.Attribute
    { 
        public enum BooleanCategory {YESNO, ONOFF, TRUEFALSE}
        public Type SubjectType { get; set; }
        public string Verb { get; set; }
        public Type ObjectType { get; set; }
        public string variableName { get; set; }
        public string ModifierString { get; set; }
        public Type ValueType { get; set; }
        public BooleanCategory BoolType { get; set; }

        public ActionAttribute(Type subjectType, string verb, Type objectType)
        {
            SubjectType = subjectType;
            Verb = verb;
            ObjectType = objectType;
            ValueType = null;
            variableName = "";
            ModifierString = "";
        }
        public ActionAttribute(Type subjectType, string verb, Type objectType, string modifierString, Type valueType)
        {
            SubjectType = subjectType;
            Verb = verb;
            ObjectType = objectType;
            ValueType = valueType;
            variableName = "";
            ModifierString = modifierString;
        }
        public ActionAttribute(Type subjectType, string verb, string variable, string modifierString, Type valueType)
        {
            SubjectType = subjectType;
            Verb = verb;
            variableName = variable;
            ValueType = valueType;
            ObjectType = null;
            ModifierString = modifierString;
        }
        public ActionAttribute(Type subjectType, string verb, string variable, string modifierString, Type valueType, BooleanCategory boolType) : this(subjectType, verb, variable, modifierString, valueType)
        {
            BoolType = boolType;
        }
        public ActionAttribute(Type subjectType, string verb)
        {
            SubjectType = subjectType;
            Verb = verb;
            ObjectType = null;
            ValueType = null;
            variableName = "";
            ModifierString = "";
        }

        public override string ToString()
        {
            if (variableName != "")
                return $"{SubjectType.Name} {Verb} {variableName} {ModifierString} {ValueType?.Name}";
            if (ValueType != null)
                return $"{SubjectType.Name} {Verb} {ObjectType?.Name} {ModifierString} {ValueType.Name}";
            if (ObjectType != null)
                return $"{SubjectType.Name} {Verb} {ObjectType.Name}";
            return $"{SubjectType.Name} {Verb}";

        }

    }

    public class ECARuleInfo
    {
        public Type EcaRuleType { get; set; }
        public List<ActionAttribute> Actions { get; internal set; }

        public ECARuleInfo()
        {
            Actions = new List<ActionAttribute>();
        }

    }
}
