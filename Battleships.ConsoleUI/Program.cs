// See https://aka.ms/new-console-template for more information
using Battleships.Core.Models.Dtos;
using Battleships.Core.Services;
using Microsoft.Extensions.DependencyInjection;

var serviceProvider = new ServiceCollection()
                .AddScoped<IBattleshipService, BattleshipService>()
                .BuildServiceProvider();

var battleshipService = serviceProvider.GetService<IBattleshipService>();

battleshipService.Init();
battleshipService.GenerateShips();

HitResult hitResult = new HitResult();

Print();

while (!hitResult.GameOver)
{
  Console.WriteLine("Try hit");
  var coordinates = Console.ReadLine();

  hitResult = battleshipService.Hit(coordinates);

  if (hitResult.IsSuccess)
  {
    Print();

    if (hitResult.HitSuccessType == HitSuccessType.Missed)
      Console.WriteLine("Ooops. You missed");
    else if (hitResult.HitSuccessType == HitSuccessType.Injured)
      Console.WriteLine("Well done. It's an injure");
    else if (hitResult.HitSuccessType == HitSuccessType.Destroyed)
      Console.WriteLine("Well done. It's a destroy");
  }
  else
  {
    if (hitResult.HitErrorType == HitErrorType.NotValid)
      Console.WriteLine("Coordinates are not valid");
    else if (hitResult.HitErrorType == HitErrorType.OutOfRange)
      Console.WriteLine("Coordinates are out of range");
    else if (hitResult.HitErrorType == HitErrorType.AlreadyHit)
      Console.WriteLine("Ooops. Already hit");
  }
}

Print();
Console.WriteLine("GAME OVER");



void Print()
{
  Console.Write(battleshipService.GetPointGridString());
  Console.WriteLine(battleshipService.GetLegendInfo());
}