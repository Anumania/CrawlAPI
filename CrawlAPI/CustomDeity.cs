using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace CrawlAPI
{
    public class CustomDeity  //entirely public version of the Deity class, edit whatever variables you want, then when you add this through DeityAPI, its converted to a real deity.
    {
        public CustomDeity()
        {

        }

        //should probably make this recursive somehow.
        public override string ToString()
        {
            FieldInfo[] fieldInfos = this.GetType().GetFields();
            foreach (FieldInfo info in fieldInfos)
            {
                if (info.FieldType == typeof(exSpriteAnimClip))
                {
                    return "still working on it";
                }
                else
                {
                    return (string)info.GetValue(this);
                }
            }
            return "still working on it";
        }
        public void SetToDefaults()
        {
            //FieldInfo[] targetFieldInfo = typeof(Deity).GetFields(BindingFlags.Instance | BindingFlags.NonPublic); //reference to field info of the class
            FieldInfo[] thisFieldInfo = typeof(CustomDeity).GetFields(BindingFlags.Instance | BindingFlags.Public); //reference to the field info here
            if (SystemDeity.Instance == null) //if there are any dieties calling this early,
                GameObject.Instantiate(Resources.FindObjectsOfTypeAll<SystemDeity>().FirstOrDefault()); //spawn SystemDeity early so that things can use GetDeity.
            Deity inst = SystemDeity.GetDeity(0); //hold this bc its referenced a ton

            //go through all fields in CustomDeity
            foreach(FieldInfo fieldInfo in thisFieldInfo)
            {
                //if Deity has a field that has the same name
                FieldInfo targetFieldInfo = typeof(Deity).GetField(fieldInfo.Name, BindingFlags.Instance | BindingFlags.NonPublic);
                if(targetFieldInfo!= null)
                {
                    //get the value of the customdeity field's value
                    object val = targetFieldInfo.GetValue(inst);
                    
                    if(val != null)
                    {
                        //if its a game object, try to clone the gameobject, this wont do much but it might fix a couple easy errors.
                        if(val.GetType() == typeof(GameObject[]))
                        {
                            val = ((GameObject[])val).Clone();
                        }
                        else
                        {
                            //set the value of this customdeity to the values in the default deity
                            fieldInfo.SetValue(this, val);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("oops" + fieldInfo.Name);
                }
            }
        }
        //text
        public string m_name = "";
        public string m_text = "";
        public string m_textFlavour = "";

        //anim
        public exSpriteAnimClip m_portrait;

        //audio
        public AudioCue m_soundSelected;

        //misc
        public bool m_neverChooseRandomly; //no idea what this is
        public bool m_isTrial; //neither

        //trial stuff
        public GameObject[] m_startingMonsters;
        public PlayerModifier[] m_modifiers;

        //starting modifiers
        public List<HeroStatModifier> m_heroStatModifiers = new List<HeroStatModifier>();
        public int m_gold; //starting gold
        public Weapon m_startingWeapon;
        public Powerup[] m_startingPowerups;

        //idk
        public bool m_useLargeFodder;
        public exSpriteAnimClip m_winHeroAnimation;
        public string m_winText;

        //hinderences 
        public bool m_weaponsForbidden;
        public bool m_spellsForbidden;
        public bool m_artifactsForbidden;
        public bool m_potionsForbidden;
    }
}
