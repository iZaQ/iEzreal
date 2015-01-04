using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

namespace PrimeiroScript
{
    class Program
    {
        public static string ChampName = "Ezreal";
        public static Orbwalking.Orbwalker Orbwalker;
        private static readonly Obj_AI_Hero player = ObjectManager.Player;
        public static Spell Q, W, R;
        public static SpellSlot IgniteSlot;
        public static Items.Item Dfg, Gunblade;

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }
        public static Menu iMenu;


        private static void Game_OnGameLoad(EventArgs args)
        {
            if (player.BaseSkinName != ChampName) return;

            Q = new Spell(SpellSlot.Q, 1100);
            W = new Spell(SpellSlot.W, 900);

            R = new Spell(SpellSlot.R, float.MaxValue);

            Q.SetSkillshot(0.5f, 80f, 1200, true, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.5f, 80f, 1200, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(1f, 160f, 2000, false, SkillshotType.SkillshotLine);

            IgniteSlot = player.GetSpellSlot("SummonerDot");

            Dfg = new Items.Item(3128, 750f);
            Gunblade = new Items.Item(3146, 700f);
         
            iMenu = new Menu("i" + ChampName, ChampName, true);
          
            iMenu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            Orbwalker = new Orbwalking.Orbwalker(iMenu.SubMenu("Orbwalker"));
          
            var ts = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(ts);
            iMenu.AddSubMenu(ts);
           
            iMenu.AddSubMenu(new Menu("Combo", "Combo"));
            iMenu.SubMenu("Combo").AddItem(new MenuItem("useQ", "Use Q?").SetValue(true));
            iMenu.SubMenu("Combo").AddItem(new MenuItem("useW", "Use W?").SetValue(true));
            iMenu.SubMenu("Combo").AddItem(new MenuItem("useR", "Use R?").SetValue(true));
            iMenu.SubMenu("Combo").AddItem(new MenuItem("useItems", "Use Items?").SetValue(true));
            iMenu.SubMenu("Combo").AddItem(new MenuItem("ComboActive", "Combo").SetValue(new KeyBind(32, KeyBindType.Press)));

            iMenu.AddSubMenu(new Menu("Harass", "Harass"));
            iMenu.SubMenu("Harass").AddItem(new MenuItem("hQ", "Use Q?").SetValue(true));
            iMenu.SubMenu("Harass").AddItem(new MenuItem("hW", "Use W?").SetValue(true));
            iMenu.SubMenu("Harass").AddItem(new MenuItem("Harassmana", "Mana % ").SetValue(new Slider(30)));
            iMenu.SubMenu("Harass").AddItem(new MenuItem("HarassActive", "Harass").SetValue(new KeyBind('C', KeyBindType.Press)));
            iMenu.SubMenu("Harass").AddItem(new MenuItem("HarassToggle", "Harass").SetValue(new KeyBind('T', KeyBindType.Toggle)));

            iMenu.AddSubMenu(new Menu("Killsteal", "Killsteal"));
            iMenu.SubMenu("Killsteal").AddItem(new MenuItem("KillQ", "Use  Q?").SetValue(true));
            iMenu.SubMenu("Killsteal").AddItem(new MenuItem("KillW", "Use  W?").SetValue(true));
            iMenu.SubMenu("Killsteal").AddItem(new MenuItem("KillR", "Use  R?").SetValue(true));
            iMenu.SubMenu("Killsteal").AddItem(new MenuItem("KillI", "Use  Ignite?").SetValue(true));

            iMenu.AddSubMenu(new Menu("Drawing", "Drawing"));
            iMenu.SubMenu("Drawing").AddItem(new MenuItem("DrawQ", "Draw Q?").SetValue(true));
            iMenu.SubMenu("Drawing").AddItem(new MenuItem("DrawW", "Draw W?").SetValue(true));
            iMenu.SubMenu("Drawing").AddItem(new MenuItem("DrawAA", "Draw Range?").SetValue(true));
            


            
            iMenu.AddItem(new MenuItem("Packet", "Packet Casting").SetValue(true));
            iMenu.AddItem(new MenuItem("Packet", "V.0.1")
            iMenu.AddItem(new MenuItem("Packet", "iZaQ")
           
            iMenu.AddToMainMenu();

            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnGameUpdate += Game_OnGameUpdate;

            Game.PrintChat("<font color=\"#FFF300\">iEzreal by iZaQ</font>"+ " Injected!");
        }

        static void Game_OnGameUpdate(EventArgs args)
        {
            if (iMenu.Item("ComboActive").GetValue<KeyBind>().Active)
            {
                Combo();
            }

            if (iMenu.Item("HarassActive").GetValue<KeyBind>().Active || iMenu.Item("HarassToggle").GetValue<KeyBind>().Active)
            {
                Harass();
            }

            KillSteal();
        }


      

        static void Drawing_OnDraw(EventArgs args)
        {
            if (iMenu.Item("DrawQ").GetValue<bool>() == true)
            {
                Utility.DrawCircle(player.Position, Q.Range, Color.Red);
            }

            if (iMenu.Item("DrawW").GetValue<bool>() == true)
            {
                Utility.DrawCircle(player.Position, W.Range, Color.Orange);
            }
            if (iMenu.Item("DrawAA").GetValue<bool>() == true)
            {
                Utility.DrawCircle(player.Position, player.AttackRange, Color.Blue);
            }
            




        }

        public static void KillSteal()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (target == null) return;
            var igniteDmg = player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);

            if (target.IsValidTarget(Q.Range) && Q.IsReady() && iMenu.Item("KillQ").GetValue<bool>() == true && ObjectManager.Player.GetSpellDamage(target, SpellSlot.Q) > target.Health)
            {
                Q.Cast(target, iMenu.Item("Packet").GetValue<bool>());
            }

            if (target.IsValidTarget(W.Range) && W.IsReady() && iMenu.Item("KillW").GetValue<bool>() == true && ObjectManager.Player.GetSpellDamage(target, SpellSlot.W) > target.Health)
            {
                W.Cast(target, iMenu.Item("Packet").GetValue<bool>());
            }

            if (target.IsValidTarget(3000) && R.IsReady() && iMenu.Item("KillR").GetValue<bool>() == true && ObjectManager.Player.GetSpellDamage(target, SpellSlot.R) > target.Health)
            {
                R.Cast(target, iMenu.Item("Packet").GetValue<bool>());
            }


            if (iMenu.Item("KillI").GetValue<bool>() == true && player.Spellbook.CanUseSpell(IgniteSlot) == SpellState.Ready)
            {
                if (igniteDmg > target.Health && player.Distance(target, false) < 600)
                {
                    player.Spellbook.CastSpell(IgniteSlot, target);
                }

            }




        }



        public static void Harass()
        {
             if (player.Mana / player.MaxMana * 100 > iMenu.SubMenu("Harass").Item("Harassmana").GetValue<Slider>().Value)
                return;
            

            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (target == null)
                return;
            {
                if (target.IsValidTarget(Q.Range) && Q.IsReady() && iMenu.Item("hQ").GetValue<bool>() == true)
                {
                    Q.Cast(target, iMenu.Item("Packet").GetValue<bool>());
                }

                if (target.IsValidTarget(W.Range) && W.IsReady() && iMenu.Item("hW").GetValue<bool>() == true)
                {
                    W.Cast(target, iMenu.Item("Packet").GetValue<bool>());
                }


            }






        }


        public static void Combo()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (target == null) return;


            if (Gunblade.IsReady() && iMenu.Item("useItems").GetValue<bool>() == true)
            {
                Gunblade.Cast(target);
            }

            if (Dfg.IsReady() && iMenu.Item("useItems").GetValue<bool>() == true)
            {
                Dfg.Cast(target);
            }

            if (target.IsValidTarget(Q.Range) && Q.IsReady() && iMenu.Item("useQ").GetValue<bool>() == true)
            {
                Q.Cast(target, iMenu.Item("Packet").GetValue<bool>());



            }

            if (target.IsValidTarget(W.Range) && W.IsReady() && iMenu.Item("useW").GetValue<bool>() == true)
            {
                W.Cast(target, iMenu.Item("Packet").GetValue<bool>());
            }



            if (target.IsValidTarget(R.Range) && R.IsReady() && ObjectManager.Player.GetSpellDamage(target, SpellSlot.R) > target.Health && iMenu.Item("useR").GetValue<bool>() == true)
            {
                R.CastOnUnit(target, iMenu.Item("Packet").GetValue<bool>());
            }


            Game.PrintChat("<font=\"#FFF300\">iEzreal by iZaQ", " Injected!");





        }
    }
}
