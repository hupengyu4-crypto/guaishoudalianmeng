using RootScript.Config;
using System.Collections.Generic;

public class GameConfigRegister
{
    public Dictionary<string, int> Groups { get; private set; }

    public void Initialize(ConfigManagerImpl impl)
    {
        impl.RegisterSheetConfig("BattleDefineCfg", false, "ConfigByteNew/Battle/System/BattleDefineCfg", () => new BattleDefineCfg());
        impl.RegisterSheetConfig("BattleFighterCfg", false, "ConfigByteNew/Battle/System/BattleFighterCfg", () => new BattleFighterCfg());
        impl.RegisterSheetConfig("BattleSceneCfg", false, "ConfigByteNew/Battle/System/BattleSceneCfg", () => new BattleSceneCfg());
        impl.RegisterSheetConfig("BattleSkillCfg", false, "ConfigByteNew/Battle/System/BattleSkillCfg", () => new BattleSkillCfg());
        Groups = new Dictionary<string, int>();
        impl.RegisterSheetConfig("BaseEffectCfg", "AddStateEffectConfig", false, "ConfigByteNew/Battle/System/Effect/AddStateEffectConfig", ()=>new AddStateEffectConfig());
        impl.RegisterSheetConfig("BaseEffectCfg", "CheckBlockEffectConfig", false, "ConfigByteNew/Battle/System/Effect/CheckBlockEffectConfig", ()=>new CheckBlockEffectConfig());
        impl.RegisterSheetConfig("BaseEffectCfg", "CheckEffectConfig", false, "ConfigByteNew/Battle/System/Effect/CheckEffectConfig", ()=>new CheckEffectConfig());
        impl.RegisterSheetConfig("BaseEffectCfg", "CheckEventEffectConfig", false, "ConfigByteNew/Battle/System/Effect/CheckEventEffectConfig", ()=>new CheckEventEffectConfig());
        impl.RegisterSheetConfig("BaseEffectCfg", "CheckStateEffectConfig", false, "ConfigByteNew/Battle/System/Effect/CheckStateEffectConfig", ()=>new CheckStateEffectConfig());
        impl.RegisterSheetConfig("BaseEffectCfg", "ClearEffectConfig", false, "ConfigByteNew/Battle/System/Effect/ClearEffectConfig", ()=>new ClearEffectConfig());
        impl.RegisterSheetConfig("BaseEffectCfg", "CheckPropertyEffectConfig", false, "ConfigByteNew/Battle/System/Effect/CheckPropertyEffectConfig", ()=>new CheckPropertyEffectConfig());
        impl.RegisterSheetConfig("BaseEffectCfg", "ClearStateEffectConfig", false, "ConfigByteNew/Battle/System/Effect/ClearStateEffectConfig", ()=>new ClearStateEffectConfig());
        impl.RegisterSheetConfig("BaseEffectCfg", "DamageChangeEffectConfig", false, "ConfigByteNew/Battle/System/Effect/DamageChangeEffectConfig", ()=>new DamageChangeEffectConfig());
        impl.RegisterSheetConfig("BaseEffectCfg", "DamageEffectCfg", false, "ConfigByteNew/Battle/System/Effect/DamageEffectCfg", ()=>new DamageEffectCfg());
        impl.RegisterSheetConfig("BaseEffectCfg", "DamageUpEffectConfig", false, "ConfigByteNew/Battle/System/Effect/DamageUpEffectConfig", ()=>new DamageUpEffectConfig());
        impl.RegisterSheetConfig("BaseEffectCfg", "FindTargetEffectConfig", false, "ConfigByteNew/Battle/System/Effect/FindTargetEffectConfig", ()=>new FindTargetEffectConfig());
        impl.RegisterSheetConfig("BaseEffectCfg", "HealEffectConfig", false, "ConfigByteNew/Battle/System/Effect/HealEffectConfig", ()=>new HealEffectConfig());
        impl.RegisterSheetConfig("BaseEffectCfg", "ImmuneDamageConfig", false, "ConfigByteNew/Battle/System/Effect/ImmuneDamageConfig", ()=>new ImmuneDamageConfig());
        impl.RegisterSheetConfig("BaseEffectCfg", "PropertyEffectConfig", false, "ConfigByteNew/Battle/System/Effect/PropertyEffectConfig", ()=>new PropertyEffectConfig());
        impl.RegisterSheetConfig("BaseEffectCfg", "ProbabilityEffectConfig", false, "ConfigByteNew/Battle/System/Effect/ProbabilityEffectConfig", ()=>new ProbabilityEffectConfig());
        impl.RegisterSheetConfig("BaseEffectCfg", "ReplaceEndEffectConfig", false, "ConfigByteNew/Battle/System/Effect/ReplaceEndEffectConfig", ()=>new ReplaceEndEffectConfig());
        impl.RegisterSheetConfig("BaseEffectCfg", "ReplaceSkillEffectConfig", false, "ConfigByteNew/Battle/System/Effect/ReplaceSkillEffectConfig", ()=>new ReplaceSkillEffectConfig());
        impl.RegisterSheetConfig("BaseEffectCfg", "ShareDamageEffectConfig", false, "ConfigByteNew/Battle/System/Effect/ShareDamageEffectConfig", ()=>new ShareDamageEffectConfig());
        impl.RegisterSheetConfig("BaseEffectCfg", "ShieldAddEffectConfig", false, "ConfigByteNew/Battle/System/Effect/ShieldAddEffectConfig", ()=>new ShieldAddEffectConfig());
        impl.RegisterSheetConfig("BaseEffectCfg", "ShieldRemoveEffectConfig", false, "ConfigByteNew/Battle/System/Effect/ShieldRemoveEffectConfig", ()=>new ShieldRemoveEffectConfig());
        impl.RegisterSheetConfig("BaseEffectCfg", "TriggerEffectConfig", false, "ConfigByteNew/Battle/System/Effect/TriggerEffectConfig", ()=>new TriggerEffectConfig());
        impl.RegisterSheetConfig("BaseEffectCfg", "UseSkillEffectConfig", false, "ConfigByteNew/Battle/System/Effect/UseSkillEffectConfig", ()=>new UseSkillEffectConfig());
        impl.RegisterSheetConfig("BaseEffectCfg", "VampireEffectConfig", false, "ConfigByteNew/Battle/System/Effect/VampireEffectConfig", ()=>new VampireEffectConfig());
        impl.RegisterSheetConfig("BaseEffectCfg", "ShareVampireEffectConfig", false, "ConfigByteNew/Battle/System/Effect/ShareVampireEffectConfig", ()=>new ShareVampireEffectConfig());
        impl.RegisterSheetConfig("BaseEffectCfg", "ConditionConsumeEffectConfig", false, "ConfigByteNew/Battle/System/Effect/ConditionConsumeEffectConfig", ()=>new ConditionConsumeEffectConfig());
        impl.RegisterSheetConfig("BaseEffectCfg", "DamageReduceEffectConfig", false, "ConfigByteNew/Battle/System/Effect/DamageReduceEffectConfig", ()=>new DamageReduceEffectConfig());
        impl.RegisterSheetConfig("BaseEffectCfg", "ReliveEffectConfig", false, "ConfigByteNew/Battle/System/Effect/ReliveEffectConfig", ()=>new ReliveEffectConfig());
        impl.RegisterSheetConfig("BaseEffectCfg", "ShieldAbsorptionReflectiveEffectConfig", false, "ConfigByteNew/Battle/System/Effect/ShieldAbsorptionReflectiveEffectConfig", ()=>new ShieldAbsorptionReflectiveEffectConfig());
        impl.RegisterSheetConfig("BaseEffectCfg", "CheckFighterEffectConfig", false, "ConfigByteNew/Battle/System/Effect/CheckFighterEffectConfig", ()=>new CheckFighterEffectConfig());
        impl.RegisterSheetConfig("BaseEffectCfg", "ConductionEffectConfig", false, "ConfigByteNew/Battle/System/Effect/ConductionEffectConfig", ()=>new ConductionEffectConfig());
        impl.RegisterSheetConfig("BaseEffectCfg", "KillEffectConfig", false, "ConfigByteNew/Battle/System/Effect/KillEffectConfig", ()=>new KillEffectConfig());
        impl.RegisterSheetConfig("BaseEffectCfg", "TauntEffectConfig", false, "ConfigByteNew/Battle/System/Effect/TauntEffectConfig", ()=>new TauntEffectConfig());
        impl.RegisterSheetConfig("BaseEffectCfg", "ApportionDamageEffectConfig", false, "ConfigByteNew/Battle/System/Effect/ApportionDamageEffectConfig", ()=>new ApportionDamageEffectConfig());
        impl.RegisterSheetConfig("BaseEffectCfg", "ShieldStealEffectConfig", false, "ConfigByteNew/Battle/System/Effect/ShieldStealEffectConfig", ()=>new ShieldStealEffectConfig());
        impl.RegisterSheetConfig("BaseEffectCfg", "ComboEffectConfig", false, "ConfigByteNew/Battle/System/Effect/ComboEffectConfig", ()=>new ComboEffectConfig());
        impl.RegisterSheetConfig("BaseEffectCfg", "DoLogicConfig", false, "ConfigByteNew/Battle/System/Effect/DoLogicConfig", ()=>new DoLogicConfig());
        impl.RegisterSheetConfig("BaseEffectCfg", "ResistEffectConfig", false, "ConfigByteNew/Battle/System/Effect/ResistEffectConfig", ()=>new ResistEffectConfig());
        impl.RegisterSheetConfig("BaseEffectCfg", "TransferDamageEffectConfig", false, "ConfigByteNew/Battle/System/Effect/TransferDamageEffectConfig", ()=>new TransferDamageEffectConfig());
        impl.RegisterSheetConfig("BaseEffectCfg", "DamageToPropertyEffectConfig", false, "ConfigByteNew/Battle/System/Effect/DamageToPropertyEffectConfig", ()=>new DamageToPropertyEffectConfig());
        impl.RegisterSheetConfig("BaseEffectCfg", "SwitchTargetToAuthorEffectConfig", false, "ConfigByteNew/Battle/System/Effect/SwitchTargetToAuthorEffectConfig", ()=>new SwitchTargetToAuthorEffectConfig());
    }
}
