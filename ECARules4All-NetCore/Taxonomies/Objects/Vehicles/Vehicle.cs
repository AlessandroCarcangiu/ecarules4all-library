using System;
using UnityEngine;
using ECARules4AllPack.Utils;


namespace ECARules4AllPack.Taxonomies.Objects.Vehicles
{
    [ECARules4All("vehicle")]
    [RequireComponent(typeof(ECAObject))]
    [DisallowMultipleComponent]
    public class Vehicle : MonoBehaviour
    {
        [StateVariable("speed", ECARules4AllType.Float)]
        public float speed;

        [StateVariable("on", ECARules4AllType.Boolean)]
        public ECABoolean on = new ECABoolean(ECABoolean.BoolType.OFF);

        private Vector3 localForward;
        private ECAObject reference;

        [Action(typeof(Vehicle), "starts")]
        public void Starts()
        {
            on.Assign(ECABoolean.BoolType.ON);
        }

        //TODO: verb not present in grammar
        [Action(typeof(Vehicle), "steers-at", typeof(float))]
        public void Steers(float angle)
        {
            Quaternion rotation = gameObject.transform.rotation;
            gameObject.transform.Rotate(rotation.x, rotation.y + angle, rotation.z);
        }

        [Action(typeof(Vehicle), "accelerates-by", typeof(float))]
        public void Accelerates(float f)
        {
            speed += f;
        }

        [Action(typeof(Vehicle), "slows-by", typeof(float))]
        public void SlowsDown(float f)
        {
            speed -= f;
        }

        [Action(typeof(Vehicle), "stops")]
        public void Stops()
        {
            on.Assign(ECABoolean.BoolType.OFF);
            speed = 0;
        }

        private void Start()
        {
            localForward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
            reference = GetComponent<ECAObject>();
            reference.p.Assign(gameObject.transform.position);
            reference.r.Assign(gameObject.transform.rotation);
        }

        private void Update()
        {
            if (on)
            {
                transform.Translate(speed * Time.deltaTime * localForward);
                reference.p.Assign(gameObject.transform.position);
                reference.r.Assign(gameObject.transform.rotation);
            }
        }
    }
}
