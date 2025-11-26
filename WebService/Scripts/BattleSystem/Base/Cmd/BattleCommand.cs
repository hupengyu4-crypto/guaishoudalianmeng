using System;
using System.Collections.Generic;
using System.Text;
using Google.Protobuf;
using Google.Protobuf.Collections;

namespace BattleSystem
{
    public class BattleCommand
    {
        public BattleCommand(BaseBattle battle)
        {
            this.Battle = battle;
        }

        public BaseBattle Battle { get; }

        public Dictionary<long, List<BattleCommandHandler>> externalActions = new Dictionary<long, List<BattleCommandHandler>>();
        /// <summary>
        /// 所有指令
        /// </summary>
        public List<BattleCommandHandler> allAction = new List<BattleCommandHandler>();

        private Dictionary<long, Dictionary<long, MakeDamageCmd>> _mergeMakeDamageCmds = new();

        /// <summary>
        ///
        /// </summary>
        /// <param name="offset">无队伍火把战不会推帧，这里偏移1才能执行指令</param>
        public void OnUpdate(int offset = 0)
        {
            if (externalActions.TryGetValue(Battle.Frame + offset, out var actions))
            {
                for (int i = 0; i < actions.Count; i++)
                {
                    actions[i].Do(Battle);
                    actions[i] = null;
                }
                externalActions.Remove(Battle.Frame + offset);
                _mergeMakeDamageCmds.Remove(Battle.Frame + offset);
            }
        }
        /// <summary>
        /// 操作指令
        /// </summary>
        /// <param name="cmdHandler">指令</param>
        public void PushCmd(BattleCommandHandler cmdHandler)
        {
            Battle.DispatchEvent(Battle, BaseBattle.Event.CmdBegin, EventParams<BattleCommandHandler>.Create(Battle, cmdHandler));
            PushCmd(cmdHandler, Battle.Frame + 1);
        }

        /// <summary>
        /// 在指定帧操作指令
        /// </summary>
        /// <param name="cmdHandler">指令</param>
        /// <param name="frame">帧</param>
        private void PushCmd(BattleCommandHandler cmdHandler, uint frame)
        {
            if (frame > Battle.Frame)
            {
                MakeDamageCmd makeDamageCmd = null;
                Dictionary<long, MakeDamageCmd> cmds = null;
                if (cmdHandler is MakeDamageCmd mdc/* && mdc.AttackUid == BattleDef.fireUid*/)
                {
                    makeDamageCmd = mdc;
                    if (!_mergeMakeDamageCmds.TryGetValue(frame, out cmds))
                    {
                        cmds = new();
                        _mergeMakeDamageCmds.Add(frame, cmds);
                    }

                    if (cmds.TryGetValue(makeDamageCmd.DefendUid, out var cmd))
                    {
                        cmd.Damage += makeDamageCmd.Damage;
                        return;
                    }
                }

                if (!externalActions.TryGetValue(frame, out var actions))
                {
                    actions = new List<BattleCommandHandler>();
                    externalActions.Add(frame, actions);
                }
                cmdHandler.SetFrame(frame);
                actions.Add(cmdHandler);
                allAction.Add(cmdHandler);

                if (makeDamageCmd != null && cmds != null)
                {
                    cmds.Add(makeDamageCmd.DefendUid, makeDamageCmd);
                }
            }
            else
            {
                Log.error($"指令失效，添加帧：{frame}，当前帧：{Battle.Frame}");
            }
        }

        public byte[] GetCmd()
        {
            if (allAction == null || allAction.Count == 0)
                return null;
            Dictionary<long, frame_cmd> tempDic = new Dictionary<long, frame_cmd>();

            for (int i = 0; i < allAction.Count; i++)
            {
                var action = allAction[i];
                if (BattleDef.cmdType.TryGetValue(action.GetType(), out int type))
                {
                    byte[] cmd = action.Serialize();
                    if (!tempDic.TryGetValue(action.Frame, out frame_cmd frameCmd))
                    {
                        frameCmd = new frame_cmd();
                        frameCmd.Frame = action.Frame;
                        frameCmd.List = new list_int_bytes();
                        tempDic.Add(action.Frame, frameCmd);
                    }
                    frameCmd.List.List.Add(new int_bytes() { K = type, V = ByteString.CopyFrom(cmd) });
                }
            }
            allAction.Clear();
            all_cmd allCmd = new all_cmd();
            foreach (var value in tempDic.Values)
            {
                allCmd.Froms.Add(value);
            }
            return allCmd.ToByteArray();
        }

        /// <summary>
        /// 设置命令
        /// </summary>
        /// <param name="byteArray"></param>
        public void SetCmd(byte[] byteArray)
        {
            if (byteArray == null) return;
            externalActions.Clear();
            _mergeMakeDamageCmds.Clear();
            ParseCmd(byteArray);
        }

        /// <summary>
        /// 解析命令
        /// </summary>
        /// <param name="byteArray"></param>
        private void ParseCmd(byte[] byteArray)
        {
            if (byteArray == null) return;
            all_cmd allCmd = PbManager<all_cmd>.ParseFrom(byteArray);
            if (allCmd == null) return;
            foreach (var frameCmd in allCmd.Froms)
            {
                if (!externalActions.TryGetValue(frameCmd.Frame, out var dataList))
                {
                    dataList = new List<BattleCommandHandler>();
                    externalActions.Add(frameCmd.Frame, dataList);
                }

                foreach (var data in frameCmd.List.List)
                {
                    BattleCommandHandler handler = BattleDef.NewBattleCommandHandler(data.K, data.V.ToByteArray());
                    if (handler != null)
                    {
                        handler.SetFrame(frameCmd.Frame);
                        dataList.Add(handler);
                    }
                }
            }
        }

    }
}
