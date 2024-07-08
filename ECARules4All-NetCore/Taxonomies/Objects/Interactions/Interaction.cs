using UnityEngine;
using ECARules4AllPack.Utils;

namespace ECARules4AllPack.Taxonomies.Objects.Interactions
{
    /// <summary>
    /// <b>Interaction</b> contains all the elements allowing some interaction with the scene and its objects.
    /// The difference between interactions and <see cref="ECARules4All.RuleEngine.Behaviour">Behaviours </see> is that interactions
    /// are the ones that a typical user would perceive as physical entities of their own, which would exists
    /// independently from other objects.
    /// </summary>
    [ECARules4All("interaction")]
    [RequireComponent(typeof(ECAObject))]
    [DisallowMultipleComponent]
    public class Interaction : MonoBehaviour
    {

    }
}
