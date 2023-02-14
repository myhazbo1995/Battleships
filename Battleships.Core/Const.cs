using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleships.Core
{
  public class Const
  {
    public static string NotHit = "_ ";
    public static string Missed = "O ";
    public static string Injured = "* ";
    public static string Destroyed = "X ";

    public static char[] Letters = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J' };
    public static short ColsAmount = 10;
  }
}
