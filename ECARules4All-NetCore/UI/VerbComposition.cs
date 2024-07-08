using ECARules4AllPack.Utils;

namespace ECARules4AllPack.UI
{
    public class VerbComposition
    {
        private string verb;
        private ActionAttribute _actionAttribute;

        public VerbComposition(string verb, ActionAttribute actionAttribute)
        {
            Verb = verb;
            ActionAttribute = actionAttribute;
        }

        public string Verb
        {
            get => verb;
            set => verb = value;
        }

        public ActionAttribute ActionAttribute
        {
            get => _actionAttribute;
            set => _actionAttribute = value;
        }

        public static bool operator ==(VerbComposition obj1, VerbComposition obj2)
        {
            return obj1.ActionEquals(obj2);
        }

        public static bool operator !=(VerbComposition obj1, VerbComposition obj2)
        {
            return !obj1.ActionEquals(obj2);
        }

        public bool ActionEquals(VerbComposition action)
        {
            if (!Verb.Equals(action.Verb))
                return false;
            if (!(ActionAttribute.ObjectType == action.ActionAttribute.ObjectType))
                return false;
            if (!(ActionAttribute.SubjectType == action.ActionAttribute.SubjectType))
                return false;
            if (!(ActionAttribute.Verb.Equals(action.ActionAttribute.Verb)))
                return false;
            return true;
        }
    }
}