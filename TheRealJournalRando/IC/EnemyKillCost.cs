﻿using ItemChanger;
using TheRealJournalRando.Data;

namespace TheRealJournalRando.IC
{
    public record EnemyKillCost(string EnemyPdName, string EnemyConvoName, int Total) : Cost
    {
        private JournalKillCounterModule? module;

        public override void Load()
        {
            module = ItemChangerMod.Modules.GetOrAdd<JournalKillCounterModule>();
        }

        public override bool HasPayEffects() => false;

        public int GetBalanceDue()
        {
            if (module == null)
            {
                Load();
            }
            return Total - module!.GetKillCount(EnemyPdName);
        }

        public override bool CanPay() => GetBalanceDue() <= 0;

        public override void OnPay() { }

        public override string GetCostText()
        {
            // todo - will need to get the enemy name convo and appropriate localized enemy name;
            // will be easiest to do once special cases are handled and then
            // can just dump it back into the struct and re-serialize
            int bal = GetBalanceDue();
            string localizedEnemyName = Language.Language.Get($"NAME_{EnemyConvoName}", "Journal");
            if (bal == 1)
            {
                return string.Format(Language.Language.Get("DEFEAT_ENEMY", "Fmt"), localizedEnemyName);
            }
            else if (bal > 1)
            {
                return string.Format(Language.Language.Get("DEFEAT_ENEMIES", "Fmt"), bal, localizedEnemyName);
            }
            else
            {
                return string.Format(Language.Language.Get("DEFEATED_ENEMIES", "Fmt"), localizedEnemyName);
            }
        }

        public static EnemyKillCost ConstructEntryCost(string icKey)
        {
            MinimalEnemyDef def = EnemyData.NormalData[icKey];
            return new EnemyKillCost(def.pdName, def.convoName, 1);
        }

        public static EnemyKillCost ConstructNotesCost(string icKey)
        {
            MinimalEnemyDef def = EnemyData.NormalData[icKey];
            return new EnemyKillCost(def.pdName, def.convoName, def.notesCost);
        }

        public static EnemyKillCost ConstructCustomCost(string icKey, int amount)
        {
            try
            {
                MinimalEnemyDef def = EnemyData.NormalData[icKey];
                return new EnemyKillCost(def.pdName, def.convoName, amount);
            }
            catch
            {
                TheRealJournalRando.Instance.LogError($"Failed to construct cost: {amount} {icKey}");
                throw;
            }
        }
    }
}
