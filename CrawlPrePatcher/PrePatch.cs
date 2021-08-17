using Mono.Cecil;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Scripting;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

namespace CrawlPrePatcher
{
    public static class CrawlAPI
    {
        static List<string> UnityFunctions = new List<string>() { "Update", "Awake", "Initialize", "FixedUpdate", "Start", "OnGUI", "OnEnable", "OnDisable", "OnDestroy" };
        public static IEnumerable<string> TargetDLLs { get; } = new[] { "Assembly-CSharp.dll" };

        //the real patcher, all this one does is makes Player.EvolveCostOverride public for use in the api.
        public static void Patch(AssemblyDefinition assembly) 
        {
            //MakeSavesLocal(assembly);
            foreach (TypeDefinition typedef in assembly.MainModule.GetType("Player").NestedTypes)
            {
                if (typedef.Name == "EvolveCostOverride" || typedef.Name == "ePlayerState")
                {
                    typedef.IsPublic = true;
                }
            }
            foreach (MethodDefinition methdef in assembly.MainModule.GetType("MenuDeitySelectPlayer").Methods)
            {
                methdef.IsPublic = true;
            }
        }
        //this one is just for testing, and goes unused.
        public static void Patch2(AssemblyDefinition assembly)
        {
            Collection<TypeDefinition> baseTypes = new Collection<TypeDefinition>();
            //MakeSavesLocal(assembly);
            Collection<TypeDefinition> col = assembly.MainModule.Types;
            
            foreach (TypeDefinition def in col)
            {
                if (def.Namespace == "" && def.Name != "<PrivateImplementationDetails>")
                {
                    baseTypes.Add(def);
                    foreach (MethodDefinition methdef in def.Methods)
                    {
                        methdef.IsPublic = true;
                    }
                    if (def.Name == "BotHero") //
                    {
                        //Console.WriteLine(def.IsSerializable);
                        foreach (FieldDefinition fielddef in def.Fields)
                        {
                            if (!def.IsSerializable || def.CustomAttributes.Where(e => e.AttributeType.Name == "SerializeField").Count() != 0)
                            {
                                //Console.WriteLine(fielddef.Name);
                                fielddef.IsPublic = true;
                            }
                        }
                    }
                    else if (def.Name == "Player")
                    {
                        foreach (TypeDefinition typedef in def.NestedTypes)
                        {
                            typedef.IsPublic = true;
                        }
                    }
                }
            }
        }

        private static void makePublic(AssemblyDefinition ass, string _type, string _method)
        {
            MethodDefinition mthd = ass.MainModule.GetType(_type).Methods.Where(e => e.Name == _method).FirstOrDefault();
            mthd.IsPublic = true;
        }

        //does not work anymore oops dont use linq
        private static void MakeSavesLocal(AssemblyDefinition ass) //this makes the game save locally instead of syncing with steam.
        {
            try
            {
                //MethodDefinition mthd = ass.MainModule.GetType("SaveHelper").NestedTypes.Where(e => e.Name.Contains("CoroutineStartSave")).FirstOrDefault().Methods.First(); //enumerators suck to edit.
                var mthd = ass.MainModule.GetType("SaveHelper").NestedTypes;
                foreach( var a in mthd)
                {
                    Console.WriteLine(a);
                }
                Console.WriteLine(mthd);
                /*ILProcessor il = mthd.Body.GetILProcessor();
                MethodReference fileStratRef = ass.MainModule.GetType("SaveStrategyFile").Methods.Where(a => a.Name == ".ctor").FirstOrDefault();
                MethodReference steamStratRef = ass.MainModule.GetType("SaveStrategySteam").Methods.Where(a => a.Name == ".ctor").FirstOrDefault();
                Collection<MethodDefinition> bruh = ass.MainModule.GetType("SaveHelper").NestedTypes.Where(e => e.Name.Contains("CoroutineStartSave")).FirstOrDefault().Methods;
                foreach (var inst in bruh[3].Body.Instructions)
                {
                    if (inst.OpCode == OpCodes.Newobj && steamStratRef != null && (MethodReference)inst.Operand == steamStratRef) //check to make sure user doesnt already have no steam
                    {
                        inst.Operand = fileStratRef; //make the game use file strategy instead of steam strategy.
                        break;
                    }
                }*/
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
