using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace karateclubb
{
    public class Club
    {
        public int NumClub { get; set; }
        public string NomClub { get; set; }

        public Club(int numClub, string nomClub)
        {
            NumClub = numClub;
            NomClub = nomClub;
        }
    }

}
