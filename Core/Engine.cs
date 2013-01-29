using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain;

namespace Core
{
    /// <summary>
    /// Templatize the engine with the type of cell it can work with
    /// Engine<T> and Generation<T> where T is ICell. Will  need to implement ICell
    /// </summary>
    public class Engine<T>  
        where T : ICell, new()
    {
        public Generation<T> CurrentGeneration { get; set; }
        //getter because next generation is calculated here
        public Generation<T> NextGeneration { get; private set; }

        //private copy
        private Generation<T> _originalSeed;

        public int MaxGenerations { get; set; }

        public Engine()
        {
            CurrentGeneration = new Generation<T>();
            NextGeneration = new Generation<T>();
            MaxGenerations = 0; //defaulted to 0. We won't run without a seed
            _originalSeed = CurrentGeneration;
        }

        public Engine(Generation<T> seedGeneration)
        {
            CurrentGeneration = seedGeneration;
            NextGeneration = new Generation<T>();
            MaxGenerations = 5; //defaulted to 5
        }

        public Engine(Generation<T> seedGeneration, int maxGenerations)
        {
            CurrentGeneration = seedGeneration;
            NextGeneration = new Generation<T>();
            MaxGenerations = maxGenerations; //user supplied
        }

        /// <summary>
        /// Ok. So I had a choice between passing an action as a handler for generationprocessed OR declaring and 
        /// using an event. Sending an action down seemed simpler (simple is better :) ) given that we do not have
        /// detailed requirements on what to do when a generation gets processed, except display it.
        /// Although a delegate could have been async, but the consumer can take care of that headache in this case also
        /// </summary>
        /// <param name="onGenerationProcessed"></param>
        public void Process(Action<Generation<T>, int> onGenerationProcessed)
        {
            /*
             * Processing Rules (source http://en.wikipedia.org/wiki/Conway's_Game_of_Life):
             * 1. Any live cell with fewer than two live neighbours dies, as if caused by under-population.
             * 2. Any live cell with two or three live neighbours lives on to the next generation.
             * 3. Any live cell with more than three live neighbours dies, as if by overcrowding.
             * 4. Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction.
            */

            //to begin with check sanity
            if (CheckSanity() == false)
                throw new ApplicationException("Something is not setup right. Recheck seed.");

            //display seed
            onGenerationProcessed(CurrentGeneration, 0);
            System.Threading.Thread.Sleep(1000);

            //now process current gen and put it in next gen
            int localGenCtr = MaxGenerations; //don't mess up with the public member
            while (localGenCtr-- > 0)
            {
                
                NextGeneration = new Generation<T>(CurrentGeneration.Cells[0].Count, CurrentGeneration.Cells.Count);
                for (int y = 0; y < CurrentGeneration.Cells.Count; y++)
                {
                    var currRow = CurrentGeneration.Cells[y];
                    for (int x = 0; x < currRow.Count; x++)
                    {
                        var cell = CurrentGeneration.Cells[y][x];
                        var neighborHood = CurrentGeneration.GetNeighborHood(cell, x, y, Generation<T>.CreateCopy(CurrentGeneration.Cells));

                        int totalLiveNeighbours = neighborHood.Where(c => c.IsAlive).ToList().Count;
                        bool shouldDie = RuleProcessor.ShouldCellDie(totalLiveNeighbours, cell.IsAlive);
                        NextGeneration.Cells[y][x].IsAlive = (shouldDie == false);
                    }
                }
                onGenerationProcessed(NextGeneration, MaxGenerations - localGenCtr);
                System.Threading.Thread.Sleep(1000);
                CurrentGeneration = new Generation<T>(NextGeneration.Cells);
            }
        }

        private bool CheckSanity()
        {
            if ((CurrentGeneration == null) ||
                (NextGeneration == null))
                return false;

            if (CurrentGeneration.Cells.Count == 0)
                return false;

            return true;
        }
    }

    public class RuleProcessor
    {
        public static bool ShouldCellDie(int totalAliveNeighborCount, bool isCellAliveRightNow)
        {
            //1. Any live cell with fewer than two live neighbours dies, as if caused by under-population.
            if ((isCellAliveRightNow) && (totalAliveNeighborCount < 2))
            {
                //the next gen state of this cell is dead
                return true;
            }

            //2. Any live cell with two or three live neighbours lives on to the next generation.
            if ((isCellAliveRightNow) && ((totalAliveNeighborCount == 2) || (totalAliveNeighborCount == 3)))
            {
                //u got lucky dude, u get to live :). Thank thee neighbors
                return false;
            }

            //3. Any live cell with more than three live neighbours dies, as if by overcrowding.
            if ((isCellAliveRightNow) && (totalAliveNeighborCount > 3))
            {
                //dude, u gotta understand, too many friends are deadly at times ;)
                return true;
            }

             //4. Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction.
            if ((isCellAliveRightNow == false) && (totalAliveNeighborCount == 3))
            {
                //exactly 3 parents came to help -- u're alive & kicking
                return false;
            }

            //all dead cells, stay put
            return true;
        }
    }
}
