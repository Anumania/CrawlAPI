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
            //TODO: figure out how to fix stack traces/
            //probable causes: loading by bepin causes mono to forget where pdb is.(very probable)
            //version difference (use mono to compile instead of ms.net)
            //unity doesnt like debugging (no because it still shows SOME line info later in the stack)
            //StackTrace st = new StackTrace(true);
            //Console.WriteLine(System.Environment.);
            //TypeReference thevoid = AssemblyDefinition.ReadAssembly("C:\\Program Files (x86)\\Steam\\steamapps\\common\\Crawl\\BepInEx\\plugins\\CrawlAPI\\CrawlAPI.dll").MainModule.ImportReference(typeof(void));
            //ISymbolReader sr = new Mono.Cecil.Pdb.NativePdbReaderProvider().GetSymbolReader(AssemblyDefinition.ReadAssembly("C:\\Program Files (x86)\\Steam\\steamapps\\common\\Crawl\\BepInEx\\plugins\\CrawlAPI\\CrawlAPI.dll").MainModule, "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Crawl\\BepInEx\\plugins\\CrawlAPI\\CrawlAPI.pdb");
            //Console.WriteLine(sr.Read(new MethodDefinition("Awake",Mono.Cecil.MethodAttributes.Public,thevoid)));
            //Console.WriteLine(new MethodDefinition("Awake", Mono.Cecil.MethodAttributes.Public, thevoid).Name);

            //Console.WriteLine(st.GetFrame(0));
            //System.Diagnostics.
            //exAtlas[] array = (exAtlas[])Resources.FindObjectsOfTypeAll(typeof(exAtlas));

            APIHelpers.Init();
            MonsterAPI.Init();
            DeityAPI.Init();
            IFormatter form = new BinaryFormatter();
            On.MenuMain.Update += (a, b) => { Debug.developerConsoleVisible = false; }; //in debug mode, this dev console sucks

            UnityEngine.Object[] objects = Resources.FindObjectsOfTypeAll(typeof(UnityEngine.Object));

            GameObject noConsole = new GameObject();
            noConsole.AddComponent<NoConsole>();
            GameObject.DontDestroyOnLoad(noConsole);

            SystemDebug.AddDebugItem("I - Player", "change to debug char", new Action(this.changeToTestChar));

            List<Type> objList = new List<Type>();
            foreach (UnityEngine.Object i in objects)
            {
                if (!objList.Contains(i.GetType()))
                {
                    objList.Add(i.GetType());
                    //Console.WriteLine(i.GetType());
                }
            }
            On.MenuDeitySelectPlayer.Start += MenuDeitySelectPlayer_Start;
            IL.MenuDeitySelectPlayer.Update += IL_MenuDeitySelectPlayer_Update;
            On.MenuDeitySelectPlayer.Update += MenuDeitySelectPlayer_Update;
        }

        private void MenuDeitySelectPlayer_Update(On.MenuDeitySelectPlayer.orig_Update orig, MenuDeitySelectPlayer self)
        {

            orig(self);
            //throw new NotImplementedException();
        }

        private void IL_MenuDeitySelectPlayer_Update(ILContext il)
        {
            il.IL.Body.Instructions[32].OpCode = Mono.Cecil.Cil.OpCodes.Ldc_I4_2; //make trials display as the second to last option, not the last, because custom deities display as last.
        }

        private void MenuDeitySelectPlayer_Start(On.MenuDeitySelectPlayer.orig_Start orig, MenuDeitySelectPlayer self)
        {
            self.gameObject.AddComponent<MenuDeitySelectPlayerExtension>(); // catch custom mod deity related messages.
            orig(self);
            FieldInfo menu = self.GetType().GetField("m_menu", BindingFlags.Instance | BindingFlags.NonPublic); //add a new button that shows mod deities
            MethodInfo addItemToMenu = typeof(MenuTextMenu).GetMethod("AddItem", BindingFlags.Public | BindingFlags.Instance);
            MenuTextMenuItemData modDeitiesList = new MenuTextMenuItemData
            {
                m_enabled = true,
                m_prefabMenuItem = (MenuTextMenuItem)self.GetType().GetField("m_prefabMenuItemTrial", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(self),
                m_message = "MsgOnSelectModDeities",
            };
            addItemToMenu.Invoke(menu.GetValue(self), new object[] { modDeitiesList });
        }
        public void changeToTestChar() //debug command TODO:only add once
        {
            
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
