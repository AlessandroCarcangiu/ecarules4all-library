using System.Collections;
using UnityEngine;
using ECARules4AllPack.Utils;


namespace ECARules4AllPack.Taxonomies.Objects.Vehicles.Subcategories
{
    [ECARules4All("air-vehicle")]
    [RequireComponent(typeof(Vehicle))]
    [DisallowMultipleComponent]
    public class AirVehicle : MonoBehaviour
    {
        private bool isBusyMoving = false;

        [Action(typeof(AirVehicle), "takes-off", typeof(Position))]
        public void TakesOff(Position p)
        {
            float speed = 10.0F;
            Vector3 endMarker = new Vector3(p.x, p.y, p.z);
            StartCoroutine(MoveObject(speed, endMarker));
        }

        [Action(typeof(AirVehicle), "lands", typeof(Position))]
        public void Lands(Position p)
        {
            float speed = 10.0F;
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