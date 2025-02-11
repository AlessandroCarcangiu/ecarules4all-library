﻿using ECARules4AllPack.Taxonomies.Objects.Props.Subcategories;
using UnityEngine;
using ECARules4AllPack.Utils;


namespace ECARules4AllPack.Taxonomies.Objects.Characters.Animals.Subcategories
{
    /// <summary>
    /// The <b>Mannequin</b> class provides a way to include a character in the scene to wear 3D models of clothes that do not
    /// have rigging skeletons. Since the mannequin is supposed to stay still in the environment, the implementation contains
    /// for automatically positioning it on top of the mannequin according to the specified position (e.g., head, torso,
    /// left or right leg, arm, etc.). Provided that the distinction between a mannequin and a not-playable human is
    /// technical, it is up to the TB to decide which object offers the best configuration options considering the
    /// template under development.
    /// </summary>
    [ECARules4All("mannequin")]
    [RequireComponent(typeof(Animal))]
    [DisallowMultipleComponent]
    public class Mannequin : MonoBehaviour
    {
        /// <summary>
        /// <b>Torso</b>: is the GameObject that represents the torso in the mannequin.
        /// </summary>
        public GameObject torso;

        /// <summary>
        /// <b>leftLeg</b>: is the GameObject that represents the left leg in the mannequin.
        /// </summary>
        public GameObject leftLeg;

        /// <summary>
        /// <b>rightLeg</b>: is the GameObject that represents the right leg in the mannequin.
        /// </summary>
        public GameObject rightLeg;

        /// <summary>
        /// <b>leftFoot</b>: is the GameObject that represents the left foot in the mannequin.
        /// </summary>
        public GameObject leftFoot;

        /// <summary>
        /// <b>rightFoot</b>: is the GameObject that represents the right foot in the mannequin.
        /// </summary>
        public GameObject rightFoot;

        /// <summary>
        /// <b>head</b>: is the GameObject that represents the head in the mannequin.
        /// </summary>
        public GameObject head;

        public GameObject hip;
        
        
        private Clothing hat;
        private Clothing top;
        private Clothing pants;
        private Clothing shoes;

        /// <summary>
        /// <b>AssignDress</b>: This method is called to assign a dress to the mannequin.
        /// </summary>
        /// <param name="c">The <see cref="Clothing"/> object</param>
        public void AssignDress(Clothing c)
        {
            switch (c.type)
            {
                case Clothing.ClothingCategories.HAT:
                    if (hat != null && hat != c)
                        hat._Unwears(this);
                    hat = c;
                    break;
                case Clothing.ClothingCategories.TOP:
                    if (top != null && top != c)
                        top._Unwears(this);
                    top = c;
                    break;
                case Clothing.ClothingCategories.PANTS:
                    if (pants != null && pants != c)
                        pants._Unwears(this);
                    pants = c;
                    break;
                case Clothing.ClothingCategories.SHOES:
                    if (shoes != null && shoes != c)
                        shoes._Unwears(this);
                    shoes = c;
                    break;
            }
        }

        /// <summary>
        /// This method is called to remove a dress from the mannequin.
        /// </summary>
        /// <param name="c">The <see cref="Clothing"/> object</param>
        public void RemoveDress(Clothing c)
        {
            switch (c.type)
            {
                case Clothing.ClothingCategories.HAT:
                    if (c == hat)
                        hat = null;
                    break;
                case Clothing.ClothingCategories.TOP:
                    if (c == top)
                        top = null;
                    break;
                case Clothing.ClothingCategories.PANTS:
                    if (c == pants)
                        pants = null;
                    break;
                case Clothing.ClothingCategories.SHOES:
                    if (c == shoes)
                        shoes = null;
                    break;
            }
        }
    }
}