// See https://aka.ms/new-console-template for more information
using System.Text;
using Battleships.Core.Services;
using Microsoft.Extensions.DependencyInjection;

var serviceProvider = new ServiceCollection()
                .AddScoped<IBattleshipService, BattleshipService>()
                .BuildServiceProvider();

var battleshipService = serviceProvider.GetService<IBattleshipService>();

battleshipService.Init();
battleshipService.GenerateShips();
Print();

void Print()
{
  Console.Write(battleshipService.GetPointGridString());
  Console.WriteLine(battleshipService.GetLegendInfo());
}