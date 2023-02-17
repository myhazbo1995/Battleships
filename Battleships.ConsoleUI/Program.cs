// See https://aka.ms/new-console-template for more information
using Battleships.Core.Services;
using Microsoft.Extensions.DependencyInjection;

var serviceProvider = new ServiceCollection()
                .AddScoped<IBattleshipService, BattleshipService>()
                .BuildServiceProvider();

var battleshipService = serviceProvider.GetService<IBattleshipService>();

battleshipService.Init();
battleshipService.GenerateShips();

for (int i = 1; i <= 10; i++)
{
  for (char y = 'A'; y <= 'J'; y++)
  {
    battleshipService.Hit($"{y}{i}");
    Print();
    Console.Read();
  }
}
Print();
Console.Read();


void Print()
{
  Console.Write(battleshipService.GetPointGridString());
  Console.WriteLine(battleshipService.GetLegendInfo());
}