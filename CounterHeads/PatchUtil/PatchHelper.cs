using System;
using System.Collections.Generic;
using BepInEx;
using HarmonyLib;

namespace CounterHeads.PatchUtil;

public class PatchHelper
{
    public static void LogPatchName(string patchClass, string patchMethod)
    {
        CounterHeads.Logger.LogMessage("------ Begin Patch: " + patchClass + "  " + patchMethod + " ------");
    }
    
    public static void PatchAndLogSurroundingIfValid(CodeMatcher match, Action<CodeMatcher> modifier, string name, int surroundingInstructions = 2)
    {
        if (!match.IsValid)
        {
            CounterHeads.Logger.LogError("Code match '" + name + "' did not match against any target instructions. Some mod features may not work correctly!");
            return;
        }
        
        CounterHeads.Logger.LogMessage("-- PATCHING INSTRUCTION: " + InstructionString(match.Instruction));
        CounterHeads.Logger.LogMessage("  -- Before Patch");
        LogSurrounding(surroundingInstructions, match, "    -- ");
        modifier.Invoke(match);
        CounterHeads.Logger.LogMessage(" - - After Patch");
        LogSurrounding(surroundingInstructions, match, "    -- ");
    }

    public static void LogSurrounding(int surroundingInstructions, CodeMatcher match, string prefix = "")
    {
        var copyMatch = match.Clone();
        copyMatch.Advance(-surroundingInstructions - 1);
        for (int i = 0; i < (surroundingInstructions * 2) + 1; i++)
        {
            if (i == surroundingInstructions)
            {
                LogInstruction(copyMatch.Advance(1).Instruction, "    <------ TARGET", prefix);
                continue;
            }
            LogInstruction(copyMatch.Advance(1).Instruction, prefix: prefix);
        }
    }

    public static void LogInstructions(IEnumerable<CodeInstruction> ins)
    {
        foreach (var i in ins)
        {
            LogInstruction(i);
        }
    }

    public static void LogInstruction(CodeInstruction i, string suffix = "", string prefix = "")
    {
        CounterHeads.Logger.LogMessage(prefix + InstructionString(i) + suffix);
    }

    public static string InstructionString(CodeInstruction i)
    {
        return "[ " + i.opcode + "    " + i.operand + " ]";
    }
}