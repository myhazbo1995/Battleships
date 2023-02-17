using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleships.Core
{
  internal class Const
  {
    internal static string NotHit = "_ ";
    internal static string Missed = "O ";
    internal static string Injured = "* ";
    internal static string Destroyed = "X ";

    internal static char[] Letters = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J' };
    internal static short ColsAmount = 10;
  }
}
