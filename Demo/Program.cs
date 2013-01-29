using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using Core.Domain;

namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            Generation<SimpleCell> seedGeneration = new Generation<SimpleCell>(5, 5);
            //setup seed
            //Blinker - Oscillator
            seedGeneration.Cells[2][1].IsAlive = true;
            seedGeneration.Cells[2][2].IsAlive = true;
            seedGeneration.Cells[2][3].IsAlive = true;
            
            Engine<SimpleCell> engine = new Engine<SimpleCell>(seedGeneration, 10);

            engine.Process(Display);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Done");
            Console.ReadLine();
            
        }

        public static void Display(Generation<SimpleCell> g, int currLevel)
        {
            Console.Clear();
            for (int outerCtr = 0; outerCtr < g.Cells.Count; outerCtr++ )
            {
                string[] line = g.Cells[outerCtr].Select(c => c.IsAlive ? "X" : "-").ToArray<string>();
                foreach(string part in line)
                    Console.Write(part);
                Console.WriteLine();
            }
        }
    }
}
