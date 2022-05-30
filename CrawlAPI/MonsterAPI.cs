using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace CrawlAPI
{
    public static class MonsterAPI //used to add monsters. probably not gonna work well, replacing is probably better.
    {
        public static Player[] monsters;
        public static void Init()
        {
            monsters = (Player[])Resources.FindObjectsOfTypeAll(typeof(Player)); //resources will never add to this, so we should be safe to set it once.
        }

        public static Player GetMonster(string monsterName) //this does not work with mod added monsters!
        {
            return monsters.Where(e => e.name == monsterName).FirstOrDefault();
        }

        public static void AddMonster(this CustomDeity deity, CustomMonster monst, int slotIndex) //just in case you want to not just say deity.m_startingMonsters[0] = monst?
        {
            GameObject[] startingMonsters = deity.m_startingMonsters;
            GameObject startingMonster = GameObject.Instantiate(startingMonsters[slotIndex]);
            //Player p = startingMonsters[slotIndex].GetComponent<Player>();
            monst.ToMonster(startingMonster.GetComponent<Player>());
            startingMonsters[slotIndex] = startingMonster;
            GameObject.DontDestroyOnLoad(startingMonster);
            startingMonster.SetActive(false);
        }
        public static void AddMonster(this Deity deity, CustomMonster monst, int slotIndex) //extension method for adding monsters to deity
        {
            GameObject[] startingMonsters = deity.GetStartingMonsters();
            GameObject startingMonster = GameObject.Instantiate(startingMonsters[slotIndex]);
            //Player p = startingMonsters[slotIndex].GetComponent<Player>();
            monst.ToMonster(startingMonster.GetComponent<Player>());
            startingMonsters[slotIndex] = startingMonster;
            GameObject.DontDestroyOnLoad(startingMonster);
            startingMonster.SetActive(false);
        }
        //i wrote this a year ago im not really sure what the convert functions do.
        public class EvolveCostOverride
        {
            public EvolveCostOverride(GameObject evolveTo, float _price = 0)
            {
                price = _price;
                m_evolveTo = evolveTo;
            }
            public static implicit operator Player.EvolveCostOverride(EvolveCostOverride e)
            {
                var eo = new Player.EvolveCostOverride();
                eo.m_evolveTo = e.m_evolveTo;
                eo.price = e.price;
                return eo;
            }
            public static Player.EvolveCostOverride[] convert(EvolveCostOverride[] e)
            {
                var eoa = new Player.EvolveCostOverride[e.Length];
                for (int i = 0; i < eoa.Length; i++)
                {
                    var eo = new Player.EvolveCostOverride();
                    eo.m_evolveTo = e[i].m_evolveTo;
                    eo.price = e[i].price;
                    eoa[i] = eo;
                }
                return eoa;
            }
            public GameObject m_evolveTo;
            public float price;
        }

    }
}
