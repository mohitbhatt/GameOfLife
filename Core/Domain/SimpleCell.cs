using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain
{
    /// <summary>
    /// Although in it's simplest sense, Game of Life can just be done with a simple array of bools,
    /// but on the flip side it is too restrictive. Simple domain objects always go a long way
    /// </summary>
    public class SimpleCell : ICell
    {
        public bool IsAlive { get; set; }

        //constructor to keep the new() constraint happy
        public SimpleCell()
        {
            IsAlive = false; //dead by default
        }

        public SimpleCell(bool isAlive)
        {
            IsAlive = isAlive;
        }

        //we could have had a method here that returns a X or - based on IsAlive, but we chose not to
        //because this is the core and we do not want to be sprinkling ui stuff here
    }
}
