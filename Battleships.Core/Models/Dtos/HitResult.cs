using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleships.Core.Models.Dtos
{
  public class HitResult
  {
    public HitResult()
    {
      HitErrorType = HitErrorType.None;
    }

    public bool IsSuccess => HitErrorType == HitErrorType.None;

    public HitErrorType HitErrorType { get; set; }
  }

  public enum HitErrorType
  {
    None,
    NotValid,
    OutOfRange,
    AlreadyHit
  }
}
