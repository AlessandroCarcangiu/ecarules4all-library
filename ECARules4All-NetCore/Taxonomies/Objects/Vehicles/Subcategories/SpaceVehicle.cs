using System.Collections;
using UnityEngine;
using ECARules4AllPack.Utils;


namespace ECARules4AllPack.Taxonomies.Objects.Vehicles.Subcategories
{
    [ECARules4All("space-vehicle")]
    [RequireComponent(typeof(Vehicle))]
    [DisallowMultipleComponent]
    public class SpaceVehicle : MonoBehaviour
    {
        private bool isBusyMoving;

        [StateVariable("oxygen", ECARules4AllType.Float)]
        public float oxygen;

        [StateVariable("gravity", ECARules4AllType.Float)]
        public float gravity;

        [Action(typeof(SpaceVehicle), "takes-off", typeof(Position))]
        public void TakesOff(Position p)
        {
            float speed = 20.0F;
            Vector3 endMarker = new Vector3(p.x, p.y, p.z);
            StartCoroutine(MoveObject(speed, endMarker));
        }

        [Action(typeof(SpaceVehicle), "lands", typeof(Position))]
        public void Lands(Position p)
        {
            float speed = 20.0F;
            Vector3 endMarker = new Vector3(p.x, p.y, p.z);
            StartCoroutine(MoveObject(speed, endMarker));
        }

        private IEnumerator MoveObject(float speed, Vector3 endMarker)
        {
            isBusyMoving = true;
            Vector3 startMarker = gameObject.transform.position;
            float startTime = Time.time;
            float journeyLength = Vector3.Distance(startMarker, endMarker);
            while (gameObject.transform.position != endMarker)
            {
                float distCovered = (Time.time - startTime) * speed;

                // Fraction of journey completed equals current distance divided by total distance.
                float fractionOfJourney = distCovered / journeyLength;

                // Set our position as a fraction of the distance between the markers.

                gameObject.transform.position = Vector3.Lerp(startMarker, endMarker, fractionOfJourney);
                yield return null;
            }

            isBusyMoving = false;
        }
    }
}
