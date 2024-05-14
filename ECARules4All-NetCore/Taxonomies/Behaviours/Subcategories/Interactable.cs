using UnityEngine;
using ECARules4AllPack.Utils;


namespace ECARules4AllPack.Taxonomies.Behaviours.Subcategories
{
    /// <summary>
    /// <b>Interactable</b> is a <see cref="Behaviour">Behaviour</see> that can be attached to an object in order to make it
    /// interactable with the player collison. If the action is not player initiated, then refer to <see cref="Trigger"/>
    /// </summary>
    [ECARules4All("interactable")]
    [RequireComponent(typeof(Behaviour))]
    [RequireComponent(typeof(Collider))]
    [DisallowMultipleComponent]
    public class Interactable : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            //PROVE
            EventBus.GetInstance().Publish(new Action(other.gameObject, "interacts with", this.gameObject));

        }

        private void OnCollisionEnter(Collision other)
        {
            EventBus.GetInstance().Publish(new Action(other.gameObject, "interacts with", this.gameObject));
        }

        private void OnTriggerExit(Collider other)
        {
            //PROVE
            EventBus.GetInstance().Publish(new Action(other.gameObject, "stops-interacting with", this.gameObject));

        }

        private void OnCollisionExit(Collision other)
        {
            EventBus.GetInstance().Publish(new Action(other.gameObject, "stops-interacting with", this.gameObject));
        }
    }
}
