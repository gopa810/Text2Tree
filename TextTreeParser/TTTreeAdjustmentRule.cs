using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TextTreeParser
{
    /// <summary>
    /// Rules and operations for post-adjustments in the tree.
    /// 
    /// Merge Child with Parent
    /// -----------------------
    /// Input data:
    ///     parent type + comparing type
    ///     child type + comparing type
    /// Output action:
    ///     Move subnodes of child into parent.
    ///     
    /// </summary>
    public class TTTreeAdjustmentRule
    {
        public static readonly int RULE_NO_RULE = 0;
        public static readonly int RULE_MERGE_CHILD_TYPE_TYPE = 1;

        public static readonly int PART_START = 1;
        public static readonly int PART_WHOLE = 2;
        public static readonly int PART_END = 3;
        public static readonly int PART_SUB = 4;

        public int rule;
        public int comparePart1;
        public int comparePart2;

        public string str1;
        public string str2;


        public void setTypeType(string s1, int part1, string s2, int part2)
        {
            rule = TTTreeAdjustmentRule.RULE_MERGE_CHILD_TYPE_TYPE;
            comparePart1 = part1;
            str1 = s1;
            comparePart2 = part2;
            str2 = s2;
        }

        public void setTypeType(string s1, string s2)
        {
            setTypeType(s1, TTTreeAdjustmentRule.PART_WHOLE, s2, TTTreeAdjustmentRule.PART_WHOLE);
        }

        public bool evaluate1(string s)
        {
            return evaluate(comparePart1, str1, s);
        }

        public bool evaluate2(string s)
        {
            return evaluate(comparePart2, str2, s);
        }

        private bool evaluate(int cp1, string sp1, string s)
        {
            if (cp1 == TTTreeAdjustmentRule.PART_START)
            {
                return s.StartsWith(sp1);
            }
            else if (cp1 == TTTreeAdjustmentRule.PART_SUB)
            {
                return s.IndexOf(sp1) >= 0;
            }
            else if (cp1 == TTTreeAdjustmentRule.PART_WHOLE)
            {
                return s.Equals(sp1);
            }
            else if (cp1 == TTTreeAdjustmentRule.PART_END)
            {
                return s.EndsWith(sp1);
            }

            return false;
        }
    }
}
