using System;
using System.Linq;
using System.Collections.Generic;

using Bio;
using Bio.BWA;
using Bio.IO;
using Bio.Variant;
using PacBio.BAM;
using Bio.Algorithms.Alignment;
using Bio.SimilarityMatrices;

namespace ccsviewer
{
    public class AlignedSequence
    {
        public string Name;
        public string Sequence;
       
        /// <summary>
        /// Array that indicates what position in the alignment is responsible for each BP.
        /// </summary>
        public int[] OffSets;
        public AlignedSequence (Sequence read, Sequence reference, Sequence reference_rc)
        {
            // Align the read and reference
            var algo = new NeedlemanWunschAligner ();
            algo.GapOpenCost = -20;
            algo.GapExtensionCost = -19;
            algo.SimilarityMatrix = new DiagonalSimilarityMatrix (20, -20);
            var aln = algo.Align (read, reference).First ().AlignedSequences.First () as PairwiseAlignedSequence;
            var reverse_aln = algo.Align (read, reference_rc).First ().AlignedSequences.First () as PairwiseAlignedSequence;
            Sequence Ref, Query;
            Name = read.ID;
            if (aln.Score < reverse_aln.Score) {
                aln = reverse_aln;
                Query = (aln.Sequences [0] as Sequence).GetReverseComplementedSequence () as Sequence;
                Ref = (aln.Sequences [1] as Sequence).GetReverseComplementedSequence () as Sequence;
                Name = Name + "_RC";
            } else {
                Query = (aln.Sequences [0] as Sequence);
                Ref = (aln.Sequences [1] as Sequence);                
            }

            // Now calculate the offsets
            OffSets = new int[reference.Count];
            int refBases = -1;
            for (int i = 0; i < Ref.Count; i++) {
                if (Ref [i] != '-') {
                    refBases++;
                    OffSets [refBases] = i;
                }
            }
            Sequence = Query.ConvertToString ();
            
            //Console.WriteLine (read.ID);
            //Console.WriteLine ((aln.Sequences [0] as Sequence).ConvertToString ());
            //Console.WriteLine ((aln.Sequences [1] as Sequence).ConvertToString ());

        }
        public int GetInsertsBeforePosition(int ref_pos)
        {
            if (ref_pos == 0) {
                return OffSets [0];
            } else {
                return OffSets [ref_pos] - OffSets [ref_pos - 1] - 1;
            }

        }
        public void ReJiggerAlignment(int[] metaOffSets) {
            if (this.Name == "94971/20880_21023_RC") {
                Console.WriteLine ("Test1");
            }
            List<char> newQuery = new List<char>();

            for (int i = 0; i < (metaOffSets.Length - 1); i++) {
                var neededInserts = metaOffSets [i];
                var start = i > 0 ? OffSets [i - 1] + 1 : 0;
                var end = OffSets [i];
                var inserts = end - start;
                var addspace = neededInserts - inserts;
                // Add Padding
                for(int j=0; j < addspace; j++) {
                    newQuery.Add ('-');
                }
                // Add Inserts
                for(int j = start; j < end; j++) {
                    newQuery.Add (Sequence [j]);
                }
                // Add Actual Base
                newQuery.Add(Sequence[OffSets[i]]);
            }
            // Not to add on the rest
            for (int k = OffSets [OffSets.Length - 1] + 1; k < Sequence.Length; k++) {
                newQuery.Add (Sequence [k]);
            }
            Sequence = new string (newQuery.ToArray ());
        }

    }
}

