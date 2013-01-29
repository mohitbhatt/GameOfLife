using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain
{
    public class Generation<T>
        where T : ICell, new()
    {
        public List<List<T>> Cells { get; set; }

        public Generation()
        {
            Cells = new List<List<T>>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x">Is number of columns</param>
        /// <param name="y">is number of rows</param>
        public Generation(int x, int y)
        {
            if (x > 0 && y > 0)
            {
                var outerList = new List<List<T>>();
                for (int ctr1 = 0; ctr1 < y; ctr1++)
                {
                    var innerList = new List<T>(x);
                    for (int innerCtr = 0; innerCtr < x; innerCtr++)
                    {
                        var cell = new T();
                        innerList.Add(cell);
                    }
                    outerList.Add(innerList);
                }

                Cells = outerList;
                return;
            }
        }

        public Generation(List<List<T>> cells)
        {
            Cells = cells;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="cellX">is the number of column in row specified by cellY where cell is present</param>
        /// <param name="cellY">is the number of row in which cell is present</param>
        /// <returns></returns>
        public List<T> GetNeighborHood(T cell, int cellX, int cellY)
        {
            bool selfCellAdded = false;
            List<T> neighborHood = new List<T>();
            //add left neighbor
            if (cellX - 1 <= 0)
            {
                neighborHood.Add(cell);
                selfCellAdded = true;
            }
            else
            {
                neighborHood.Add(Cells[cellY][cellX - 1]);
            }

            //add right neighbor
            if (cellX + 1 >= Cells[0].Count)
            {
                if(selfCellAdded == false)
                    neighborHood.Add(cell);
            }
            else
            {
                neighborHood.Add(Cells[cellY][cellX + 1]);
            }

            //add top  
            if (cellY - 1 <= 0)
            {
                if(selfCellAdded == false)
                    neighborHood.Add(cell);
            }
            else
            {
                neighborHood.Add(Cells[cellY - 1][cellX]);
            }

            //add bottom
            if (cellY + 1 >= Cells.Count)
            {
                if (selfCellAdded == false)
                    neighborHood.Add(cell);
            }
            else
            {
                neighborHood.Add(Cells[cellY + 1][cellX]);
            }

            //Add Left Top
            if((cellY - 1 <= 0) || (cellX - 1 <= 0))
            {
                if (selfCellAdded == false)
                    neighborHood.Add(cell);
            }
            else
            {
                neighborHood.Add(Cells[cellY - 1][cellX - 1]);
            }

            //Add Right Top
            if ((cellY - 1 <= 0) || (cellX + 1 >= Cells[0].Count))
            {
                if (selfCellAdded == false)
                    neighborHood.Add(cell);
            }
            else
            {
                neighborHood.Add(Cells[cellY - 1][cellX + 1]);
            }

            //Add left bottom
            if ((cellY + 1 >= Cells.Count) || (cellX - 1 <= 0))
            {
                if (selfCellAdded == false)
                    neighborHood.Add(cell);
            }
            else
            {
                neighborHood.Add(Cells[cellY + 1][cellX - 1]);
            }


            //Add right bottom
            if ((cellY + 1 >= Cells.Count) || (cellX + 1 >= Cells[0].Count))
            {
                if (selfCellAdded == false)
                    neighborHood.Add(cell);
            }
            else
            {
                neighborHood.Add(Cells[cellY + 1][cellX + 1]);
            }

            return neighborHood;
        }
    }
}
