using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace ccsviewer
{
    public class MSA
    {
        public List<AlignedSequence> Sequences;
        public string ConsensusString;
        public MSA ()
        {
            Sequences = new List<AlignedSequence> () {
                new AlignedSequence () { Name = "Test1", Sequence = "TTCTATATCATCACTGAGTTCATGACCTACGGGAACCTCCTTTCTATATCATCACTGAGTTCATGACCTACGGGAACCTCCTTTCTATATCATCACTGAGTTCATGACCTACGGGAACCTCCTTTCTATATCATCACTGAGTTCATGACCTACGGGAACCTCCT" },
                new AlignedSequence () { Name = "Test2", Sequence = "TTCTATATCATCACTGAGTTGATGACCTACGGGAACCTCCTTTCTATATCATCACTGAGTTGATGACCTACGGGAACCTCCTTTCTATATCATCACTGAGTTGATGACCTACGGGAACCTCCTTTCTATATCATCACTGAGTTGATGACCTACGGGAACCTCCT" }
            };

        }
        public void AddSequence(AlignedSequence seq) {
            if (Sequences.Count > 0 && seq.Sequence.Length != Sequences.First ().Sequence.Length) {
                throw new ArgumentException ("Sequence was not the same length as the others, problem sequence was: " + seq.Name);
            }
            ConsensusString = null;
            Sequences.Add (seq);
        }

        public string CreateConsensusString() {
            if
        }
    }
}

