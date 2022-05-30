using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CrawlAPI
{
    public static class APIHelpers
    {
        //this whole script SUCKS and i want it gone.
        public static AssemblyDefinition AssemblyCS; //unity c#
        public static void Init()
        {
            AssemblyCS = AssemblyDefinition.ReadAssembly(BepInEx.Paths.ManagedPath + "\\Assembly-CSharp.dll");
        }
        public static MethodDefinition FindMethod(AssemblyDefinition ass, string type, string method)
        {
            return null;
        }
        public static MethodDefinition FindMethod(string type, string method)
        {
            return AssemblyCS.MainModule.GetType(type).Methods.Where(e => { return e.Name == method; }).First();
        }
        public static void RecursivePrintFields(int howMuch) //pog idea
        {

        }
        //this sucks, and should go away
        public static void PrintPlayer(Player i)
        {
            foreach (var j in i.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
            {
                if (j.GetCustomAttributes(true).Where(e => e.GetType().Name == "SerializeField").FirstOrDefault() != null)
                {
                    Console.WriteLine(j.Name + ":" + j.GetValue(i));
                    if (j.GetValue(i) != null && j.GetValue(i).GetType() == typeof(exSpriteAnimClip))
                    {
                        foreach (var m in j.GetValue(i).GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
                        {
                            var val = m.GetValue(j.GetValue(i));
                            Console.WriteLine("\t" + m.Name + ":" + val);
                            if (m.Name == "frameInfos")
                            {

                                foreach (exSpriteAnimClip.FrameInfo val2 in (List<exSpriteAnimClip.FrameInfo>)val)
                                {

                                    Console.WriteLine("\t\t" + "textureGUID" + ":" + val2.textureGUID);
                                    Console.WriteLine("\t\t" + "length" + ":" + val2.length);
                                    Console.WriteLine("\t\t" + "atlas" + ":" + val2.atlas);
                                    Console.WriteLine("\t\t" + "index" + ":" + val2.index);
                                }
                            }
                            else if (m.Name == "eventInfos") //this should work, idk though
                            {
                                foreach (exSpriteAnimClip.EventInfo val2 in (List<exSpriteAnimClip.EventInfo>)val)
                                {
                                    foreach (var val3 in val2.GetType().GetFields())
                                    {
                                        Console.WriteLine("\t\t" + val3.Name + ":" + val3.GetValue(val2));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
