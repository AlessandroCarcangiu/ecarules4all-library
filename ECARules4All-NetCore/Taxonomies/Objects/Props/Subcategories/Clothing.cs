﻿using System.Collections.Generic;
using ECARules4AllPack.Taxonomies.Objects.Characters;
using ECARules4AllPack.Taxonomies.Objects.Characters.Animals.Subcategories;
using UnityEngine;
using ECARules4AllPack.Utils;


namespace ECARules4AllPack.Taxonomies.Objects.Props.Subcategories
{
    /// <summary>
    /// <b>Clothing</b>: This class is used to define the clothing properties of the objects.
    /// </summary>
    [ECARules4All("clothing")]
    [RequireComponent(typeof(Prop))]
    [DisallowMultipleComponent]
    public class Clothing : MonoBehaviour
    {
        /// <summary>
        /// <b>ClothingCategories</b>: This enum is used to define the clothing categories: TOP, PANTS, SHOES and HAT.
        /// </summary>
        public enum ClothingCategories
        {
            TOP,
            PANTS,
            SHOES,
            HAT
        }

        private Vector3 defaultPosition;
        private Quaternion defaultRotation;

        /// <summary>
        /// <b>Brand</b>: This property is used to define the brand of the clothing.
        /// </summary>
        [StateVariable("brand", ECARules4AllType.Text)]
        public string brand;

        /// <summary>
        /// <b>Color</b>: This property is used to define the color of the clothing.
        /// </summary>
        [StateVariable("color", ECARules4AllType.Color)]
        public Color color;

        /// <summary>
        /// <b>Size/b> This property is used to define the size of the clothing.
        /// </summary>
        [StateVariable("size", ECARules4AllType.Text)]
        public string size;

        /// <summary>
        /// <b>Weared</b>: This property is used to define if the clothing is weared or not.
        /// </summary>
        [StateVariable("weared", ECARules4AllType.Boolean)]
        public ECABoolean weared = new ECABoolean(ECABoolean.BoolType.NO);

        [HideInInspector] public GameObject wearedBy;


        private SkinnedMeshRenderer characterRenderer;
        private bool allowNonMatchingSkeletons = true;
        private SkinnedMeshRenderer[] clothMeshRenderers;
        private Dictionary<string, Transform> boneMap = new Dictionary<string, Transform>();

        public ClothingCategories type;

        private void Start()
        {
            defaultPosition = transform.position;
            defaultRotation = transform.rotation;
        }

        /// <summary>
        /// <b>_Wears</b>: This method is used to allow the mannequin to wear the clothing.
        /// </summary>
        /// <param name="m">The mannequin that wears the clothing</param>
        [Action(typeof(Mannequin), "wears", typeof(Clothing))]
        public void _Wears(Mannequin m)
        {
            wearedBy = m.gameObject;
            Vector3 midPoint;
            Vector3 pos1;
            Vector3 pos2;

            switch (type)
            {
                case ClothingCategories.HAT:
                    transform.position = m.head.transform.position;
                    transform.rotation = m.head.transform.rotation;
                    break;
                case ClothingCategories.TOP:
                    transform.position = m.torso.transform.position;
                    transform.rotation = m.torso.transform.rotation;
                    break;
                case ClothingCategories.PANTS:
                    pos1 = m.leftLeg.transform.position;
                    pos2 = m.rightLeg.transform.position;
                    midPoint = (pos1 + pos2) / 2f;
                    transform.position = midPoint;
                    break;
                case ClothingCategories.SHOES:
                    pos1 = m.leftFoot.transform.position;
                    pos2 = m.rightFoot.transform.position;
                    midPoint = (pos1 + pos2) / 2f;
                    transform.position = midPoint;
                    break;
            }

            weared.Assign(ECABoolean.BoolType.YES);
            m.AssignDress(this);
        }

        /// <summary>
        /// <b>_Unwears</b>: This method is used to allow the mannequin to unwear the clothing.
        /// </summary>
        /// <param name="m">The mannequin that unwears the clothing</param>
        [Action(typeof(Mannequin), "unwears", typeof(Clothing))]
        public void _Unwears(Mannequin m)
        {
            if (m.gameObject == wearedBy)
            {
                weared.Assign(ECABoolean.BoolType.NO);
                m.RemoveDress(this);
                transform.position = defaultPosition;
                transform.rotation = defaultRotation;
                wearedBy = null;
            }
        }

        /// <summary>
        /// <b>_Wears</b>: This method is used to allow the character to wear the clothing.
        /// </summary>
        /// <param name="c">The character that wears the clothing</param>
        [Action(typeof(Character), "wears", typeof(Clothing))]
        public void _Wears(Character c)
        {
            wearedBy = c.gameObject;
            characterRenderer = c.GetComponentInChildren<SkinnedMeshRenderer>();
            clothMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();

            if (clothMeshRenderers.Length > 0)
            {
                MapBones();
            }

            weared.Assign(ECABoolean.BoolType.YES);
        }

        /// <summary>
        /// <b>_Unwears</b>: This method is used to allow the character to unwear the clothing.
        /// </summary>
        /// <param name="c">The character that unwears the clothing</param>
        [Action(typeof(Character), "unwears", typeof(Clothing))]
        public void _Unwears(Character c)
        {
            if (c.gameObject == wearedBy)
            {
                weared.Assign(ECABoolean.BoolType.NO);
                wearedBy = null;
            }
        }


        private void MapBones()
        {
            //Create Map
            char[] splitChars = { '.', ',', ':', ';' };
            foreach (Transform bone in characterRenderer.bones)
            {
                //Remove eventual prefixes
                string[] split = bone.gameObject.name.Split(splitChars);
                boneMap[split[split.Length - 1]] = bone;
            }

            //Do the actual mapping if it is possibile
            foreach (SkinnedMeshRenderer clothMeshRenderer in clothMeshRenderers)
            {
                bool mappingIsRight = true;

                Transform[] newBones = new Transform[clothMeshRenderer.bones.Length];
                for (int i = 0; i < clothMeshRenderer.bones.Length; ++i)
                {
                    GameObject bone = clothMeshRenderer.bones[i].gameObject;
                    string[] split = bone.gameObject.name.Split(splitChars);

                    if (!boneMap.TryGetValue(split[split.Length - 1], out newBones[i]))
                    {
                        Debug.Log("The bone " + bone.name + " doesn't exist in the target skeleton.");
                        mappingIsRight = false;
                    }
                }

                if (mappingIsRight || (!mappingIsRight && allowNonMatchingSkeletons))
                {
                    clothMeshRenderer.bones = newBones;
                    clothMeshRenderer.rootBone = characterRenderer.rootBone;
                }
            }
        }
    }
}