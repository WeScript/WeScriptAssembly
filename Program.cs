using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Mathematics;
using SharpDX.Direct3D9;
using SharpDX.XInput;
using WeScriptWrapper;
using WeScript.SDK.UI;
using WeScript.SDK.UI.Components;
//using WeScript.SDK.Rendering;
using System.Runtime.InteropServices;



namespace WeScript.Assembly
{
    class Program
    {
        
        public static IntPtr procHnd = IntPtr.Zero;
        public static bool drawyourstuff = false;
        public static bool isWow64Process = false;
        public static bool isGameOnTop = false;
        public static bool isOverlayOnTop = false;
        public static uint PROCESS_ALL_ACCESS = 0x1FFFFF;
        public static Vector2 wndMargins = new Vector2(0, 0);
        public static Vector2 wndSize = new Vector2(0, 0);


        public static Menu Root { get; private set; }
        public static Menu General { get; private set; }
        public static Menu Drawings { get; private set; }
        public static Menu Champion { get; private set; }
        public static Menu Gapcloser { get; set; }
        public static Menu Interrupter { get; set; }
        public static string CharName = "Ryze";


        [StructLayout(LayoutKind.Explicit)]
        public struct GameEntityStruct
        {
            [FieldOffset(0x04)]
            public Vector3 headPos;

            [FieldOffset(0x34)]
            public Vector3 feetPos;

            [FieldOffset(0xF8)]
            public Int32 health;

            [FieldOffset(0xFC)]
            public Int32 armor;

            [FieldOffset(0x32C)]
            public Int32 team;
        }


        class Components
        {
            public static class General
            {
                public static readonly MenuBool OrbwalkerOnlyMenuBool = new MenuBool("orbwalker", "Only Orbwalker", false);

                public static readonly MenuBool CastOnSmallJungleMinionsMenuBool =
                    new MenuBool("junglesmall", "Cast to small minions too in JungleClear", false);

                public static readonly MenuBool StormrazorComboMenubool = new MenuBool("Combo", "Combo", false);
                public static readonly MenuBool StormrazorLaneClearMenubool = new MenuBool("LaneClear", "LaneClear", false);
                public static readonly MenuBool StormrazorHarassMenubool = new MenuBool("Harass", "Harass", false);
                public static readonly MenuBool StormrazorLasthitMenubool = new MenuBool("Lasthit", "Lasthit", false);

                public static readonly MenuBool IgnoreManaManagerBlue =
                    new MenuBool("nomanagerifblue", "Ignore ManaManagers if you have Blue Buff", false);

            }

            public static class DrawingMenu
            {
                public static readonly MenuBool SharpDXMode = new MenuBool("sharpDXMode", "SharpDX Mode", false);
                public static readonly MenuSlider CircleThickness = new MenuSlider("CircleThickness", "Circle Thickness", 1, 1, 10);

                public static readonly MenuColor ColorQ = new MenuColor("color1", "Color Q", new Color(255, 0, 0));
                public static readonly MenuColor ColorW = new MenuColor("color2", "Color W", new Color(0, 255, 0));
                public static readonly MenuColor ColorE = new MenuColor("color3", "Color E", new Color(0, 0, 255));
                public static readonly MenuColor ColorR = new MenuColor("color4", "Color R", new Color(255, 255, 255));
                public static readonly MenuColor ColorExtra = new MenuColor("color5", "Color Extra", new Color(0, 0, 0));

                public static readonly MenuBool QBool = new MenuBool("ryzeqbool", "Draw Q", false);

                public static readonly MenuKeyBind keyBindz = new MenuKeyBind("keybindz", "Key Bind Combo", VirtualKeyCode.Space, KeybindType.Toggle, false, true);

                //public static readonly MenuBool WBool = new MenuBool($"Ryze.WR", "Draw W", false);

                //public static readonly MenuBool EBool = new MenuBool($"Ryze.ER","Draw E",true);

                //public static readonly MenuBool RBool = new MenuBool($"Ryze.RR", "Draw R", false);

                //public static readonly MenuBool QDamageBool =
                //    new MenuBool($"Ryze.QD", "Draw Q Damage", false);

                //public static readonly MenuBool WDamageBool =
                //    new MenuBool($"Ryze.WD", "Draw W Damage", false);

                //public static readonly MenuBool EDamageBool = new MenuBool($"Ryze.ED","Draw E Damage",true);

                //public static readonly MenuBool RDamageBool =
                //    new MenuBool($"Ryze.RD", "Draw R Damage", false);

                //public static readonly MenuSliderBool AutoDamageSliderBool =
                //    new MenuSliderBool($"Ryze.autos", "Include x Autos' damage", false, 1, 1, 10);
            }

            public static class Gapcloser
            {
                public static Dictionary<string, Menu> AntiGapcloserSpellSlot = new Dictionary<string, Menu>();
                public static Dictionary<string, Menu> EnemyChampionName = new Dictionary<string, Menu>();
                public static Dictionary<string, MenuBool> EnemySpell = new Dictionary<string, MenuBool>();
            }
        }


        public static void InitializeMenu()
        {
            General = new Menu("general", "General Menu")
            {
                Components.General.CastOnSmallJungleMinionsMenuBool,

                new Menu("stormrazor", "Stormrazor Menu")
                {
                    new MenuSeperator("stormsep", "Stop AA'ing until it procs in:"),
                    Components.General.StormrazorComboMenubool.SetToolTip("this is the tooltip i wanted!"),
                    Components.General.StormrazorLaneClearMenubool,
                    Components.General.StormrazorHarassMenubool,
                    Components.General.StormrazorLasthitMenubool
                }
            };


            Drawings = new Menu("Drawings", "Drawing Menu")
            {
                Components.DrawingMenu.SharpDXMode,
                Components.DrawingMenu.CircleThickness,
                Components.DrawingMenu.ColorQ,
                Components.DrawingMenu.ColorW,
                Components.DrawingMenu.ColorE,
                Components.DrawingMenu.ColorR,
                Components.DrawingMenu.ColorExtra,
                Components.DrawingMenu.QBool,
                Components.DrawingMenu.keyBindz
            };

            //This is for each champion have its configuration file
            Root = new Menu("aio", "AIO", true)
            {
                Components.General.OrbwalkerOnlyMenuBool.SetToolTip("F7 to enable"),
                General,
                Drawings
            };

            Champion = new Menu(CharName.ToLower(), CharName);

            if (1 < 2)
            {
                General.Add(Components.General.IgnoreManaManagerBlue);
            }

            Root.Add(new MenuSeperator("sep1"));

            Root.Add(Champion);

            //Root.Permashow($"AIO - {CharName}", true);

            Root.Attach();
        }





        static void Main(string[] args)
        {
            Console.WriteLine("Hello from AssaultCube.Assembly!!! :)");
            Renderer.OnRenderer += OnRenderer;
            Memory.OnTick += OnTick;
            //Input.OnInput += OnInput;
            //Overlay.OnWndProc += OnWndProc;


           // var menu = new Menu("as_bronx", "AssaultCube by Bronx", true);

            //menu.Add(customization);
            //menu.Add(drawings);
            //menu.Add(kleptomancyItemsMenu);
            //menu.Add(buffEnabled);
            //menu.Add(itemsMenu);

            //menu.Attach();

            InitializeMenu();



        }

        private static void OnWndProc(uint message, ulong wparam, long lparam, EventArgs args)
        {
            var x = lparam & 0xFFFF;
            var y = (lparam >> 16) & 0xFFFF;
            Console.WriteLine($"message: {message.ToString()} wparam: {wparam.ToString()} lparam: {lparam.ToString()} x: {x.ToString()} y: {y.ToString()}");
        }



        public static bool WorldToScreen(Vector3 pos, out Vector2 screen, Matrix matrix, Vector2 wndMargins, Vector2 wndSize) // 3D to 2D
        {
            //Matrix-vector Product, multiplying world(eye) coordinates by projection matrix = clipCoords
            Vector4 clipCoords = new Vector4();
            screen = new Vector2(0, 0);
            clipCoords.X = pos.X * matrix[0] + pos.Y * matrix[4] + pos.Z * matrix[8] + matrix[12];
            clipCoords.Y = pos.X * matrix[1] + pos.Y * matrix[5] + pos.Z * matrix[9] + matrix[13];
            clipCoords.W = pos.X * matrix[3] + pos.Y * matrix[7] + pos.Z * matrix[11] + matrix[15];

            if (clipCoords.W < 0.1f) return false;

            //perspective division, dividing by clip.W = Normalized Device Coordinates
            Vector3 NDC = new Vector3();
            NDC.X = clipCoords.X / clipCoords.W;
            NDC.Y = clipCoords.Y / clipCoords.W;
            //NDC.Z = clipCoords.Z / clipCoords.W;

            //Transform to window coordinates and addup window margin
            screen.X = (wndSize.X / 2 * NDC.X) + (NDC.X + wndSize.X / 2) + wndMargins.X;
            screen.Y = -(wndSize.Y / 2 * NDC.Y) + (NDC.Y + wndSize.Y / 2) + wndMargins.Y;

            //stop drawing outside the game window (in case it's smaller than desktop res)
            if (screen.X < wndMargins.X) return false;
            if (screen.X > wndMargins.X + wndSize.X) return false;
            if (screen.Y < wndMargins.Y) return false;
            if (screen.Y > wndMargins.Y + wndSize.Y) return false;

            return true;
        }

        public static bool m2Pressed  = false;

        private static void OnInput(VirtualKeyCode key, bool isPressed, EventArgs args)
        {
            Console.WriteLine($"The key : {key.ToString()} isPressed: {isPressed.ToString()}");
            if (key == VirtualKeyCode.A)
            {
                m2Pressed = isPressed;
            }
        }

        private static void OnTick(int counter, EventArgs args)
        {
            if (procHnd == IntPtr.Zero) //if we still don't have a handle to the process
            {
                var wndHnd = Memory.FindWindowName("AssaultCube"); //try finding the window of the process (check if it's spawned and loaded)
                if (wndHnd != IntPtr.Zero) //if it exists
                {
                    var calcPid = Memory.GetPIDFromHWND(wndHnd); //get the PID of that same process
                    if (calcPid > 0) //if we got the PID
                    {
                        procHnd = Memory.OpenProcess(PROCESS_ALL_ACCESS, calcPid); //get full access to the process so we can use it later
                        if (procHnd != IntPtr.Zero)
                        {
                            //if we got access to the game, check if it's x64 bit
                            isWow64Process = Memory.IsProcess64Bit(procHnd);
                            //Console.WriteLine($"Process is 64bit: {isWow64Process.ToString()}");
                        }
                    }
                }
            }
            else //else we have a handle, lets check if we should close it, or use it
            {
                var wndHnd = Memory.FindWindowName("AssaultCube");
                if (wndHnd != IntPtr.Zero) //window still exists, so handle should be valid? let's keep using it
                {
                    drawyourstuff = true;
                    wndMargins = Renderer.GetWindowMargins(wndHnd);
                    wndSize = Renderer.GetWindowSize(wndHnd);
                    isGameOnTop = Renderer.IsGameOnTop(wndHnd);
                    isOverlayOnTop = Overlay.IsOnTop();



                    //var modBase = Memory.GetModule(procHnd, null, isWow64Process);
                    //var modSize = Memory.GetModuleSize(procHnd, null, isWow64Process);

                    //var wtf = Memory.FindSignature(procHnd, IntPtr.Zero, IntPtr.Zero, "B9 ? ? ? ? 8D 54 24 30 E8 ? ? ? ? 8B C6",-4);
                    //var wtf = Memory.FindSignature(procHnd, IntPtr.Zero, IntPtr.Zero, "04 19 00 00 B1 E1 13 00 04 19 00 00 22 19 00 00 EC", 0);

                    //Console.WriteLine($"modBase: {modBase.ToString("X")} modSize: {modSize.ToString("X")} wtf: {wtf.ToString("X")}");

                    //Console.WriteLine($"wndMargins.X: {wndMargins.X.ToString()} wndMargins.Y: {wndMargins.Y.ToString()}");
                    //Console.WriteLine($"wndSize.X: {wndSize.X.ToString()} wndSize.Y: {wndSize.Y.ToString()}");
                    //Console.WriteLine($"GameWnd: {wndHnd.ToString()} Foregrnd: {GetForegroundWindow().ToString()}");
                    //you can also scan for patterns here once or w/e
                }
                else //else most likely the process is dead, clean up
                {
                    Memory.CloseHandle(procHnd);
                    procHnd = IntPtr.Zero;
                    drawyourstuff = false;
                }
            }
        }

        //public static T GetStructure<T>(byte[] bytes)
        //{
        //    var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        //    var structure = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
        //    handle.Free();
        //    return structure;
        //}

        //public static T Read<T>(Int32 address)
        //{
        //    int length = Marshal.SizeOf(typeof(T));
        //    //if (typeof(T) == typeof(bool)) length = 1;
        //    byte[] buffer = new byte[length];
        //    bool success = Memory.ReadBytes(procHnd, (IntPtr)address, ref buffer, (UInt32)length);
        //    return GetStructure<T>(buffer);
        //}

        public static T ReadType<T>(IntPtr processHandle, IntPtr address, int structSize = 0)
        {
            if (structSize == 0)
                structSize = Marshal.SizeOf(typeof(T));
            byte[] bytes = new byte[structSize];
            if (!Memory.ReadBytes(processHandle, address, ref bytes, (uint)bytes.Length)) throw new Exception("ReadProcessMemory failed");
            //Console.WriteLine($"structSize: {structSize.ToString()}");
            //if (numRead != bytes.Length)throw new Exception("Number of bytes read does not match structure size");
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            T structure = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();
            return structure;
        }

        public static T ByteArrayToStructure<T>(int pOffset, int pSize) where T : struct
        {
            //byte[] buffer = ReadMem(pOffset, pSize);
            byte[] buffer = new byte[pSize];
            Memory.ReadBytes(procHnd, (IntPtr)pOffset, ref buffer, (uint)pSize);
            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            T stuff = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(),
                typeof(T));
            handle.Free();
            return stuff;
        }


        private static void OnRenderer(int fps, EventArgs args)
        {

            //ImGui.Text("text");
            //float value = 33.0f;
            //ImGui.SliderFloat("tova e sliderFloat", ref value, 0.0f, 100.0f);

            if (!drawyourstuff) return; //process is dead, don't bother drawing
            if ((!isGameOnTop) && (!isOverlayOnTop)) return; //if game and overlay are not on top, don't draw

            //Renderer.Direct3DDevice.Clear(ClearFlags.Target, new SharpDX.Mathematics.Interop.RawColorBGRA(127, 127, 127, 127), 1.0f, 0);

            //var white = new Color(255, 255, 255, 255);
            ////UInt32 white = (0xFFFFFFFF);

            //float[] myFloats = 
            //    { 100.0f, 200.0f,
            //    333.0f, 444.0f,
            //    400.0f, 700.0f,
            //    789, 654,
            //    333 , 900}; 

            //Renderer.DrawLines(myFloats, 5, white, 2, true);

            //Renderer.DrawText(fps.ToString(), wndMargins.X, wndMargins.Y, 32, new Color(255,255,255,255));
            //Renderer.DrawText(fps.ToString(), wndMargins.X, wndMargins.Y, new Color(255, 255, 255, 255));
            //Renderer.DrawText(Components.DrawingMenu.keyBindz.Key.ToString(), 300, 0, 32, white);
            //Renderer.DrawText(Components.DrawingMenu.keyBindz.Value.ToString(), 350, 0, 32, white);
            //Renderer.DrawText(Components.DrawingMenu.keyBindz.Enabled.ToString(), 400, 0, 32, white);



            //Renderer.DrawText(m2Pressed.ToString(), 600, 600, 0xFFFFFFFF);
            //var theName = Memory.ReadString(procHnd, (IntPtr)0x0646827A, true);
            //var wtff = Memory.GetModule(procHnd, "kernel32.dll");
            //Renderer.DrawText(wtff.ToString("X"), 600, 600, 32, 0xFFFFFFFF);
            //var theName = ReadString(0x0CA53EED, 14, Encoding.Default);
            //var textRect = Renderer.GetTextWidth("кот доди | 𝒞vllum カルム // ｓｅｒｅｎｉｔｙ \n hehehe xDD", 16);
            //Renderer.DrawRect(600 - 2 - textRect.X/2, 630, textRect.X, textRect.Y, 0x6000FF00);
            //Renderer.DrawText("кот доди | 𝒞vllum カルム // ｓｅｒｅｎｉｔｙ \n hehehe xDD", 600, 630, new Color(0xFFFFFF60), 16, TextAlignment.centered);
            //Renderer.DrawLine(600, 630, 10, 10, 2, true, 0x6000FF00);




            //ArrowRendering.Render(new Vector2(700, 700), 5.0f, 2.0f, true);
            //CircleRendering2D.Render(new Vector2(700, 750), 50, new Color(255, 255, 255, 255), 5, false);
            //LineRendering.Render(new Color(255, 0, 255, 255), 3, new Vector2(100, 100), new Vector2(345, 556), new Vector2(754, 556));
            //ParallelogramRendering.Render(new Vector2(300, 300), 10, 10, 0, new Color(255, 255, 255, 255));
            //RectangleRendering.RenderOutline(new Vector2(250, 250), 100, 100, 2, new Color(255, 0, 0, 127));
            //RectangleRendering.Render(new Vector2(250, 250), new Vector2(350, 350), new Color(127, 127, 127, 127));
            //SquircleRendering.Render(new Vector2(500, 500), new Vector2(600, 350), new Color(255, 0, 255, 255));
            //SquircleRendering.RenderOutline(new Vector2(500, 500), 100, 100, 2, new Color(255, 255, 255, 255));
            //TextRendering.Render("testing SDK drawer", new Color(0, 255, 255, 255), new Vector2(700, 200));


            //Renderer.DrawRect(100, 100, 100, 100, 0x7FFF0000);
            //Renderer.DrawLine(200, 200, 345, 678, 2, true, 0x7F0000FF);
            //Renderer.DrawLine(250, 250, 345, 678, 2, false, 0x7F007FFF);
            //Renderer.DrawCircle(960, 540, 500, 0, 2500, true, 0x7f4F4F00);


            //Renderer.DrawCircle(800, 800, 400, 1, true, new Color(0xFFFFFFFF));

            var matrix = Memory.ReadMatrix(procHnd, (IntPtr)0x501AE8);
            var EntityListPtr = Memory.ReadPointer(procHnd, (IntPtr)0x50F4F8, isWow64Process);
            
            if (EntityListPtr != IntPtr.Zero)
            {
                var entityCount = Memory.ReadUInt32(procHnd, (IntPtr)0x50F500);
                if (entityCount > 0)
                {
                    for (uint i = 0; i <= entityCount; i++)
                    {
                        var entityAddr = Memory.ReadPointer(procHnd, (IntPtr)(EntityListPtr.ToInt64() + i * 4), isWow64Process);
                        //Console.WriteLine($"Entities: {entityAddr.ToString()}");
                        if (entityAddr != IntPtr.Zero)
                        {

                            //CClientInfo gameEnt = new CClientInfo();
                            //gameEnt = ByteArrayToStructure<CClientInfo>((entityAddr.ToInt32()), Marshal.SizeOf(gameEnt));
                            //Console.WriteLine($"{gameEnt.Team.ToString()}");

                            //var byted_str = new byte[32];
                            //Memory.ReadBytes(procHnd, (IntPtr)(entityAddr.ToInt64() + 0x225),ref byted_str, 32);
                            //string rezult = System.Text.Encoding.Default.GetString(byted_str).TrimEnd('\0');
                            //string rezult = BitConverter.ToString(byted_str);
                            //Console.WriteLine($"{rezult}");


                            //var health = Read<Int32>(entityAddr.ToInt32() + 0xF8);
                            var health = Memory.ReadInt32(procHnd, (IntPtr)(entityAddr.ToInt64() + 0xF8));
                            
                            if ((health >= 1) && (health <= 100)) //making sure it's alive
                            {
                                var armor = Memory.ReadInt32(procHnd, (IntPtr)(entityAddr.ToInt64() + 0xFC));
                                var playerName = Memory.ReadString(procHnd, (IntPtr)(entityAddr.ToInt64() + 0x225), false);
                                var headpos = Memory.ReadVector3(procHnd, (IntPtr)(entityAddr.ToInt64() + 0x04));
                                var feetpos = Memory.ReadVector3(procHnd, (IntPtr)(entityAddr.ToInt64() + 0x034));
                                var stanceFlt = Memory.ReadFloat(procHnd, (IntPtr)(entityAddr.ToInt64() + 0x05C));
                                Vector2 vScreen_head = new Vector2(0, 0);
                                Vector2 vScreen_foot = new Vector2(0, 0);
                                headpos.Z += 0.9f;
                                //if (WorldToScreen(headpos, out vScreen_head, matrix, wndMargins, wndSize))
                                if (Renderer.WorldToScreen(headpos, out vScreen_head, matrix, wndMargins, wndSize,W2SType.TypeOGL))
                                {

                                    //Renderer.DrawRect(vScreen.X, vScreen.Y, 3, 3, 0xFFFFFFFF);
                                    //if (WorldToScreen(feetpos, out vScreen_foot, matrix, wndMargins, wndSize))
                                    Renderer.WorldToScreen(feetpos, out vScreen_foot, matrix, wndMargins, wndSize, W2SType.TypeOGL);
                                    {
                                        var boxHeight = vScreen_foot.Y - vScreen_head.Y;
                                        //Renderer.DrawBox(vScreen_head.X - boxHeight / 4, vScreen_head.Y, boxHeight / 2, boxHeight, 4, 0x7FFF0000);
                                        //Renderer.DrawRect(vScreen_head.X - 1, vScreen_head.Y - 1, 3, 3, new Color(0xFFFFFFFF));
                                        //Renderer.DrawText(playerName, vScreen_foot.X, vScreen_foot.Y + 20, Components.DrawingMenu.ColorQ.Color, 12, TextAlignment.centered,false);

                                        //Renderer.DrawFPSBox(vScreen_head.X, vScreen_head.Y, vScreen_foot.X, vScreen_foot.Y, Components.DrawingMenu.ColorQ.Color, /*Components.DrawingMenu.CircleThickness.Value*/0, (stanceFlt == 4.50f ? 0 : 1), health, 100, armor, 100, playerName, 0 );

                                        Renderer.DrawFPSBox(vScreen_head.X, vScreen_head.Y, vScreen_foot.X, vScreen_foot.Y, Components.DrawingMenu.ColorQ.Color, (stanceFlt == 4.50f ? BoxStance.standing : BoxStance.crouching), 0, true, false, health, 100, armor, 100, 0, "hello", "line2", "", "wooh", "haha");

                                        //var dummyVec = new Vector3(feetpos.X, feetpos.Z, feetpos.Y);
                                        //CircleRendering.Render(matrix, new Color(255, 255, 255, 255), 30, dummyVec);
                                        //Renderer.DrawText(gameEnt.PlayerHardName.ToString(), vScreen_foot.X, vScreen_foot.Y + 20, new Color(0xFFFFFFFF));
                                    }
                                }    
                            }
                        }
                    }
                }
            }
        }
    }
}
