﻿using EnsoulSharp;
using EnsoulSharp.SDK;
using FunnySlayerCommon;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominationAIO.NewPlugins
{
    public static class EventsIrelia
    {
        public static void KS(EventArgs args)
        {
            if (!Irelia.Q.IsReady())
                return;

            var targets = ObjectManager.Get<AIHeroClient>().Where(i => !i.IsAlly && !i.IsDead && Irelia.Q.CanCast(i)).Where(i => i.Health <= Helper.GetQDmg(i) + Helper.GetQDmg(i) * 0.08f).OrderBy(i => i.DistanceToPlayer());

            if (targets == null)
                return;

            foreach (var target in targets)
            {
                if (!Helper.UnderTower(target.Position) || MenuSettings.KeysSettings.TurretKey.Active)
                {
                    if (Irelia.Q.Cast(target) == CastStates.SuccessfullyCasted)
                        return;
                }
            }
        }
        public static void AIBaseClient_OnProcessSpellCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.SData.Name == "IreliaEMissile")
            {
                Irelia.E1Pos = args.End;
            }
        }

        public static void Combo(EventArgs args)
        {
            if(Orbwalker.ActiveMode != OrbwalkerMode.Combo || ObjectManager.Player.IsDead)
            {
                return;
            }

            var target = TargetSelector.GetTarget(1000);
            if (target == null)
                return;

            if (LogicR.IRELIA_RCOMBO() && MenuSettings.RSettings.Rcombo.Enabled)
            {
                return;
            }
            else
            {
                if(MenuSettings.ESettings.Ecombo.Enabled)
                    LogicE.EPrediction(true);

                if (MenuSettings.QSettings.Qcombo.Enabled)
                {
                    LogicQ.GapCloserTargetCanKillable();

                    switch (MenuSettings.QSettings.QListComboMode.Index)
                    {
                        case 1:
                            LogicQ.NewHighLogic(target);
                            break;
                        case 2:
                            LogicQ.NewHighLogic(target);
                            break;
                        case 3:
                            LogicQ.NewExtreamLogic(target);
                            break;
                        case 0:
                            LogicQ.QGapCloserPos(target.Position);
                            break;
                    }
                }               
            }
        }
        public static void AIBaseClient_OnBuffLose(AIBaseClient sender, AIBaseClientBuffRemoveEventArgs args)
        {
            if (sender.NetworkId == ObjectManager.Player.NetworkId || sender.IsMe)
            {
                if (args.Buff.Name == "sheen"
                    || args.Buff.Name == "3078trinityforce"
                    || args.Buff.Name == "6632buff"
                    || args.Buff.Name.Contains("sheen")
                    || args.Buff.Name.Contains("trinity")
                    || args.Buff.Name.Contains("iceborn")
                    || args.Buff.Name.Contains("Lich"))
                {
                    Irelia.SheenTimer = Variables.TickCount;
                }
            }
        }

        public static void Game_OnUpdate(EventArgs args)
        {
            if (MenuSettings.KeysSettings.FleeKey.Active)
            {
                #region New E pred
                LogicE.EPrediction(false);
                #endregion
                LogicQ.QGapCloserPos(Game.CursorPos);
            }
            if (MenuSettings.KeysSettings.SemiE.Active && Irelia.E.IsReady())
            {
                #region New E pred
                LogicE.EPrediction(false);
                #endregion
            }
            if (MenuSettings.KeysSettings.SemiR.Active && Irelia.R.IsReady())
            {
                #region R
                try
                {
                    var targets = TargetSelector.GetTargets(900);
                    Vector3 Rpos = Vector3.Zero;

                    if (targets != null)
                    {
                        foreach (var Rprediction in targets.Select(i => Irelia.R.GetPrediction(i)).Where(i => i.Hitchance >= EnsoulSharp.SDK.HitChance.High || (i.Hitchance >= EnsoulSharp.SDK.HitChance.Medium && i.AoeTargetsHitCount > 1)).OrderByDescending(i => i.AoeTargetsHitCount))
                        {
                            Rpos = Rprediction.CastPosition;
                        }
                        if (Rpos != Vector3.Zero)
                        {
                            Irelia.R.Cast(Rpos);
                        }
                    }                   
                }
                catch (Exception ex)
                {
                    Console.WriteLine("R.cast Error" + ex);
                }
                #endregion
            }
        }

        public static void Drawing_OnDraw(EventArgs args)
        {

            Drawing.DrawCircle(ObjectManager.Player.Position, Irelia.Q.Range, System.Drawing.Color.Red);
            /*if (!MenuSettings.ClearSettings.DrawMinions.Enabled) return;

            var minions = GameObjects.GetMinions(2000).Where(i => Helper.CanQ(i)).OrderByDescending(i => i.DistanceToPlayer()).ToList();

            if (minions == null) return;

            foreach (var min in minions)
            {
                Drawing.DrawCircle(min.Position, 70, System.Drawing.Color.White);
            }*/
        }
    }
}
