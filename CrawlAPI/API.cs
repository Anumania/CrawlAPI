using System;
using BepInEx;
using MonoMod.RuntimeDetour;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using Mono.Collections.Generic;
using Mono.Collections;
using MonoMod.Utils;
using Mono.Cecil;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Reflection.Emit;
using System.Diagnostics;

namespace CrawlAPI
{
    [BepInPlugin("org.anumania.plugins.crawlapi", "CrawlAPI", "1.0.0.0")]
    public class CrawlAPI : BaseUnityPlugin
    {
        public void Awake()
        {
            //run init for all apis.
            APIHelpers.Init();
            MonsterAPI.Init();
            DeityAPI.Init();

            Util.GetField(new CustomMonster(), "m_isHero");

            //make an object that will continuously turn off unity's debug console, because it opens every time anyone Debug.Log()'s
            On.MenuMain.Update += (a, b) => { Debug.developerConsoleVisible = false; }; //in debug mode, this dev console sucks
            GameObject noConsole = new GameObject();
            noConsole.AddComponent<NoConsole>();
            GameObject.DontDestroyOnLoad(noConsole);

            //add an entry to the debug menu
            SystemDebug.AddDebugItem("I - Player", "change to debug char", new Action(this.changeToTestChar));

            //add a bunch of hooks that we will use to set up the custom deity menu
            On.MenuDeitySelectPlayer.Start += MenuDeitySelectPlayer_Start;
            IL.MenuDeitySelectPlayer.Update += IL_MenuDeitySelectPlayer_Update;
            On.MenuDeitySelectPlayer.Update += MenuDeitySelectPlayer_Update;
        }

        private void MenuDeitySelectPlayer_Update(On.MenuDeitySelectPlayer.orig_Update orig, MenuDeitySelectPlayer self)
        {

            //currently unused
            orig(self);
            //throw new NotImplementedException();
        }

        private void IL_MenuDeitySelectPlayer_Update(ILContext il)
        {
            //this has to be an il patch, assuming crawl never updates this function, this should always work, but we should look out for alternatives.
            il.IL.Body.Instructions[32].OpCode = Mono.Cecil.Cil.OpCodes.Ldc_I4_2; //make trials display as the second to last option, not the last, because custom deities display as last.
        }

        private void MenuDeitySelectPlayer_Start(On.MenuDeitySelectPlayer.orig_Start orig, MenuDeitySelectPlayer self)
        {
            //we need a component to catch unity messages, which is used in the menu system.
            self.gameObject.AddComponent<MenuDeitySelectPlayerExtension>();
            orig(self);

            //these next couple of lines all are used to add a new button and make sure that button is hooked correctly
            MethodInfo addItemToMenu = typeof(MenuTextMenu).GetMethod("AddItem", BindingFlags.Public | BindingFlags.Instance);
            MenuTextMenuItemData modDeitiesList = new MenuTextMenuItemData
            {
                m_enabled = true,
                m_prefabMenuItem = (MenuTextMenuItem)Util.GetField(self, "m_prefabMenuItemTrial") ,
                m_message = "MsgOnSelectModDeities",
            };
            addItemToMenu.Invoke(Util.GetField(self,"m_menu"), new object[] { modDeitiesList });
        }
        public void changeToTestChar() //debug command TODO:only add once
        {
            //does not work currently, but should.
            GameObject debugMonst = SystemDeity.GetDeity(0).GetStartingMonster(0);
            SystemPlayers.GetPlayer(0).OnGameObjectAssigned(Instantiate(debugMonst),false,true,true);
        }
    }

    

    public class WeaponAPI //used to add weapons. will work better probably.
    {

    }
    public class ItemAPI //items(not weapons, like the passive ones, ill look into this more)
    {

    }
    public class NoConsole : MonoBehaviour //in my setup, an annoying console keeps popping up and i cant see the screen at all, this should fix
    {
        void OnGUI()
        {
            Debug.developerConsoleVisible = false;
        }
    }
    [AttributeUsage(AttributeTargets.Method,AllowMultiple = true)]
    public class CrawlPlugin : Attribute
    {

    }
}
