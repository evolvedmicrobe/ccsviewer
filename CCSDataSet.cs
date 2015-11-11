using System;
using System.Linq;
using System.Collections.Generic;

using Bio;
using Bio.BWA;
using Bio.IO;
using Bio.Variant;
using PacBio.BAM;

namespace ccsviewer
{
    public class CCSDataSet
    {
        public List<AlignedSequence> Reads;
        public Sequence CCS;
        public Sequence Reference;
        List<Variant> Variants;
        BWAPairwiseAlignment Aln;
        public CCSDataSet (Sequence ccs, List<Sequence> subreads, BWAPairwiseAlignment aln)
        {
            // Call Variants
            var results = VariantCaller.LeftAlignIndelsAndCallVariants (aln);
            Variants = results.Item2;
            Variants.ForEach (p => {
                p.StartPosition += aln.AlignedSAMSequence.Pos;
                p.RefName = aln.Reference;
            });

            // Get the ungapped reference
            var ccs_aligned = results.Item1.SecondSequence;

            // Generate an ungapped reference 
            var refseq = results.Item1.PairwiseAlignedSequences.First().FirstSequence;
            var reference = new Sequence (DnaAlphabet.Instance,
                                refseq.Where (bp => bp != '-').ToArray (),
                                false) 
            { ID = aln.Reference + "/" + aln.AlignedSAMSequence.Pos + "-" + aln.AlignedSAMSequence.RefEndPos };

            var reference_rc = reference.GetReverseComplementedSequence () as Sequence;

            Reads = subreads.Select(z => new AlignedSequence(z, reference, reference_rc)).Where( x => (Math.Abs((x.Sequence.Length - reference.Count)) / (double) reference.Count) < 0.2).ToList();

            Reads.Insert (0, new AlignedSequence (reference, reference, reference_rc));

            ccs.ID = "CCS";
            Reads.Insert(1, new AlignedSequence(ccs, reference, reference_rc));

            // Now to rejigger the alignments
            int[] metaOffSets = Enumerable.Range(0, (int)reference.Count).
                Select (i => Reads.Max( r => r.GetInsertsBeforePosition(i))).
                ToArray();
            Reads.ForEach(r => {
                r.ReJiggerAlignment(metaOffSets);
                Console.WriteLine(r.Sequence);});
                

            CCS = ccs;
            Reference = reference;
            Aln = aln;
        }
    }
}

