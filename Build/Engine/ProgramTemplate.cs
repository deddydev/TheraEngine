using System;
using System.Threading.Tasks;
using TheraEngine;
using TheraEngine.Core.Files;

namespace {7}
@
    static class Program
    @
        [STAThread]
        static async Task Main()
        @
            {1} game = await {2}.{3}<{1}>("{0}.{6}");
            {4}.{5}(game);
        #
    #
#

//0 = game name
//1 = game class name
//2 = file object class name
//3 = LoadAsync static method name
//4 = Engine class name
//5 = Run method in Engine class
//6 = xtgame, game class extension (in XML)
//7 = default namespace
