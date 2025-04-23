using DotAge.Core.Control;
using DotAge.Core.View;
using System.Threading;

Engine _engine = new Engine();
Thread AddEvent = new Thread(() => _engine.MainLoop(new string[0]));
AddEvent.IsBackground = true;
AddEvent.Name = "MainLoop";
AddEvent.Start();

using var game = new DotAge.Core.View.Graphic();

_engine.AddRenderProperty += new Engine.AddRP(game.AddRPBuffer);
_engine.RemoveRenderProperty += new Engine.RemoveRP(game.RemoveRPBuffer);
_engine.ModifyRenderProperty += new Engine.ModifyRP(game.ModifyRPBuffer);
_engine.GraphicEventAble = true;

game.Run();
