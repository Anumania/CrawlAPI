using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace CrawlAPI
{
    public class CustomMonster
    {
        public CustomMonster()
        {

        }
        //still working on it...
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
                    return "still working on it";
                }
            }
            return "still working on it";
        }
        //gives ingame immutable player equivalent of custom monster. TODO: make the opposite for immutable player
        public void ToMonster(Player p)
        {
            //Player p = new Player();

            foreach (var field in this.GetType().GetFields())
            {
                try
                {
                    //Console.WriteLine(field.Name);
                    FieldInfo fldInfo = p.GetType().GetField(field.Name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    if (fldInfo == null)
                    {
                        continue;
                    }
                    if (fldInfo.Name == "m_evolveToCostOverride")
                    {
                        Player.EvolveCostOverride[] e = MonsterAPI.EvolveCostOverride.convert((MonsterAPI.EvolveCostOverride[])field.GetValue(this));

                        fldInfo.SetValue(p, e);
                    }
                    else
                    {
                        try
                        {
                            fldInfo.SetValue(p, field.GetValue(this));
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            fldInfo.SetValue(p, null);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    Console.WriteLine(field.Name);
                }
            }
            //return p;
        }
        public void SetToDefaults()
        {
            Player hero = MonsterAPI.GetMonster("EnemyGhoul");
            m_evolveTo = new Player[] { MonsterAPI.GetMonster("EnemyGnome"), MonsterAPI.GetMonster("EnemyGnome") };
            m_evolveToCostOverride = new MonsterAPI.EvolveCostOverride[]{
                new MonsterAPI.EvolveCostOverride(MonsterAPI.GetMonster("EnemyGnome").gameObject),
                new MonsterAPI.EvolveCostOverride(MonsterAPI.GetMonster("EnemyGnome").gameObject)
                };
            m_animationIdle = (exSpriteAnimClip)hero.GetType().GetField("m_animationIdle", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(hero);
            m_animationRun = (exSpriteAnimClip)hero.GetType().GetField("m_animationRun", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(hero);
            m_animationDead = (exSpriteAnimClip)hero.GetType().GetField("m_animationDead", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(hero);
            m_animationKnockback = (exSpriteAnimClip)hero.GetType().GetField("m_animationKnockback", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(hero);
            m_animationStatue = (exSpriteAnimClip)hero.GetType().GetField("m_animationStatue", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(hero);
            m_shadowPrefab = (PlayerShadow)hero.GetType().GetField("m_shadowPrefab", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(hero);
            m_spawnOnHit = (DamageEffectData[])hero.GetType().GetField("m_spawnOnHit", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(hero);
            m_spawnOnDeath = (DamageEffectData[])hero.GetType().GetField("m_spawnOnDeath", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(hero);

            m_attackOrderType = (Attacks.eAttackOrderType)hero.GetType().GetField("m_attackOrderType", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(hero);
            m_comboResetTime = (float)hero.GetType().GetField("m_comboResetTime", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(hero);
            m_showDirectionIndicator = true;
            m_hasSpecialAttack = (bool)hero.GetType().GetField("m_hasSpecialAttack", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(hero);
            m_specialAttack = (AttackData)hero.GetType().GetField("m_specialAttack", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(hero);
            m_attackPowerupPrefab = (Powerup)hero.GetType().GetField("m_attackPowerupPrefab", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(hero);
            m_holdToAim = true;

            m_knockbackCancelsAttack = true;

            m_soundHit = (AudioCue)hero.GetType().GetField("m_soundHit", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(hero);
            m_soundDie = (AudioCue)hero.GetType().GetField("m_soundDie", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(hero);

            m_portraitFlipped = (bool)hero.GetType().GetField("m_portraitFlipped", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(hero);
            m_portraitAnim = (exSpriteAnimClip)hero.GetType().GetField("m_portraitAnim", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(hero);
        }
        //Type
        public bool m_isHero = false;
        public bool m_isGhost = false;

        //XP
        public MinMaxRange m_xpOnKill = new MinMaxRange(3f, 4f);

        //Evolve
        public float m_evolveCost = 1f;
        public Player[] m_evolveTo; //fill
        public MonsterAPI.EvolveCostOverride[] m_evolveToCostOverride; //fill

        //Visuals
        public exSpriteAnimClip m_animationSpawn; //does not need to be filled
        public exSpriteAnimClip m_animationIdle; //fill
        public exSpriteAnimClip m_animationRun; //fill
        public exSpriteAnimClip m_animationDead; //fill
        public exSpriteAnimClip m_animationKnockback; //fill
        public exSpriteAnimClip m_animationStatue; //fill 
        public PlayerShadow m_shadowPrefab; //fill
        public bool m_allowRedShadow = true;
        public DamageEffectData[] m_spawnOnHit; //fill
        public DamageEffectData[] m_spawnOnDeath; //fill
        public bool m_bloodEffectsEnabled = true;
        public bool m_delayGhostSpawnOnDeath = false; //not fill

        //Attacks
        public Vector2 m_attackSpawnNode = Vector2.right;
        public Attacks.eAttackOrderType m_attackOrderType;
        public float m_comboResetTime;
        public bool m_showDirectionIndicator;
        public bool m_specialCancelsCooldown = true;
        public List<AttackData> m_attacks = new List<AttackData>(1);
        public bool m_hasSpecialAttack;
        public float m_specialAttackChargeTime = 5f;
        public AttackData m_specialAttack;
        public List<DamageEffectData> m_attackDamageEffects = new List<DamageEffectData>(1);
        public Powerup m_attackPowerupPrefab;
        public bool m_holdToAim;

        //Knockback
        public float m_knockbackSpeed = 200f;
        public float m_knockbackTime = 0.3f;
        public bool m_knockbackCancelsAttack;

        //Sound
        public AudioCue m_soundHit;
        public AudioCue m_soundDie;

        //Portrait
        public bool m_portraitEnabled = true;
        public Vector2 m_portraitOffset = Vector2.zero;
        public Vector2 m_portraitOffsetEvolve = Vector2.zero;
        public bool m_portraitFlipped;
        public exSpriteAnimClip m_portraitAnim;

        //Bots
        public float m_botTargetChance = 1f; //If less than 1 then there'll be a chance that hero  won't prioritise this as a target above others, lower the value, less the chance
    }
}
