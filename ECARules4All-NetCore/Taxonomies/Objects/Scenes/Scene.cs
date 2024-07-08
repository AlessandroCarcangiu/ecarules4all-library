using UnityEngine;
using ECARules4AllPack.Taxonomies.Objects.Characters;
using ECARules4AllPack.Utils;


namespace ECARules4AllPack.Taxonomies.Objects.Scenes
{
    [ECARules4All("scene")]
    [RequireComponent(typeof(ECAObject))]
    [DisallowMultipleComponent]
    public class Scene : MonoBehaviour
    {
        //DOUBT: Aggiunto in questo file nome e posizione della scena di arrivo, può avere senso?
        //This component renames the GameObject to the Scene name, for "rule starting on start" purposes
        [StateVariable("name", ECARules4AllType.Text)]
        public string name;

        [StateVariable("position", ECARules4AllType.Position)]
        public Position position;

        private void Start()
        {
            name = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            gameObject.name = name;

        }

        [Action(typeof(Character), "teleports to", typeof(Scene))]
        public void _Teleports()
        {

        }
    }
}
