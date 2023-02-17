
namespace Battleships.Core.Models.Dtos
{
  public class HitResult
  {
    public HitResult()
    {
      HitErrorType = HitErrorType.None;
      HitSuccessType = HitSuccessType.Missed;
    }

    public bool IsSuccess => HitErrorType == HitErrorType.None;
    public bool GameOver { get; set; }

    public HitErrorType HitErrorType { get; set; }
    public HitSuccessType HitSuccessType { get; set; }
  }

  public enum HitErrorType
  {
    None,
    NotValid,
    OutOfRange,
    AlreadyHit
  }

  public enum HitSuccessType
  {
    Missed,
    Injured,
    Destroyed
  }
}
