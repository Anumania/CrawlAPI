using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace CrawlAPI
{
    public class DeityAPI
    {
        //TODO: standardize either the use of "custom deity" or "modded deity" terminology in the api;
        public static List<Deity> customDeities;
        public static List<MenuDeitySelectPlayer> deitySelectMenu;
        public static List<MenuTextMenu> customDeityMenu;

        public static void Init()
        {
            customDeities = new List<Deity>();
            On.MenuMain.Start += MenuStartHook;

            On.MenuDeitySelectPlayer.Update += (a, b) => //god i should stop using a and b for these.
            {
                
                a(b);
                
                int plindex = (int)typeof(MenuDeitySelectPlayer).GetField("m_playerId", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(b); //player index
                
                bool isModMenuEnabled = customDeityMenu[plindex].gameObject.activeInHierarchy;

                if (isModMenuEnabled)
                {
                    //Console.WriteLine(customDeityMenu[plindex].GetSelectedId());
                    //Console.WriteLine(customDeityMenu[plindex].GetSelectedId() - 1);

                    ((GameObject)typeof(MenuDeitySelectPlayer).GetField("m_arrowL", (BindingFlags)36).GetValue(b)).SetActive(customDeityMenu[plindex].HasPrevColumn());
                    ((GameObject)typeof(MenuDeitySelectPlayer).GetField("m_arrowR", (BindingFlags)36).GetValue(b)).SetActive(customDeityMenu[plindex].HasNextColumn());
                    b.SetDeity(customDeities[customDeityMenu[plindex].GetSelectedId()], true);
                }
                //Console.WriteLine(typeof(MenuDeitySelectPlayer).GetField("m_menuTrials",BindingFlags.)); //fuck i forgot binding flags
                else if (!((MenuTextMenu)typeof(MenuDeitySelectPlayer).GetField("m_menuTrials",BindingFlags.Instance | BindingFlags.NonPublic).GetValue(b)).gameObject.activeSelf) //if we arent in trials either (we must be in normal select menu)
                {
                    MenuTextMenu normalMenu = ((MenuTextMenu)typeof(MenuDeitySelectPlayer).GetField("m_menu", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(b));;
                    //Console.WriteLine("in normal select menu");
                    //typeof(MenuDeitySelectPlayer).GetField("m_selectedItemType", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(b,5); //normally we would use an enum, but the eMenuItemType enum doesnt have a type for the mod menu, so make one (ish)
                    if (normalMenu.GetSelectedId() == normalMenu.GetItemCount() - 1) //if we are on the last index of the entries
                    {
                        
                        ((global::GUIText)typeof(MenuDeitySelectPlayer).GetField("m_text", (BindingFlags)36).GetValue(b)).SetText("Access Deities Added by Mods"); //getting lazy typing out the same binding flags every time.
                        ((TextMesh)typeof(MenuDeitySelectPlayer).GetField("m_name", (BindingFlags)36).GetValue(b)).text = "Custom";

                        float randomAnimTimer = (float)typeof(MenuDeitySelectPlayer).GetField("m_randomAnimTimer", (BindingFlags)36).GetValue(b);
                        randomAnimTimer -= Time.deltaTime;
                        if (randomAnimTimer < 0f)
                        {
                            int randomId = (int)UnityEngine.Random.Range(0, DeityAPI.customDeities.Count);
                            Deity deity = customDeities[(randomId)];
                            b.SetDeityPortrait(deity);
                            b.SetDeityMonsters(deity);
                            randomAnimTimer = 0.15f;
                        }
                        typeof(MenuDeitySelectPlayer).GetField("m_randomAnimTimer", (BindingFlags)36).SetValue(b, randomAnimTimer); ;
                    }
                }
            };
        }

        private static void MenuStartHook(On.MenuMain.orig_Start orig, MenuMain self)
        {
            orig(self);
            MenuDeitySelectPlayer[] asd = Resources.FindObjectsOfTypeAll<MenuDeitySelectPlayer>();
            deitySelectMenu = new List<MenuDeitySelectPlayer>(new MenuDeitySelectPlayer[] { null, null, null, null }); //for some reason new list constructor doesnt work :/
            customDeityMenu = new List<MenuTextMenu>(new MenuTextMenu[] { null, null, null, null }); 
            foreach (MenuDeitySelectPlayer mdsp in asd)
            {
                try
                {
                    int index = MenuPlayerNumber.GetPlayerNumber(mdsp.gameObject);
                    deitySelectMenu[index] = mdsp;

                    MenuStateMachine sm = mdsp.transform.parent.gameObject.GetComponent<MenuStateMachine>();

                    MenuState deityTrialSelectState = sm.m_states[sm.GetStateId("DeityTrialSelect")];
                    MenuState deityCustomSelectState = new MenuState();

                    foreach (FieldInfo field in typeof(MenuState).GetFields()) //perform shallow copy to make a new state
                    {
                        field.SetValue(deityCustomSelectState, field.GetValue(deityTrialSelectState));
                    }
                    deityCustomSelectState.m_actions = new List<MenuStateAction>();
                    deityCustomSelectState.m_turnOn = new List<GameObject>(); //reset this baby because its a copy of the trials state
                    deityCustomSelectState.m_name = "DeityModdedSelect";
                    sm.m_states.Add(deityCustomSelectState);


                    GameObject deityCustomMenu = GameObject.Instantiate(mdsp.gameObject.transform.FindChild("ContainerInfo").FindChild("DeityMenu").gameObject);
                    GameObject deityCustomText = GameObject.Instantiate(mdsp.gameObject.transform.FindChild("TextWorships").gameObject);

                    deityCustomMenu.SetActive(false);
                    deityCustomText.SetActive(false);

                    deityCustomMenu.AddComponent<DeityModMenu>().index = index;

                    customDeityMenu[index] = deityCustomMenu.GetComponent<MenuTextMenu>();

                    deityCustomMenu.name = "DeityCustomMenu";
                    deityCustomText.name = "DeityCustomText";

                    deityCustomMenu.transform.parent = mdsp.gameObject.transform;
                    deityCustomText.transform.parent = mdsp.gameObject.transform;

                    deityCustomMenu.transform.localPosition = mdsp.gameObject.transform.FindChild("ContainerInfo").FindChild("DeityMenu").gameObject.transform.localPosition;

                    deityCustomText.transform.localPosition = mdsp.gameObject.transform.FindChild("TextWorships").gameObject.transform.localPosition;

                    deityCustomText.GetComponent<TextMesh>().text = "Custom Deities";

                    sm.m_states[sm.GetStateId("DeityModdedSelect")].m_turnOn.AddRange(new List<GameObject> { deityCustomMenu, deityCustomText });
                    sm.m_states[sm.GetStateId("DeityModdedSelect")].m_turnOff.AddRange(new List<GameObject> { mdsp.gameObject.transform.FindChild("ContainerInfo").FindChild("DeityMenu").gameObject, mdsp.gameObject.transform.FindChild("TextWorships").gameObject });

                    sm.m_states[sm.GetStateId("DeitySelect")].m_turnOff.AddRange(new GameObject[] { deityCustomMenu, deityCustomText });
                    

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    //Console.WriteLine(e.StackTrace);
                }

            }
        }

        public static void AddDeity(CustomDeity customDeity)
        {
            Deity deity = new Deity();
            foreach (var field in typeof(CustomDeity).GetFields())
            {
                FieldInfo fldInfo = typeof(Deity).GetField(field.Name, BindingFlags.Instance | BindingFlags.NonPublic);
                if (fldInfo == null)
                {
                    continue;
                }
                else
                {
                    try
                    {
                        fldInfo.SetValue(deity, field.GetValue(customDeity));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        fldInfo.SetValue(deity, null);
                    }
                }
            }
            customDeities.Add(deity);
        }
        //dont run this too early or get big crash
        public static void UpdateCustomDeities(int index)
        {

            MenuTextMenu customMenu = customDeityMenu[index];
            for(int i = 0; i < customMenu.GetItemCount(); i++) //reset menu
            {
                customMenu.RemoveItem(i);
            }
            foreach (Deity deity in customDeities)
            {
                MenuTextMenuItemData modDeitiesList = new MenuTextMenuItemData
                {
                    m_enabled = true,
                    m_prefabMenuItem = (MenuTextMenuItem)typeof(MenuDeitySelectPlayer).GetField("m_prefabMenuItemTrial", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(deitySelectMenu[index]),
                    m_message = "MsgOnSelectModDeity",
                };
                customMenu.AddItem(modDeitiesList);
            }
        }
    }
    //
    public class DeityModMenu:MonoBehaviour
    {
        public int index;
        private void OnEnable()
        {
            DeityAPI.UpdateCustomDeities(index);
        }

        private void Update()
        {
            
        }
    }
    public class MenuDeitySelectPlayerExtension : MonoBehaviour //adding additional methods to MenuDeitySelectPlayer
    {
        public void Awake()
        {

        }
        public void MsgOnSelectModDeities()//opening the menu with all of the custom deities.
        {
            Console.WriteLine("Deity Mod Menu selected");

            base.SendMessageUpwards("MsgQueueState", "DeityModdedSelect");

        }
        public void MsgOnSelectModDeity()//selecting a custom deity from the custom deities menu.
        {
            MenuDeitySelectPlayer mdsp = GetComponent<MenuDeitySelectPlayer>();
            int index = MenuPlayerNumber.GetPlayerNumber(mdsp.gameObject);

            mdsp.SetDeity(DeityAPI.customDeities[DeityAPI.customDeityMenu[index].GetSelectedId()],false);
            base.SendMessageUpwards("MsgQueueState", "DeitySelected");

        }
    }
}
