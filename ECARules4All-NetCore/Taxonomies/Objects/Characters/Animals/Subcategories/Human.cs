﻿using System.Collections;
using ECARules4AllPack.Utils;
using UnityEngine;


namespace ECARules4AllPack.Taxonomies.Objects.Characters.Animals.Subcategories
{
    /// <summary>
    /// The <b>Human</b> class is used to represent a human character.
    /// A human can run, walk and swim. 
    /// </summary>
    [ECARules4All("human")]
    [RequireComponent(typeof(Animal))]
    [DisallowMultipleComponent]
    public class Human : MonoBehaviour
    {
        private bool isBusyMoving = false;

        /// <summary>
        /// <b>IdleAnimation</b>: is the animation that is played when the human is idle.
        /// </summary>
        public string IdleAnimation;

        /// <summary>
        /// <b>SwimAnimation</b>: is the animation that is played when the human is swimming.
        /// </summary>
        public string SwimAnimation;

        /// <summary>
        /// <b>RunAnimation</b>: is the animation that is played when the human is running.
        /// </summary>
        public string RunAnimation;

        /// <summary>
        /// <b>WalkAnimation</b>: is the animation that is played when the human is walking.
        /// </summary>
        public string WalkAnimation;

        private string selected;

        /// <summary>
        /// <b>Runs</b>: This method is used to move the human to a specific position with a running animation.
        /// </summary>
        /// <param name="p">The position where to run</param>
        [Action(typeof(Human), "runs to", typeof(Position))]
        public void Runs(Position p)
        {
            float speed = 2.0F;
            Vector3 endMarker = new Vector3(p.x, p.y, p.z);
            selected = RunAnimation;
            StartCoroutine(MoveObject(speed, endMarker));
        }

        /// <summary>
        /// <b>Runs</b>: This method is used to move the human through a path with a running animation.
        /// </summary>
        /// <param name="p">The path</param>
        [Action(typeof(Human), "runs on", typeof(Path))]
        public void Runs(Path p)
        {
            selected = RunAnimation;
            StartCoroutine(WaitForOrderedMovement(p, "runs"));
        }

        /// <summary>
        /// <b>Swims</b>: This method is used to move the human to a specific position with a swimming animation.
        /// </summary>
        /// <param name="p">The position where to swim</param>
        [Action(typeof(Human), "swims to", typeof(Position))]
        public void Swims(Position p)
        {
            float speed = 0.5F;
            Vector3 endMarker = new Vector3(p.x, p.y, p.z);
            selected = SwimAnimation;
            StartCoroutine(MoveObject(speed, endMarker));
        }

        /// <summary>
        /// <b>Swims</b>: This method is used to move the human through a path with a swimming animation.
        /// </summary>
        /// <param name="p">The path</param>
        [Action(typeof(Human), "swims on", typeof(Path))]
        public void Swims(Path p)
        {
            selected = SwimAnimation;
            StartCoroutine(WaitForOrderedMovement(p, "swims"));
        }

        /// <summary>
        /// <b>Walks</b>: This method is used to move the human to a specific position with a waling animation.
        /// </summary>
        /// <param name="p">The position where to walk</param>
        [Action(typeof(Human), "walks to", typeof(Position))]
        public void Walks(Position p)
        {
            float speed = 1.0F;
            Vector3 endMarker = new Vector3(p.x, p.y, p.z);
            selected = WalkAnimation;
            StartCoroutine(MoveObject(speed, endMarker));

        }

        /// <summary>
        /// <b>Walks</b>: This method is used to move the human through a path with a walking animation.
        /// </summary>
        /// <param name="p">The path</param>
        [Action(typeof(Human), "walks on", typeof(Path))]
        public void Walks(Path p)
        {
            selected = WalkAnimation;
            StartCoroutine(WaitForOrderedMovement(p, "walks"));
        }

        private IEnumerator MoveObject(float speed, Vector3 endMarker)
        {
            isBusyMoving = true;
            Animate(selected);
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
                GetComponent<ECAObject>().p.Assign(gameObject.transform.position);
                yield return null;
            }

            GetComponent<ECAObject>().p.Assign(gameObject.transform.position);
            isBusyMoving = false;
            Animate(IdleAnimation);
        }

        private IEnumerator WaitForOrderedMovement(Path p, string method)
        {
            foreach (Position pos in p.Points)
            {
                while (isBusyMoving)
                {
                    yield return null;
                }

                switch (method)
                {
                    case "runs":
                        Runs(pos);
                        break;
                    case "swims":
                        Swims(pos);
                        break;
                    case "walks":
                        Walks(pos);
                        break;
                }

            }
        }

        private void Animate(string animation)
        {
            gameObject.GetComponent<Animator>().Play(animation);
        }

    }
}
